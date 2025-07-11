using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GSS.AI;


[RequireComponent(typeof(CheckoutQueue))]
public class CheckoutObject : MonoBehaviour, ICheckout
{
    [SerializeField] private InputData inputData;

    // StockID (key), Amount (value)
    private Dictionary<int, int> checkoutItems = new();
    
    private CheckoutQueue checkoutQueue;
    private AIAgent currentCustomer = null;

    #region Initialise

    private void OnEnable()
    {
        checkoutQueue = GetComponent<CheckoutQueue>();
        RegisterCheckout();
        inputData.OnPlaceEvent += NextCustomer;
        checkoutQueue.OnAgentToServe += CustomerToServe;
    }

    private void OnDisable()
    {
        UnregisterCheckout();
        inputData.OnPlaceEvent -= NextCustomer;
        checkoutQueue.OnAgentToServe -= CustomerToServe;
    }

    public void RegisterCheckout()
    {
        CheckoutDatabase.RegisterCheckout(this);
    }

    public void UnregisterCheckout()
    {
        CheckoutDatabase.UnregisterCheckout(this);
    }

    #endregion

    #region Checkout Logic

    /// <summary>
    /// Called on a sale completion call
    /// </summary>
    private async void NextCustomer()
    {
        currentCustomer = checkoutQueue.CurrentCustomer();
        if (currentCustomer == null)
        {
            return;
        }

        foreach (var item in currentCustomer.Inventory)
        {
            if (item == null)
            {
                continue;
            }
            
            StockData obj = await StockDataRegistry.GetStockDataAsync(item.StockID);
            if (obj == null)
            {
                continue;
            }

            float price = StorePricingEvents.GetPrice(item.StockID);
            Debug.Log($"[Checkout] Scanned: {obj.StockName} @ ${price}");
            StoreFinanceEvents.SellItem(price);
        }

        checkoutQueue.DequeueAgent();
    }

    /// <summary>
    /// Called when customer is first in line
    /// </summary>
    /// <param name="customer"></param>
    private void CustomerToServe(AIAgent customer)
    {
        foreach (var item in customer.Inventory)
        {
            if (item == null)
            {
                continue;
            }

            if (checkoutItems.ContainsKey(item.StockID))
            {
                checkoutItems[item.StockID] += item.Amount;
                continue;
            }

            checkoutItems.Add(item.StockID, item.Amount);
        }
    }

    /// <summary>
    /// Adds a given agent to this checkouts queue
    /// </summary>
    /// <param name="agent"></param>
    /// <returns>Task Completion Source (boolean)</returns>
    public Task<bool> AddToQueueAsync(AIAgent agent)
    {
        return checkoutQueue.StartQueuingAsync(agent);
    }

    #endregion

    #region Checkout Queries

    public Vector3 GetCheckoutPosition()
    {
        return transform.position;
    }

    public bool HasOpenSlot()
    {
        return checkoutQueue.HasOpenSlot();
    }

    #endregion
}