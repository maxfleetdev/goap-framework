using System.Threading.Tasks;

namespace GSS.AI
{
    /// <summary>
    /// Composite Task which waits for agent to queue, checks out and then leaves the store
    /// </summary>
    public class CheckoutTask : RetryableCompositeTask
    {
        public CheckoutTask() : base("Checkout")
        {
            subTasks.Enqueue(new QueueForCheckoutTask());
            subTasks.Enqueue(new LeaveStoreTask());
        }

        public override TaskBase Clone()
        {
            return new CheckoutTask();
        }

        protected override void ResetSubTasks()
        {
            subTasks.Clear();
            subTasks.Enqueue(new WaitUntilQueueOpensTask());
            subTasks.Enqueue(new QueueForCheckoutTask());
            subTasks.Enqueue(new LeaveStoreTask());
        }

        protected override Task OnRetryDelayAsync(AIAgent agent)
        {
            return Task.CompletedTask;
        }
    }
}