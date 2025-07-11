using System.Threading.Tasks;
using UnityEngine;

namespace GSS.AI
{
    /// <summary>
    /// Used for debugging AI. When given this task, they will simply browse all shelves forever.
    /// </summary>
    public class RandomBrowseTask : TaskBase
    {
        private IShelf currentShelf;
        private const int browseTimeMin = 1500;
        private const int browseTimeMax = 3000;
        private int browseTime = 0;

        public RandomBrowseTask(int browse_time)
        {
            TaskName = "Browse Store";
            browseTime = browse_time;
        }

        public override async Task<bool> ExecuteAsync(AIAgent agent)
        {
            // Calculate time to loop
            var endTime = Time.time + (browseTime / 1000f);

            while (Time.time < endTime)
            {
                // Get all available shelves
                var allShelves = ShelfDatabase.GetAllShelves();
                if (allShelves.Count == 0)
                {
                    await Task.Delay(500);
                    continue;
                }

                // Define a random shelf to move towards
                currentShelf = allShelves[Random.Range(0, allShelves.Count)];
                if (!currentShelf.GetBrowsePoint(agent, out Vector3 browsePosition))
                {
                    await Task.Delay(500);
                    continue;
                }

                // Define agents position target and shelf
                agent.CurrentTargetShelf = currentShelf;
                agent.CurrentTargetPosition = browsePosition;

                // Move agent using MoveToPosition in Agent
                bool reached = await agent.MoveToPositionAsync(browsePosition);
                if (!reached)
                {
                    currentShelf.ReleaseBrowsePoint(agent);
                    continue;
                }

                // Idle at shelf for random amount of time
                int browseTime = Random.Range(browseTimeMin, browseTimeMax);
                await Task.Delay(browseTime);

                // Release shelf and repeat
                currentShelf.ReleaseBrowsePoint(agent);
            }

            // Browisng is complete
            return true;
        }

        public override TaskBase Clone()
        {
            return new RandomBrowseTask(browseTime);
        }
    }
}