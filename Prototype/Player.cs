#region

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using DewDrop;
using DewDrop.Collision;
using DewDrop.Entities;
using DewDrop.Graphics;
using DewDrop.Inspector;
using DewDrop.UserInput;
using DewDrop.Utilities;
using Prototype;
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

        Color = fillColor;
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
     [Tooltip("Player Color")]
     public Color Color;

     [Tooltip("Player Speed")]
     public float Speed = 2f;
    public override void Update()
    {
        base.Update();
        Velocity = ((Input.Instance.Axis) * Speed);
        lastPosition = _position;

        if (Velocity!=Vector2.Zero)
        {
            this.direction = Vector2.VectorToDirection(Velocity);
            Depth = (int)_position.y;
            _pipeline.ForceSort();
 
            //DDDebug.Log("Velocity: " + Velocity);
        }

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
    [ButtonMethod("Genghis Khan")]
    public void GenghisKhan(string khan)
    {
        Outer.Log(khan);
    }

    public void HandleCollision(ICollidable[] collisionResults)
    {
    }
    
    public bool CanMove = true;
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
            FillColor = Color;
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
    
    public void OnTriggerStay (ICollidable context) { throw new NotImplementedException(); }

    public void OnTriggerEnter (ICollidable context) { throw new NotImplementedException(); }
    public void OnTriggerExit (ICollidable context) { throw new NotImplementedException(); }
    public List<ICollidable> CollidingWith { get; }
}