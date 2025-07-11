using System.Threading.Tasks;

namespace GSS.AI
{
    /// <summary>
    /// Composite Task for agent to find, move and take any given item
    /// </summary>
    public class BuyItemTask : RetryableCompositeTask
    {
        private readonly int itemID;

        public BuyItemTask(int itemID) : base($"Buy Item {itemID}")
        {
            this.itemID = itemID;

            // Manually enque subtasks
            subTasks.Enqueue(new FindItemAtShelfTask(itemID, true));
            subTasks.Enqueue(new MoveToPositionTask());
            subTasks.Enqueue(new TakeItemTask());
        }

        public override TaskBase Clone()
        {
            return new BuyItemTask(this.itemID);
        }

        protected override void ResetSubTasks()
        {
            subTasks.Clear();
            subTasks.Enqueue(new FindItemAtShelfTask(itemID, true));
            subTasks.Enqueue(new MoveToPositionTask());
            subTasks.Enqueue(new TakeItemTask());
        }

        // Override delay/retry for this composite task
        protected override Task OnRetryDelayAsync(AIAgent agent)
        {
            // Get personality
            AIPersonality personality = agent.AIPersonality;
            if (personality == null)
            {
                return Task.CompletedTask;
            }

            // Impatient Agent (retry instantly)
            if (personality.IsImpatient)
            {
                return Task.CompletedTask;
            }

            // Other agents (browse for a while)
            return new RandomBrowseTask(personality.BrowseDelayMs).ExecuteAsync(agent);
        }
    }
}
