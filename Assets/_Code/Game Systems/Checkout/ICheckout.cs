using UnityEngine;

public interface ICheckout
{
    public void RegisterCheckout();
    public void UnregisterCheckout();
    public Vector3 GetCheckoutPosition();
}