namespace GSS.AI
{
    public class LeaveTask : CompositeTask
    {
        public LeaveTask() : base("Leave")
        {
            subTasks.Enqueue(new LeaveStoreTask());
        }

        public override TaskBase Clone()
        {
            return new LeaveTask();
        }
    }
}