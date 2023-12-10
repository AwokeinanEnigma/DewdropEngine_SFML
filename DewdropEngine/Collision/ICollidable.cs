#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion
namespace DewDrop.Collision;

public interface ICollidable {
    /// <summary>
    ///     The position of this collider
    /// </summary>
    Vector2 Position { get; set; }
    /// <summary>
    ///     The AABB of this collider
    /// </summary>
    AABB AABB { get; }
    /// <summary>
    ///     The mesh of this collider
    /// </summary>
    Mesh Mesh { get; }
    /// <summary>
    ///     Can we walk through this collider or not?
    /// </summary>
    bool Solid { get; set; }
    /// <summary>
    ///  Is this collider a trigger?
    /// </summary>
    bool IsTrigger { get; set; }
    /// <summary>
    ///     Debug vertices to draw when debug mode is enabled.
    /// </summary>
    VertexArray DebugVerts { get; }
    void OnTriggerStay(ICollidable context);
    void OnTriggerEnter (ICollidable context);
    void OnTriggerExit (ICollidable context);
    List<ICollidable> CollidingWith { get; }
}
