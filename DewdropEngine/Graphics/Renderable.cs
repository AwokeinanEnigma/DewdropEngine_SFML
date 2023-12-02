#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Graphics;

/// <summary>
///     Defines a renderable and its fields. Renderables are anything that can be rendered within a game.
/// </summary>
public abstract class Renderable : IRenderable {
	#region Properties

    /// <summary>
    ///     The position of the renderable object.
    /// </summary>
    public virtual Vector2 RenderPosition {
		get => _position;
		set => _position = value;
	}

    /// <summary>
    ///     The origin of the renderable object.
    /// </summary>
    public virtual Vector2 Origin {
		get => _origin;
		set => _origin = value;
	}

    /// <summary>
    ///     The size of the renderable object.
    /// </summary>
    public virtual Vector2 Size {
		get => _size;
		set => _size = value;
	}

    /// <summary>
    ///     The depth of the renderable object.
    /// </summary>
    public virtual int Depth {
		get => _depth;
		set => _depth = value;
	}

    /// <summary>
    ///     Determines whether or not the renderable is visible. Handled by the RenderPipeline
    /// </summary>
    public virtual bool Visible {
		get => _visible;
		set => _visible = value;
	}

    /// <summary>
    ///     Rotates the renderable object.
    /// </summary>
    public virtual float Rotation {
		get => _rotation;
		set => _rotation = value;
	}
	public bool IsBeingDrawn { get; set; }

	#endregion

	#region Fields

	protected Vector2 _position;

	protected Vector2 _origin;

	protected Vector2 _size;

	protected float _rotation;

	protected int _depth;

	protected bool _visible = true;

	protected bool _disposed;

	#endregion

    /// <summary>
    ///     This is where you draw onto. The render target will handle the actual rendering
    /// </summary>
    /// <param name="target">The render target handling the actual rendering</param>
    public abstract void Draw (RenderTarget target);

	#region Disposable implementation.

	// called by the system to clean up, meaning we can only get rid of unmanaged stuff
	~Renderable () {
		// free only unmanaged resources
		Dispose(false);
	}

	/// <summary>
	///     Here, you must dispose of unmanaged and managed resources.
	/// </summary>
	/// <param name="disposing">
	///     If true, then we can get rid of managed and unmanaged resources. If false, we can only get rid
	///     of unmanaged resources.
	/// </param>
	protected virtual void Dispose (bool disposing) {
		_disposed = true;
	}

	/// <summary>
	///     Disposes of all managed and unmanaged resources.
	/// </summary>
	public void Dispose () {
		//
		Dispose(true);

		// we manually disposed, we don't need to finalize
		GC.SuppressFinalize(this);
	}

	#endregion
}
