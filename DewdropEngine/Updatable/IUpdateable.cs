namespace DewDrop.Updatable;

/// <summary>
/// Defines an interface for objects that can be updated each frame or tick.
/// </summary>
/// <remarks>
/// This interface is typically used in game development or similar applications where objects have an `Update` method that is called every frame or tick.
/// </remarks>
public interface IUpdateable
{
    int Priority { get; }
    /// <summary>
    /// Updates the state of the object.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    void Update();
}