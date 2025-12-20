"""
Training Routes - Experience Domain
API endpoints để trigger training pipeline
"""

from fastapi import APIRouter, BackgroundTasks, HTTPException
from pathlib import Path
import json
from datetime import datetime

from services.training_service import training_service

router = APIRouter(prefix="/api/training", tags=["training"])


@router.post("/retrain")
async def retrain_model(background_tasks: BackgroundTasks):
    """
    🔄 Retrain ALS model từ MongoDB interactions
    
    **Chức năng:**
    - Load tất cả interactions từ MongoDB
    - Train lại ALS model với data mới nhất
    - Save model mới vào models/ directory
    
    **Sử dụng:**
    - Manual trigger khi cần update model ngay
    - Auto chạy mỗi 6 giờ bởi scheduler
    
    **Response:** Background task started, model sẽ reload sau khi train xong
    
    **Example:**
    ```bash
    POST /api/training/retrain
    ```
    """
    if training_service.training_status == "training":
        raise HTTPException(
            status_code=409, 
            detail="Training is already in progress. Please wait for it to complete."
        )
    
    background_tasks.add_task(training_service.run_full_pipeline)
    return {
        "message": "Model retraining started in background",
        "status": "pending",
        "note": "Model will be available after training completes (~5-10 minutes)",
        "check_status": "GET /api/training/status",
        "view_metrics": "GET /api/training/metrics"
    }


@router.get("/status")
async def get_training_status():
    """
    📊 Lấy trạng thái training hiện tại
    
    **Response:**
    - status: idle | preprocessing | training | completed | failed
    - last_training: thông tin lần training gần nhất
    
    **Example:**
    ```bash
    GET /api/training/status
    ```
    """
    return {
        "status": training_service.training_status,
        "last_training": training_service.last_training,
        "timestamp": datetime.utcnow().isoformat()
    }


@router.get("/metrics")
async def get_training_metrics():
    """
    📈 Lấy metrics của model training gần nhất
    
    **Metrics bao gồm:**
    - Precision@K: Độ chính xác recommendations
    - Recall@K: Coverage của relevant items
    - NDCG@K: Chất lượng ranking
    - Hit Rate@K: Tỷ lệ users hài lòng
    
    **Đánh giá:**
    - Precision@10 >= 0.35: TỐT ✅
    - Precision@10 >= 0.50: XUẤT SẮC 🎉
    - Precision@10 < 0.25: CẦN CẢI THIỆN ⚠️
    
    **Example:**
    ```bash
    GET /api/training/metrics
    ```
    """
    metadata_path = Path("models/als_metadata.json")
    
    if not metadata_path.exists():
        raise HTTPException(
            status_code=404,
            detail="No training metadata found. Please train the model first."
        )
    
    try:
        with open(metadata_path, 'r') as f:
            metadata = json.load(f)
        
        # Extract metrics
        metrics = metadata.get('metrics', {})
        
        # Đánh giá chất lượng
        precision_10 = metrics.get('precision@10', 0)
        quality_assessment = {
            "overall_quality": "XUẤT SẮC 🎉" if precision_10 >= 0.50 else
                              "TỐT ✅" if precision_10 >= 0.35 else
                              "ĐẠT YÊU CẦU ⚠️" if precision_10 >= 0.25 else
                              "CẦN CẢI THIỆN ❌",
            "ready_for_production": precision_10 >= 0.30,
            "precision_benchmark": {
                "value": precision_10,
                "minimum": 0.25,
                "good": 0.35,
                "excellent": 0.50
            }
        }
        
        return {
            "trained_at": metadata.get('trained_at'),
            "model_info": {
                "algorithm": metadata.get('algorithm'),
                "n_users": metadata.get('n_users'),
                "n_items": metadata.get('n_items'),
                "n_train_interactions": metadata.get('n_train_interactions'),
                "n_test_interactions": metadata.get('n_test_interactions')
            },
            "hyperparameters": {
                "factors": metadata.get('factors'),
                "regularization": metadata.get('regularization'),
                "iterations": metadata.get('iterations'),
                "alpha": metadata.get('alpha')
            },
            "metrics": {
                "precision": {
                    "@5": metrics.get('precision@5'),
                    "@10": metrics.get('precision@10'),
                    "@20": metrics.get('precision@20'),
                    "description": "Tỷ lệ items relevant được recommend"
                },
                "recall": {
                    "@5": metrics.get('recall@5'),
                    "@10": metrics.get('recall@10'),
                    "@20": metrics.get('recall@20'),
                    "description": "Tỷ lệ relevant items được tìm thấy"
                },
                "ndcg": {
                    "@5": metrics.get('ndcg@5'),
                    "@10": metrics.get('ndcg@10'),
                    "@20": metrics.get('ndcg@20'),
                    "description": "Chất lượng ranking (relevant items ở top)"
                },
                "hit_rate": {
                    "@5": metrics.get('hit_rate@5'),
                    "@10": metrics.get('hit_rate@10'),
                    "@20": metrics.get('hit_rate@20'),
                    "description": "% users có ít nhất 1 relevant recommendation"
                }
            },
            "quality_assessment": quality_assessment,
            "recommendations": {
                "for_improvement": [
                    "Thu thập thêm user interactions" if metadata.get('n_train_interactions', 0) < 1000 else None,
                    "Thêm experiences (items)" if metadata.get('n_items', 0) < 20 else None,
                    "Tăng factors lên 150" if precision_10 < 0.30 else None,
                    "Tune regularization" if precision_10 < 0.25 else None
                ],
                "status": "Model sẵn sàng cho production" if precision_10 >= 0.35 else
                         "Model cần cải thiện trước khi deploy"
            }
        }
        
    except json.JSONDecodeError:
        raise HTTPException(
            status_code=500,
            detail="Failed to parse training metadata"
        )
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Error reading training metrics: {str(e)}"
        )


@router.get("/history")
async def get_training_history():
    """
    📜 Lấy lịch sử training (5 lần gần nhất)
    
    **Response:** Danh sách các lần training với metrics
    
    **Example:**
    ```bash
    GET /api/training/history
    ```
    """
    metadata_path = Path("models/als_metadata.json")
    
    if not metadata_path.exists():
        return {
            "message": "No training history found",
            "history": []
        }
    
    try:
        with open(metadata_path, 'r') as f:
            metadata = json.load(f)
        
        # Chỉ trả về thông tin cơ bản
        return {
            "latest_training": {
                "trained_at": metadata.get('trained_at'),
                "algorithm": metadata.get('algorithm'),
                "n_users": metadata.get('n_users'),
                "n_items": metadata.get('n_items'),
                "precision@10": metadata.get('metrics', {}).get('precision@10'),
                "hit_rate@10": metadata.get('metrics', {}).get('hit_rate@10')
            }
        }
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Error reading training history: {str(e)}"
        )


@router.get("/health")
async def check_model_health():
    """
    🏥 Kiểm tra sức khỏe model
    
    **Checks:**
    - Model file exists
    - Encoders file exists  
    - Metadata exists
    - Model metrics meet minimum requirements
    
    **Example:**
    ```bash
    GET /api/training/health
    ```
    """
    model_path = Path("models/als_model.pkl")
    encoders_path = Path("models/encoders_als.pkl")
    metadata_path = Path("models/als_metadata.json")
    
    health_status = {
        "model_exists": model_path.exists(),
        "encoders_exist": encoders_path.exists(),
        "metadata_exists": metadata_path.exists(),
        "ready_for_inference": False,
        "issues": []
    }
    
    if not model_path.exists():
        health_status["issues"].append("Model file not found. Please train the model.")
    
    if not encoders_path.exists():
        health_status["issues"].append("Encoders file not found. Please train the model.")
    
    if metadata_path.exists():
        try:
            with open(metadata_path, 'r') as f:
                metadata = json.load(f)
            
            precision_10 = metadata.get('metrics', {}).get('precision@10', 0)
            
            # Check if model meets minimum requirements
            if precision_10 < 0.25:
                health_status["issues"].append(
                    f"Model precision@10 ({precision_10:.3f}) below minimum (0.25). Model needs improvement."
                )
            
            health_status["model_metrics"] = {
                "precision@10": precision_10,
                "meets_minimum": precision_10 >= 0.25,
                "trained_at": metadata.get('trained_at')
            }
            
        except Exception as e:
            health_status["issues"].append(f"Cannot read metadata: {str(e)}")
    else:
        health_status["issues"].append("Metadata not found.")
    
    # Overall health
    health_status["ready_for_inference"] = (
        health_status["model_exists"] and 
        health_status["encoders_exist"] and 
        len(health_status["issues"]) == 0
    )
    
    health_status["status"] = "healthy" if health_status["ready_for_inference"] else "unhealthy"
    
    return health_status
