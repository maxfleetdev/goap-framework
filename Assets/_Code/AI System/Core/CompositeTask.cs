using System.Collections.Generic;
using System.Threading.Tasks;

namespace GSS.AI
{
    public abstract class CompositeTask : TaskBase
    {
        // Holds all atmoic tasks which will execute in order
        protected Queue<TaskBase> subTasks = new();

        public CompositeTask(string task_name)
        {
            TaskName = task_name;
        }

        /// <summary>
        /// Executes each subtask in sequence with its own fallback logic.
        /// If any subtask fails, the entire composite task fails.
        /// </summary>
        public override async Task<bool> ExecuteAsync(AIAgent agent)
        {
            agent.Log($"[Composite Start] {TaskName}");

            // Execute all atomic tasks within subtasks
            while (subTasks.Count > 0)
            {
                var current = subTasks.Dequeue();
                agent.Log($"[Subtask Start] {current.TaskName}");

                // Wait for subtasks' result
                bool success = await current.ExecuteAsync(agent);
                if (!success)
                {
                    // Current subtask failed, this composite task returns as failed
                    agent.Warn($"[Composite Failed] Subtask failed: {current.TaskName}");
                    return false;
                }

                // This atomic task finished successfully
                agent.Log($"[Subtask Success] {current.TaskName}");
            }

            // Entire composite task finished
            agent.Log($"[Composite Success] {TaskName}");
            return true;
        }
    }
}