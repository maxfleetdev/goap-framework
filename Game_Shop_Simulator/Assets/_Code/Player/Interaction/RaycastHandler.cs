using System;

public static class RaycastHandler
{
    public static event Func<IInteractable> OnRaycast;

    /// <summary>
    /// Raycasts in players direction and returns hit IInteractable interface
    /// </summary>
    /// <returns></returns>
    public static IInteractable GetRaycast()
    {
        if (OnRaycast != null)
        {
            foreach (var request in OnRaycast.GetInvocationList())
            {
                if (request is Func<IInteractable> handler)
                {
                    return handler.Invoke();
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Raycasts in players direction and returns hit ShelfRow
    /// </summary>
    /// <returns></returns>
    public static ShelfRow GetShelfRow()
    {
        IInteractable obj = GetRaycast();
        if (obj is ShelfRow shelf)
        {
            return shelf;
        }
        return null;
    }
}