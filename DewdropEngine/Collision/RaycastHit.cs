using DewDrop.Utilities;
namespace DewDrop.Collision; 

public struct RaycastHit {
	public Vector2 Point;
	public Vector2 Normal;
	public float Distance;
	public ICollidable Collider;
	public bool Hit;
	
	public RaycastHit(Vector2 point, Vector2 normal, float distance, ICollidable collider, bool hit) {
		Point = point;
		Normal = normal;
		Distance = distance;
		Collider = collider;
		Hit = hit;
	}
	
	public static RaycastHit Empty => new RaycastHit(Vector2.Zero, Vector2.Zero, 0f, null, false);
	
	
}
