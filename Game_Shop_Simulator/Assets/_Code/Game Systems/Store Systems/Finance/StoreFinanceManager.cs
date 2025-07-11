using UnityEngine;

public class StoreFinanceManager : MonoBehaviour, IManageable
{
    private float storeBalance = 3000f;

    #region Initialisation and Termination

    public void Initialise()
    {
        StoreFinanceEvents.OnSellItem += AddToBalance;
        StoreFinanceEvents.OnTryPurchase += RemoveFromBalance;
        StoreFinanceEvents.BalanceUpdated(storeBalance);
    }

    public void Terminate()
    {
        StoreFinanceEvents.OnSellItem -= AddToBalance;
        StoreFinanceEvents.OnTryPurchase -= RemoveFromBalance;
    }

    #endregion

    #region Finance Logic

    private void AddToBalance(float amount)
    {
        storeBalance += amount;
        StoreFinanceEvents.BalanceUpdated(storeBalance);
    }

    private bool RemoveFromBalance(float amount)
    {
        if (storeBalance - amount < 0)
        {
            return false;
        }

        storeBalance -= amount;
        StoreFinanceEvents.BalanceUpdated(storeBalance);
        return true;
    }

    #endregion
}