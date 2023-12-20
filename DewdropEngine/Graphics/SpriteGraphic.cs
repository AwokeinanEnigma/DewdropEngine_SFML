#region

using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;
using static DewDrop.Graphics.SpriteDefinition;
// ReSharper disable MemberCanBePrivate.Global

#endregion

namespace DewDrop.Graphics;

/// <summary>
/// Represents a graphic object that can render a sprite from a spritesheet.
/// </summary>
public class SpriteGraphic : Graphic {
	#region Properties

	/// <summary>
	/// Gets or sets the current palette this SpriteGraphic is using.
	/// </summary>
    public uint CurrentPalette {
		get => _currentPalette;
		set {
			if (_currentPalette != value) {
				PreviousPalette = _currentPalette;
				_currentPalette = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets the color to blend the sprite with.
	/// </summary>
    public override Color Color {
		get => _blend;
		set {
			_blend = value;
			_realBlend = new Vec4(_blend);
		}
	}

	/// <summary>
	/// Gets or sets the mode to use when blending the texture with the color.
	/// </summary>
    public ColorBlendMode ColorBlendMode { get; set; }

	/// <summary>
	/// Gets the last palette used.
	/// </summary>
    public uint PreviousPalette { get; private set; }

	/// <summary>
	/// Gets the render states this SpriteGraphic is using.
	/// </summary>
	public RenderStates RenderStates { get; }

	/// <summary>
	/// Gets or sets a value indicating whether this graphic can be animated.
	/// </summary>
    public bool AnimationEnabled { get; set; }

	/// <summary>
	/// Gets the spritesheet texture.
	/// </summary>
    public SpritesheetTexture Spritesheet => _spritesheet;
	#endregion

	#region Fields

	static readonly int[] _ModeOneFrames = {
		0, 1, 0, 2
	};

	// this is used for multiple things so it gets a special place in the engine class
	static readonly Shader _PaletteShader = new Shader(EmbeddedResourcesHandler.GetResourceStream("pal.vert"), null, EmbeddedResourcesHandler.GetResourceStream("pal.frag"));

	// fliped stuff
	bool _flipX;
	bool _flipY;

	// palette stuff
	uint _currentPalette;

	// color stuff
	Color _blend;

	// it's all stuff
	// animation stuff
	AnimationMode _mode;
	float _betaFrame;

	readonly string _defaultSprite;
	readonly SpritesheetTexture _spritesheet;

	Vec4 _realBlend;

	#endregion

	/// <summary>
	/// Initializes a new instance of the SpriteGraphic class with specified resource, sprite name, position, depth, and palette.
	/// </summary>
	/// <param name="resource">The name of the sprite file.</param>
	/// <param name="spriteName">The sprite to initialize the SpriteGraphic with.</param>
	/// <param name="position">Where the sprite is located.</param>
	/// <param name="depth">The depth of the _sprite.</param>
	public SpriteGraphic (string resource, string spriteName, Vector2 position, int depth) {
		_defaultSprite = spriteName;

		_texture = TextureManager.Instance.UseSpritesheet(resource);

		// cast now to avoid casting in the future
		_spritesheet = (SpritesheetTexture)_texture;
		_sprite = new Sprite(_texture.Image);
		_position = position;
		_sprite.Position = _position.Vector2f;
		_depth = depth;
		_rotation = 0f;
		_scale = new Vector2f(1f, 1f);
		SetSprite(spriteName);
		_spritesheet.CurrentPalette = _currentPalette;
		_blend = Color.White;
		_realBlend = new Vec4(_blend);
		//multiply by default
		ColorBlendMode = ColorBlendMode.Multiply;
		RenderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, _PaletteShader);
		AnimationEnabled = true;
		_visible = true;
	}

	/// <summary>
	/// Initializes a new instance of the SpriteGraphic class with specified resource, sprite name, position, depth, and palette.
	/// </summary>
	/// <param name="texture">The texture to use.</param>
	/// <param name="spriteName">The sprite to initialize the SpriteGraphic with.</param>
	/// <param name="position">Where the sprite is located.</param>
	/// <param name="depth">The depth of the _sprite.</param>
    public SpriteGraphic (SpritesheetTexture texture, string spriteName, Vector2 position, int depth) {
		this._texture = texture;
		_spritesheet = (SpritesheetTexture)this._texture;
		_sprite = new Sprite(this._texture.Image);
		_position = position;
		_sprite.Position = _position.Vector2f;
		_depth = depth;
		_rotation = 0f;
		_scale = new Vector2f(1f, 1f);
		SetSprite(spriteName);
		_spritesheet.CurrentPalette = _currentPalette;
		_blend = Color.White;
		_realBlend = new Vec4(_blend);
		//multiply by default
		ColorBlendMode = ColorBlendMode.Multiply;
		RenderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, _PaletteShader);
		AnimationEnabled = true;
		_visible = true;
	}

	/// <summary>
	/// Initializes a new instance of the SpriteGraphic class with specified resource, sprite name, position, depth, and palette.
	/// </summary>
	/// <param name="resource">The name of the sprite file.</param>
	/// <param name="spriteName">The sprite to initialize the SpriteGraphic with.</param>
	/// <param name="position">Where the sprite is located.</param>
	/// <param name="depth">The depth of the sprite.</param>
	/// <param name="palette">The palette to initialize the SpriteGraphic with.</param>
	public SpriteGraphic (string resource, string spriteName, Vector2 position, int depth, uint palette) {
		_defaultSprite = spriteName;

		_texture = TextureManager.Instance.UseSpritesheet(resource);
		_sprite = new Sprite(_texture.Image);
		_spritesheet = (SpritesheetTexture)_texture;

		_position = position;
		_sprite.Position = _position.Vector2f;

		_depth = depth;

		_rotation = 0f;

		_scale = new Vector2f(1f, 1f);

		SetSprite(spriteName);

		_currentPalette = palette;
		_spritesheet.CurrentPalette = palette;

		_blend = Color.White;
		_realBlend = new Vec4(_blend);

		//multiply by default
		ColorBlendMode = ColorBlendMode.Multiply;
		RenderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, _PaletteShader);
		AnimationEnabled = true;

		_visible = true;
	}

	/// <summary>
    /// Changes the sprite definition to the one with the given name.
    /// </summary>
    /// <param name="name">The name of the new sprite definition.</param>
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
			if (Frames > 1 && AnimationEnabled) {
				UpdateAnimation();
			}

			_sprite.Position = _position.Vector2f;
			_sprite.Origin = _origin.Vector2f;
			_sprite.Rotation = _rotation;
			_finalScale.X = _flipX ? -_scale.X : _scale.X;
			_finalScale.Y = _flipY ? -_scale.Y : _scale.Y;
			_sprite.Scale = _finalScale;
			_spritesheet.CurrentPalette = _currentPalette;

			_PaletteShader.SetUniform("image", _texture.Image);
			_PaletteShader.SetUniform("palette", _spritesheet.Palette);
			_PaletteShader.SetUniform("palIndex", _spritesheet.CurrentPaletteFloat);
			//Debug.Log($"Current palette is {currentPalette}. Texture's palette is {((IndexedTexture)this.texture).CurrentPalette}. Float palette is {((IndexedTexture)this.texture).CurrentPaletteFloat} & the palette max is {((IndexedTexture)this.texture).PaletteCount}. {(float)CurrentPalette/ (float)((IndexedTexture)this.texture).PaletteCount} ");
			_PaletteShader.SetUniform("palSize", _spritesheet.PaletteSize);
			_PaletteShader.SetUniform("blend", _realBlend);
			_PaletteShader.SetUniform("blendMode", (float)ColorBlendMode);
			//SpriteGraphic.INDEXED_COLOR_SHADER.SetUniform("time", time.ElapsedTime.AsSeconds());
			if (!_disposed) {
				target.Draw(_sprite, RenderStates);
			}
		}
	}

	
	/// <summary>
	/// Retrieves the sprite definition for a given sprite name.
	/// </summary>
	/// <param name="spriteDefName">The name of the sprite.</param>
	/// <returns>The sprite definition.</returns>
	public SpriteDefinition GetSpriteDefinition(string spriteDefName)
	{
		// Calculate the hash code for the sprite name
		int hashCode = spriteDefName.GetHashCode();

		// Retrieve the sprite definition from the spritesheet using the hash code
		return _spritesheet.GetSpriteDefinition(hashCode);
	}

	/// <summary>
	/// Clones the SpriteGraphic object.
	/// </summary>
	/// <returns>A new SpriteGraphic object.</returns>
	public SpriteGraphic Clone()
	{
		return new SpriteGraphic(_spritesheet, _defaultSprite, _position, _depth);
	}
	
	public void ConvertToImageFile () {
		_spritesheet.ConvertToImageFile();
	}
}
