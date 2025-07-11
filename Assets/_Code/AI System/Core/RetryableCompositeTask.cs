using System.Threading.Tasks;

namespace GSS.AI
{
    /// <summary>
    /// Subset of CompositeTask. Enables retrying of a Composite Task and delaying between retries.
    /// </summary>
    public abstract class RetryableCompositeTask : CompositeTask
    {
        private int retriesRemaining;
        private readonly int initialRetries;

        protected RetryableCompositeTask(string task_name, int retry_limit = 2) : base(task_name)
        {
            initialRetries = retry_limit;
            retriesRemaining = retry_limit;
        }

        public override async Task<bool> ExecuteAsync(AIAgent agent)
        {
            do
            {
                // Rebuild composite's subtasks
                ResetSubTasks();

                agent.Log($"[{TaskName}] Attempting task. Retries left: {retriesRemaining}");

                // Does composite task succeed again?
                bool success = await base.ExecuteAsync(agent);
                if (success)
                {
                    // Yes, continue to next task in goal
                    return true;
                }
                
                // No, try delaying and retrying composite task
                retriesRemaining--;

                // Retry with delay (browsing/idle/wait etc)
                if (retriesRemaining > 0)
                {
                    await OnRetryDelayAsync(agent);
                }
                    
            } while (retriesRemaining > 0);

            // All retried delayed tasks have failed
            agent.Warn($"[{TaskName}] Retries exhausted.");
            return false;
        }

        protected virtual Task OnRetryDelayAsync(AIAgent agent)
        {
            return Task.CompletedTask;
        }

        protected abstract void ResetSubTasks();
    }
}