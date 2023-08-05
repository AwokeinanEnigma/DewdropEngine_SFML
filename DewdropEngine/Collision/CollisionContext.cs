using DewDrop.Utilities;

namespace DewDrop.Collision;

public struct CollisionContext
{
    public CollisionContext(ICollidable other, bool colliding, bool willCollide, Vector2 minTranslation)
    {
        Other = other;
        Colliding = colliding;
        WillCollide = willCollide;
        MinimumTranslation = minTranslation;
    }
    /// <summary>
    /// What we collided with
    /// </summary>
    public ICollidable Other { get; }
    /// <summary>
    /// Are we still colliding with it?
    /// </summary>
    public bool Colliding { get; }
    /// <summary>
    /// will 
    /// </summary>
    public bool WillCollide { get; }
    /// <summary>
    /// How much do we need to move?
    /// </summary>
    public Vector2 MinimumTranslation { get; }
}