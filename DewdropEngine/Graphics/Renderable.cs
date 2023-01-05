using DewDrop.Utilities;
using SFML.Graphics;

namespace DewDrop.Graphics
{
    /// <summary>
    /// Defines a renderable and its fields. Renderables are anything that can be rendered within a game. 
    /// </summary>
    public abstract class Renderable : IDisposable
    {
        #region Properties
        /// <summary>
        /// The position of the renderable object.
        /// </summary>
        public virtual Vector2 Position
        {
            get => this.position;
            set
            {
                this.position = value;
            }
        }

        /// <summary>
        /// The origin of the renderable object.
        /// </summary>
		public virtual Vector2 Origin
        {
            get => this.origin;
            set
            {
                this.origin = value;
            }
        }

        /// <summary>
        /// The size of the renderable object.
        /// </summary>
		public virtual Vector2 Size
        {
            get => this.size;
            set
            {
                this.size = value;
            }
        }

        /// <summary>
        /// The depth of the renderable object.
        /// </summary>
        public virtual int Depth
        {
            get => this.depth;
            set
            {
                this.depth = value;
            }
        }

        /// <summary>
        /// Determines whether or not the renderable is visible. Handled by the RenderPipeline
        /// </summary>
        public virtual bool Visible
        {
            get => this.visible;
            set
            {
                this.visible = value;
            }
        }
        #endregion

        #region Fields
        protected Vector2 position;

        protected Vector2 origin;

        protected Vector2 size;

        protected int depth;

        protected bool visible = true;

        protected bool disposed;
        #endregion

        /// <summary>
        /// This is where you draw onto. The render target will handle the actual rendering
        /// </summary>
        /// <param name="target">The render target handling the actual rendering</param>
        public abstract void Draw(RenderTarget target);

        #region Disposable implementation.
        // called by the system to clean up, meaning we can only get rid of unmanaged stuff
        ~Renderable()
        {
            // free only unmanaged resources
            this.Dispose(disposing: false);
        }

        /// <summary>
        /// Here, you must dispose of unmanaged and managed resources.
        /// </summary>
        /// <param name="disposing">If true, then we can get rid of managed and unmanaged resources. If false, we can only get rid of unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.disposed = true;
        }

        /// <summary>
        /// Disposes of all managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //
            this.Dispose(disposing: true);

            // we manually disposed, we don't need to finalize
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
