using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;
namespace DewDrop.Graphics; 

public class LineRenderer : Renderable{

	private VertexArray _line;
	public LineRenderer (Vector2 positionA, Vector2 positionB, Vector2 size, Vector2 origin, int depth, Color color = default){
		// get inbetween of position a and b
		RenderPosition = positionA;
		Size = size;
		Origin = origin;
		Depth = depth;
		_line = new VertexArray(PrimitiveType.Lines);
		_line.Append(new Vertex(positionA, color));
		_line.Append(new Vertex(positionB, color));
	}
	
	public void SetPosition(int index, Vector2 position){
		_line[(uint)index] = new Vertex(position);
	}
	
	public override void Draw (RenderTarget target) {
		target.Draw(_line);
	}
	
	protected override void Dispose (bool disposing) {
		if (!_disposed  && disposing) {
			_line.Dispose();
		}

		_disposed = true;
	}
}
