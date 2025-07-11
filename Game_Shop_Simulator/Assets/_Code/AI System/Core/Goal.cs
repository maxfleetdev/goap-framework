using System.Collections.Generic;
using System.Threading.Tasks;

namespace GSS.AI
{
    public abstract class Goal
    {
        protected Queue<TaskBase> mainTasks = new();
        protected List<TaskBase> retryTasks = new();
        protected AIAgent agent;

        /// <summary>
        /// Plan and enqueue the initial task sequence.
        /// Called once when the goal is assigned.
        /// </summary>
        public abstract void PlanTasks(AIAgent agent);

        /// <summary>
        /// Executes all phases: collect, retry, commit.
        /// </summary>
        public async Task ExecuteAsync(AIAgent agent)
        {
            this.agent = agent;

            agent.Log($"[Goal Start] {GetType().Name}");

            // --- Phase 1: Collection (Main Tasks) ---
            while (mainTasks.Count > 0)
            {
                var task = mainTasks.Dequeue();

                // Wait for this composite task result
                bool success = await task.ExecuteWithFallbackAsync(agent);
                if (!success)
                {
                    // Check if composite task was critical
                    if (task.IsCritical)
                    {
                        // It was, therefore goal resulted in a failure
                        agent.Warn($"[Goal Failed] Critical task failed: {task.TaskName}");
                        OnGoalFailed(agent);
                        return;
                    }

                    // Try again in retry phase
                    retryTasks.Add(task);
                    agent.Log($"[Retry Scheduled] {task.TaskName}");
                }
            }

            // --- Phase 2: Retry (Non-Critical Tasks) ---
            foreach (var retry in retryTasks)
            {
                // Wait for retry of composite task
                var retry_clone = retry.Clone();
                bool retry_success = await retry_clone.ExecuteWithFallbackAsync(agent);
                if (!retry_success)
                {
                    // Retry failed again, but goal carries on
                    agent.Log($"[Retry Failed] {retry.TaskName}");
                }
            }

            // --- Phase 3: Commit (Condition-based final tasks) ---
            if (!ShouldCommit(agent))
            {
                // Checks whether agent can commit (has enough items, time requirement etc)
                agent.Log($"[Goal Failed] Commit phase skipped: condition not met.");
                OnGoalFailed(agent);
                return;
            }

            foreach (var commit_task in CommitTasks(agent))
            {
                // Wait for composite task result
                bool result = await commit_task.ExecuteWithFallbackAsync(agent);
                if (!result && commit_task.IsCritical)
                {
                    // Commit composite task was critical, goal has failed
                    agent.Log($"[Goal Failed] Commit task failed: {commit_task.TaskName}");
                    OnGoalFailed(agent);
                    return;
                }
            }

            agent.Log($"[Goal Complete] {GetType().Name}");
            OnGoalComplete(agent);
        }

        /// <summary>
        /// Called if all tasks succeed.
        /// </summary>
        protected virtual void OnGoalComplete(AIAgent agent) { }

        /// <summary>
        /// Called if any critical task fails or commit condition isn't met.
        /// </summary>
        protected virtual void OnGoalFailed(AIAgent agent) { }

        /// <summary>
        /// Defines whether to run commit tasks (e.g., only checkout if holding items).
        /// </summary>
        protected virtual bool ShouldCommit(AIAgent agent) => true;

        /// <summary>
        /// Returns final commit phase tasks (like checkout, leave).
        /// </summary>
        protected virtual IEnumerable<TaskBase> CommitTasks(AIAgent agent) => new List<TaskBase>();
    }
}