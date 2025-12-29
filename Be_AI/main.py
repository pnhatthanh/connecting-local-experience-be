from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from contextlib import asynccontextmanager
from apscheduler.schedulers.background import BackgroundScheduler
import subprocess
import logging
import uvicorn
import sys
import os
from config import settings
from database import connect_to_mongodb, close_mongodb_connection
from database import connect_to_redis, close_redis_connection, clear_recommendations_cache_sync
from database import rabbitmq_consumer
from routes.recommendations import router as recommendations_router
from routes.training import router as training_router
from services.recommendation_service import recommendation_service
import asyncio



logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)


@asynccontextmanager
async def lifespan(app: FastAPI):
    logger.info("🚀 Starting Experience Recommendation System...")
    await connect_to_mongodb()
    await connect_to_redis()
    
    try:
        await rabbitmq_consumer.connect()
        asyncio.create_task(rabbitmq_consumer.start_consuming())
    except Exception as e:
        logger.error(f"Failed to start RabbitMQ consumer: {e}")
    
    logger.info("✓ Application started successfully")

    yield

    logger.info("Shutting down...")
    await rabbitmq_consumer.close()
    await close_mongodb_connection()
    await close_redis_connection()
    logger.info("✓ Application shutdown complete")


def retrain_job():
    try:
        logger.info("🔄 Starting scheduled model retraining...")
        logger.info(f"Working directory: {os.getcwd()}")
        
        # Import and run retrain function directly
        import retrain_from_mongodb
        
        # Run async retrain in new event loop
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)
        loop.run_until_complete(retrain_from_mongodb.main())
        loop.close()
        
        logger.info("✓ Model retraining completed successfully")
        
        # Clear old model and mark as not loaded
        recommendation_service.model_data = None  
        recommendation_service.encoders = None
        recommendation_service._loaded = False
        logger.info("✓ Cleared old model from memory, will reload on next request")
        
        # Clear all cached recommendations using sync version (no event loop issues)
        try:
            cleared_count = clear_recommendations_cache_sync()
            if cleared_count > 0:
                logger.info(f"✓ Cleared {cleared_count} cached recommendations")
            else:
                logger.info("No cached recommendations to clear")
        except Exception as e:
            logger.warning(f"Failed to clear cache: {e}")
        
    except subprocess.TimeoutExpired:
        logger.error("⚠️ Retraining timeout - process killed")
    except Exception as e:
        logger.error(f"❌ Retraining job failed: {e}", exc_info=True)


scheduler = BackgroundScheduler()
scheduler.add_job(
    retrain_job, 
    'interval', 
    hours=1,
    id='retrain_als_model',
    name='Retrain ALS Model',
    replace_existing=True
)
scheduler.start()
logger.info("📅 Scheduler started: Model will retrain every 6 hours") 

app = FastAPI(
    title="Experience Recommendation System (AI Service)",
    version="2.0.0",
    description="""
    ## 7-Step Workflow
    1. **Step 1**: User interaction → RabbitMQ → AI Service (POST /api/interactions)
    2. **Step 2**: Preprocessing → MongoDB interactions to CSV
    3. **Step 3**: Label encoding → user_id, experience_id to indices
    4. **Step 4**: Train ALS model (POST /api/training/train)
    5. **Step 5**: Save model + encoders to disk
    6. **Step 6**: Serve API → Get IDs (GET /api/recommendations/{user_id})
    7. **Step 7**: Experience Service → Lấy full data → Trả user
    
    ## Interaction Types & Implicit Ratings
    - **view**: 2.0 (user viewed experience detail page)
    - **search**: 1.0 (user saw in search results)
    - **wishlist**: 3.0 (user added to wishlist)
    - **booking**: 4.0 (user booked experience)
    - **rating**: 1-5 (explicit user rating)
    - **completed**: 5.0 (user completed experience)
    """,
    lifespan=lifespan,
    debug=settings.DEBUG
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.CORS_ORIGINS,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(recommendations_router)
app.include_router(training_router)

@app.get("/")
async def root():
    return {
        "app": "AI Recommendation Service",
        "version": "2.0.0",
        "status": "running",
        "docs": "/docs",
        "architecture": "Microservices - AI Service only returns IDs",
        "endpoints": {
            "get_recommendations": "GET /api/recommendations/{user_id}?top_k=10",
            "get_popular": "GET /api/recommendations/popular?top_k=10",
            "retrain_model": "POST /api/training/retrain"
        },
        "note": "This service only returns experience IDs + scores. Experience Service handles full data."
    }


@app.get("/health")
async def health_check():
    return {
        "status": "healthy",
        "version": "1.0.0",
        "model_loaded": False  
    }


if __name__ == "__main__":
    uvicorn.run(
        "main:app",
        host=settings.HOST,
        port=settings.PORT,
        reload=settings.DEBUG,
        log_level="info"
    )
