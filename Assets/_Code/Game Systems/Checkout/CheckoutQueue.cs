using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GSS.AI;

public class CheckoutQueue : MonoBehaviour
{
    // Holds all transform positions for queuing spots
    [SerializeField] private List<Transform> queuePoints = new();

    // Holds a queue of customers
    private Queue<AIAgent> currentCustomers = new();

    // Holds the agents waiting and their task completion source
    private Dictionary<AIAgent, TaskCompletionSource<bool>> waitingAgents = new();

    public event Action<AIAgent> OnQueuePositionChanged;
    public event Action<AIAgent> OnAgentDequeued;
    public event Action<AIAgent> OnAgentToServe;

    #region Initialisation

    private void OnEnable()
    {
        waitingAgents.Clear();
        currentCustomers.Clear();
    }

    #endregion

    #region Queuing Logic

    /// <summary>
    /// Return a task completion source on queuing success or failure
    /// </summary>
    /// <param name="agent"></param>
    /// <returns>Task Completion Source (boolean)</returns>
    public async Task<bool> StartQueuingAsync(AIAgent agent)
    {
        // Agent already queued
        if (waitingAgents.ContainsKey(agent))
        {
            return false;
        }

        // Add agent to queue and update all positions
        currentCustomers.Enqueue(agent);
        waitingAgents.Add(agent, null);
        UpdateQueuePositions();

        // Agent is now waiting to leave queue
        bool success = await WaitForTurnAsync(agent);
        
        // Remove agent from dictionary
        waitingAgents.Remove(agent);
        
        // Return result to agent
        return success;
    }

    /// <summary>
    /// Set TaskCompletionSource for queued agent
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    private Task<bool> WaitForTurnAsync(AIAgent agent)
    {
        // Create new tasksource for agent
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        waitingAgents[agent] = tcs;

        // Checks if agent is first
        if (currentCustomers.Peek() == agent)
        {
            // Calls to checkout that agent is to be served
            OnAgentToServe?.Invoke(agent);
            tcs.SetResult(true);
        }

        // Return task result
        return tcs.Task;
    }

    /// <summary>
    /// Checkout has finished serving customer, call next customer
    /// </summary>
    public void DequeueAgent()
    {
        // No customers to serve
        if (currentCustomers.Count == 0)
        {
            return;
        }

        // Remove served agent from dictionary
        AIAgent agent = currentCustomers.Peek();
        
        OnAgentDequeued?.Invoke(agent);
        
        currentCustomers.Dequeue();
        waitingAgents.Remove(agent);
        
        UpdateQueuePositions();

        // Complete task for next customer (now new customer)
        if (waitingAgents.Count > 0)
        {
            AIAgent next = currentCustomers.Peek();
            if (waitingAgents.TryGetValue(next, out TaskCompletionSource<bool> tcs))
            {
                // Calls to checkout that agent is to be served
                OnAgentToServe?.Invoke(agent);
                tcs.SetResult(true);
            }
        }
    }

    /// <summary>
    /// Updates all waiting agents' target positions for updating AIMovement
    /// </summary>
    private void UpdateQueuePositions()
    {
        int i = 0;
        foreach (var agent in currentCustomers)
        {
            if (i < queuePoints.Count)
            {
                agent.CurrentTargetPosition = queuePoints[i].position;
                OnQueuePositionChanged?.Invoke(agent);
            }
            i++;
        }
    }

    /// <summary>
    /// Gets the first customer in queue
    /// </summary>
    /// <returns>First AIAgent in Queue</returns>
    public AIAgent CurrentCustomer()
    {
        if (currentCustomers.Count == 0)
        {
            return null;
        }
        return currentCustomers.Peek();
    }

    public bool HasOpenSlot()
    {
        return currentCustomers.Count < queuePoints.Count;
    }

    #endregion

    #region Gizmos
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (queuePoints.Count == 0)
        {
            return;
        }

        foreach (Transform pos in queuePoints)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pos.position, 0.15f);
        }
    }

#endif
    #endregion
}