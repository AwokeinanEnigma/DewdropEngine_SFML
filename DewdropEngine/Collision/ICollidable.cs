using DewDrop.Utilities;
using SFML.Graphics;

namespace DewDrop.Collision; 

/// <summary>
/// Defines the properties and methods required for an object to participate in collision detection.
/// </summary>
public interface ICollidable {
    /// <summary>
    /// Gets or sets the position of this collider.
    /// </summary>
    Vector2 Position { get; set; }

    /// <summary>
    /// Gets the Axis-Aligned Bounding Box (AABB) of this collider.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    AABB AABB { get; }

    /// <summary>
    /// Gets the mesh of this collider.
    /// </summary>
    Mesh Mesh { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this collider is solid or not.
    /// </summary>
    bool Solid { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this collider is a trigger.
    /// </summary>
    bool IsTrigger { get; set; }

    /// <summary>
    /// Gets the debug vertices to draw when debug mode is enabled.
    /// </summary>
    VertexArray DebugVerts { get; }

    /// <summary>
    /// Method to be called when another collider stays in contact with this collider.
    /// </summary>
    /// <param name="context">The collider in contact with this collider.</param>
    void OnTriggerStay(ICollidable context);

    /// <summary>
    /// Method to be called when another collider enters the contact with this collider.
    /// </summary>
    /// <param name="context">The collider entering contact with this collider.</param>
    void OnTriggerEnter (ICollidable context);

    /// <summary>
    /// Method to be called when another collider exits the contact with this collider.
    /// </summary>
    /// <param name="context">The collider exiting contact with this collider.</param>
    void OnTriggerExit (ICollidable context);

    /// <summary>
    /// Gets the list of colliders currently in contact with this collider.
    /// </summary>
    List<ICollidable> CollidingWith { get; }
}