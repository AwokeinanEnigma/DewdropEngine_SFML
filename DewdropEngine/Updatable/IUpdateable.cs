namespace DewDrop.Updatable;

/// <summary>
/// Defines an interface for objects that can be updated each frame or tick.
/// </summary>
/// <remarks>
/// This interface is typically used when you have objects that aren't entities or renderables.
/// </remarks>
public interface IUpdateable
{
    int Priority { get; }
    /// <summary>
    /// Updates the state of the object.
    /// </summary>
    void Update();
}