#region

using System.Diagnostics;
using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Graphics;

/// <summary>
///     A render pipeline sorts IRenderables by their depth. IRenderables with a higher depth are rendered over those with
///     a
///     lower depth.
///     Additionally (and most importantly) it allows IRenderables to draw onto a render target.
/// </summary>
public class RenderPipeline
{
    private class IRenderableComparer : IComparer<IRenderable>
    {
        //used for uids
        private RenderPipeline pipeline;

        /// <summary>
        ///     Creates a new IRenderable comparer using a renderpipeline
        /// </summary>
        /// <param name="pipeline">The render pipeline to use</param>
        public IRenderableComparer(RenderPipeline pipeline)
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
        public int Compare(IRenderable x, IRenderable y)
        {
            return x.Depth != y.Depth ? x.Depth - y.Depth : pipeline.IRenderableIds[y] - pipeline.IRenderableIds[x];
        }
    }

    #region Properties

    /// <summary>
    ///     The render target that this render pipeline is targeting.
    /// </summary>
    public RenderTarget Target => target;

    #endregion

    #region Fields

    // the render target to target. this what every IRenderable is drawing to
    private RenderTarget target;

    // the IRenderables in this mothafuckin' pipeline!
    private List<IRenderable> IRenderables;

    // stacks to determine what IRenderables to add and remove
    private Stack<IRenderable> IRenderablesToAdd;
    private Stack<IRenderable> IRenderablesToRemove;

    // called through Update(), if true then we need to use IRenderableComparer to sort our list of IRenderables
    private bool needToSort;

    // compares IRenderables
    private IRenderableComparer depthCompare;

    // when we can't sort by depth, we sort by IRenderable ids. this is determined when a IRenderable is added
    private Dictionary<IRenderable, int> IRenderableIds;

    // how many IRenderables this pipeline is rendering rendering 
    private int IRenderableCount;

    private FloatRect viewRect;

    private FloatRect IRenderableRect;

    #endregion

    /// <summary>
    ///     Creates a new RenderPipeline using a render target
    /// </summary>
    /// <param name="target">The render target to render to</param>
    public RenderPipeline(RenderTarget target)
    {
        // set target
        this.target = target;

        // create list of IRenderables
        IRenderables = new List<IRenderable>();

        // create stack of IRenderables (wowie!)
        IRenderablesToAdd = new Stack<IRenderable>();
        IRenderablesToRemove = new Stack<IRenderable>();

        IRenderableIds = new Dictionary<IRenderable, int>();

        depthCompare = new IRenderableComparer(this);

        viewRect = new FloatRect();
        IRenderableRect = new FloatRect();
    }

    /// <summary>
    ///     Adds a IRenderable object to the stack of objects to render
    /// </summary>
    /// <param name="IRenderable">The IRenderable to add</param>
    public void Add(IRenderable IRenderable)
    {
        // if we don't already have this IRenderable in this pipeline
        if (IRenderables.Contains(IRenderable))
        {
            DDDebug.LogError("Tried to add IRenderable that already exists in the RenderPipeline.", null);
            return;
        }

        IRenderablesToAdd.Push(IRenderable);
    }

    /// <summary>
    ///     Adds a list of IRenderables.
    /// </summary>
    /// <param name="IRenderablesToAdd">IRenderables to add</param>
    public void AddAll(List<IRenderable> IRenderablesToAdd)
    {
        IRenderablesToAdd.ForEach(x => Add(x));
    }

    /// <summary>
    ///     Adds a list of IRenderables.
    /// </summary>
    /// <param name="IRenderablesToAdd">IRenderables to add</param>
    public void AddAll(IRenderable[] IRenderablesToAdd)
    {
        for (int i = 0; i < IRenderablesToAdd.Length; i++)
        {
            Add(IRenderablesToAdd[i]);
        }
    }

    /// <summary>
    ///     Adds a list of IRenderables
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="addIRenderables">IRenderables to add</param>
    public void AddAll<T>(IList<T> addIRenderables) where T : IRenderable
    {
        int count = addIRenderables.Count;
        for (int i = 0; i < count; i++)
        {
            Add(addIRenderables[i]);
        }
    }

    /// <summary>
    ///     Removes a IRenderable.
    /// </summary>
    /// <param name="IRenderable">The IRenderable to remove.</param>
    public void Remove(IRenderable IRenderable)
    {
        if (IRenderable != null)
        {
            IRenderablesToRemove.Push(IRenderable);
        }
    }

    /// <summary>
    ///     Forces the render pipeline to sort again
    /// </summary>
    public void ForceSort()
    {
        needToSort = true;
    }

    private void DoAdditions()
    {
        while (IRenderablesToAdd.Count > 0)
        {
            // remove the thing from the top of this
            IRenderable key = IRenderablesToAdd.Pop();

            // add it to the list
            IRenderables.Add(key);

            // determine its IRenderable ID
            IRenderableIds.Add(key, IRenderableCount);

            // force our render pipeline to sort IRenderables after adding
            needToSort = true;

            ++IRenderableCount;
        }
    }

    private void DoRemovals()
    {
        while (IRenderablesToRemove.Count > 0)
        {
            IRenderable key = IRenderablesToRemove.Pop();
            IRenderables.Remove(key);
            IRenderableIds.Remove(key);

            // unlike DoAdditions, we don't need to force sort our IRenderables again
            // this is pretty obvious, but you don't need to sort again if something was removed 
        }
    }

    /// <summary>
    ///     Executes a function for every IRenderable with a IRenderable parameter
    /// </summary>
    /// <param name="forEachFunc">The function to use on each IRenderable</param>
    public void Each(Action<IRenderable> forEachFunc)
    {
        IRenderables.ForEach(forEachFunc);
    }

    public void Clear()
    {
        Clear(true);
    }

    public void Clear(bool dispose)
    {
        IRenderablesToRemove.Clear();
        if (dispose)
        {
            foreach (IRenderable IRenderable in IRenderables)
            {
                IRenderable.Dispose();
            }
        }

        IRenderables.Clear();

        if (dispose)
        {
            while (IRenderablesToAdd.Count > 0)
            {
                IRenderablesToAdd.Pop().Dispose();
            }
        }

        IRenderablesToAdd.Clear();
        IRenderableIds.Clear();
    }

    public void Draw()
    {
        DoAdditions();
        DoRemovals();
        if (needToSort)
        {
            IRenderables.Sort(depthCompare);
            needToSort = false;
        }

        View view = target.GetView();

        viewRect.Left = view.Center.X - view.Size.X / 2f;
        viewRect.Top = view.Center.Y - view.Size.Y / 2f;
        viewRect.Width = view.Size.X;
        viewRect.Height = view.Size.Y;

        int count = IRenderables.Count;
        // go through each IRenderable
        for (int index = 0; index < count; ++index)
        {

            // get IRenderable at index
            IRenderable iRenderable = IRenderables[index];

            // if the IRenderable is visible, allow it to draw
            if (iRenderable.Visible)
            {

                // fancy code to determine if a IRenderable is in the view of the game
                
                // basically, you can think of the origin as an offset from renderable's position
                IRenderableRect.Left = iRenderable.RenderPosition.x - iRenderable.Origin.x;
                IRenderableRect.Top = iRenderable.RenderPosition.y - iRenderable.Origin.y;
                IRenderableRect.Width = iRenderable.Size.x;
                IRenderableRect.Height = iRenderable.Size.y;

                // if it's in the view of the game, allow that shit to draw baby!
                if (true)//IRenderableRect.Intersects(viewRect))
                {
                    iRenderable.Draw(target);
                    iRenderable.IsBeingDrawn = true;
                }
                else
                {
                    iRenderable.IsBeingDrawn = false;
                }
            }
            else
            {
                iRenderable.IsBeingDrawn = false;
            }
        }
    }
}