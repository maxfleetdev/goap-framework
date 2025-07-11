using System;
using System.Threading.Tasks;
using UnityEngine;

public static class AIPoolEvents
{
    public static event Action<int, GameObject> OnReturnToPool;
    public static event Func<int, Task<GameObject>> OnGetAI;

    /// <summary>
    /// Instantiates or Enables a pooled AI prefab with a given AI ID
    /// </summary>
    /// <param name="aiID"></param>
    /// <returns>AI Prefab</returns>
    public static async Task<GameObject> GetAI(int aiID)
    {
        if (OnGetAI != null)
        {
            return await OnGetAI.Invoke(aiID);
        }
        return null;
    }

    /// <summary>
    /// Enqueues a given object for future use
    /// </summary>
    /// <param name="aiID"></param>
    /// <param name="obj"></param>
    public static void ReturnAI(int aiID, GameObject obj)
    {
        OnReturnToPool?.Invoke(aiID, obj);
    }
}