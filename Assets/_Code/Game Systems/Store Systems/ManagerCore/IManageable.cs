using System.Threading.Tasks;

public interface IManageable
{
    /// <summary>
    /// Called by ManagerInitialiser on Awake
    /// </summary>
    void Initialise();
    /// <summary>
    /// Called by ManagerInitialiser at OnDestroy
    /// </summary>
    void Terminate();
}

public interface IManageableAsync
{
    /// <summary>
    /// Called by ManagerInitialiser on Awake, calls an async task (StockDatabase etc)
    /// </summary>
    /// <returns>Async Task</returns>
    Task InitialiseAsync();
    /// <summary>
    /// Called by ManagerInitialiser at OnDestroy
    /// </summary>
    void Terminate();
}