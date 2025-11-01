namespace User.Domain.Constants
{
    public static class TopicConstants
    {
        public static readonly string[] ValidTopics = 
        [
            "Art & Culture",
            "Food & Drink",
            "Sports & Fitness",
            "Music & Entertainment",
            "Nature & Wildlife",
            "History & Heritage",
            "Technology & Innovation",
            "Photography",
            "Adventure & Outdoor",
            "Wellness & Meditation",
            "Shopping & Fashion",
            "Nightlife & Parties",
            "Local Cuisine",
            "Architecture",
            "Traditional Crafts",
            "Language Exchange",
            "Business & Networking",
            "Family Activities",
            "Eco-Tourism",
            "Spiritual & Religious"
        ];

        public static bool IsValidTopic(string topic)
        {
            return ValidTopics.Contains(topic, StringComparer.OrdinalIgnoreCase);
        }
    }
}
