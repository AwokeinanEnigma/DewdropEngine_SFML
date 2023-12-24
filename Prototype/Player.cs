/*
#region

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using DewDrop;
using DewDrop.Collision;
using DewDrop.Entities;
using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.Inspector;
using DewDrop.Inspector.Attributes;
using DewDrop.UserInput;
using DewDrop.Utilities;
using Prototype;
using SFML.Graphics;
using SFML.Window;

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
    public bool IsTrigger { get; set; }
    public override string Name =>  "Player";


    public Player(Shape shape, Vector2 position, Vector2 size, Vector2 origin, int depth, RenderPipeline pipeline, CollisionManager manager, Color fillColor = default, Color outlineColor = default)
    {
        RenderPosition = position;
        Size = size;
        Origin = origin;
        Depth = depth;
        IsTrigger = false;

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
     public Vector2 CheckVector;
     Vector2 LastMoveVector;
     
     public ICollidable[] collisionResults = new ICollidable[8];
public string Username = "Joe";
     [Tooltip("Player Speed")]
     public float Speed = 2f;
     
     public enum EnumValue
     {
         One,
         Two
     }
     public EnumValue Goo = EnumValue.One;
     
     [ButtonMethod("PrintEnumValue")]
     public void EnumatorGoo (EnumValue goo) {
         Outer.Log(goo);
     }
     public override void Update()
    {
        base.Update();
        Velocity = ((Input.Instance.Axis) * Speed);
        lastPosition = _position;
       // Outer.Log(Goo);
        if (Velocity!=Vector2.Zero)
        {
            this.direction = Vector2.VectorToDirection(Velocity);
            Depth = (int)_position.y;
            _pipeline.ForceSort();
 
            //DDDebug.Log("Velocity: " + Velocity);
        }
        if (Input.Instance[Keyboard.Key.Space])
        {
            Outer.Log("Space pressed");
            ViewManager.Instance.SetZoom(1.25f);
        }
        if (Input.Instance[Keyboard.Key.LShift])
        {
            
            Outer.Log("Shift pressed");
            ViewManager.Instance.SetZoom(0.75f);
        }
        ViewManager.Instance.Center = _position;

        if (_manager != null && CanMove)
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
                
                LastMoveVector = Velocity;
                if (Velocity.X != 0f)
                {
                    moveTemp = new Vector2(_position.X + Velocity.X, _position.Y);

                    if (_manager.PlaceFree(this,
                            moveTemp,collisionResults)) // && collisionManager.ObjectsAtPosition(position).Count() < 0)
                    {
                        _position = moveTemp;
                    }
                    else
                    {
                        HandleCollision(collisionResults);
                        HandleCornerSliding();
                        Velocity.X = 0f;
                    }
                }

                if (Velocity.Y != 0f)
                {
                    moveTemp = new Vector2(_position.X, _position.Y + Velocity.Y);
                    if (_manager.PlaceFree(this, moveTemp, collisionResults))
                    {
                        _position = moveTemp;
                    }
                    else
                    {
                        HandleCollision(collisionResults);
                        HandleCornerSliding();
                        Velocity.Y = 0f;
                    }
                }
                if ((Velocity.X != 0f || Velocity.Y != 0f))
                {
                    CheckVector = Vector2.Truncate(Vector2.Normalize(Velocity) * 10);
                }
            }
            _manager.Update(this, lastPosition, _position);
        }
        ///Depth = Int32.MaxValue;
        //Console.WriteLine(_depth);
    }
    
    [Tooltip("Prints stuff to the console")]
    [ButtonMethod("Print")]
    public void Print(string print)
    {
        Outer.Log(print);
    }

    public Vector2 Vector = new Vector2(1, 2);
    public string String = "This is a string.";
    public bool CanMove = true;

    public void HandleCollision(ICollidable[] collisionResults)
    {
    }
    
    /// <summary>
    /// Handles the corner sliding behavior.
    /// </summary>
    private void HandleCornerSliding () {
        // If the direction is even, execute the sliding logic
        if (direction%2 == 0) {
            // Convert the direction to a vector
            Vector2 directionVector = Vector2.DirectionToVector(direction);
            // Compute the left normal to the direction vector
            Vector2 leftNormalVector = Vector2.LeftNormal(directionVector);
            // Based on the direction, decide the number of iterations for the sliding check
            int numIterations = (direction == 0 || direction == 4) ? 8 : 10;
            // Initialize variables to store the free distances in both directions
            int distance1 = -1;
            int distance2 = -1;

            // Check for free positions in the direction of the left normal vector
            for (int i = numIterations; i > 0; i--) {
                // If a free position is found, store the distance and exit
                bool isPositionFree = _manager.PlaceFree(this, _position + directionVector + leftNormalVector*i);
                if (isPositionFree) {
                    distance1 = i;
                    break;
                }
            }

            // Check for free positions in the opposite direction
            for (int j = numIterations; j > 0; j--) {
                // If a free position is found, store the distance and exit
                bool isPositionFree = _manager.PlaceFree(this, _position + directionVector - leftNormalVector*j);
                if (isPositionFree) {
                    distance2 = j;
                    break;
                }
            }

            // If a free position was found in either direction, execute the corner sliding logic
            if (distance1 >= 0 || distance2 >= 0) {
                // Choose the direction with the greater free distance
                Vector2 newPosition = _position + ((distance1 > distance2) ? leftNormalVector : (-leftNormalVector));
                // Check if the new position is free
                bool isPositionFree = _manager.PlaceFree(this, newPosition);
                // If the new position is free, move there
                if (isPositionFree) {
                    // Update the last position
                    this.lastPosition = _position;
                    // Update the current position
                    _position = newPosition;
                    // Notify the manager of the position change
                    _manager.Update(this, this.lastPosition, _position);

                    // Calculate the next potential position
                    newPosition = _position + directionVector;
                    // Check if the next position is free
                    isPositionFree = _manager.PlaceFree(this, newPosition);
                    // If the next position is free, move there
                    if (isPositionFree) {
                        // Update the last position
                        this.lastPosition = _position;
                        // Update the current position
                        _position = newPosition;
                        // Notify the manager of the position change
                        _manager.Update(this, this.lastPosition, _position);
                    }
                }
            }
        }
    }
    protected override void BecomeVisible()
    {
        base.BecomeVisible();
        Outer.Log("We're barack.");
    }

    protected override void BecomeInvisible()
    {
        base.BecomeInvisible();
        Outer.Log("It's joeover.");
    }
    
    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing && _shape != null) {
            _shape.Dispose();
        }
        _disposed = true;
    }

    public override void Draw(RenderTarget target) {
        if (_disposed) return;
        _shape.Origin = Origin;
        Vector2 positionCopy = _position;
        positionCopy.y -= Size.y;
        positionCopy.x -= Size.x / 2;
        _shape.Position = positionCopy;
        //_shape.Position = _position - Size;
        target.Draw(_shape);
    }

    public Vector2 Velocity;
    public AABB AABB { get; }
    public Mesh Mesh { get; }
    public bool Solid { get; set; }
    
    public List<int> Integers = new List<int>();
    public List<string> Strings = new List<string>();
    public List<float> Floats = new List<float>();
    public List<Vector2> Vectors = new List<Vector2>();
    public List<Color> Colors = new List<Color>();
    public string[] ArrayOfStrings = new string[5];
    public VertexArray DebugVerts { get; set; }
    
    public void OnTriggerStay (ICollidable context) { ; }

    public void OnTriggerEnter (ICollidable context) { ; }
    public void OnTriggerExit (ICollidable context) { ; }
    public List<ICollidable> CollidingWith { get; }
}*/