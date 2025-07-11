namespace GSS.AI
{
    public class AIPersonality
    {
        public string Name { get; set; }
        
        public int RetryLimit { get; set; }
        public int BrowseDelayMs { get; set; }
        
        public float MoveSpeedMultiplier { get; set; }

        public bool IsImpatient => RetryLimit <= 1;

        public AIPersonality(string name, int retryLimit, int browseDelay, float moveSpeed = 1f)
        {
            Name = name;
            RetryLimit = retryLimit;
            BrowseDelayMs = browseDelay;
            MoveSpeedMultiplier = moveSpeed;
        }

        // Static presets
        public static AIPersonality Relaxed => new("Relaxed", 3, 8000, 0.8f);
        public static AIPersonality Normal => new("Normal", 2, 4000, 1f);
        public static AIPersonality Impatient => new("Impatient", 1, 0, 1.2f);
    }

    public enum AIPersonalityType
    {
        Relaxed,
        Normal,
        Impatient
    }
}