#region

using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;
using static DewDrop.Graphics.SpriteDefinition;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

#endregion

namespace DewDrop.Graphics;

/// <summary>
/// Represents a renderable object loaded from a .aseprite file.
/// </summary>
public class AsepriteGraphic : Graphic {
	#region Fields
	readonly RenderStates _renderStates;

	static readonly int[] _ModeOneFrames = {
		0, 1, 0, 2
	};
	
	// fliped stuff
	bool _flipX;
	bool _flipY;
	// it's all stuff
	// animation stuff
	AnimationMode _mode;
	float _betaFrame;
	
	readonly string _defaultSprite;
	readonly AsepriteTexture _spritesheet;
	
	#endregion

	/// <summary>
	/// Creates a new AsepriteGraphic.
	/// </summary>
	/// <param name="resource">The name of the sprite file.</param>
	/// <param name="spriteName">The sprite to initialize the AsepriteGraphic with. This will be the starting sprite it uses.</param>
	/// <param name="position">Where the sprite is located.</param>
	/// <param name="depth">The depth of the sprite.</param>
	public AsepriteGraphic (string resource, string spriteName, Vector2 position, int depth) {
		_defaultSprite = spriteName;

		_texture = TextureManager.Instance.UseAsepriteTexture(resource);

		// cast now to avoid casting in the future
		_spritesheet = (AsepriteTexture)_texture;
		_sprite = new Sprite(_texture.Image);
		_position = position;
		_sprite.Position = _position.Vector2f;
		_depth = depth;
		_rotation = 0f;
		_scale = new Vector2f(1f, 1f);
		SetSprite(spriteName);
		//multiply by default
		_renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, null);
		_visible = true;
	}
	
	/// <summary>
	/// Creates a new AsepriteGraphic.
	/// </summary>
	/// <param name="spritesheet">The spritesheet to use.</param>
	/// <param name="spriteName">The sprite to initialize the AsepriteGraphic with. This will be the starting sprite it uses.</param>
	/// <param name="position">Where the sprite is located.</param>
	/// <param name="depth">The depth of the sprite.</param>
	AsepriteGraphic (AsepriteTexture spritesheet, string spriteName, Vector2 position, int depth) {
		_spritesheet = spritesheet;
		_texture = spritesheet;
		_sprite = new Sprite(_texture.Image);
		_position = position;
		_sprite.Position = _position.Vector2f;
		_depth = depth;
		_rotation = 0f;
		_scale = new Vector2f(1f, 1f);
		SetSprite(spriteName);
		//multiply by default
		_renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, null);
		_visible = true;
	}

	/// <summary>
	/// Sets the sprite for the object.
	/// </summary>
	/// <param name="name">The name of the sprite.</param>
	/// <param name="reset">Whether to reset the sprite animation.</param>
	public void SetSprite(string name, bool reset = true)
	{
		// Get the sprite definition from the spritesheet
		SpriteDefinition spriteDefinition = _spritesheet.GetSpriteDefinition(name);

		// If the sprite definition is not valid, get the default sprite definition
		if (!spriteDefinition.IsValid)
		{
			spriteDefinition = _spritesheet.GetDefaultSpriteDefinition();
			Outer.LogError($"Sprite definition '{spriteDefinition.Name}' is invalid!", null);
		}

		// Set the sprite's origin
		_sprite.Origin = spriteDefinition.Offset.Vector2f;
		_origin = spriteDefinition.Offset;

		// Set the texture rectangle for the sprite
		_sprite.TextureRect = new IntRect(
			(int)spriteDefinition.Coords.X, 
			(int)spriteDefinition.Coords.Y, 
			(int)spriteDefinition.Bounds.X, 
			(int)spriteDefinition.Bounds.Y
			);
		_startTextureRect = _sprite.TextureRect;

		// Set the size of the sprite
		Size = new Vector2(_sprite.TextureRect.Width, _sprite.TextureRect.Height);

		// Set the flip flags for the sprite
		_flipX = spriteDefinition.FlipX;
		_flipY = spriteDefinition.FlipY;

		// Set the final scale for the sprite
		_finalScale.X = _flipX ? -_scale.X : _scale.X;
		_finalScale.Y = _flipY ? -_scale.Y : _scale.Y;

		_sprite.Scale = _finalScale;

		// Set the frames, speeds, and mode for the sprite
		Frames = spriteDefinition.Frames;
		Speeds = spriteDefinition.Speeds;
		_mode = spriteDefinition.Mode;

		// Reset the sprite animation if requested
		if (reset)
		{
			_frame = 0f;
			_betaFrame = 0f;
			_speedIndex = 0f;
			_speedModifier = 1f;
			return;
		}

		_frame %= Frames;
	}
	protected override void IncrementFrame () {
		float frameSpeed = GetFrameSpeed();

		switch (_mode) {
		case AnimationMode.Continous:
			_frame = (_frame + frameSpeed)%Frames;
			break;

		case AnimationMode.ZeroTwoOneThree:
			_betaFrame = (_betaFrame + frameSpeed)%4f;
			_frame = _ModeOneFrames[(int)_betaFrame];
			break;
		}

		_speedIndex = (int)_frame%_speeds.Length;
	}

	public override void Draw (RenderTarget target) {
		if (!_disposed && _visible) {
			//if (Frames > 1 && AnimationEnabled) {
				//UpdateAnimation();
			//}

			_sprite.Position = _position.Vector2f;
			_sprite.Origin = _origin.Vector2f;
			_sprite.Rotation = _rotation;
			_finalScale.X = _flipX ? -_scale.X : _scale.X;
			_finalScale.Y = _flipY ? -_scale.Y : _scale.Y;
			_sprite.Scale = _finalScale;
			if (!_disposed) {
				target.Draw(_sprite, _renderStates);
			}
		}
	}

	
	/// <summary>
	/// Retrieves the sprite definition for a given sprite name.
	/// </summary>
	/// <param name="sprite">The name of the sprite.</param>
	/// <returns>The sprite definition.</returns>
	public SpriteDefinition GetSpriteDefinition(string sprite)
	{
		// Calculate the hash code for the sprite name
		int hashCode = _sprite.GetHashCode();

		// Retrieve the sprite definition from the spritesheet using the hash code
		return _spritesheet.GetSpriteDefinition(hashCode);
	}

	/// <summary>
	/// Clones the SpriteGraphic object.
	/// </summary>
	/// <returns>A new SpriteGraphic object if _resourceName is not null, else null.</returns>
	public AsepriteGraphic Clone()
	{
		return new AsepriteGraphic(_spritesheet, _defaultSprite, _position, _depth);
	}

	public void ConvertToImageFile () {
		_spritesheet.ConvertToImageFile();
	}
}
