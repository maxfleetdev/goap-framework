/// <summary>
/// Generic interface for components that provide a specific data type
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IProvidable<T>
{
    /// <summary>
    /// Returns the stored data of a specific type within a monobehaviour provider
    /// </summary>
    /// <returns></returns>
    T Get();
}