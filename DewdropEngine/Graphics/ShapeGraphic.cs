#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Graphics;

public class ShapeGraphic : Renderable {
	public Shape Shape { get; }
	//set => _shape = value;
	public Color OutlineColor {
		get => Shape.OutlineColor;
		set => Shape.OutlineColor = value;
	}

	public Color FillColor {
		get => Shape.FillColor;
		set => Shape.FillColor = value;
	}

	public ShapeGraphic (Shape shape, Vector2 position, Vector2 size, Vector2 origin, int depth, Color fillColor = default, Color outlineColor = default) {
		RenderPosition = position;
		Size = size;
		Origin = origin;
		Depth = depth;
		

		// ensure that we can actually see our fucking square
		if (fillColor == default) {
			fillColor = Color.White;
		}

		if (outlineColor == default) {
			outlineColor = Color.White;
		}

		Shape = shape;
		Shape.FillColor = fillColor;
		Shape.OutlineColor = outlineColor;

		Shape.Origin = Origin;
		Shape.Position = RenderPosition;
		//_shape.Scale = Size;
		//_shape.Rotation = Rotation;

	}

	public override void Draw (RenderTarget target) {
		Shape.Origin = Origin;
		Shape.Position = RenderPosition;
		target.Draw(Shape);
	}

	protected override void Dispose (bool disposing) {
		if (!_disposed) {
			if (disposing && Shape != null) {
				Shape.Dispose();
			}

		}

		_disposed = true;
	}
}
