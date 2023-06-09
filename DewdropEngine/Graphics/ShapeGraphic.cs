using DewDrop.Utilities;
using SFML.Graphics;

namespace DewDrop.Graphics;

public class ShapeGraphic : Renderable
{
    
    public Shape Shape
    {
        get => _shape;
        //set => _shape = value;
    }

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
    
    public ShapeGraphic(Shape shape, Vector2 position, Vector2 size, Vector2 origin, int depth, Color fillColor = default , Color outlineColor = default)
    {
        Position = position;
        Size = size;
        Origin = origin;
        Depth = depth;
        
        _shape = shape;
        _shape.FillColor = fillColor;
        _shape.OutlineColor = outlineColor;
        
        _shape.Origin = Origin;
        _shape.Position = Position;
        //_shape.Scale = Size;
        //_shape.Rotation = Rotation;
        
    }

    public override void Draw(RenderTarget target)
    {
        _shape.Origin = Origin;
        _shape.Position = Position;
        target.Draw(_shape);
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
}