using System.Threading.Tasks;
using UnityEngine;

namespace GSS.AI
{
    /// <summary>
    /// Agent will move towards a position in the queue, depending on their position in the queue
    /// </summary>
    public class QueueForCheckoutTask : TaskBase
    {
        private ICheckout targetCheckout;
        private CheckoutObject checkoutObject;
        private AIAgent agent;
        private bool checkoutComplete = false;

        public QueueForCheckoutTask()
        {
            TaskName = "Queue For Checkout";
        }

        public override async Task<bool> ExecuteAsync(AIAgent agent)
        {
            this.agent = agent;

            // Cannot checkout with less than 1 item
            if (agent.Inventory.Count < 1)
            {
                return false;
            }

            targetCheckout = CheckoutDatabase.FindNearestCheckout(agent.transform.position);

            if (targetCheckout is not CheckoutObject checkout)
            {
                return false;
            }

            checkoutObject = checkout;
            var queue = checkoutObject.GetComponent<CheckoutQueue>();

            // Subscribe to queue update
            queue.OnQueuePositionChanged += OnQueuePositionChanged;
            queue.OnAgentDequeued += OnAgentDequeued;

            // Queue the agent (this waits until they're first in line)
            bool queued = await checkoutObject.AddToQueueAsync(agent);

            // Wait until they're dequeued (checkout complete)
            while (!checkoutComplete)
            {
                await Task.Yield();
            }

            // Unsubscribe
            queue.OnQueuePositionChanged -= OnQueuePositionChanged;
            queue.OnAgentDequeued -= OnAgentDequeued;
            agent.CurrentTargetPosition = Vector3.zero;

            return true;
        }

        private void OnQueuePositionChanged(AIAgent changedAgent)
        {
            _ = OnQueuePositionChangedAsync(changedAgent);
        }

        private async Task OnQueuePositionChangedAsync(AIAgent changedAgent)
        {
            if (changedAgent != agent) return;

            var destination = agent.CurrentTargetPosition;
            await agent.MoveToPositionAsync(destination);
        }

        private void OnAgentDequeued(AIAgent dequeuedAgent)
        {
            if (dequeuedAgent == agent)
            {
                checkoutComplete = true;
            }
        }

        public override TaskBase Clone()
        {
            return new QueueForCheckoutTask();
        }
    }
}