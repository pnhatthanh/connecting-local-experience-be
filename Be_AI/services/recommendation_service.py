"""
Recommendation Service - Experience Domain
Sử dụng ALS (Alternating Least Squares) model cho collaborative filtering
"""

import pickle
import numpy as np
from pathlib import Path
from typing import List, Dict, Optional
from datetime import datetime
from bson import ObjectId
import logging

from database import get_database
from database.redis_cache import get_redis

logger = logging.getLogger(__name__)

# Paths
MODELS_DIR = Path("models")
MODEL_PATH = MODELS_DIR / "als_model.pkl"
ENCODERS_PATH = MODELS_DIR / "encoders_als.pkl"

# Collections
EXPERIENCES_COLLECTION = "experiences"
INTERACTIONS_COLLECTION = "interactions"  # Hoặc collection name của bạn


class RecommendationService:
    """Service cho Experience recommendations sử dụng ALS model"""
    
    def __init__(self):
        self.model_data = None
        self.encoders = None
        self._loaded = False
        self.redis_client = None
    
    def load_model(self):
        """Load ALS model và encoders một lần"""
        if self._loaded:
            return
        
        try:
            logger.info("Loading ALS model...")
            
            # Load model
            with open(MODEL_PATH, 'rb') as f:
                self.model_data = pickle.load(f)
            
            # Load encoders
            with open(ENCODERS_PATH, 'rb') as f:
                self.encoders = pickle.load(f)
            
            # Load Redis
            self.redis_client = get_redis()
            
            self._loaded = True
            logger.info(
                f"Model loaded successfully: "
                f"{self.model_data['user_factors'].shape[0]} users, "
                f"{self.model_data['item_factors'].shape[0]} items"
            )
            
        except FileNotFoundError as e:
            logger.error(f"Model files not found: {e}")
            raise Exception("Model not trained yet. Please run training first.")
        except Exception as e:
            logger.error(f"Failed to load model: {e}")
            raise
    
    async def get_recommendations(
        self,
        user_id: str,
        top_k: int = 10,
        use_cache: bool = True
    ) -> Dict:
        """
        Generate personalized recommendations cho user - CHỈ TRẢ IDs + SCORES
        
        Args:
            user_id: User ID
            top_k: Số lượng recommendations
            use_cache: Sử dụng Redis cache hay không
            
        Returns:
            Dict với recommendations (chỉ IDs + scores) và metadata
        """
        self.load_model()
        
        # Check cache
        if use_cache and self.redis_client:
            cache_key = f"recommendations:{user_id}:{top_k}"
            cached = await self._get_cached_recommendations(cache_key)
            if cached:
                logger.info(f"Cache hit for user {user_id}")
                return cached
        
        db = get_database()
        user_encoder = self.encoders['user_encoder']
        item_encoder = self.encoders['item_encoder']
        
        # Check nếu là new user (cold start)
        if user_id not in user_encoder.classes_:
            logger.info(f"New user {user_id}, using popularity-based recommendations")
            result = await self._get_popular_experience_ids(db, top_k)
        else:
            # Collaborative Filtering prediction
            user_idx = user_encoder.transform([user_id])[0]
            user_vec = self.model_data['user_factors'][user_idx]
            
            # Calculate scores cho tất cả items
            scores = self.model_data['item_factors'].dot(user_vec)
            
            # Get top-K items
            top_indices = np.argsort(scores)[-top_k:][::-1]
            top_item_ids = item_encoder.inverse_transform(top_indices)
            top_scores = scores[top_indices]
            
            # Build response CHỈ VỚI IDs + scores
            result = self._build_id_response(
                user_id, top_item_ids, top_scores, "Based on your preferences"
            )
        
        # Cache result
        if use_cache and self.redis_client:
            await self._cache_recommendations(cache_key, result, ttl=3600)
        
        return result
    
    async def get_similar_experiences(
        self,
        experience_id: str,
        top_k: int = 10
    ) -> Dict:
        """
        Tìm experiences tương tự dựa trên item factors - CHỈ TRẢ IDs + SCORES
        
        Args:
            experience_id: Experience ID
            top_k: Số lượng similar items
            
        Returns:
            Dict với similar experience IDs
        """
        self.load_model()
        db = get_database()
        item_encoder = self.encoders['item_encoder']
        
        # Check experience exists
        if experience_id not in item_encoder.classes_:
            logger.warning(f"Experience {experience_id} not in training data")
            return await self._get_popular_experience_ids(db, top_k)
        
        # Get item vector
        item_idx = item_encoder.transform([experience_id])[0]
        item_vec = self.model_data['item_factors'][item_idx]
        
        # Calculate cosine similarity với all items
        item_factors = self.model_data['item_factors']
        similarities = item_factors.dot(item_vec)
        
        # Normalize by magnitude
        norms = np.linalg.norm(item_factors, axis=1) * np.linalg.norm(item_vec)
        similarities = similarities / (norms + 1e-10)
        
        # Top-K (exclude chính nó)
        top_indices = np.argsort(similarities)[-(top_k+1):][::-1]
        top_indices = [idx for idx in top_indices if item_encoder.inverse_transform([idx])[0] != experience_id][:top_k]
        
        top_item_ids = item_encoder.inverse_transform(top_indices)
        top_scores = similarities[top_indices]
        
        return self._build_id_response(
            None, top_item_ids, top_scores, "Similar to selected experience"
        )
    
    def _build_id_response(
        self,
        user_id: Optional[str],
        item_ids: List[str],
        scores: np.ndarray,
        reason: str = "Based on your preferences"
    ) -> Dict:
        """Build response CHỈ VỚI IDs + scores - KHÔNG fetch experience data"""
        
        # Build recommendations list with only IDs and scores
        recommendations = []
        for exp_id, score in zip(item_ids, scores):
            recommendations.append({
                "experience_id": exp_id,
                "score": float(score),
                "reason": reason
            })
        
        return {
            "user_id": user_id or "anonymous",
            "recommendations": recommendations,
            "total": len(recommendations),
            "generated_at": datetime.utcnow().isoformat(),
            "model": "ALS Collaborative Filtering"
        }
    
    async def _get_popular_experience_ids(self, db, top_k: int) -> Dict:
        """
        Fallback cho new users: popular items - CHỈ TRẢ IDs
        Dựa trên số lượng interactions (views, bookings, etc.)
        """
        # Aggregate interactions để tính popularity
        pipeline = [
            {"$group": {
                "_id": "$experience_id",
                "interaction_count": {"$sum": 1},
                "booking_count": {
                    "$sum": {"$cond": [{"$eq": ["$interaction_type", "booking"]}, 1, 0]}
                }
            }},
            {"$sort": {"booking_count": -1, "interaction_count": -1}},
            {"$limit": top_k}
        ]
        
        popular_exp_ids = []
        async for doc in db[INTERACTIONS_COLLECTION].aggregate(pipeline):
            popular_exp_ids.append(doc["_id"])
        
        # Nếu chưa có interactions, trả empty
        if not popular_exp_ids:
            logger.warning("No interactions found, returning empty recommendations")
            return {
                "user_id": "new_user",
                "recommendations": [],
                "total": 0,
                "generated_at": datetime.utcnow().isoformat(),
                "model": "Popularity-based"
            }
        
        # Build ID-only response
        recommendations = []
        for exp_id in popular_exp_ids:
            recommendations.append({
                "experience_id": exp_id,
                "score": 1.0,
                "reason": "Popular experience"
            })
        
        return {
            "user_id": "new_user",
            "recommendations": recommendations,
            "total": len(recommendations),
            "generated_at": datetime.utcnow().isoformat(),
            "model": "Popularity-based"
        }
    
    async def _get_cached_recommendations(self, cache_key: str) -> Optional[Dict]:
        """Get recommendations từ Redis cache"""
        try:
            if self.redis_client:
                import json
                cached = await self.redis_client.get(cache_key)
                if cached:
                    return json.loads(cached)
        except Exception as e:
            logger.warning(f"Cache read failed: {e}")
        return None
    
    async def _cache_recommendations(self, cache_key: str, data: Dict, ttl: int = 3600):
        """Cache recommendations vào Redis"""
        try:
            if self.redis_client:
                import json
                await self.redis_client.setex(cache_key, ttl, json.dumps(data))
        except Exception as e:
            logger.warning(f"Cache write failed: {e}")
    
    async def invalidate_user_cache(self, user_id: str):
        """Invalidate cache khi user có interaction mới"""
        try:
            if self.redis_client:
                pattern = f"recommendations:{user_id}:*"
                keys = await self.redis_client.keys(pattern)
                if keys:
                    await self.redis_client.delete(*keys)
                    logger.info(f"Invalidated cache for user {user_id}")
        except Exception as e:
            logger.warning(f"Cache invalidation failed: {e}")


# Singleton instance
recommendation_service = RecommendationService()
