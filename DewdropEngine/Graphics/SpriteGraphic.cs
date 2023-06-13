#region

using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;
using static DewDrop.Graphics.SpriteDefinition;

#endregion

namespace DewDrop.Graphics;

public class SpriteGraphic : Graphic
{
    #region Properties

    /// <summary>
    ///     The current palette this IndexedColorGraphic is using.
    /// </summary>
    public uint CurrentPalette
    {
        get => currentPalette;
        set
        {
            if (currentPalette != value)
            {
                previousPalette = currentPalette;
                currentPalette = value;
            }
        }
    }

    /// <summary>
    ///     The color to overlay the texture with.
    /// </summary>
    public override Color Color
    {
        get => blend;
        set => blend = value;
    }

    /// <summary>
    ///     The mode to use when blending the texture with the color.
    /// </summary>
    public ColorBlendMode ColorBlendMode
    {
        get => blendMode;
        set => blendMode = value;
    }

    /// <summary>
    ///     Self explanatory. The last palette used.
    /// </summary>
    public uint PreviousPalette => previousPalette;

    /// <summary>
    ///     The render states this IndexedColorGraphic is using.
    /// </summary>
    public RenderStates RenderStates => renderStates;

    /// <summary>
    ///     Can this graphic be animated?
    /// </summary>
    public bool AnimationEnabled
    {
        get => _animationEnabled;
        set => _animationEnabled = value;
    }

    #endregion

    #region Fields

    private static readonly int[] MODE_ONE_FRAMES =
    {
        0,
        1,
        0,
        2
    };

    private RenderStates renderStates;

    // this is used for multiple things so it gets a special place in the engine class
    private static readonly Shader PaletteShader = new(EmbeddedResourcesHandler.GetResourceStream("pal.vert"), null, EmbeddedResourcesHandler.GetResourceStream("pal.frag"));

    // fliped stuff
    private bool flipX;
    private bool flipY;

    // palette stuff
    private uint previousPalette;
    private uint currentPalette;

    // color stuff
    private Color blend;
    private ColorBlendMode blendMode;

    // it's all stuff
    // animation stuff
    private AnimationMode mode;
    private float betaFrame;
    private bool _animationEnabled;

    private string _resourceName;
    private string _defaultSprite;

    private SpritesheetTexture _spritesheet;

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
    public SpriteGraphic(string resource, string spriteName, Vector2 position, int depth)
    {
        _resourceName = resource;
        _defaultSprite = spriteName;

        texture = TextureManager.Instance.Use(resource);

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
        //multiply by default
        blendMode = ColorBlendMode.Multiply;
        renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, PaletteShader);
        _animationEnabled = true;
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
    public SpriteGraphic(SpritesheetTexture texture, string spriteName, Vector2 position, int depth)
    {
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
        //multiply by default
        blendMode = ColorBlendMode.Multiply;
        renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, PaletteShader);
        _animationEnabled = true;
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
    public SpriteGraphic(string resource, string spriteName, Vector2 position, int depth, uint palette)
    {
        _resourceName = resource;
        _defaultSprite = spriteName;

        texture = TextureManager.Instance.Use(resource);
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
        //multiply by default
        blendMode = ColorBlendMode.Multiply;
        renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, PaletteShader);
        _animationEnabled = true;

        Visible = true;
    }

    public void SetSprite(string name)
    {
        SetSprite(name, true);
    }

    public void SetSprite(string name, bool reset)
    {
        SpriteDefinition spriteDefinition = _spritesheet.GetSpriteDefinition(name);
        if (!spriteDefinition.IsValid)
        {
            spriteDefinition = _spritesheet.GetDefaultSpriteDefinition();
        }

        sprite.Origin = spriteDefinition.Offset.Vector2f;
        _origin = spriteDefinition.Offset;

        sprite.TextureRect = new IntRect((int)spriteDefinition.Coords.X, (int)spriteDefinition.Coords.Y, (int)spriteDefinition.Bounds.X, (int)spriteDefinition.Bounds.Y);
        startTextureRect = sprite.TextureRect;
        Size = new Vector2(sprite.TextureRect.Width, sprite.TextureRect.Height);

        flipX = spriteDefinition.FlipX;
        flipY = spriteDefinition.FlipY;

        finalScale.X = flipX ? -scale.X : scale.X;
        finalScale.Y = flipY ? -scale.Y : scale.Y;

        sprite.Scale = finalScale;
        Frames = spriteDefinition.Frames;
        Speeds = spriteDefinition.Speeds;
        mode = spriteDefinition.Mode;

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

    protected override void IncrementFrame()
    {
        float frameSpeed = GetFrameSpeed();

        switch (mode)
        {
            case AnimationMode.Continous:
                frame = (frame + frameSpeed) % Frames;
                break;

            case AnimationMode.ZeroTwoOneThree:
                betaFrame = (betaFrame + frameSpeed) % 4f;
                frame = MODE_ONE_FRAMES[(int)betaFrame];
                break;
        }

        speedIndex = (int)frame % speeds.Length;
    }

    public override void Draw(RenderTarget target)
    {
        if (!_disposed && _visible)
        {
            if (Frames > 1 && _animationEnabled)
            {
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
            PaletteShader.SetUniform("blend", new Vec4(blend));
            PaletteShader.SetUniform("blendMode", (float)blendMode);
            //IndexedColorGraphic.INDEXED_COLOR_SHADER.SetUniform("time", time.ElapsedTime.AsSeconds());
            if (!_disposed)
            {
                target.Draw(sprite, renderStates);
            }
        }
    }

    public SpriteDefinition GetSpriteDefinition(string sprite)
    {
        int hashCode = sprite.GetHashCode();
        return _spritesheet.GetSpriteDefinition(hashCode);
    }

    public SpriteGraphic Clone()
    {
        if (_resourceName != null)
        {
            return new SpriteGraphic(_resourceName, _defaultSprite, _position, _depth, currentPalette);
        }

        return null;
    }
}