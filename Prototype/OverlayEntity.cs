#region

using System.Runtime.CompilerServices;
using DewDrop;
using DewDrop.Entities;
using DewDrop.Graphics;
using DewDrop.UserInput;
using DewDrop.Utilities;
using SFML.Graphics;

#endregion

public class ShapeEntity2 : RenderableEntity    
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
    private RenderPipeline _pipeline;
    public ShapeEntity2(Shape shape, Vector2 position, Vector2 size, Vector2 origin, int depth, RenderPipeline pipeline, Color fillColor = default, Color outlineColor = default)
    {
        RenderPosition = position;
        Size = size;
        Origin = origin;
        Depth = depth;

        _pipeline = pipeline;
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
        _shape.Position = RenderPosition;
        //_shape.Scale = Size;
        
        //_shape.Rotation = Rotation;

    }

    public override void Update()
    {
        base.Update();
        _position += AxisManager.Instance.Axis * 2;
        Depth = int.MaxValue;
        _pipeline.ForceSort();
        ///Depth = Int32.MaxValue;
        //Console.WriteLine(_depth);
    }

    public bool move = true;

    public override void BecomeVisible()
    {
        base.BecomeVisible();
        Outer.Log("We're barack.");
    }
    
    public override void BecomeInvisible()
    {
        base.BecomeInvisible();
        Outer.Log("It's joeover.");
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
        Vector2 position = _position; 
        position.y -= _shape.TextureRect.Height;
        _shape.Position = position;
        target.Draw(_shape);
    }
}