using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;

namespace DewDrop.Graphics
{
    public class Graphic : AnimatedRenderable
    {
        #region Properties
        
        /// <summary>
        /// The color of this graphic
        /// </summary>
        public virtual Color Color
        {
            get
            {
                return this.sprite.Color;
            }
            set
            {
                this.sprite.Color = value;
            }
        }
        /// <summary>
        /// The scale of the graphic, dictates how big it is.
        /// </summary>
        public virtual Vector2f Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                this.scale = value;
            }
        }
        /// <summary>
        /// Keeps information about the texture.
        /// </summary>
        public IntRect TextureRect
        {
            get
            {
                return this.sprite.TextureRect;
            }
            set
            {
                this.sprite.TextureRect = value;
                this.Size = new Vector2(value.Width, value.Height);
                this.startTextureRect = value;
                this.frame = 0f;
            }
        }
        /// <summary>
        /// The texture assosicated with the graphic
        /// </summary>
        public ITexture Texture
        {
            get
            {
                return this.texture;
            }
        }

        #endregion

        #region Fields

        protected Sprite sprite;
        protected ITexture texture;
        protected IntRect startTextureRect;
        protected Vector2f scale;
        protected Vector2f finalScale;

        #endregion
        
        /// <summary>
        /// Creates a new graphic
        /// </summary>
        /// <param name="resource">The name of the IDewdropTexture to pull from the TextureManager</param>
        /// <param name="position">the position of the sprite relative to the graphic</param>
        /// <param name="textureRect">Information about the texture's integer coordinates</param>
        /// <param name="origin">Origin of the texture relative to the graphic</param>
        /// <param name="depth">The depth of this object</param>
        public Graphic(string resource, Vector2 position, IntRect textureRect, Vector2 origin, int depth)
        {
           // this.texture = TextureManager.Instance.UseUnprocessed(resource);
            this.sprite = new Sprite(this.texture.Image);
            this.sprite.TextureRect = textureRect;
            this.startTextureRect = textureRect;
            _position = position;
            this.Origin = origin;
            this.Size = new Vector2(textureRect.Width, textureRect.Height);
            this.Depth = depth;
            this.Rotation = 0f;
            this.scale = new Vector2f(1f, 1f);
            this.finalScale = this.scale;
            this.speedModifier = 1f;
            this.sprite.Position = _position.Vector2f;
            this.sprite.Origin = this.Origin.Vector2f;
            this.speeds = new float[]
            {
                1f
            };
            this.speedIndex = 0f;
            this.Visible = true;
        }
        protected Graphic()
        {
        }
        /// <summary>
        /// Pushes the animation forward
        /// </summary>
        protected void UpdateAnimation()
        {
            int num = this.startTextureRect.Left + (int)this.frame * (int)this._size.X;
            int left = num % (int)this.sprite.Texture.Size.X;
            int top = this.startTextureRect.Top + num / (int)this.sprite.Texture.Size.X * (int)this._size.Y;
            this.sprite.TextureRect = new IntRect(left, top, (int)this._size.X, (int)this._size.Y);
            if (this.frame + this.GetFrameSpeed() >= Frames)
            {
                base.AnimationComplete();
            }
            this.speedIndex = (this.speedIndex + this.GetFrameSpeed()) % speeds.Length;
            this.IncrementFrame();
        }
        /// <summary>
        /// Pushes the current frame forward
        /// </summary>
        protected virtual void IncrementFrame()
        {
            this.frame = (this.frame + this.GetFrameSpeed()) % Frames;
        }
        /// <summary>
        /// Returns the animation speed
        /// </summary>
        /// <returns>The animation speed</returns>
        protected float GetFrameSpeed()
        {
            return this.speeds[(int)this.speedIndex % this.speeds.Length] * this.speedModifier;
        }
        public void Translate(Vector2f v)
        {
            this.Translate(v.X, v.Y);
        }
        /// <summary>
        /// Moves the position forward by the specified X and Y
        /// </summary>
        /// <param name="x">The X to move forward by</param>
        /// <param name="y">The Y to move forward by</param>
        public virtual void Translate(float x, float y)
        {
            _position.X = _position.X + x;
            _position.Y = _position.Y + y;
        }
        /// <summary>
        /// Draws the graphic
        /// </summary>
        /// <param name="target">The RenderTarget to draw to</param>
        public override void Draw(RenderTarget target)
        {
            if (this._visible)
            {
                if (base.Frames > 0)
                {
                    this.UpdateAnimation();
                }
                this.sprite.Position = _position.Vector2f;
                this.sprite.Origin = this.Origin.Vector2f;
                this.sprite.Rotation = this.Rotation;
                this.finalScale = this.scale;
                this.sprite.Scale = this.finalScale;
                target.Draw(this.sprite);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing && this.sprite != null)
                {
                    this.sprite.Dispose();
                }
                TextureManager.Instance.Unuse(this.texture);
            }
            this._disposed = true;
        }

    }
}
