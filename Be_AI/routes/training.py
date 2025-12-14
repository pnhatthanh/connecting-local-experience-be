"""
Training Routes - Experience Domain
API endpoints để trigger training pipeline
"""

from fastapi import APIRouter, BackgroundTasks, HTTPException

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
    background_tasks.add_task(training_service.run_full_pipeline)
    return {
        "message": "Model retraining started in background",
        "status": "pending",
        "note": "Model will be available after training completes (~5-10 minutes)"
    }
