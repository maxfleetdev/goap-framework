using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class ManagerDatabase
{
    private static readonly List<IManageable> syncManagers = new List<IManageable>();
    private static readonly List<IManageableAsync> asyncManagers = new List<IManageableAsync>();
    
    private static bool isInitialised = false;

    #region Reset Registry

    /// <summary>
    /// Resets the registers variables before loading
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void ResetRegistry()
    {
        isInitialised = false;
        syncManagers.Clear();
        asyncManagers.Clear();
    }

    #endregion

    #region Register Managers

    /// <summary>
    /// Registers a Sync Manager
    /// </summary>
    /// <param name="manager"></param>
    public static void Register(IManageable manager)
    {
        if (!syncManagers.Contains(manager))
        {
            syncManagers.Add(manager);
            // Called if everything else is initialised
            if (isInitialised)
            {
                manager.Initialise();
            }
        }
    }

    /// <summary>
    /// Registers an Async Manager
    /// </summary>
    public static void Register(IManageableAsync manager)
    {
        if (!asyncManagers.Contains(manager))
        {
            asyncManagers.Add(manager);
            // Called if everything else is initialised
            if (isInitialised)
            {
                _ = manager.InitialiseAsync();
            }
        }
    }

    #endregion

    #region Initialise Managers

    /// <summary>
    /// Initialises all Managers within registry
    /// </summary>
    /// <returns></returns>
    public static async Task InitialiseAll()
    {
        isInitialised = true;

        // Loads all sync managers
        foreach (var manager in syncManagers)
        {
            manager.Initialise();
            Logger.Initialised(manager.GetType().Name);
        }

        // Loads all async managers
        await Task.WhenAll(asyncManagers.Select(m =>
        {
            Logger.Initialised(m.GetType().Name);
            return m.InitialiseAsync();
        }));
    }

    /// <summary>
    /// Terminates all Managers within registry
    /// </summary>
    /// <returns></returns>
    public static void TerminateAll()
    {
        // Terminate All SyncManagers
        foreach (var manager in syncManagers)
        {
            manager.Terminate();
        }
        
        // Terminate All AsyncManagers
        foreach (var manager in asyncManagers)
        {
            manager.Terminate();
        }

        Logger.Terminated(typeof(ManagerDatabase).Name, syncManagers.Count + asyncManagers.Count);

        // Resets to default
        ResetRegistry();
    }

    #endregion
}