#region

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using DewDrop;
using DewDrop.Collision;
using DewDrop.Entities;
using DewDrop.Graphics;
using DewDrop.UserInput;
using DewDrop.Utilities;
using SFML.Graphics;

#endregion

public class Player : RenderableEntity, ICollidable
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
    private CollisionManager _manager;
    public Player(Shape shape, Vector2 position, Vector2 size, Vector2 origin, int depth, RenderPipeline pipeline, CollisionManager manager, Color fillColor = default, Color outlineColor = default)
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
        _manager = manager;
        _visible = true;
        _shape.Origin = Origin;
        _shape.Position = RenderPosition;
        
        Mesh = new Mesh(new FloatRect(-8f, -3f, 15f, 6f));
        AABB = Mesh.AABB;
        VertexArray vertexArray = new(PrimitiveType.LineStrip, (uint)(Mesh.Vertices.Count + 1));
        for (int i = 0; i < Mesh.Vertices.Count; i++)
        {
            vertexArray[(uint)i] = new Vertex(Mesh.Vertices[i], Color.Green);
        }

        Solid = true;
        vertexArray[(uint)Mesh.Vertices.Count] = new Vertex(Mesh.Vertices[0], Color.Green);
        DebugVerts = vertexArray;
    }

    private Vector2 moveTemp;
    private Vector2 lastPosition;
    private int direction;
    public override void Update()
    {
        base.Update();
        Velocity = AxisManager.Instance.Axis * 2;
        lastPosition = _position;

        if (Velocity!=Vector2.Zero)
        {
            this.direction = Vector2.VectorToDirection(Velocity);
            Depth = (int)_position.y;
            _pipeline.ForceSort();
 
            //DDDebug.Log("Velocity: " + Velocity);
        }

        if (_manager != null)
        {


            if (
                //and we cannot move next
                !_manager.PlaceFree(this, _position) //, collisionResults, ignoreCollisionTypes)
            )
            {
                //collision
                HandleCornerSliding();
                Outer.Log("Collision!");
                //HandleCollision(collisionResults);
            }
            else
            {


                if (Velocity.X != 0f)
                {
                    moveTemp = new Vector2(_position.X + Velocity.X, _position.Y);

                    if (_manager.PlaceFree(this,
                            moveTemp)) // && collisionManager.ObjectsAtPosition(position).Count() < 0)
                    {
                        _position = moveTemp;
                    }
                    else
                    {
                        HandleCornerSliding();
                        Velocity.X = 0f;
                    }
                }

                if (Velocity.Y != 0f)
                {
                    moveTemp = new Vector2(_position.X, _position.Y + Velocity.Y);
                    if (_manager.PlaceFree(this, moveTemp))
                    {
                        _position = moveTemp;
                    }
                    else
                    {
                        HandleCornerSliding();
                        Velocity.Y = 0f;
                    }
                }

            }
            _manager.Update(this, lastPosition, _position);
        }
        ///Depth = Int32.MaxValue;
        //Console.WriteLine(_depth);
    }

    public bool move = true;
        private void HandleCornerSliding()
        {
            if (this.direction % 2 == 0)
            {
                Vector2 Vector2 = Vector2.DirectionToVector(this.direction);
                Vector2 Vector22 = Vector2.LeftNormal(Vector2);
                int num = (this.direction == 0 || this.direction == 4) ? 8 : 10;
                int num2 = -1;
                for (int i = num; i > 0; i--)
                { 
                    bool flag = _manager.PlaceFree(this, _position + Vector2 + Vector22 * i);
                    if (flag)
                    {
                        num2 = i;
                        break;
                    }
                }
                int num3 = -1;
                for (int j = num; j > 0; j--)
                {
                    bool flag2 = _manager.PlaceFree(this, _position + Vector2 - Vector22 * j);
                    if (flag2)
                    {
                        num3 = j;
                        break;
                    }
                }
                if (num2 >= 0 || num3 >= 0)
                {
                    Vector2 position = _position + ((num2 > num3) ? Vector22 : (-Vector22.Vector2f));
                    bool flag3 = _manager.PlaceFree(this, position);
                    if (flag3)
                    {
                        this.lastPosition = _position;
                        _position = position;
                        _manager.Update(this, this.lastPosition, _position);
                        position = _position + Vector2;
                        flag3 = _manager.PlaceFree(this, position);
                        if (flag3)
                        {
                            this.lastPosition = _position;
                            _position = position;
                            _manager.Update(this, this.lastPosition, _position);
                        }
                    }
                }
            }
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
        if (!_disposed)
        {

            _shape.Origin = Origin;
            Vector2 positionCopy = _position;
            positionCopy.y -= Size.y;
            positionCopy.x -= Size.x / 2;
            _shape.Position = positionCopy;
            //_shape.Position = _position - Size;
            target.Draw(_shape);
        }
    }

    public Vector2 Velocity;
    public AABB AABB { get; }
    public Mesh Mesh { get; }
    public bool Solid { get; set; }
    
    
    public VertexArray DebugVerts { get; set; }
    
    public void Collision(CollisionContext context)
    {
        Outer.Log("CollisioN!");
    }
}