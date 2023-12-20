#region

using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;
// ReSharper disable InconsistentNaming

#endregion

namespace DewDrop.Graphics;

/// <summary>
/// Graphic is a class that represents a graphical object which can use any ITexture. Can be animated
/// </summary>
public class Graphic : AnimatedRenderable {
	#region Properties

    /// <summary>
    ///     The color of this graphic
    /// </summary>
    public virtual Color Color {
		get => _sprite.Color;
		set => _sprite.Color = value;
	}
    /// <summary>
    ///     The scale of the graphic, dictates how big it is.
    /// </summary>
    public virtual Vector2f Scale {
		get => _scale;
		set => _scale = value;
	}
    /// <summary>
    ///     Keeps information about the texture.
    /// </summary>
    public IntRect TextureRect {
		get => _sprite.TextureRect;
		set {
			_sprite.TextureRect = value;
			Size = new Vector2(value.Width, value.Height);
			_startTextureRect = value;
			_frame = 0f;
		}
	}
    /// <summary>
    ///     The texture assosicated with the graphic
    /// </summary>
    public ITexture Texture => _texture;

	#endregion

	#region Fields

	protected  Sprite _sprite;
	protected ITexture _texture;
	protected IntRect _startTextureRect;
	protected Vector2f _scale;
	protected Vector2f _finalScale;

	#endregion

	/// <summary>
	/// Creates a new graphic.
	/// </summary>
	/// <param name="resource">The name of the ITexture to pull from the TextureManager</param>
	/// <param name="position">The position of the sprite relative to the graphic</param>
	/// <param name="textureRect">Information about the texture's integer coordinates</param>
	/// <param name="origin">Origin of the texture relative to the graphic</param>
	/// <param name="depth">The depth of this object</param>
    public Graphic (string resource, Vector2 position, IntRect textureRect, Vector2 origin, int depth) {
		// this.texture = TextureManager.Instance.UseUnprocessed(resource);
		_sprite = new Sprite(_texture.Image);
		_sprite.TextureRect = textureRect;
		_startTextureRect = textureRect;
		_position = position;
		_origin = origin;
		_size = new Vector2(textureRect.Width, textureRect.Height);
		_depth = depth;
		_rotation = 0f;
		_scale = new Vector2f(1f, 1f);
		_finalScale = _scale;
		_speedModifier = 1f;
		_sprite.Position = _position.Vector2f;
		_sprite.Origin = _origin.Vector2f;
		_speeds = new[] {
			1f
		};
		_speedIndex = 0f;
		_visible = true;
	}

	protected Graphic () {
	}

    /// <summary>
    ///     Pushes the animation forward
    /// </summary>
    protected void UpdateAnimation () {
		int num = _startTextureRect.Left + (int)_frame*(int)_size.X;
		int left = num%(int)_sprite.Texture.Size.X;
		int top = _startTextureRect.Top + num/(int)_sprite.Texture.Size.X*(int)_size.Y;
		_sprite.TextureRect = new IntRect(left, top, (int)_size.X, (int)_size.Y);
		if (_frame + GetFrameSpeed() >= Frames) {
			AnimationComplete();
		}

		_speedIndex = (_speedIndex + GetFrameSpeed())%_speeds.Length;
		IncrementFrame();
	}

    /// <summary>
    ///     Pushes the current frame forward
    /// </summary>
    protected virtual void IncrementFrame () {
		_frame = (_frame + GetFrameSpeed())%Frames;
	}

    /// <summary>
    ///     Returns the animation speed
    /// </summary>
    /// <returns>The animation speed</returns>
    protected float GetFrameSpeed () {
		return _speeds[(int)_speedIndex%_speeds.Length]*_speedModifier;
	}

	public void Translate (Vector2f v) {
		Translate(v.X, v.Y);
	}

    /// <summary>
    ///     Moves the position forward by the specified X and Y
    /// </summary>
    /// <param name="x">The X to move forward by</param>
    /// <param name="y">The Y to move forward by</param>
    public virtual void Translate (float x, float y) {
		_position.X += x;
		_position.Y += y;
	}

    /// <summary>
    ///     Draws the graphic
    /// </summary>
    /// <param name="target">The RenderTarget to draw to</param>
    public override void Draw (RenderTarget target) {
		if (_visible) {
			if (Frames > 0) {
				UpdateAnimation();
			}

			_sprite.Position = _position.Vector2f;
			_sprite.Origin = Origin.Vector2f;
			_sprite.Rotation = Rotation;
			_finalScale = _scale;
			_sprite.Scale = _finalScale;
			target.Draw(_sprite);
		}
	}

	protected override void Dispose (bool disposing) {
		if (!_disposed) {
			if (disposing && _sprite != null) {
				_sprite.Dispose();
			}

			TextureManager.Instance.Unuse(_texture);

		}

		_disposed = true;
	}
}
