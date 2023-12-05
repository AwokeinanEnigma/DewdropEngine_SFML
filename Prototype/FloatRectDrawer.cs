using DewDrop.Collision;
using DewDrop.Graphics;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;
using System.Security.Cryptography;
namespace Prototype; 

public class FloatRectDrawer : Renderable{

	public  FloatRect Bounds { get; }
	public VertexArray DebugVerts { get; }
	
	
	public FloatRectDrawer(FloatRect bounds,Vector2 position) {
		Bounds = bounds;
		var mesh = new Mesh(new FloatRect(bounds.Left, bounds.Top, bounds.Width, bounds.Width));
		VertexArray vertexArray = new(PrimitiveType.LineStrip, (uint)(mesh.Vertices.Count + 1));
		for (int i = 0; i < mesh.Vertices.Count; i++)
		{
			vertexArray[(uint)i] = new Vertex(mesh.Vertices[i], Color.Black);
		}
		vertexArray[(uint)mesh.Vertices.Count] = new Vertex(mesh.Vertices[0], Color.Black);

		DebugVerts = vertexArray;
		Origin = new Vector2(0, 0);
		Size = new Vector2(5000, 5000);
		RenderPosition = position;
		Depth = 50000;
	}
	public override void Draw (RenderTarget target) {
		RenderStates states = new RenderStates(BlendMode.Alpha, Transform.Identity, null, null);
		states.Transform = Transform.Identity;
		states.Transform.Translate(RenderPosition);

		// Draw the debug vertices on the render target
		target.Draw(DebugVerts, states);
	}
}
