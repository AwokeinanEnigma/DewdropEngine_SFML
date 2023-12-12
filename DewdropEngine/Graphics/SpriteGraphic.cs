#region

using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;
using static DewDrop.Graphics.SpriteDefinition;

#endregion

namespace DewDrop.Graphics;

public class SpriteGraphic : Graphic {
	#region Properties

    /// <summary>
    ///     The current palette this IndexedColorGraphic is using.
    /// </summary>
    public uint CurrentPalette {
		get => currentPalette;
		set {
			if (currentPalette != value) {
				PreviousPalette = currentPalette;
				currentPalette = value;
			}
		}
	}

    /// <summary>
    ///     The color to overlay the texture with.
    /// </summary>
    public override Color Color {
		get => blend;
		set {
			blend = value;
			_realBlend = new Vec4(blend);
		}
	}

    /// <summary>
    ///     The mode to use when blending the texture with the color.
    /// </summary>
    public ColorBlendMode ColorBlendMode { get; set; }

    /// <summary>
    ///     Self explanatory. The last palette used.
    /// </summary>
    public uint PreviousPalette { get; private set; }

    /// <summary>
    ///     The render states this IndexedColorGraphic is using.
    /// </summary>
    public RenderStates RenderStates { get; }

    /// <summary>
    ///     Can this graphic be animated?
    /// </summary>
    public bool AnimationEnabled { get; set; }

    public SpritesheetTexture Spritesheet => _spritesheet;
	#endregion

	#region Fields

	static readonly int[] MODE_ONE_FRAMES = {
		0, 1, 0, 2
	};

	// this is used for multiple things so it gets a special place in the engine class
	static readonly Shader PaletteShader = new Shader(EmbeddedResourcesHandler.GetResourceStream("pal.vert"), null, EmbeddedResourcesHandler.GetResourceStream("pal.frag"));

	// fliped stuff
	bool flipX;
	bool flipY;

	// palette stuff
	uint currentPalette;

	// color stuff
	Color blend;

	// it's all stuff
	// animation stuff
	AnimationMode mode;
	float betaFrame;

	string _resourceName;
	string _defaultSprite;

	SpritesheetTexture _spritesheet;

	Vec4 _realBlend;

	#endregion

    /// <summary>
    ///     Creates a new IndexedColorGraphic.
    /// </summary>
    /// <param name="resource">The name of the sprite file.</param>
    /// <param name="spriteName">
    ///     The sprite to initialize the IndexedColorGraphic with. This will be the starting sprite it
    ///     uses.
    /// </param>
    /// <param name="position">Where the sprite is located.</param>
    /// <param name="depth">The depth of the sprite.</param>
    public SpriteGraphic (string resource, string spriteName, Vector2 position, int depth) {
		_resourceName = resource;
		_defaultSprite = spriteName;

		texture = TextureManager.Instance.UseSpritesheet(resource);

		// cast now to avoid casting in the future
		_spritesheet = (SpritesheetTexture)texture;
		sprite = new Sprite(texture.Image);
		_position = position;
		sprite.Position = _position.Vector2f;
		_depth = depth;
		_rotation = 0f;
		scale = new Vector2f(1f, 1f);
		SetSprite(spriteName);
		_spritesheet.CurrentPalette = currentPalette;
		blend = Color.White;
		_realBlend = new Vec4(blend);
		//multiply by default
		ColorBlendMode = ColorBlendMode.Multiply;
		RenderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, PaletteShader);
		AnimationEnabled = true;
		Visible = true;
	}

    /// <summary>
    ///     Creates a new IndexedColorGraphic.
    /// </summary>
    /// <param name="texture">The texture to use with the IndexedColorGraphic.</param>
    /// <param name="spriteName">
    ///     The sprite to initialize the IndexedColorGraphic with. This will be the starting sprite it
    ///     uses.
    /// </param>
    /// <param name="position">Where the sprite is located.</param>
    /// <param name="depth">The depth of the sprite.</param>
    public SpriteGraphic (SpritesheetTexture texture, string spriteName, Vector2 position, int depth) {
		this.texture = texture;
		_spritesheet = (SpritesheetTexture)this.texture;
		sprite = new Sprite(this.texture.Image);
		_position = position;
		sprite.Position = _position.Vector2f;
		_depth = depth;
		_rotation = 0f;
		scale = new Vector2f(1f, 1f);
		SetSprite(spriteName);
		_spritesheet.CurrentPalette = currentPalette;
		blend = Color.White;
		_realBlend = new Vec4(blend);
		//multiply by default
		ColorBlendMode = ColorBlendMode.Multiply;
		RenderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, PaletteShader);
		AnimationEnabled = true;
		Visible = true;
	}

    /// <summary>
    ///     Creates a new IndexedColorGraphic.
    /// </summary>
    /// <param name="resource">The name of the sprite file.</param>
    /// <param name="spriteName">
    ///     The sprite to initialize the IndexedColorGraphic with. This will be the starting sprite it
    ///     uses.
    /// </param>
    /// <param name="position">Where the sprite is located.</param>
    /// <param name="depth">The depth of the sprite.</param>
    /// <param name="palette">
    ///     The palette to initialize the IndexedColorGraphic with. This will be the starting palette it
    ///     uses.
    /// </param>
    public SpriteGraphic (string resource, string spriteName, Vector2 position, int depth, uint palette) {
		_resourceName = resource;
		_defaultSprite = spriteName;

		texture = TextureManager.Instance.UseSpritesheet(resource);
		sprite = new Sprite(texture.Image);
		_spritesheet = (SpritesheetTexture)texture;

		_position = position;
		sprite.Position = _position.Vector2f;

		_depth = depth;

		_rotation = 0f;

		scale = new Vector2f(1f, 1f);

		SetSprite(spriteName);

		currentPalette = palette;
		_spritesheet.CurrentPalette = palette;

		blend = Color.White;
		_realBlend = new Vec4(blend);

		//multiply by default
		ColorBlendMode = ColorBlendMode.Multiply;
		RenderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, PaletteShader);
		AnimationEnabled = true;

		Visible = true;
	}

    /// <summary>
    /// Sets the sprite with the given name.
    /// </summary>
    /// <param name="name">The name of the sprite.</param>
    public void SetSprite(string name)
    {
	    SetSprite(name, true);
    }

	/// <summary>
	/// Sets the sprite for the object.
	/// </summary>
	/// <param name="name">The name of the sprite.</param>
	/// <param name="reset">Whether to reset the sprite animation.</param>
	public void SetSprite(string name, bool reset)
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
		sprite.Origin = spriteDefinition.Offset.Vector2f;
		_origin = spriteDefinition.Offset;

		// Set the texture rectangle for the sprite
		sprite.TextureRect = new IntRect(
			(int)spriteDefinition.Coords.X, 
			(int)spriteDefinition.Coords.Y, 
			(int)spriteDefinition.Bounds.X, 
			(int)spriteDefinition.Bounds.Y
			);
		startTextureRect = sprite.TextureRect;

		// Set the size of the sprite
		Size = new Vector2(sprite.TextureRect.Width, sprite.TextureRect.Height);

		// Set the flip flags for the sprite
		flipX = spriteDefinition.FlipX;
		flipY = spriteDefinition.FlipY;

		// Set the final scale for the sprite
		finalScale.X = flipX ? -scale.X : scale.X;
		finalScale.Y = flipY ? -scale.Y : scale.Y;

		sprite.Scale = finalScale;

		// Set the frames, speeds, and mode for the sprite
		Frames = spriteDefinition.Frames;
		Speeds = spriteDefinition.Speeds;
		mode = spriteDefinition.Mode;

		// Reset the sprite animation if requested
		if (reset)
		{
			frame = 0f;
			betaFrame = 0f;
			speedIndex = 0f;
			speedModifier = 1f;
			return;
		}

		frame %= Frames;
	}
	protected override void IncrementFrame () {
		float frameSpeed = GetFrameSpeed();

		switch (mode) {
		case AnimationMode.Continous:
			frame = (frame + frameSpeed)%Frames;
			break;

		case AnimationMode.ZeroTwoOneThree:
			betaFrame = (betaFrame + frameSpeed)%4f;
			frame = MODE_ONE_FRAMES[(int)betaFrame];
			break;
		}

		speedIndex = (int)frame%speeds.Length;
	}

	public override void Draw (RenderTarget target) {
		if (!_disposed && _visible) {
			if (Frames > 1 && AnimationEnabled) {
				UpdateAnimation();
			}

			sprite.Position = _position.Vector2f;
			sprite.Origin = _origin.Vector2f;
			sprite.Rotation = _rotation;
			finalScale.X = flipX ? -scale.X : scale.X;
			finalScale.Y = flipY ? -scale.Y : scale.Y;
			sprite.Scale = finalScale;
			_spritesheet.CurrentPalette = currentPalette;

			PaletteShader.SetUniform("image", texture.Image);
			PaletteShader.SetUniform("palette", _spritesheet.Palette);
			PaletteShader.SetUniform("palIndex", _spritesheet.CurrentPaletteFloat);
			//Debug.Log($"Current palette is {currentPalette}. Texture's palette is {((IndexedTexture)this.texture).CurrentPalette}. Float palette is {((IndexedTexture)this.texture).CurrentPaletteFloat} & the palette max is {((IndexedTexture)this.texture).PaletteCount}. {(float)CurrentPalette/ (float)((IndexedTexture)this.texture).PaletteCount} ");
			PaletteShader.SetUniform("palSize", _spritesheet.PaletteSize);
			PaletteShader.SetUniform("blend", _realBlend);
			PaletteShader.SetUniform("blendMode", (float)ColorBlendMode);
			//IndexedColorGraphic.INDEXED_COLOR_SHADER.SetUniform("time", time.ElapsedTime.AsSeconds());
			if (!_disposed) {
				target.Draw(sprite, RenderStates);
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
		int hashCode = sprite.GetHashCode();

		// Retrieve the sprite definition from the spritesheet using the hash code
		return _spritesheet.GetSpriteDefinition(hashCode);
	}

	/// <summary>
	/// Clones the SpriteGraphic object.
	/// </summary>
	/// <returns>A new SpriteGraphic object if _resourceName is not null, else null.</returns>
	public SpriteGraphic Clone()
	{
		if (_resourceName != null)
		{
			return new SpriteGraphic(_resourceName, _defaultSprite, _position, _depth, currentPalette);
		}

		return null;
	}
}
