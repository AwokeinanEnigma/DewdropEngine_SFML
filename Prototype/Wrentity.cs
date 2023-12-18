#region

using System.Runtime.CompilerServices;
using DewDrop;
using DewDrop.Collision;
using DewDrop.Entities;
using DewDrop.Graphics;
using DewDrop.Inspector;
using DewDrop.UserInput;
using DewDrop.Utilities;
using DewDrop.Wren;
using Prototype;
using SFML.Graphics;

#endregion

public class Wrentity : RenderableEntity, ICollidable   
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

    Shape _shape; 
    RenderPipeline _pipeline;
    Wreno _wreno;
    
    public AABB AABB { get; }
    public Mesh Mesh { get; }
    public bool Solid { get; set; }

    public Color Color;
    public VertexArray DebugVerts { get; set; }
    public Vector2 Velocity { get; set; }

    public void OnTriggerStay (ICollidable context) { throw new NotImplementedException(); }
    public void OnTriggerEnter (ICollidable context) { throw new NotImplementedException(); }
    public void OnTriggerExit (ICollidable context) { throw new NotImplementedException(); }
    public List<ICollidable> CollidingWith { get; }
    private Vector2 lastPosition;
    public override string Name => "Wrentity";
    public override Vector2 Position
    {
        get => _position;
        set
        {
            lastPosition = _position;
            _position = value;
        }
    }

    public CollisionManager CollisionManager { get; set; }
    public bool IsTrigger { get; set; }
    public bool IsTalking;
    public Wrentity(string script, Shape shape, Vector2 position, Vector2 size, Vector2 origin, int depth, RenderPipeline pipeline, CollisionManager collisionManager, Color fillColor = default, Color outlineColor = default)
    {
        RenderPosition = position;
        Size = size;
        Origin = origin;
        Depth = depth;
        
        Color = fillColor;
        
        IsTrigger = false;
        CollisionManager = collisionManager;
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
        _wreno = WrenManager.MakeWreno(script);
        _wreno.Run();
        _visible = true;
        _shape.Origin = Origin;
        _shape.Position = RenderPosition;
        Depth = int.MaxValue;
        Position = position;
        Mesh = new Mesh(new FloatRect(-8f, -3f, 15f, 6f));
        AABB = Mesh.AABB;
        VertexArray vertexArray = new(PrimitiveType.LineStrip, (uint)(Mesh.Vertices.Count + 1));
        for (int i = 0; i < Mesh.Vertices.Count; i++)
        {
            vertexArray[(uint)i] = new Vertex(Mesh.Vertices[i], Color.Cyan);
        }

        Solid = true;
        vertexArray[(uint)Mesh.Vertices.Count] = new Vertex(Mesh.Vertices[0], Color.Green);
        DebugVerts = vertexArray;
    }

    public override void Update()
    {
        base.Update();
        //_position += Input.Instance.Axis * 2;
        //_pipeline.ForceSort();
        ///Depth = Int32.MaxValue;
        //Console.WriteLine(_depth);
        IsTalking = WrenContext.TextBox.Visible;
        CollisionManager.Update(this, lastPosition, _position);
    }
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
    
    [Tooltip("Calls Wren code")]
    [ButtonMethod("Interact")]
    public void Interact()
    {
        //WrenContext.TextBox.Reset(File.ReadAllText("totallynormaltext.txt"), "cumler", true, true);
        //WrenContext.TextBox.Show();
        _wreno.Call("Interact");
    }
    
    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
                _shape.Dispose();
                _wreno.Dispose();
            }
        _disposed = true;
    }

    public override void Draw(RenderTarget target)
    {
        _shape.FillColor = Color;
        _shape.Origin = Origin;
        Vector2 positionCopy = _position;
        positionCopy.y -= Size.y;
        positionCopy.x -= Size.x / 2;
        _shape.Position = positionCopy;
        target.Draw(_shape);
    }
}