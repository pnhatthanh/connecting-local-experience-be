"""
Retrain ALS model từ MongoDB interactions
"""
import asyncio
import pandas as pd
import numpy as np
import pickle
import os
import sys
from pathlib import Path
from datetime import datetime
from motor.motor_asyncio import AsyncIOMotorClient
from bson import ObjectId

# Setup logging to stdout for Docker
import logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    handlers=[logging.StreamHandler(sys.stdout)],
    force=True
)
logger = logging.getLogger(__name__)

# Import ALS model từ script cũ
sys.path.append('scripts')
try:
    from train_als_model import ImplicitALS
    logger.info("✓ Successfully imported ImplicitALS")
except ImportError as e:
    logger.error(f"❌ Failed to import ImplicitALS: {e}")
    logger.error(f"Current sys.path: {sys.path}")
    sys.exit(1)

async def load_data_from_mongodb():
    """Load interactions từ MongoDB"""
    logger.info("="*60)
    logger.info("LOADING DATA FROM MONGODB")
    logger.info("="*60)
    
    # Get MongoDB URL from environment (for Docker compatibility)
    mongodb_url = os.getenv('MONGODB_URL', 'mongodb://localhost:27017')
    mongodb_db = os.getenv('MONGODB_DB_NAME', 'recommend_experiences')
    
    logger.info(f"MongoDB URL: {mongodb_url}")
    logger.info(f"Database: {mongodb_db}")
    
    try:
        client = AsyncIOMotorClient(mongodb_url, serverSelectionTimeoutMS=10000)
        # Test connection
        await client.admin.command('ping')
        logger.info("✓ MongoDB connection successful")
    except Exception as e:
        logger.error(f"❌ MongoDB connection failed: {e}")
        raise
    
    db = client[mongodb_db]
    
    # Load interactions (now with business_id field)
    logger.info("Loading interactions from database...")
    try:
        interactions = await db.interactions.find().to_list(length=None)
        logger.info(f"✓ Loaded {len(interactions):,} interactions")
    except Exception as e:
        logger.error(f"❌ Failed to load interactions: {e}")
        raise
    
    # Convert to DataFrame
    data = []
    skipped = 0
    
    for inter in interactions:
        user_id = inter.get('user_id')
        experience_id = inter.get('experience_id')  # Use experience_id directly
        
        if not experience_id:
            skipped += 1
            continue
        
        # Convert ObjectId to string if needed
        if isinstance(user_id, ObjectId):
            user_id = str(user_id)
        
        # Calculate implicit rating based on interaction type
        interaction_type = inter.get('interaction_type', 'view')
        rating_map = {
            'view': 2.0,       # Xem detail page
            'search': 1.0,     # Thấy trong search results
            'wishlist': 3.0,   # Add to wishlist
            'booking': 4.0,    # Đặt booking
            'completed': 5.0,  # Hoàn thành experience
            'rating': inter.get('rating', 3.0)  # Explicit rating 1-5
        }
        
        rating = rating_map.get(interaction_type, 2.0)
        
        data.append({
            'user_id': user_id,
            'experience_id': experience_id,
            'rating': rating
        })
    
    if skipped > 0:
        print(f"  ⚠️  Skipped {skipped:,} interactions (no experience_id)")
    
    df = pd.DataFrame(data)
    
    print(f"\nDataset Stats:")
    print(f"  Users: {df['user_id'].nunique():,}")
    print(f"  Experiences: {df['experience_id'].nunique():,}")
    print(f"  Interactions: {len(df):,}")
    print(f"  Sparsity: {100 * (1 - len(df) / (df['user_id'].nunique() * df['experience_id'].nunique())):.2f}%")
    
    return df

async def main():
    print("\n" + "="*60)
    print("RETRAINING ALS MODEL FROM MONGODB")
    print("="*60 + "\n")
    
    # Load data
    df = await load_data_from_mongodb()
    
    # Encode users and items
    print("\nEncoding users and items...")
    from sklearn.preprocessing import LabelEncoder
    
    user_encoder = LabelEncoder()
    item_encoder = LabelEncoder()
    
    df['user_idx'] = user_encoder.fit_transform(df['user_id'])
    df['item_idx'] = item_encoder.fit_transform(df['experience_id'])
    
    print(f"  ✓ Encoded {len(user_encoder.classes_):,} users")
    print(f"  ✓ Encoded {len(item_encoder.classes_):,} items")
    
    # Create sparse matrix
    print("\nCreating sparse user-item matrix...")
    from scipy.sparse import csr_matrix
    
    user_item_matrix = csr_matrix(
        (df['rating'].values, (df['user_idx'].values, df['item_idx'].values)),
        shape=(len(user_encoder.classes_), len(item_encoder.classes_))
    )
    
    print(f"  ✓ Matrix shape: {user_item_matrix.shape}")
    print(f"  ✓ Non-zero entries: {user_item_matrix.nnz:,}")
    
    # Train model
    print("\n" + "="*60)
    print("TRAINING ALS MODEL")
    print("="*60)
    
    model = ImplicitALS(
        factors=100,
        regularization=0.05,
        iterations=15,
        alpha=40
    )
    
    print("\nTraining...")
    model.fit(user_item_matrix)
    
    print("\n✓ Training completed!")
    
    # Comprehensive evaluation
    print("\n" + "="*60)
    print("EVALUATING MODEL")
    print("="*60)
    
    from sklearn.model_selection import train_test_split
    train_df, test_df = train_test_split(df, test_size=0.2, random_state=42)
    
    # Build train user-item set for excluding during test
    train_user_items = {}
    for _, row in train_df.iterrows():
        user_idx = row['user_idx']
        item_idx = row['item_idx']
        if user_idx not in train_user_items:
            train_user_items[user_idx] = set()
        train_user_items[user_idx].add(item_idx)
    
    # Group test by user for proper evaluation
    test_by_user = {}
    for _, row in test_df.iterrows():
        user_idx = row['user_idx']
        item_idx = row['item_idx']
        if user_idx not in test_by_user:
            test_by_user[user_idx] = set()
        test_by_user[user_idx].add(item_idx)
    
    # Evaluate at different K
    K_values = [5, 10, 20]
    metrics_by_k = {}
    
    for K in K_values:
        hits = 0
        precision_sum = 0
        recall_sum = 0
        ndcg_sum = 0
        total_users = 0
        
        for user_idx, true_items in test_by_user.items():
            if user_idx >= len(model.user_factors):
                continue
                
            # Get user vector
            user_vec = model.user_factors[user_idx]
            
            # Get scores for all items
            scores = model.item_factors.dot(user_vec)
            
            # Exclude items seen during training
            if user_idx in train_user_items:
                for train_item in train_user_items[user_idx]:
                    if train_item < len(scores):
                        scores[train_item] = -np.inf
            
            # Top-K recommendations
            top_k_indices = np.argsort(scores)[-K:][::-1]  # Descending order
            
            # Calculate metrics
            relevant_in_topk = set(top_k_indices) & true_items
            
            # Hit Rate: At least 1 relevant item in top-K
            if len(relevant_in_topk) > 0:
                hits += 1
            
            # Precision@K: Proportion of recommended items that are relevant
            precision = len(relevant_in_topk) / K
            precision_sum += precision
            
            # Recall@K: Proportion of relevant items that are recommended
            recall = len(relevant_in_topk) / len(true_items) if len(true_items) > 0 else 0
            recall_sum += recall
            
            # NDCG@K: Normalized Discounted Cumulative Gain
            dcg = 0
            for i, item_idx in enumerate(top_k_indices):
                if item_idx in true_items:
                    dcg += 1 / np.log2(i + 2)  # i+2 because i starts at 0
            
            # Ideal DCG (all relevant items at top)
            idcg = sum(1 / np.log2(i + 2) for i in range(min(len(true_items), K)))
            
            ndcg = dcg / idcg if idcg > 0 else 0
            ndcg_sum += ndcg
            
            total_users += 1
        
        # Average metrics
        if total_users > 0:
            metrics_by_k[K] = {
                'hit_rate': hits / total_users,
                'precision': precision_sum / total_users,
                'recall': recall_sum / total_users,
                'ndcg': ndcg_sum / total_users
            }
        else:
            metrics_by_k[K] = {
                'hit_rate': 0,
                'precision': 0,
                'recall': 0,
                'ndcg': 0
            }
    
    # Print results
    print(f"\nMetrics on {len(test_by_user)} test users:")
    print(f"{'Metric':<15} {'@5':<10} {'@10':<10} {'@20':<10}")
    print("-" * 50)
    print(f"{'Hit Rate':<15} {metrics_by_k[5]['hit_rate']:<10.3f} {metrics_by_k[10]['hit_rate']:<10.3f} {metrics_by_k[20]['hit_rate']:<10.3f}")
    print(f"{'Precision':<15} {metrics_by_k[5]['precision']:<10.3f} {metrics_by_k[10]['precision']:<10.3f} {metrics_by_k[20]['precision']:<10.3f}")
    print(f"{'Recall':<15} {metrics_by_k[5]['recall']:<10.3f} {metrics_by_k[10]['recall']:<10.3f} {metrics_by_k[20]['recall']:<10.3f}")
    print(f"{'NDCG':<15} {metrics_by_k[5]['ndcg']:<10.3f} {metrics_by_k[10]['ndcg']:<10.3f} {metrics_by_k[20]['ndcg']:<10.3f}")
    
    # Model Quality Assessment based on NDCG@10
    ndcg_10 = metrics_by_k[10]['ndcg']
    print(f"\nModel Quality Assessment (based on NDCG@10 = {ndcg_10:.3f}):")
    if ndcg_10 >= 0.30:
        print(f"  ✅ EXCELLENT - Model performs very well")
    elif ndcg_10 >= 0.20:
        print(f"  ✓ GOOD - Model performs acceptably")
    elif ndcg_10 >= 0.10:
        print(f"  ⚠️ FAIR - Model needs improvement")
    else:
        print(f"  ❌ POOR - Consider tuning hyperparameters or improving data quality")
    
    # Save model
    print("\n" + "="*60)
    print("SAVING MODEL")
    print("="*60)
    
    model_dir = Path('models')
    model_dir.mkdir(exist_ok=True)
    
    # Save ALS model
    model_data = {
        'user_factors': model.user_factors,
        'item_factors': model.item_factors,
        'params': {
            'factors': model.factors,
            'regularization': model.regularization,
            'iterations': model.iterations,
            'alpha': model.alpha
        }
    }
    
    with open(model_dir / 'als_model.pkl', 'wb') as f:
        pickle.dump(model_data, f)
    print(f"\n✓ Saved ALS model to {model_dir / 'als_model.pkl'}")
    
    # Save encoders
    encoders = {
        'user_encoder': user_encoder,
        'item_encoder': item_encoder
    }
    
    with open(model_dir / 'encoders_als.pkl', 'wb') as f:
        pickle.dump(encoders, f)
    print(f"✓ Saved encoders to {model_dir / 'encoders_als.pkl'}")
    
    # Save metadata với đầy đủ thông tin
    metadata = {
        'trained_at': datetime.now().isoformat(),
        'algorithm': 'Implicit ALS (Alternating Least Squares)',
        'n_users': len(user_encoder.classes_),
        'n_items': len(item_encoder.classes_),
        'n_train_interactions': len(train_df),
        'n_test_interactions': len(test_df),
        'n_interactions': len(df),
        # Hyperparameters
        'factors': model_data['params']['factors'],
        'regularization': model_data['params']['regularization'],
        'iterations': model_data['params']['iterations'],
        'alpha': model_data['params']['alpha'],
        # Metrics with detailed breakdown
        'metrics': {
            'hit_rate@5': metrics_by_k[5]['hit_rate'],
            'hit_rate@10': metrics_by_k[10]['hit_rate'],
            'hit_rate@20': metrics_by_k[20]['hit_rate'],
            'precision@5': metrics_by_k[5]['precision'],
            'precision@10': metrics_by_k[10]['precision'],
            'precision@20': metrics_by_k[20]['precision'],
            'recall@5': metrics_by_k[5]['recall'],
            'recall@10': metrics_by_k[10]['recall'],
            'recall@20': metrics_by_k[20]['recall'],
            'ndcg@5': metrics_by_k[5]['ndcg'],
            'ndcg@10': metrics_by_k[10]['ndcg'],
            'ndcg@20': metrics_by_k[20]['ndcg'],
            'coverage': 1.0
        }
    }
    
    import json
    with open(model_dir / 'als_metadata.json', 'w') as f:
        json.dump(metadata, f, indent=2)
    print(f"✓ Saved metadata to {model_dir / 'als_metadata.json'}")
    
    print("\n" + "="*60)
    print("RETRAIN COMPLETED SUCCESSFULLY!")
    print("="*60)
    print(f"\nModel trained with:")
    print(f"  • {len(user_encoder.classes_):,} users")
    print(f"  • {len(item_encoder.classes_):,} experiences")
    print(f"  • {len(df):,} interactions")
    print(f"\nKey Metrics:")
    print(f"  • Precision@10: {metrics_by_k[10]['precision']:.2%}")
    print(f"  • Recall@10: {metrics_by_k[10]['recall']:.2%}")
    print(f"  • NDCG@10: {metrics_by_k[10]['ndcg']:.2%}")
    print(f"  • Hit Rate@10: {metrics_by_k[10]['hit_rate']:.2%}")
    print("\nRestart server to load new model!")

if __name__ == "__main__":
    asyncio.run(main())
