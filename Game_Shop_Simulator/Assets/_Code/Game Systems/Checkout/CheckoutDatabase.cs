using System.Collections.Generic;
using UnityEngine;

public static class CheckoutDatabase
{
    private static List<ICheckout> checkoutMap = new List<ICheckout>();

    #region Reset Registery

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void ResetRegistery()
    {
        checkoutMap.Clear();
    }

    #endregion

    #region Register Checkouts

    /// <summary>
    /// Called when shelf is first created
    /// </summary>
    /// <param name="checkout"></param>
    public static void RegisterCheckout(ICheckout checkout)
    {
        if (checkoutMap.Contains(checkout))
        {
            return;
        }
        checkoutMap.Add(checkout);
    }

    /// <summary>
    /// Called when shelf is destroyed
    /// </summary>
    /// <param name="checkout"></param>
    public static void UnregisterCheckout(ICheckout checkout)
    {
        if (!checkoutMap.Contains(checkout))
        {
            return;
        }
        checkoutMap.Remove(checkout);
    }

    #endregion

    #region Register Queries

    public static ICheckout FindNearestCheckout(Vector3 position)
    {
        float distance = Mathf.Infinity;
        ICheckout closest_checkout = null;

        foreach (var checkout in checkoutMap)
        {
            if (Vector3.Distance(position, checkout.GetCheckoutPosition()) < distance)
            {
                distance = Vector3.Distance(position, checkout.GetCheckoutPosition());
                closest_checkout = checkout;
            }
        }
        return closest_checkout;
    }

    #endregion
}
