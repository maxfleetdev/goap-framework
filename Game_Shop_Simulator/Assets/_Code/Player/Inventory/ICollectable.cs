public interface ICollectable
{
    /// <summary>
    /// Adds a given object to the players inventory
    /// </summary>
    public void Collect();

    /// <summary>
    /// Removes a given object from players inventory
    /// </summary>
    public void Drop();
}