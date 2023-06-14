using DewDrop.Graphics;
using DewDrop.Utilities;
using SFML.Graphics;

namespace DewDrop.Entities;

/// <summary>
/// A renderable entity is an entity that is updated and drawn as well. 
/// </summary>
public abstract class RenderableEntity : Entity, IRenderable
{
    public override string Name { get; }
    
    #region Properties
    /// <summary>
    ///     The position of the renderable object.
    /// </summary>
    public virtual Vector2 Position
    {
        get => _position;
        set => _position = value;
    }
    
    /// <summary>
    ///     The origin of the renderable object.
    /// </summary>
    public virtual Vector2 Origin
    {
        get => _origin;
        set => _origin = value;
    }

    /// <summary>
    ///     The size of the renderable object.
    /// </summary>
    public virtual Vector2 Size
    {
        get => _size;
        set => _size = value;
    }

    /// <summary>
    ///     The depth of the renderable object.
    /// </summary>
    public virtual int Depth
    {
        get => _depth;
        set => _depth = value;
    }

    /// <summary>
    ///     Determines whether or not the renderable is visible. Handled by the RenderPipeline
    /// </summary>
    public virtual bool Visible
    {
        get => _visible;
        set => _visible = value;
    }

    /// <summary>
    ///     Rotates the renderable object.
    /// </summary>
    public virtual float Rotation
    {
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
   
    #endregion

    public override void Update()
    {
        base.Update();
        if (_wasDrawn != IsBeingDrawn)
        {
            if (IsBeingDrawn)
            {
                BecomeVisible();
            }
            else
            {
                BecomeInvisible();
            }
        }
        _wasDrawn = IsBeingDrawn;
    }
    
    public virtual void BecomeVisible()
    {
        //IsBeingDrawn = true;
    }
    
    public virtual void BecomeInvisible()
    {
        //IsBeingDrawn = false;
    }

    public abstract void Draw(RenderTarget target);
}