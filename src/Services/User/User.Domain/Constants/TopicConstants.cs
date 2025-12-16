namespace User.Domain.Constants
{
    public static class TopicConstants
    {
        public static readonly string[] ValidTopics = 
        [
            "Wellness & Relaxation",
            "Food & Drink",
            "Culture & History",
            "Shopping & Markets",
            "Nature & Wildlife",
            "Art & Crafts",
            "Sports & Adventure",
            "Music & Entertainment",
            "Nightlife & Social",
            "Photography & Tours"
        ];

        public static bool IsValidTopic(string topic)
        {
            return ValidTopics.Contains(topic, StringComparer.OrdinalIgnoreCase);
        }
    }
}
