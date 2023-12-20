#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion
namespace DewDrop.Collision;

/// <summary>
/// Represents a static collider used for collision detection.
/// </summary>
public class StaticCollider : ICollidable {
	public List<ICollidable> CollidingWith { get; } = new List<ICollidable>();
	public bool IsTrigger { get; set; }
	public Vector2 Position { get; set; }
	public AABB AABB { get; }
	public Mesh Mesh { get; }
	public bool Solid { get; set; }
	public VertexArray DebugVerts { get; }
	public Vector2 Velocity { get; set; }
	/// <summary>
	/// Initializes a new instance of the StaticCollider class with a specified mesh.
	/// </summary>
	/// <param name="mesh">The mesh of the StaticCollider.</param>
	public StaticCollider (Mesh mesh) {
		Mesh = mesh;
		AABB = mesh.AABB;
		Position = new Vector2(0f, 0f);
		Solid = true;
		IsTrigger = false;
		VertexArray vertexArray = new VertexArray(PrimitiveType.LineStrip, (uint)(mesh.Vertices.Count + 1));
		for (int i = 0; i < mesh.Vertices.Count; i++) {
			vertexArray[(uint)i] = new Vertex(mesh.Vertices[i], Color.Red);
		}

		vertexArray[(uint)mesh.Vertices.Count] = new Vertex(mesh.Vertices[0], Color.Red);
		DebugVerts = vertexArray;
	}
	
	public void OnTriggerStay (ICollidable context) {
		Outer.LogError("A StaticCollider should not be a trigger.", new Exception());
	}
	public void OnTriggerEnter (ICollidable context) {
		Outer.LogError("A StaticCollider should not be a trigger.", new Exception());
	}
	public void OnTriggerExit (ICollidable context) {
		Outer.LogError("A StaticCollider should not be a trigger.", new Exception());
	}
}
