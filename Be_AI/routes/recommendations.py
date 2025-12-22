"""
Recommendation Routes - Experience Domain
Step 6: Serve API Top-K personalized recommendations

NOTE: Chỉ trả về IDs + scores, Experience Service sẽ lấy full data
"""

from fastapi import APIRouter, HTTPException, Query
from typing import List
from services.recommendation_service import recommendation_service
from schemas.experience_schemas import RecommendationResponse

router = APIRouter(prefix="/api/recommendations", tags=["recommendations"])


@router.get("/{user_id}", response_model=RecommendationResponse)
async def get_recommendations(
    user_id: str,
    top_k: int = Query(10, ge=1, le=50, description="Number of recommendations")
):
    """
    🎯 **Main Endpoint: Get Personalized Recommendations (IDs Only)**
    
    **Chức năng:**
    - Nhận userId từ Experience Service
    - Trả về top-K experience IDs + scores dựa trên ALS model
    - Experience Service sẽ tự lấy full data từ DB của nó
    
    **Parameters:**
    - `user_id` (path): GUID của user từ C# service
    - `top_k` (query, optional): Số lượng recommendations (default: 10, max: 50)
    
    **Response:** RecommendationResponse containing:
    - `user_id`: User ID
    - `recommendations`: Array of {experience_id, score, reason}
    - `total`: Total count
    - `generated_at`: Timestamp
    - `model`: Model name
    
    **Caching:** Auto cache trong Redis 1 giờ cho performance
    
    **Example Request:**
    ```bash
    GET /api/recommendations/3fa85f64-5717-4562-b3fc-2c963f66afa6?top_k=10
    ```
    
    **Example Response:**
    ```json
    {
      "user_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "recommendations": [
        {"experience_id": "exp-123", "score": 0.89, "reason": "Based on your preferences"},
        {"experience_id": "exp-456", "score": 0.85, "reason": "Based on your preferences"}
      ],
      "total": 2,
      "generated_at": "2025-12-13T10:00:00",
      "model": "ALS Collaborative Filtering"
    }
    ```
    
    **Flow:**
    1. AI Service trả IDs + scores
    2. Experience Service nhận response
    3. Experience Service query DB để lấy full experience data
    4. Experience Service trả về cho user
    """
    try:
        result = await recommendation_service.get_recommendations(
            user_id=user_id,
            top_k=top_k,
            use_cache=True  # Always use cache for production
        )
        return result
    except Exception as e:
        raise HTTPException(
            status_code=500, 
            detail=f"Failed to get recommendations: {str(e)}"
        )


@router.get("/popular", response_model=RecommendationResponse)
async def get_popular_experiences(
    top_k: int = Query(10, ge=1, le=50, description="Number of popular experiences")
):
    """
    📈 Lấy popular experience IDs (Hot/Trending)
    
    **Use case:**
    - Homepage "Popular Experiences"
    - Cold start cho new users
    - Fallback khi model chưa có data
    
    **Response:** Chỉ IDs + scores, Experience Service sẽ lấy full data
    
    **Example:**
    ```
    GET /api/recommendations/popular?top_k=10
    ```
    """
    try:
        from database import get_database
        db = get_database()
        result = await recommendation_service._get_popular_experience_ids(db, top_k)
        return result
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Failed to get popular experiences: {str(e)}"
        )
