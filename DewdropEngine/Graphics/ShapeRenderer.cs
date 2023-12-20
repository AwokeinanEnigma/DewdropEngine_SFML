#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Graphics;

/// <summary>
/// Handles the rendering of shapes in the game.
/// </summary>
public class ShapeRenderer : Renderable {

	/// <summary>
	/// Gets the SFML _shape.
	/// </summary>
	readonly Shape _shape;

	/// <summary>
	/// Gets or sets the position of the ShapeRenderer.
	/// </summary>
	public override Vector2 RenderPosition {
		get =>
			_position;
		set {
			_shape.Position = value;
			_position = value;
		}
	}

	/// <summary>
	/// Gets or sets the size of the ShapeRenderer.
	/// </summary>
	public override Vector2 Size {
		get => _size;
		set {
			_shape.Scale = value;
			_size = value;
		}
	}
	
	/// <summary>
	/// Gets or sets the origin of the ShapeRenderer.
	/// </summary>
	public override Vector2 Origin {
		get => _size;
		set {
			_shape.Origin = value;
			_size = value;
		}
	}

	public override float Rotation {
		get => _rotation;
		set {
			_shape.Rotation = value;
			_rotation = value;
		}
	}

	/// <summary>
	/// Gets or sets the outline color of the _shape.
	/// </summary>
	public Color OutlineColor {
		get => _shape.OutlineColor;
		set => _shape.OutlineColor = value;
	}

	/// <summary>
	/// Gets or sets the fill color of the _shape.
	/// </summary>
	public Color FillColor {
		get => _shape.FillColor;
		set => _shape.FillColor = value;
	}

	/// <summary>
	/// Initializes a new instance of the ShapeRenderer class with specified shape, position, size, origin, depth, fill color, and outline color.
	/// </summary>
	/// <param name="shape">The shape to be rendered.</param>
	/// <param name="position">The position of the ShapeRenderer.</param>
	/// <param name="size">The size of the ShapeRenderer.</param>
	/// <param name="origin">The origin of the ShapeRenderer.</param>
	/// <param name="depth">The depth of the ShapeRenderer.</param>
	/// <param name="fillColor">The fill color of the _shape.</param>
	/// <param name="outlineColor">The outline color of the _shape.</param>
	public ShapeRenderer (Shape shape, Vector2 position, Vector2 size, Vector2 origin, int depth, Color fillColor = default, Color outlineColor = default) {
		// ensure that we can actually see our fucking square
		if (fillColor == default) {
			fillColor = Color.White;
		}

		if (outlineColor == default) {
			outlineColor = Color.White;
		}

		_shape = shape;
		_shape.FillColor = fillColor;
		_shape.OutlineColor = outlineColor;
		
		_position = position;
		_size = size;
		_origin = origin;
		_depth = depth;
		
		_shape.Origin = _origin;
		_shape.Position = _position;
		_shape.Scale = _size;
		_shape.Rotation = _rotation;
		
		//__shape.Scale = Size;
		//__shape.Rotation = Rotation;

	}

	public override void Draw (RenderTarget target) {
		target.Draw(_shape);
	}

	protected override void Dispose (bool disposing) {
		if (!_disposed) {
			if (disposing && _shape != null) {
				_shape.Dispose();
			}

		}

		_disposed = true;
	}
}
