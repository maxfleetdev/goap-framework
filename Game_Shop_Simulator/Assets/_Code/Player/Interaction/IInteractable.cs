/// <summary>
/// Generic Interface used with any Monobehaviour which requires Player Raycast interaction
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called by PlayerRaycaster to interact with a given object
    /// </summary>
    public void OnInteract();
}