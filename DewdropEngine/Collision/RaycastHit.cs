using DewDrop.Utilities;
namespace DewDrop.Collision; 

/// <summary>
/// Represents the result of a raycast in a 2D physics simulation.
/// </summary>
public struct RaycastHit {
	/// <summary>
	/// The point in world space where the raycast hit a collider.
	/// </summary>
	public Vector2 Point;

	/// <summary>
	/// The normal vector at the point of contact.
	/// </summary>
	public Vector2 Normal;

	/// <summary>
	/// The distance from the ray's origin to the hit point.
	/// </summary>
	public float Distance;

	/// <summary>
	/// The collider that was hit by the ray.
	/// </summary>
	public readonly ICollidable Collider;

	/// <summary>
	/// Indicates whether the raycast hit a collider.
	/// </summary>
	public bool Hit;

	/// <summary>
	/// Initializes a new instance of the RaycastHit struct with specified hit data.
	/// </summary>
	/// <param name="point">The point in world space where the raycast hit a collider.</param>
	/// <param name="normal">The normal vector at the point of contact.</param>
	/// <param name="distance">The distance from the ray's origin to the hit point.</param>
	/// <param name="collider">The collider that was hit by the ray.</param>
	/// <param name="hit">Indicates whether the raycast hit a collider.</param>
	public RaycastHit(Vector2 point, Vector2 normal, float distance, ICollidable collider, bool hit) {
		Point = point;
		Normal = normal;
		Distance = distance;
		Collider = collider;
		Hit = hit;
	}

	/// <summary>
	/// Gets an empty RaycastHit.
	/// </summary>
	public static RaycastHit Empty => new RaycastHit(Vector2.Zero, Vector2.Zero, 0f, null, false);
}