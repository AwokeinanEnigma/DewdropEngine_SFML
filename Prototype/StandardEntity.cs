#region

using DewDrop;
using DewDrop.Entity;
using DewDrop.Graphics;
using DewDrop.Utilities;
using SFML.Graphics;

#endregion

public class ShapeEntity : RenderableEntity
{   
    public Shape Shape => _shape;
    //set => _shape = value;
    public Color OutlineColor
    {
        get => _shape.OutlineColor;
        set => _shape.OutlineColor = value;
    }

    public Color FillColor
    {
        get => _shape.FillColor;
        set => _shape.FillColor = value;
    }

    private Shape _shape;
    public ShapeEntity(Shape shape, Vector2 position, Vector2 size, Vector2 origin, int depth, Color fillColor = default, Color outlineColor = default)
    {
        Position = position;
        Size = size;
        Origin = origin;
        Depth = depth;

        // ensure that we can actually see our fucking square
        if (fillColor == default)
        {
            fillColor = Color.White;
        }

        if (outlineColor == default)
        {
            outlineColor = Color.White;
        }

        _shape = shape;
        _shape.FillColor = fillColor;
        _shape.OutlineColor = outlineColor;

        _visible = true;
        _shape.Origin = Origin;
        _shape.Position = Position;
        //_shape.Scale = Size;
        //_shape.Rotation = Rotation;

    }
    
    public override void Awake()
    {
        base.Awake();
    }

    public override void Update()
    {
        base.Update();
        _position = new Vector2(160, (_position.y + 90) * (float)MathF.Sin((2 * MathF.PI * Engine.SessionTimer.ElapsedTime.AsSeconds()) / 2));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing && _shape != null)
            {
                _shape.Dispose();
            }
        }
        _disposed = true;
    }

    public override void Draw(RenderTarget target)
    {
        _shape.Origin = Origin;
        _shape.Position = Position;
        target.Draw(_shape);
    }
}