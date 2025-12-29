"""
Training Service - Orchestrate model training pipeline
"""

import subprocess
import sys
import asyncio
from pathlib import Path
from typing import Dict, Optional
from datetime import datetime
import json
import logging

logger = logging.getLogger(__name__)

# Import retrain module directly
try:
    import retrain_from_mongodb
except ImportError:
    logger.warning("Could not import retrain_from_mongodb, retrain functionality will be limited")
    retrain_from_mongodb = None

MODELS_DIR = Path("models")
METADATA_FILE = MODELS_DIR / "als_metadata.json"


class TrainingService:
    """Service để quản lý training pipeline"""
    
    def __init__(self):
        self.training_status = "idle"  # idle, preprocessing, training, completed, failed
        self.last_training = None
        self._load_metadata()
    
    def _load_metadata(self):
        """Load training metadata từ file"""
        try:
            if METADATA_FILE.exists():
                with open(METADATA_FILE, 'r') as f:
                    metadata = json.load(f)
                    self.last_training = metadata
        except Exception as e:
            logger.warning(f"Could not load metadata: {e}")
    
    def _save_metadata(self, metadata: Dict):
        """Save training metadata"""
        try:
            MODELS_DIR.mkdir(exist_ok=True)
            with open(METADATA_FILE, 'w') as f:
                json.dump(metadata, f, indent=2)
            self.last_training = metadata
        except Exception as e:
            logger.error(f"Failed to save metadata: {e}")
    
    def run_preprocessing(self) -> Dict:
        """
        Chạy preprocessing: MongoDB → CSV
        Step 2 trong flow
        """
        if self.training_status == "preprocessing":
            return {"status": "error", "message": "Preprocessing already running"}
        
        self.training_status = "preprocessing"
        
        try:
            logger.info("🔄 Starting preprocessing...")
            print("🔄 Starting preprocessing...", flush=True)
            
            # Run preprocessing script
            result = subprocess.run(
                [sys.executable, "scripts/preprocess_interactions.py"],
                capture_output=True,
                text=True,
                timeout=300  # 5 minutes timeout
            )
            
            if result.returncode == 0:
                self.training_status = "idle"
                return {
                    "status": "success",
                    "message": "Preprocessing completed",
                    "output": result.stdout,
                    "timestamp": datetime.utcnow().isoformat()
                }
            else:
                self.training_status = "failed"
                return {
                    "status": "error",
                    "message": "Preprocessing failed",
                    "error": result.stderr
                }
                
        except subprocess.TimeoutExpired:
            self.training_status = "failed"
            return {
                "status": "error",
                "message": "Preprocessing timeout (>5 minutes)"
            }
        except Exception as e:
            self.training_status = "failed"
            logger.error(f"Preprocessing error: {e}")
            return {
                "status": "error",
                "message": str(e)
            }
    
    def run_training(self) -> Dict:
        """
        Chạy model training
        Steps 3-5 trong flow: Label encoding → Train ALS → Save model
        """
        if self.training_status == "training":
            return {"status": "error", "message": "Training already running"}
        
        self.training_status = "training"
        
        try:
            logger.info("🚀 Starting model training...")
            print("🚀 Starting model training...", flush=True)
            
            # Run training script
            result = subprocess.run(
                [sys.executable, "scripts/train_als_model.py"],
                capture_output=True,
                text=True,
                timeout=1800  # 30 minutes timeout
            )
            
            if result.returncode == 0:
                self.training_status = "completed"
                
                # Parse metrics từ output (nếu có)
                metadata = {
                    "status": "success",
                    "trained_at": datetime.utcnow().isoformat(),
                    "output": result.stdout
                }
                self._save_metadata(metadata)
                
                return metadata
            else:
                self.training_status = "failed"
                return {
                    "status": "error",
                    "message": "Training failed",
                    "error": result.stderr
                }
                
        except subprocess.TimeoutExpired:
            self.training_status = "failed"
            return {
                "status": "error",
                "message": "Training timeout (>30 minutes)"
            }
        except Exception as e:
            self.training_status = "failed"
            logger.error(f"Training error: {e}")
            return {
                "status": "error",
                "message": str(e)
            }
    
    def run_full_pipeline(self) -> Dict:
        """
        Chạy full pipeline: Preprocessing → Training
        """
        logger.info("="*60)
        logger.info("🎯 Starting full training pipeline...")
        print("\n" + "="*60)
        print("🎯 Starting full training pipeline...")
        print("="*60 + "\n", flush=True)
        
        # Step 1: Preprocessing
        logger.info("Step 1: Preprocessing interactions from MongoDB...")
        print("📊 Step 1: Preprocessing interactions from MongoDB...", flush=True)
        preprocess_result = self.run_preprocessing()
        if preprocess_result['status'] != 'success':
            logger.error(f"❌ Preprocessing failed: {preprocess_result}")
            print(f"❌ Preprocessing failed: {preprocess_result}", flush=True)
            return {
                "status": "error",
                "step": "preprocessing",
                "details": preprocess_result
            }
        logger.info("✅ Preprocessing completed successfully")
        print("✅ Preprocessing completed successfully\n", flush=True)
        
        # Step 2: Training
        logger.info("Step 2: Training ALS model...")
        print("🧠 Step 2: Training ALS model...", flush=True)
        training_result = self.run_training()
        if training_result['status'] != 'success':
            logger.error(f"❌ Training failed: {training_result}")
            print(f"❌ Training failed: {training_result}", flush=True)
            return {
                "status": "error",
                "step": "training",
                "details": training_result
            }
        logger.info("✅ Training completed successfully")
        print("✅ Training completed successfully\n", flush=True)
        
        logger.info("🎉 Full pipeline completed successfully!")
        print("="*60)
        print("🎉 Full pipeline completed successfully!")
        print("="*60 + "\n", flush=True)
        
        return {
            "status": "success",
            "message": "Full pipeline completed successfully",
            "preprocessing": preprocess_result,
            "training": training_result
        }
    
    def run_retrain_script(self) -> Dict:
        """
        Chạy retrain từ MongoDB (gọi trực tiếp async function)
        """
        if self.training_status == "training":
            return {"status": "error", "message": "Training already running"}
        
        if retrain_from_mongodb is None:
            return {
                "status": "error",
                "message": "retrain_from_mongodb module not available"
            }
        
        self.training_status = "training"
        
        try:
            logger.info("="*60)
            logger.info("🔄 Starting retrain from MongoDB...")
            print("\n" + "="*60)
            print("🔄 Starting retrain from MongoDB...")
            print("="*60 + "\n", flush=True)
            
            # Run async retrain function
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
            loop.run_until_complete(retrain_from_mongodb.main())
            loop.close()
            
            self.training_status = "completed"
            
            # Clear old model from memory - force reload on next request
            from services.recommendation_service import recommendation_service
            recommendation_service.model_data = None  
            recommendation_service.encoders = None
            recommendation_service._loaded = False
            logger.info("✓ Cleared old model from memory, will reload on next request")
            
            # Clear all cached recommendations using sync version
            try:
                from database import clear_recommendations_cache_sync
                cleared_count = clear_recommendations_cache_sync()
                if cleared_count > 0:
                    logger.info(f"✓ Cleared {cleared_count} cached recommendations")
                    print(f"✓ Cleared {cleared_count} cached recommendations\n", flush=True)
                else:
                    logger.info("No cached recommendations to clear")
                    print("No cached recommendations to clear\n", flush=True)
            except Exception as e:
                logger.warning(f"Failed to clear cache: {e}")
                print(f"⚠️ Failed to clear cache: {e}\n", flush=True)
            
            # Don't save metadata here - retrain_from_mongodb already saved it with full details
            logger.info("✅ Retrain completed successfully")
            print("✅ Retrain completed successfully\n", flush=True)
            
            return {
                "status": "success",
                "message": "Retrain completed, cache cleared, metadata saved by retrain script"
            }
                
        except Exception as e:
            self.training_status = "failed"
            logger.error(f"❌ Retrain error: {e}", exc_info=True)
            print(f"❌ Retrain error: {e}", flush=True)
            return {
                "status": "error",
                "message": str(e)
            }
    
    def get_status(self) -> Dict:
        """Get current training status"""
        return {
            "current_status": self.training_status,
            "last_training": self.last_training
        }
    
    def get_metrics(self) -> Optional[Dict]:
        """Get training metrics từ metadata"""
        if self.last_training and self.last_training.get('status') == 'success':
            return {
                "trained_at": self.last_training.get('trained_at'),
                "output": self.last_training.get('output', '')
            }
        return None


# Singleton instance
training_service = TrainingService()
