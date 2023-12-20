#region

using DewDrop.Graphics;
using DewDrop.Utilities;
using SFML.Graphics;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

#endregion
namespace DewDrop.Entities;

/// <summary>
///     A renderable entity is an entity that is updated and drawn as well.
/// </summary>
public abstract class RenderableEntity : Entity, IRenderable {
	// ReSharper disable once UnassignedGetOnlyAutoProperty
	public override string Name { get; }

	#region Properties

	public virtual Vector2 RenderPosition { get; set; }

    /// <summary>
    ///     The position of the renderable object.
    /// </summary>
    public override Vector2 Position {
		get => _position;
		set {
			_position = value;
			RenderPosition = value;
		}
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
    ///     Determines whether or not the renderable is visible.
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

	protected Vector2 _origin;

	protected Vector2 _size;

	protected float _rotation;

	protected int _depth;

	protected bool _visible = true;

	protected bool _wasDrawn;
	
	public bool DrawRegardlessOfVisibility { get; set; }
	#endregion
	/// <summary>
	/// Updates the renderable entity.
	/// </summary>
	public override void Update () {
		base.Update();
		if (_wasDrawn != IsBeingDrawn) {
			if (IsBeingDrawn) {
				BecomeVisible();
			} else {
				BecomeInvisible();
			}
		}
		_wasDrawn = IsBeingDrawn;

		// sync the render position with the entity position
		RenderPosition = _position;
	}
	/// <summary>
	/// Called when the renderable entity becomes visible.
	/// </summary>
	protected virtual void BecomeVisible () {
		//IsBeingDrawn = true;
	}
	
	/// <summary>
	/// Called when the renderable entity becomes invisible.
	/// </summary>
	protected virtual void BecomeInvisible () {
		//IsBeingDrawn = false;
	}
	
	/// <summary>
	/// Draws the renderable entity.
	/// </summary>
	/// <param name="target">The render target on which to draw.</param>
	public abstract void Draw (RenderTarget target);
}
