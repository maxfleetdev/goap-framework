using UnityEngine;

namespace GSS.AI
{
    public class AIManager : MonoBehaviour, IManageable
    {
        [Header("AI Prefab")]
        [SerializeField] private GameObject aiPrefab;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private AIProfileData aiProfileData;

        [Header("Spawning Timer")]
        [SerializeField] private bool spawnOnStartup;
        [SerializeField][Min(1)] private float minSpawnTime;
        [SerializeField][Min(1)] private float maxSpawnTime;
        [SerializeField] private int spawnDelayTime;

        [Header("Spawn Pool")]
        [SerializeField][Min(1)] private int maxSpawnedAI;

        private bool isSpawning = false;
        private float nextSpawnTime;
        private int spawnedCount = 0;
        
        private int aiDataID;

        #region Manager Lifespan

        public void Initialise()
        {
            spawnedCount = 0;

            AIProfileData profile = aiPrefab.GetComponent<AIProfileProvider>().ProfileData;
            if (profile == null)
            {
                Debug.LogWarning("[AIManager] ProfileData is null on assigned aiPrefab!");
                return;
            }

            aiDataID = profile.ObjectID;

            TimeEvents.OnShiftStart += StartSpawner;
            TimeEvents.OnShiftEnd += StopSpawner;
        }

        public void Terminate()
        {
            isSpawning = false;

            TimeEvents.OnShiftStart -= StartSpawner;
            TimeEvents.OnShiftEnd -= StopSpawner;
        }

        #endregion

        #region AI Spawner

        private void StartSpawner()
        {
            isSpawning = true;
        }

        private void StopSpawner()
        {
            isSpawning = false;
        }

        private void Update()
        {
            if (isSpawning)
            {
                if (ShouldSpawn())
                {
                    SpawnAI();
                }
            }
        }

        private bool ShouldSpawn()
        {
            // Next spawn time check
            if (Time.time < nextSpawnTime)
            {
                return false;
            }

            // AI Pool Count check
            if (spawnedCount + 1 > maxSpawnedAI)
            {
                return false;
            }

            // Update next spawn time with min/max value
            nextSpawnTime = Time.time + UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);
            return true;
        }

        private async void SpawnAI()
        {
            Debug.Log("Spawning AI");
            spawnedCount++;

            // Get AIAgent and Initialise
            GameObject pooled_ai = await AIPoolEvents.GetAI(aiDataID);
            CustomerAgent agent = pooled_ai.GetComponent<CustomerAgent>();

            // Get OnComplete Event to despawn
            agent.OnAgentFinished = DespawnAI;

            // Set AI Spawn Point
            pooled_ai.transform.position = spawnPosition.position;

            // start provided ai()
            agent.OnAISpawn();
        }

        private void DespawnAI(AIAgent agent)
        {
            AIPoolEvents.ReturnAI(agent.ProfileData.ObjectID, agent.gameObject);
            spawnedCount--;
        }

        #endregion
    }
}