from pydantic import BaseModel, Field
from typing import Optional, List
from datetime import datetime
from enum import Enum


class RecommendationItem(BaseModel):
    """Schema cho 1 recommendation item - CHỈ ID + SCORE"""
    experience_id: str = Field(..., description="ID của experience (Guid)")
    score: float = Field(..., description="Confidence score từ model (0-1)")
    reason: str = Field(default="Based on your preferences", description="Lý do recommend")
    
    class Config:
        json_schema_extra = {
            "example": {
                "experience_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                "score": 0.89,
                "reason": "Based on your preferences"
            }
        }


class RecommendationResponse(BaseModel):
    """Schema response cho recommendations - CHỈ IDs"""
    user_id: str = Field(..., description="User ID")
    recommendations: List[RecommendationItem] = Field(..., description="List IDs + scores")
    total: int = Field(..., description="Tổng số recommendations")
    generated_at: str = Field(..., description="Thời gian generate")
    model: str = Field(..., description="Model được sử dụng")
    
    class Config:
        json_schema_extra = {
            "example": {
                "user_id": "user123",
                "recommendations": [
                    {"experience_id": "exp-001", "score": 0.89, "reason": "Based on your preferences"},
                    {"experience_id": "exp-002", "score": 0.85, "reason": "Based on your preferences"}
                ],
                "total": 2,
                "generated_at": "2025-12-13T10:00:00",
                "model": "ALS Collaborative Filtering"
            }
        }


class InteractionType(str, Enum):
    """Các loại interaction với implicit rating weights"""
    VIEW = "view"              # Weight: 2.0 - User xem experience detail page
    SEARCH = "search"          # Weight: 1.0 - User search và thấy experience trong results
    WISHLIST = "wishlist"      # Weight: 3.0 - User thêm vào wishlist
    BOOKING = "booking"        # Weight: 4.0 - User booking experience
    RATING = "rating"          # Weight: 1-5 - User đánh giá experience
    COMPLETED = "completed"    # Weight: 5.0 - User hoàn thành experience


class InteractionCreate(BaseModel):
    """Schema để tạo interaction mới - nhận từ server chính"""
    user_id: str = Field(..., description="ID của user từ server chính")
    experience_id: str = Field(..., description="ID của experience")
    interaction_type: InteractionType = Field(..., description="Loại interaction")
    rating: Optional[float] = Field(None, ge=1.0, le=5.0, description="Rating 1-5 sao")
    booked: bool = Field(default=False, description="Đã booking chưa")
    completed: bool = Field(default=False, description="Đã hoàn thành chưa")
    
    class Config:
        json_schema_extra = {
            "example": {
                "user_id": "user123",
                "experience_id": "507f1f77bcf86cd799439011",
                "interaction_type": "wishlist",
                "rating": 4.5,
                "booked": False,
                "completed": False
            }
        }


class InteractionResponse(BaseModel):
    """Schema response cho interaction"""
    id: str
    user_id: str
    experience_id: str
    interaction_type: InteractionType
    rating: Optional[float] = None
    booked: bool = False
    completed: bool = False
    created_at: datetime
    
    class Config:
        from_attributes = True


# ==================== DEPRECATED: ExperienceCreate không còn dùng ====================
# AI service không lưu experience data nữa, chỉ lưu interactions


# ==================== DEPRECATED: Xóa ExperienceRecommendation và RecommendationResponse cũ ====================
# Các schema này đã được thay thế bởi RecommendationItem và RecommendationResponse ở trên


class TrainingStatus(BaseModel):
    """Schema cho training status"""
    status: str = Field(..., description="Status: pending, running, completed, failed")
    progress: float = Field(default=0.0, ge=0.0, le=100.0, description="Progress percentage")
    message: str = Field(default="", description="Status message")
    started_at: Optional[datetime] = None
    completed_at: Optional[datetime] = None
    
    class Config:
        json_schema_extra = {
            "example": {
                "status": "running",
                "progress": 45.5,
                "message": "Training model iteration 7/15",
                "started_at": "2025-11-27T10:00:00Z",
                "completed_at": None
            }
        }


class TrainingMetrics(BaseModel):
    """Schema cho training metrics"""
    precision_at_5: float
    precision_at_10: float
    precision_at_20: float
    recall_at_5: float
    recall_at_10: float
    recall_at_20: float
    ndcg_at_5: float
    ndcg_at_10: float
    ndcg_at_20: float
    hit_rate_at_5: float
    hit_rate_at_10: float
    hit_rate_at_20: float
    
    class Config:
        json_schema_extra = {
            "example": {
                "precision_at_5": 0.0554,
                "precision_at_10": 0.0493,
                "precision_at_20": 0.0419,
                "recall_at_5": 0.0322,
                "recall_at_10": 0.0572,
                "recall_at_20": 0.0984,
                "ndcg_at_5": 0.0619,
                "ndcg_at_10": 0.0661,
                "ndcg_at_20": 0.0775,
                "hit_rate_at_5": 0.2288,
                "hit_rate_at_10": 0.3460,
                "hit_rate_at_20": 0.4767
            }
        }


class TrainingResponse(BaseModel):
    """Schema response cho training"""
    training_id: str
    status: TrainingStatus
    metrics: Optional[TrainingMetrics] = None
    num_users: int = 0
    num_experiences: int = 0
    num_interactions: int = 0
    
    class Config:
        json_schema_extra = {
            "example": {
                "training_id": "train_20251127_103000",
                "status": {
                    "status": "completed",
                    "progress": 100.0,
                    "message": "Training completed successfully"
                },
                "metrics": {},
                "num_users": 2519,
                "num_experiences": 9862,
                "num_interactions": 169110
            }
        }
