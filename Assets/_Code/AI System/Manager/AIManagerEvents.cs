using System;

public static class AIManagerEvents
{
    public static event Action OnUpdateNavmesh;

    public static void UpdateNavmesh()
    {
        OnUpdateNavmesh?.Invoke();
    }
}