using System;
using System.Threading.Tasks;
using UnityEngine;

public static class BuildPoolEvents
{
    public static event Func<int, Task<GameObject>> OnGetBuildable;
    public static event Action<int, GameObject> OnReturnBuildable;

    /// <summary>
    /// Enable or Create Prefab with given ID - returns created/enabled object
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Task<GameObject> GetBuildable(int id)
    {
        return OnGetBuildable?.Invoke(id);
    }

    /// <summary>
    /// Disables a given object with ID within a pool
    /// </summary>
    /// <param name="id"></param>
    /// <param name="obj"></param>
    public static void ReturnBuildable(int id, GameObject obj)
    { 
        OnReturnBuildable?.Invoke(id, obj);
    }
}