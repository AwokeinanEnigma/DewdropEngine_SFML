using DewDrop.Utilities;

namespace DewDrop.Entities;

/// <summary>
///     An entity is a non-static object within the game world that is capable of being updated.
/// </summary>
public abstract class Entity : IDisposable
{
    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }
    
    protected Vector2 _position;
    protected bool _disposed;

    /// <summary>
    ///     The name of the entity. Used for name lookup
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    ///     This is your constructor. You can do whatever you want with it.
    /// </summary>
    public Entity()
    {

    }

    /// <summary>
    ///     This is called when the entity is added to the entity manager and directly before it's updated.
    /// </summary>
    public virtual void Awake()
    {
    }

    /// <summary>
    ///     This is called every frame.
    /// </summary>
    public virtual void Update()
    {
    }

    /// <summary>
    ///     This is called when the entity is removed from the entity manager.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Dispose.
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }
}