using System.Collections.Generic;
using System.Threading.Tasks;

namespace GSS.AI
{
    public abstract class TaskBase
    {
        /// <summary>
        /// For debugging purposes
        /// </summary>
        public string TaskName = "Unnamed Task";

        /// <summary>
        /// If true, the entire goal will fail
        /// </summary>
        public bool IsCritical = true;

        /// <summary>
        /// Static tasks to preform on task failure
        /// </summary>
        public List<TaskBase> FallbackTasks { get; } = new();

        /// <summary>
        /// Main execution entry for this task
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public abstract Task<bool> ExecuteAsync(AIAgent agent);

        /// <summary>
        /// Executes the task with fallback logic: 
        /// 1) Tries main task 
        /// 2) Tries static fallbacks 
        /// 3) Tries dynamic fallback
        /// </summary>
        public virtual async Task<bool> ExecuteWithFallbackAsync(AIAgent agent)
        {
            // 1. Try the main task again
            bool success = await ExecuteAsync(agent);
            if (success)
                return true;

            // 2. Try each defined static fallback (if added)
            foreach (var fallback in FallbackTasks)
            {
                success = await fallback.ExecuteAsync(agent);
                if (success)
                {
                    agent.Log($"[Composite Task] Fallback succeeded: {fallback.TaskName}");
                    return true;
                }
            }

            // 3. Try generating dynamic fallback
            var dynamicFallback = await TryGenerateDynamicFallbackAsync(agent);
            if (dynamicFallback != null)
            {
                agent.Log($"[Composite Task] Dynamic fallback triggered: {dynamicFallback.TaskName}");
                return await dynamicFallback.ExecuteAsync(agent);
            }

            // Task & Fallbacks failed: Composite task has failed to complete
            agent.Log($"[Composite Task] All fallbacks failed for: {TaskName}");
            return false;
        }

        /// <summary>
        /// Dynamically generate a fallback task based on agent, world state, or personality.
        /// </summary>
        public virtual Task<TaskBase> TryGenerateDynamicFallbackAsync(AIAgent agent)
        {
            return Task.FromResult<TaskBase>(null);
        }

        /// <summary>
        /// Returns a new Task of this type
        /// </summary>
        /// <returns></returns>
        public abstract TaskBase Clone();
    }
}