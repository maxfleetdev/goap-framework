using System.Collections.Generic;

namespace GSS.AI
{
    public class BuyItemGoal : Goal
    {
        private readonly List<int> rawItemIDs = new();

        public BuyItemGoal(List<int> item_ids)
        {
            rawItemIDs = item_ids;
        }

        public override void PlanTasks(AIAgent agent)
        {
            foreach (int id in rawItemIDs)
            {
                if (!StockDataRegistry.IsStockDataLoaded(id))
                {
                    continue;
                }
                Logger.Info($"{id}");
                var buyTask = new BuyItemTask(id) { IsCritical = false };
                mainTasks.Enqueue(buyTask);
            }
        }

        protected override bool ShouldCommit(AIAgent agent)
        {
            return agent.Inventory.Count > 0;
        }

        protected override IEnumerable<TaskBase> CommitTasks(AIAgent agent)
        {
            return new List<TaskBase>
            {
                new CheckoutTask()
            };
        }

        protected override void OnGoalFailed(AIAgent agent)
        {
            agent.Warn("Goal Failed");
            agent.StartGoal(new LeaveStoreGoal());
        }

        protected override void OnGoalComplete(AIAgent agent)
        {
            agent.DespawnAgent();
        }
    }
}
