#region

using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;
using static DewDrop.Graphics.SpriteDefinition;

#endregion

namespace DewDrop.Graphics;

public class AsepriteGraphic : Graphic {
	#region Properties
	/// <summary>
    ///     The render states this IndexedColorGraphic is using.
    /// </summary>
    public RenderStates RenderStates { get; }

    #endregion

	#region Fields

	static readonly int[] MODE_ONE_FRAMES = {
		0, 1, 0, 2
	};
	
	// fliped stuff
	bool flipX;
	bool flipY;
	
	// color stuff
	Color blend;

	// it's all stuff
	// animation stuff
	AnimationMode mode;
	float betaFrame;

	string _resourceName;
	string _defaultSprite;

	AsepriteTexture _spritesheet;

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
    public AsepriteGraphic (string resource, string spriteName, Vector2 position, int depth) {
		_resourceName = resource;
		_defaultSprite = spriteName;

		texture = TextureManager.Instance.UseAsepriteTexture(resource);

		// cast now to avoid casting in the future
		_spritesheet = (AsepriteTexture)texture;
		sprite = new Sprite(texture.Image);
		_position = position;
		sprite.Position = _position.Vector2f;
		_depth = depth;
		_rotation = 0f;
		scale = new Vector2f(1f, 1f);
		SetSprite(spriteName);
		blend = Color.White;
		_realBlend = new Vec4(blend);
		//multiply by default
		RenderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, null);
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
			//if (Frames > 1 && AnimationEnabled) {
				//UpdateAnimation();
			//}

			sprite.Position = _position.Vector2f;
			sprite.Origin = _origin.Vector2f;
			sprite.Rotation = _rotation;
			finalScale.X = flipX ? -scale.X : scale.X;
			finalScale.Y = flipY ? -scale.Y : scale.Y;
			sprite.Scale = finalScale;
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
	public AsepriteGraphic Clone()
	{
		if (_resourceName != null)
		{
			return new AsepriteGraphic(_resourceName, _defaultSprite, _position, _depth);
		}

		return null;
	}

	public void ToFullColorTexture () {
		_spritesheet.ToFullColorTexture();
	}
}
