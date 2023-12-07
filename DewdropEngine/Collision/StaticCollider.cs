#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion
namespace DewDrop.Collision;

/// <summary>
///     A static object that is static (unmoving)
/// </summary>
public class StaticCollider : ICollidable {
    /// <summary>
    ///     Creates a new solid static object from a mesh
    /// </summary>
    /// <param name="mesh"></param>
    public StaticCollider (Mesh mesh) {
		Mesh = mesh;
		AABB = mesh.AABB;
		Position = new Vector2(0f, 0f);
		Solid = true;
		VertexArray vertexArray = new VertexArray(PrimitiveType.LineStrip, (uint)(mesh.Vertices.Count + 1));
		for (int i = 0; i < mesh.Vertices.Count; i++) {
			vertexArray[(uint)i] = new Vertex(mesh.Vertices[i], Color.Red);
		}

		vertexArray[(uint)mesh.Vertices.Count] = new Vertex(mesh.Vertices[0], Color.Red);
		DebugVerts = vertexArray;
	}

	public Vector2 Position { get; set; }

	public AABB AABB { get; }

	public Mesh Mesh { get; }

	public bool Solid { get; set; }

	public VertexArray DebugVerts { get; }
	public Vector2 Velocity { get; set; }

	public void Collision (CollisionContext context) {
	}
}
