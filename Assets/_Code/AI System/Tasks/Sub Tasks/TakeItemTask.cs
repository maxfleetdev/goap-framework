using System.Threading.Tasks;

namespace GSS.AI
{
    public class TakeItemTask : TaskBase
    {
        public TakeItemTask()
        {
            TaskName = "Take Item Task";
        }

        public override async Task<bool> ExecuteAsync(AIAgent agent)
        {
            // Make function async
            await Task.Yield();

            // The item we want to take
            int requested_id = agent.CurrentItemTarget;

            // Release agents browsing spot
            agent.CurrentTargetShelf.ReleaseBrowsePoint(agent);

            // Conditionals for task
            bool can_take = agent.CurrentTargetShelf.TryTakeItem(requested_id);
            if (!can_take)
            {
                return false;
            }

            // Now only add to inventory **if** the shelf gave the item
            bool added = agent.TryAddToInventory(requested_id);
            if (!added)
            {
                Logger.Info($"[AI] Agent {agent.name} tried to add item {requested_id} to inventory but failed.");
            }
            return added;
        }

        public override TaskBase Clone()
        {
            return new TakeItemTask();
        }
    }
}