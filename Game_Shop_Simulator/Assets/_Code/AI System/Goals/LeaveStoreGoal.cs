namespace GSS.AI
{
    public class LeaveStoreGoal : Goal
    {
        public override void PlanTasks(AIAgent agent)
        {
            mainTasks.Enqueue(new LeaveTask());
        }

        protected override void OnGoalFailed(AIAgent agent)
        {
            agent.Warn("Goal Failed");
        }

        protected override void OnGoalComplete(AIAgent agent)
        {
            agent.DespawnAgent();
        }
    }
}
