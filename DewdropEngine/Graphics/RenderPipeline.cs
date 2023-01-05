using DewDrop.Utilities;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DewDrop.Graphics
{
    /// <summary>
    /// A render pipeline sorts renderables by their depth. Renderables with a higher depth are sorted over those with a lower depth. 
    /// Additionally (and most importantly) it allows renderables to draw onto a render target.
    /// </summary>
    public class RenderPipeline
    {
        private class RenderableComparer : IComparer<Renderable>
        {
            //used for uids
            private RenderPipeline pipeline;

            /// <summary>
            /// Creates a new renderable comparer using a renderpipeline
            /// </summary>
            /// <param name="pipeline">The render pipeline to use</param>
            public RenderableComparer(RenderPipeline pipeline)
            {
                this.pipeline = pipeline;
            }

            // i'll break this down
            //
            // if our depths aren't equal to each other
            // then subtract their depths to see which one is greater
            //
            // but if they are not
            // then subtract their ids to see which one is greater.
            public int Compare(Renderable x, Renderable y) => x.Depth != y.Depth ? x.Depth - y.Depth : this.pipeline.renderableIds[y] - this.pipeline.renderableIds[x];
        }

        #region Properties
        /// <summary>
        /// The render target that this render pipeline is targeting.
        /// </summary>
        public RenderTarget Target
        {
            get
            {
                return this.target;
            }
        }
        #endregion

        #region Fields
        // the render target to target. this what every renderable is drawing to
        private RenderTarget target;

        // the renderables in this mothafuckin' pipeline!
        private List<Renderable> renderables;
        
        // stacks to determine what renderables to add and remove
        private Stack<Renderable> renderablesToAdd;
        private Stack<Renderable> renderablesToRemove;

        // called through Update(), if true then we need to use RenderableComparer to sort our list of renderables
        private bool needToSort;

        // compares renderables
        private RenderableComparer depthCompare;

        // when we can't sort by depth, we sort by renderable ids. this is determined when a renderable is added
        private Dictionary<Renderable, int> renderableIds;

        // how many renderables this pipeline is rendering rendering 
        private int renderableCount;

        private FloatRect viewRect;

        private FloatRect renderableRect;
        #endregion 

        /// <summary>
        /// Creates a new RenderPipeline using a render target
        /// </summary>
        /// <param name="target">The render target to render to</param>
        public RenderPipeline(RenderTarget target)
        {
            // set target
            this.target = target;

            // create list of renderables
            this.renderables = new List<Renderable>();
            
            // create stack of renderables (wowie!)
            this.renderablesToAdd = new Stack<Renderable>();
            this.renderablesToRemove = new Stack<Renderable>();
          
            this.renderableIds = new Dictionary<Renderable, int>();
            
            this.depthCompare = new RenderableComparer(this);

            this.viewRect = new FloatRect();
            this.renderableRect = new FloatRect();
        }

        /// <summary>
        /// Adds a renderable object to the stack of objects to render
        /// </summary>
        /// <param name="renderable">The renderable to add</param>
        public void Add(Renderable renderable)
        {
            // if we don't already have this renderable in this pipeline
            if (!this.renderables.Contains(renderable))
            {
                this.renderablesToAdd.Push(renderable);
                return;
            }
            Debug.LogError("Tried to add renderable that already exists in the RenderPipeline.", null);
        }

        /// <summary>
        /// Adds a list of renderables.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="renderablesToAdd">Renderables to add</param>
        public void AddAll<T>(IList<T> renderablesToAdd) where T : Renderable
        {
            int count = renderablesToAdd.Count;
            for (int i = 0; i < count; i++)
            {
                this.Add(renderablesToAdd[i]);
            }
        }

        /// <summary>
        /// Removes a renderable.
        /// </summary>
        /// <param name="renderable">The renderable to remove.</param>
        public void Remove(Renderable renderable)
        {
            if (renderable != null)
            {
                this.renderablesToRemove.Push(renderable);
            }
        }

        /// <summary>
        /// Forces the render pipeline to sort again
        /// </summary>
        public void Update()
        {
            this.needToSort = true;
        }
        
        private void DoAdditions()
        {
            while (this.renderablesToAdd.Count > 0)
            {
                Renderable key = this.renderablesToAdd.Pop();

                // add it to the list
                this.renderables.Add(key);

                // determine its renderable ID
                this.renderableIds.Add(key, this.renderableCount);

                // force our render pipeline to sort renderables after adding
                this.needToSort = true;

                ++this.renderableCount;
            }
        }

        private void DoRemovals()
        {
            while (this.renderablesToRemove.Count > 0)
            {
                Renderable key = this.renderablesToRemove.Pop();
                this.renderables.Remove(key);
                this.renderableIds.Remove(key);

                // unlike DoAdditions, we don't need to force sort our renderables again
                // this is pretty obvious, but you don't need to sort again if something was removed 
            }
        }

        /// <summary>
        /// Executes a function for every renderable with a renderable parameter
        /// </summary>
        /// <param name="forEachFunc">The function to use on each renderable</param>
        public void Each(Action<Renderable> forEachFunc)
        {
            //int count = this.renderables.Count;
            
            renderables.ForEach(x => forEachFunc(x));
            
            /*for (int i = 0; i < count; i++)
            {
                forEachFunc(this.renderables[i]);
            }*/
        }
        public void Clear()
        {
            this.Clear(true);
        }  
        public void Clear(bool dispose)
        {
            this.renderablesToRemove.Clear();
            if (dispose)
            {
                foreach (Renderable renderable in this.renderables)
                {
                    renderable.Dispose();
                }
            }
            this.renderables.Clear();

            if (dispose)
            {
                while (this.renderablesToAdd.Count > 0)
                {
                    this.renderablesToAdd.Pop().Dispose();
                }
            }
            this.renderablesToAdd.Clear();
            renderableIds.Clear();
        }
        public void Draw()
        {
            this.DoAdditions();
            this.DoRemovals();
            if (this.needToSort)
            {
                this.renderables.Sort(depthCompare);
                this.needToSort = false;
            }
            View view = this.target.GetView();

            this.viewRect.Left = view.Center.X - view.Size.X / 2f;
            this.viewRect.Top = view.Center.Y - view.Size.Y / 2f;
            this.viewRect.Width = view.Size.X;
            this.viewRect.Height = view.Size.Y;
            
            int count = this.renderables.Count;
            // go through each renderable
            for (int index = 0; index < count; ++index)
            {
                // get renderable at index
                Renderable renderable = this.renderables[index];
                
                // if the renderable is visible, allow it to draw
                if (renderable.Visible)
                {

                    // fancy code to determine if a renderable is in the view of the game
                    this.renderableRect.Left = renderable.Position.x - renderable.Origin.x;
                    this.renderableRect.Top = renderable.Position.y - renderable.Origin.y;
                    this.renderableRect.Width = renderable.Size.x;
                    this.renderableRect.Height = renderable.Size.y;

                    // if it's in the view of the game, allow that shit to draw baby!
                    if (this.renderableRect.Intersects(this.viewRect))
                    {
                        renderable.Draw(this.target);
                    }
                }
            }
        }


    }
}
