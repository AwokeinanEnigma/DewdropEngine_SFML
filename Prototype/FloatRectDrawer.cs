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
	
	RenderStates _states;
	
	public FloatRectDrawer(FloatRect bounds,Vector2 position) {
		Bounds = bounds;
	

		var mesh = new Mesh(new FloatRect(bounds.Left, bounds.Top, bounds.Width, bounds.Width));
		DebugVerts = new(PrimitiveType.LineStrip, (uint)(mesh.Vertices.Count + 1));
		for (int i = 0; i < mesh.Vertices.Count; i++)
		{
			DebugVerts[(uint)i] = new Vertex(mesh.Vertices[i], Color.Black);
		}
		DebugVerts[(uint)mesh.Vertices.Count] = new Vertex(mesh.Vertices[0], Color.Black);

		_origin = new Vector2(0, 0);
		_size = new Vector2(bounds.Width *5, bounds.Height*5);
		_position = position;
		_depth = 50000;
		
		_states = new RenderStates(BlendMode.Alpha, Transform.Identity, null, null);
		_states.Transform = Transform.Identity;
		_states.Transform.Translate(_position);
	}
	public override void Draw (RenderTarget target) {
		// Draw the debug vertices on the render target
		target.Draw(DebugVerts, _states);
	}
}
