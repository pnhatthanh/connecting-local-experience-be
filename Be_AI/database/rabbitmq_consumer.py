import json
import logging
import asyncio
from datetime import datetime
from typing import Dict, Any
import aio_pika
from aio_pika import connect_robust, Message, IncomingMessage, ExchangeType

from config import settings
from database import get_database

logger = logging.getLogger(__name__)


class RabbitMQConsumer:
    def __init__(self):
        self.connection = None
        self.channel = None
        self.exchange = None
        self.queue = None
    
    async def connect(self):
        rabbitmq_url = (
            f"amqp://{settings.RABBITMQ_USER}:{settings.RABBITMQ_PASSWORD}"
            f"@{settings.RABBITMQ_HOST}:{settings.RABBITMQ_PORT}/"
        )
        
        max_retries = 5
        retry_delay = 2  # Start with 2 seconds
        
        for attempt in range(max_retries):
            try:
                logger.info(f"Connecting to RabbitMQ at {settings.RABBITMQ_HOST}:{settings.RABBITMQ_PORT}... (attempt {attempt + 1}/{max_retries})")
                
                self.connection = await connect_robust(rabbitmq_url)
                self.channel = await self.connection.channel()
                break 
                
            except Exception as e:
                if attempt < max_retries - 1:
                    wait_time = retry_delay * (2 ** attempt)  # Exponential backoff
                    logger.warning(f"Failed to connect to RabbitMQ (attempt {attempt + 1}/{max_retries}): {e}. Retrying in {wait_time}s...")
                    await asyncio.sleep(wait_time)
                else:
                    logger.error(f"Failed to connect to RabbitMQ after {max_retries} attempts: {e}")
                    raise
        
        try:
            
            # Set prefetch count for load balancing
            await self.channel.set_qos(prefetch_count=10)
            
            # Declare exchange (must match C# configuration - using DIRECT type)
            self.exchange = await self.channel.declare_exchange(
                settings.RABBITMQ_EXCHANGE,
                ExchangeType.DIRECT,
                durable=True
            )
            
            self.queue = await self.channel.declare_queue(
                settings.RABBITMQ_QUEUE,
                durable=True,
                arguments={
                    "x-message-ttl": 86400000,  # 24 hours
                    "x-max-length": 100000  # Max 100k messages
                }
            )
            
            # Bind queue to exchange with routing keys
            # C# uses event class name as routing key: @event.GetType().Name
            routing_keys = [
                "UserRatedExperienceEvent",
                "UserSearchedEvent",
                "UserViewedEvent",
                "ExperienceAddedToWishlistEvent",
                "BookingConfirmedEvent",
                "BookingCompletedEvent" 
            ]
            
            for routing_key in routing_keys:
                await self.queue.bind(self.exchange, routing_key=routing_key)
                logger.info(f"Bound queue to routing key: {routing_key}")
            
            logger.info("✓ RabbitMQ consumer connected successfully")
            
        except Exception as e:
            logger.error(f"Failed to connect to RabbitMQ: {e}")
            raise
    
    async def start_consuming(self):
        if not self.queue:
            raise RuntimeError("Not connected to RabbitMQ. Call connect() first.")
        
        async with self.queue.iterator() as queue_iter:
            async for message in queue_iter:
                async with message.process():
                    await self._handle_message(message)
    
    async def _handle_message(self, message: IncomingMessage):
        try:
            routing_key = message.routing_key
            body = json.loads(message.body.decode())
            
            logger.info(f"📨 Received message: {routing_key}")
            logger.debug(f"Message body: {body}")

            if routing_key == "UserRatedExperienceEvent":
                await self._handle_user_rated(body)
            elif routing_key == "UserSearchedEvent":
                await self._handle_user_searched(body)
            elif routing_key == "UserViewedEvent":
                await self._handle_user_viewed(body)
            elif routing_key == "ExperienceAddedToWishlistEvent":
                await self._handle_wishlist_added(body)
            elif routing_key == "BookingConfirmedEvent":
                await self._handle_booking_confirmed(body)
            elif routing_key == "BookingCompletedEvent":
                await self._handle_booking_completed(body)
            else:
                logger.warning(f"Unknown routing key: {routing_key}")
            
        except json.JSONDecodeError as e:
            logger.error(f"Failed to parse message JSON: {e}")
        except Exception as e:
            logger.error(f"Error handling message: {e}", exc_info=True)
    
    
    async def _handle_user_rated(self, data: Dict[str, Any]):
        """Handle UserRatedExperienceEvent"""
        try:
            db = get_database()
            user_id = str(data.get("userId") or data.get("UserId"))
            experience_id = str(data.get("experienceId") or data.get("ExperienceId"))
            rating = data.get("rating") or data.get("Rating")
            
            interaction_doc = {
                "user_id": user_id,
                "experience_id": experience_id,
                "interaction_type": "rating",
                "rating": rating,
                "booked": False,
                "completed": False,
                "created_at": datetime.utcnow()
            }
            
            await db["interactions"].insert_one(interaction_doc)
            
            logger.info(
                f"✓ Tracked rating: user={user_id}, "
                f"experience={experience_id}, rating={rating}"
            )
            
        except Exception as e:
            logger.error(f"Failed to handle UserRatedExperienceEvent: {e}", exc_info=True)
    
    async def _handle_user_viewed(self, data: Dict[str, Any]):
        try:
            db = get_database()
            
            user_id = str(data.get("userId") or data.get("UserId"))
            experience_id = str(data.get("experienceId") or data.get("ExperienceId"))
            
            interaction_doc = {
                "user_id": user_id,
                "experience_id": experience_id,
                "interaction_type": "view",
                "rating": None,
                "booked": False,
                "completed": False,
                "created_at": datetime.utcnow()
            }
            
            await db["interactions"].insert_one(interaction_doc)
            logger.info(f"✓ Tracked view: user={user_id}, experience={experience_id}")
            
        except Exception as e:
            logger.error(f"Failed to handle UserViewedEvent: {e}", exc_info=True)
    
    async def _handle_wishlist_added(self, data: Dict[str, Any]):
        try:
            db = get_database()
            
            user_id = str(data.get("userId") or data.get("UserId"))
            experience_id = str(data.get("experienceId") or data.get("ExperienceId"))
            
            interaction_doc = {
                "user_id": user_id,
                "experience_id": experience_id,
                "interaction_type": "wishlist",
                "rating": None,
                "booked": False,
                "completed": False,
                "created_at": datetime.utcnow()
            }
            await db["interactions"].insert_one(interaction_doc)
            logger.info(f"✓ Tracked wishlist: user={user_id}, experience={experience_id}")
            
        except Exception as e:
            logger.error(f"Failed to handle ExperienceAddedToWishlistEvent: {e}", exc_info=True)
    
    async def _handle_user_searched(self, data: Dict[str, Any]):
        try:
            db = get_database()
            
            user_id = str(data.get("userId") or data.get("UserId"))
            experience_ids = data.get("experienceIds") or data.get("ExperienceIds") or []
            experience_ids = [str(exp_id) for exp_id in experience_ids]
            interactions = []
            for exp_id in experience_ids:
                interactions.append({
                    "user_id": user_id,
                    "experience_id": exp_id,
                    "interaction_type": "search",
                    "rating": None,
                    "booked": False,
                    "completed": False,
                    "created_at": datetime.utcnow()
                })
            
            if interactions:
                await db["interactions"].insert_many(interactions)
                logger.info(
                    f"✓ Tracked search: user={user_id}, "
                    f"results={len(experience_ids)} experiences"
                )
        except Exception as e:
            logger.error(f"Failed to handle UserSearchedEvent: {e}", exc_info=True)
    
    async def _handle_booking_confirmed(self, data: Dict[str, Any]):
        try:
            db = get_database()
            
            booking_id = str(data.get("bookingId") or data.get("BookingId"))
            experience_id = str(data.get("experienceId") or data.get("ExperienceId"))
            user_id = str(data.get("userId") or data.get("UserId"))
            
            interaction_doc = {
                "user_id": user_id,
                "experience_id": experience_id,
                "booking_id": booking_id,
                "interaction_type": "booking",
                "rating": None,
                "booked": True,
                "completed": False,
                "created_at": datetime.utcnow()
            }
            
            await db["interactions"].insert_one(interaction_doc)
            logger.info(f"✓ Tracked booking: user={user_id}, booking={booking_id}, experience={experience_id}")
        except Exception as e:
            logger.error(f"Failed to handle BookingConfirmedEvent: {e}", exc_info=True)
    
    async def _handle_booking_completed(self, data: Dict[str, Any]):
        try:
            db = get_database()
            
            booking_id = str(data.get("bookingId") or data.get("BookingId"))
            experience_id = str(data.get("experienceId") or data.get("ExperienceId"))
            user_id = str(data.get("userId") or data.get("UserId"))
            
            result = await db["interactions"].update_one(
                {
                    "user_id": user_id,
                    "experience_id": experience_id,
                    "booking_id": booking_id
                },
                {
                    "$set": {
                        "completed": True,
                        "interaction_type": "completed",
                        "updated_at": datetime.utcnow()
                    }
                }
            )
            
            if result.modified_count > 0:
                logger.info(f"✓ Marked booking completed: user={user_id}, booking={booking_id}")
            else:
                interaction_doc = {
                    "user_id": user_id,
                    "experience_id": experience_id,
                    "booking_id": booking_id,
                    "interaction_type": "completed",
                    "rating": None,
                    "booked": True,
                    "completed": True,
                    "created_at": datetime.utcnow()
                }
                await db["interactions"].insert_one(interaction_doc)
                logger.info(f"✓ Created completed interaction: user={user_id}, experience={experience_id}")
            
        except Exception as e:
            logger.error(f"Failed to handle BookingCompletedEvent: {e}", exc_info=True)
    
    async def close(self):
        """Close RabbitMQ connection"""
        if self.connection and not self.connection.is_closed:
            await self.connection.close()
            logger.info("RabbitMQ connection closed")

rabbitmq_consumer = RabbitMQConsumer()
