using System.Threading.Tasks;

namespace GSS.AI
{
    /// <summary>
    /// Agent browses indefinitely until a spot in the checkout queue opens. Succeeds once a spot opens.
    /// </summary>
    public class WaitUntilQueueOpensTask : TaskBase
    {
        private ICheckout targetCheckout;
        private CheckoutObject checkoutObject;

        public override async Task<bool> ExecuteAsync(AIAgent agent)
        {
            // Nothing to queue for
            if (agent.Inventory.Count == 0)
            {
                return false;
            }

            // Get checkout interface
            targetCheckout = CheckoutDatabase.FindNearestCheckout(agent.transform.position);
            if (targetCheckout is not CheckoutObject checkout)
            {
                return false;
            }

            // Get checkout object
            checkoutObject = checkout;
            var queue = checkoutObject.GetComponent<CheckoutQueue>();
            if (queue == null)
            {
                return false;
            }

            // Main waiting loop
            while (!queue.HasOpenSlot())
            {
                await new RandomBrowseTask(6000).ExecuteAsync(agent);
            }

            return true;
        }

        public override TaskBase Clone()
        {
            return new WaitUntilQueueOpensTask();
        }
    }
}