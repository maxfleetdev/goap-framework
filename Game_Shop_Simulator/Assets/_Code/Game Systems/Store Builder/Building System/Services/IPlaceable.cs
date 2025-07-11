/// <summary>
/// Interface for any objects which can be built using BuildManager
/// </summary>
public interface IPlaceable
{
    /// <summary>
    /// Called when this prefab has been placed by BuildManager
    /// </summary>
    public void OnPlaced();
    /// <summary>
    /// Called when this prefab has been removed by BuildManager
    /// </summary>
    public void OnRemoved();
}