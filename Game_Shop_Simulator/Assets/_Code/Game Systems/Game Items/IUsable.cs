/// <summary>
/// Generic Interface which handles any usable items. Dynamically passes InputData for event subscribing alongside UseItem
/// </summary>
public interface IUsable
{
    /// <summary>
    /// Subscribe to all required Input and cache provided InputData here
    /// </summary>
    public void SubscribeToInput(InputData input);
    /// <summary>
    /// Unsubscribe all Input from the given InputData here
    /// </summary>
    public void UnsubscribeFromInput();
    /// <summary>
    /// Called when a specified Input Event is called
    /// </summary>
    public void UseItem();
}