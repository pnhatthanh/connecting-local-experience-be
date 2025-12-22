"""
Script preprocess interactions từ MongoDB → CSV
Step 2: Preprocess Interaction Table

Chuyển đổi interactions thành implicit ratings phù hợp cho CF model
"""

import asyncio
from datetime import datetime
from pathlib import Path
import pandas as pd
import logging
from motor.motor_asyncio import AsyncIOMotorClient
from bson import ObjectId

# Setup logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# Paths
BASE_DIR = Path(__file__).resolve().parent.parent
DATA_DIR = BASE_DIR / "data"
DATA_DIR.mkdir(exist_ok=True)

# MongoDB connection (adjust according to your config)
MONGO_URI = "mongodb://localhost:27017"
DB_NAME = "recommend_experiences"
INTERACTIONS_COLLECTION = "interactions"

# Thresholds
MIN_USER_INTERACTIONS = 1
MIN_EXPERIENCE_INTERACTIONS =1


class InteractionPreprocessor:
    """Preprocess interactions từ MongoDB"""
    
    def __init__(self):
        self.client = None
        self.db = None
    
    async def connect(self):
        """Connect to MongoDB"""
        logger.info("🔌 Connecting to MongoDB...")
        self.client = AsyncIOMotorClient(MONGO_URI)
        self.db = self.client[DB_NAME]
        logger.info("✅ Connected to MongoDB")
    
    async def close(self):
        """Close MongoDB connection"""
        if self.client:
            self.client.close()
            logger.info("🔌 Closed MongoDB connection")
    
    async def fetch_interactions(self):
        """Fetch all interactions từ MongoDB"""
        logger.info("📥 Fetching interactions from MongoDB...")
        
        cursor = self.db[INTERACTIONS_COLLECTION].find({})
        interactions = await cursor.to_list(length=None)
        
        logger.info(f"✅ Fetched {len(interactions):,} interactions")
        return interactions
    
    def convert_to_implicit_rating(self, interaction):
        """
        Convert interaction thành implicit rating
        
        Rules:
        - view: 1.0
        - click: 2.0
        - wishlist: 3.0
        - rating: actual rating (1-5)
        - booking: 5.0
        - completed: 5.0
        """
        interaction_type = interaction.get('interaction_type', '')
        rating = interaction.get('rating')
        booked = interaction.get('booked', False)
        completed = interaction.get('completed', False)
        
        # Priority: completed > booked > rating > interaction_type
        if completed:
            return 5.0
        elif booked:
            return 5.0
        elif rating is not None and rating > 0:
            return float(rating)
        elif interaction_type == 'wishlist':
            return 3.0
        elif interaction_type == 'search':
            return 2.0
        elif interaction_type == 'view':
            return 1.0
        else:
            return 1.0  # Default
    
    def process_interactions(self, interactions):
        """
        Process interactions thành DataFrame
        
        Steps:
        1. Convert ObjectId to string
        2. Calculate implicit ratings
        3. Aggregate multiple interactions per user-experience
        4. Keep max rating
        """
        logger.info("🔄 Processing interactions...")
        
        # Convert to list of dicts
        data = []
        for interaction in interactions:
            user_id = str(interaction.get('user_id', ''))
            experience_id = str(interaction.get('experience_id', ''))
            
            if not user_id or not experience_id:
                continue
            
            implicit_rating = self.convert_to_implicit_rating(interaction)
            
            data.append({
                'user_id': user_id,
                'experience_id': experience_id,
                'rating': implicit_rating,
                'created_at': interaction.get('created_at', datetime.utcnow())
            })
        
        df = pd.DataFrame(data)
        
        if df.empty:
            logger.warning("⚠️  No valid interactions found!")
            return df
        
        logger.info(f"📊 Total raw interactions: {len(df):,}")
        logger.info(f"   - Unique users: {df['user_id'].nunique():,}")
        logger.info(f"   - Unique experiences: {df['experience_id'].nunique():,}")
        
        # Aggregate: user-experience có nhiều interactions → lấy max rating
        logger.info("🔄 Aggregating multiple interactions (taking max rating)...")
        df_agg = df.groupby(['user_id', 'experience_id']).agg({
            'rating': 'max',  # Lấy rating cao nhất
            'created_at': 'max'  # Lấy thời gian mới nhất
        }).reset_index()
        
        logger.info(f"✅ After aggregation: {len(df_agg):,} unique user-experience pairs")
        
        return df_agg
    
    def filter_sparse_data(self, df):
        """
        Lọc bỏ sparse users và experiences
        
        Iterative filtering:
        - Remove users with < MIN_USER_INTERACTIONS
        - Remove experiences with < MIN_EXPERIENCE_INTERACTIONS
        - Repeat until convergence
        """
        logger.info("\n🔍 Filtering sparse data...")
        logger.info(f"   MIN_USER_INTERACTIONS: {MIN_USER_INTERACTIONS}")
        logger.info(f"   MIN_EXPERIENCE_INTERACTIONS: {MIN_EXPERIENCE_INTERACTIONS}")
        
        iteration = 0
        prev_size = len(df)
        
        while True:
            iteration += 1
            
            # Count interactions
            user_counts = df['user_id'].value_counts()
            experience_counts = df['experience_id'].value_counts()
            
            # Filter
            valid_users = user_counts[user_counts >= MIN_USER_INTERACTIONS].index
            valid_experiences = experience_counts[experience_counts >= MIN_EXPERIENCE_INTERACTIONS].index
            
            df = df[df['user_id'].isin(valid_users) & df['experience_id'].isin(valid_experiences)]
            
            logger.info(f"   Iteration {iteration}: {len(df):,} interactions")
            
            # Check convergence
            if len(df) == prev_size:
                logger.info(f"✅ Converged after {iteration} iterations")
                break
            
            prev_size = len(df)
            
            if iteration > 50:
                logger.warning("⚠️  Max iterations reached")
                break
        
        logger.info(f"\n📊 Final dataset:")
        logger.info(f"   - Interactions: {len(df):,}")
        logger.info(f"   - Users: {df['user_id'].nunique():,}")
        logger.info(f"   - Experiences: {df['experience_id'].nunique():,}")
        logger.info(f"   - Avg rating: {df['rating'].mean():.2f}")
        
        # Calculate density
        density = len(df) / (df['user_id'].nunique() * df['experience_id'].nunique()) * 100
        logger.info(f"   - Matrix density: {density:.3f}%")
        
        return df
    
    async def save_to_csv(self, df, filename="processed_interactions.csv"):
        """Save processed data to CSV"""
        output_file = DATA_DIR / filename
        
        logger.info(f"\n💾 Saving to {output_file}...")
        
        # Select columns
        df_output = df[['user_id', 'experience_id', 'rating']].copy()
        
        # Save
        df_output.to_csv(output_file, index=False)
        
        logger.info(f"✅ Saved {len(df_output):,} interactions to {output_file}")
        
        return output_file


async def main():
    """Main function"""
    logger.info("=" * 80)
    logger.info("PREPROCESSING INTERACTIONS - STEP 2")
    logger.info("=" * 80)
    
    preprocessor = InteractionPreprocessor()
    
    try:
        # Connect to MongoDB
        await preprocessor.connect()
        
        # Fetch interactions
        interactions = await preprocessor.fetch_interactions()
        
        if not interactions:
            logger.error("❌ No interactions found in database!")
            return
        
        # Process interactions
        df = preprocessor.process_interactions(interactions)
        
        if df.empty:
            logger.error("❌ No valid interactions to process!")
            return
        
        # Filter sparse data
        df_filtered = preprocessor.filter_sparse_data(df)
        
        if df_filtered.empty:
            logger.error("❌ All data filtered out! Try lowering MIN_USER_INTERACTIONS and MIN_EXPERIENCE_INTERACTIONS")
            return
        
        # Save to CSV
        output_file = await preprocessor.save_to_csv(df_filtered)
        
        logger.info("\n" + "=" * 80)
        logger.info("✅ PREPROCESSING COMPLETED!")
        logger.info("=" * 80)
        logger.info(f"📁 Output file: {output_file}")
        logger.info(f"📊 Ready for training!")
        
    except Exception as e:
        logger.error(f"❌ Error: {e}")
        import traceback
        traceback.print_exc()
    
    finally:
        await preprocessor.close()


if __name__ == "__main__":
    asyncio.run(main())
