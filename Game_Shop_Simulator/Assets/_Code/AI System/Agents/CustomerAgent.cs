using UnityEngine;
using System.Collections.Generic;

namespace GSS.AI
{
    [RequireComponent(typeof(AIProfileProvider))]
    public class CustomerAgent : AIAgent
    {
        [SerializeField] private AIProfileData profile;
        [SerializeField] private bool debugLogs = true;
        [SerializeField] private List<int> itemsToBuy = new();

        // Called by AI Manager when spawned
        public void OnAISpawn()
        {
            printDebug = debugLogs;
            controller = GetComponent<AIController>();

            Initialise(profile);
            StartGoal(new BuyItemGoal(itemsToBuy));
        }
    }
}