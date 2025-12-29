import redis.asyncio as redis
from config import settings


class RedisCache:
    client: redis.Redis = None

redis_cache = RedisCache()


async def connect_to_redis():
    try:
        redis_cache.client = await redis.from_url(
            f"redis://{settings.REDIS_HOST}:{settings.REDIS_PORT}/{settings.REDIS_DB}",
            password=settings.REDIS_PASSWORD if settings.REDIS_PASSWORD else None,
            encoding="utf-8",
            decode_responses=True
        )
        await redis_cache.client.ping()
        print(f"✓ Connected to Redis: {settings.REDIS_HOST}:{settings.REDIS_PORT}")
    except Exception as e:
        print(f"✗ Could not connect to Redis: {e}")
        redis_cache.client = None


async def close_redis_connection():
    if redis_cache.client:
        await redis_cache.client.close()
        print("✓ Closed Redis connection")


def get_redis():
    return redis_cache.client


async def clear_recommendations_cache():
    """Clear all recommendation cache after model retraining (async version)"""
    try:
        if redis_cache.client:
            pattern = "recommendations:*"
            keys = await redis_cache.client.keys(pattern)
            if keys:
                await redis_cache.client.delete(*keys)
                print(f"✓ Cleared {len(keys)} cached recommendations")
                return len(keys)
            else:
                print("No cached recommendations to clear")
                return 0
        else:
            print("⚠️ Redis not connected, cannot clear cache")
            return 0
    except Exception as e:
        print(f"✗ Failed to clear cache: {e}")
        return 0


def clear_recommendations_cache_sync():
    """Clear all recommendation cache (sync version for use in threads/background tasks)"""
    import redis as sync_redis
    try:
        # Create a synchronous Redis client
        client = sync_redis.Redis(
            host=settings.REDIS_HOST,
            port=settings.REDIS_PORT,
            db=settings.REDIS_DB,
            password=settings.REDIS_PASSWORD if settings.REDIS_PASSWORD else None,
            decode_responses=True
        )
        
        pattern = "recommendations:*"
        keys = client.keys(pattern)
        if keys:
            client.delete(*keys)
            count = len(keys)
            print(f"✓ Cleared {count} cached recommendations")
            client.close()
            return count
        else:
            print("No cached recommendations to clear")
            client.close()
            return 0
    except Exception as e:
        print(f"✗ Failed to clear cache: {e}")
        return 0


async def _clear_cache_helper():
    """Helper function to clear cache in a fresh event loop"""
    try:
        if redis_cache.client:
            pattern = "recommendations:*"
            keys = await redis_cache.client.keys(pattern)
            if keys:
                await redis_cache.client.delete(*keys)
                print(f"✓ Cleared {len(keys)} cached recommendations")
                return len(keys)
            else:
                print("No cached recommendations to clear")
                return 0
        else:
            print("⚠️ Redis not connected, cannot clear cache")
            return 0
    except Exception as e:
        print(f"✗ Failed to clear cache: {e}")
        return 0
