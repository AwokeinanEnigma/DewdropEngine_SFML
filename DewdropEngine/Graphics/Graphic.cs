#region

using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;

#endregion

namespace DewDrop.Graphics;

public class Graphic : AnimatedRenderable
{
    #region Properties

    /// <summary>
    ///     The color of this graphic
    /// </summary>
    public virtual Color Color
    {
        get => sprite.Color;
        set => sprite.Color = value;
    }
    /// <summary>
    ///     The scale of the graphic, dictates how big it is.
    /// </summary>
    public virtual Vector2f Scale
    {
        get => scale;
        set => scale = value;
    }
    /// <summary>
    ///     Keeps information about the texture.
    /// </summary>
    public IntRect TextureRect
    {
        get => sprite.TextureRect;
        set
        {
            sprite.TextureRect = value;
            Size = new Vector2(value.Width, value.Height);
            startTextureRect = value;
            frame = 0f;
        }
    }
    /// <summary>
    ///     The texture assosicated with the graphic
    /// </summary>
    public ITexture Texture => texture;

    #endregion

    #region Fields

    protected Sprite sprite;
    protected ITexture texture;
    protected IntRect startTextureRect;
    protected Vector2f scale;
    protected Vector2f finalScale;

    #endregion

    /// <summary>
    ///     Creates a new graphic
    /// </summary>
    /// <param name="resource">The name of the IDewdropTexture to pull from the TextureManager</param>
    /// <param name="position">the position of the sprite relative to the graphic</param>
    /// <param name="textureRect">Information about the texture's integer coordinates</param>
    /// <param name="origin">Origin of the texture relative to the graphic</param>
    /// <param name="depth">The depth of this object</param>
    public Graphic(string resource, Vector2 position, IntRect textureRect, Vector2 origin, int depth)
    {
        // this.texture = TextureManager.Instance.UseUnprocessed(resource);
        sprite = new Sprite(texture.Image);
        sprite.TextureRect = textureRect;
        startTextureRect = textureRect;
        _position = position;
        Origin = origin;
        Size = new Vector2(textureRect.Width, textureRect.Height);
        Depth = depth;
        Rotation = 0f;
        scale = new Vector2f(1f, 1f);
        finalScale = scale;
        speedModifier = 1f;
        sprite.Position = _position.Vector2f;
        sprite.Origin = Origin.Vector2f;
        speeds = new[]
        {
            1f
        };
        speedIndex = 0f;
        Visible = true;
    }

    protected Graphic()
    {
    }

    /// <summary>
    ///     Pushes the animation forward
    /// </summary>
    protected void UpdateAnimation()
    {
        int num = startTextureRect.Left + (int)frame * (int)_size.X;
        int left = num % (int)sprite.Texture.Size.X;
        int top = startTextureRect.Top + num / (int)sprite.Texture.Size.X * (int)_size.Y;
        sprite.TextureRect = new IntRect(left, top, (int)_size.X, (int)_size.Y);
        if (frame + GetFrameSpeed() >= Frames)
        {
            AnimationComplete();
        }

        speedIndex = (speedIndex + GetFrameSpeed()) % speeds.Length;
        IncrementFrame();
    }

    /// <summary>
    ///     Pushes the current frame forward
    /// </summary>
    protected virtual void IncrementFrame()
    {
        frame = (frame + GetFrameSpeed()) % Frames;
    }

    /// <summary>
    ///     Returns the animation speed
    /// </summary>
    /// <returns>The animation speed</returns>
    protected float GetFrameSpeed()
    {
        return speeds[(int)speedIndex % speeds.Length] * speedModifier;
    }

    public void Translate(Vector2f v)
    {
        Translate(v.X, v.Y);
    }

    /// <summary>
    ///     Moves the position forward by the specified X and Y
    /// </summary>
    /// <param name="x">The X to move forward by</param>
    /// <param name="y">The Y to move forward by</param>
    public virtual void Translate(float x, float y)
    {
        _position.X += x;
        _position.Y += y;
    }

    /// <summary>
    ///     Draws the graphic
    /// </summary>
    /// <param name="target">The RenderTarget to draw to</param>
    public override void Draw(RenderTarget target)
    {
        if (_visible)
        {
            if (Frames > 0)
            {
                UpdateAnimation();
            }

            sprite.Position = _position.Vector2f;
            sprite.Origin = Origin.Vector2f;
            sprite.Rotation = Rotation;
            finalScale = scale;
            sprite.Scale = finalScale;
            target.Draw(sprite);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing && sprite != null)
            {
                sprite.Dispose();
            }

            TextureManager.Instance.Unuse(texture);

        }

        _disposed = true;
    }
}