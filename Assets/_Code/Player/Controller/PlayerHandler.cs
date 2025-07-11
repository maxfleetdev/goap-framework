using System;
using UnityEngine;

public static class PlayerHandler
{
    public static event Func<Transform> OnPlayerTransform;

    public static Transform GetPlayerTransform()
    {
        if (OnPlayerTransform != null)
        {
            foreach (var request in OnPlayerTransform.GetInvocationList())
            {
                if (request is Func<Transform> handler)
                {
                    return handler.Invoke();
                }
            }
        }
        return null;
    }
}