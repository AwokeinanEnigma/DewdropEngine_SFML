#region

using DewDrop.Utilities;
using DewDrop.Wren;
using SFML.Graphics;
// ReSharper disable ForCanBeConvertedToForeach

#endregion

namespace DewDrop.Graphics;

/// <summary>
///     A render pipeline sorts IRenderables by their depth. IRenderables with a higher depth are rendered over those with alower depth.
///     Additionally (and most importantly) it allows IRenderables to draw onto a render target.
/// </summary>
public class RenderPipeline {
	class RenderableComparer : IComparer<IRenderable> {
		//used for uids
		readonly RenderPipeline _pipeline;

		/// <summary>
		///     Creates a new IRenderable comparer using a renderpipeline
		/// </summary>
		/// <param name="pipeline">The render pipeline to use</param>
		public RenderableComparer (RenderPipeline pipeline) {
			this._pipeline = pipeline;
		}

		// i'll break this down
		//
		// if our depths aren't equal to each other
		// then subtract their depths to see which one is greater
		//
		// but if they are not
		// then subtract their ids to see which one is greater.
		public int Compare (IRenderable x, IRenderable y) {
			return x.Depth != y.Depth ? x.Depth - y.Depth : _pipeline._renderableIds[y] - _pipeline._renderableIds[x];
		}
	}

	#region Properties

    /// <summary>
    ///     The render target that this render pipeline is targeting.
    /// </summary>
    public RenderTarget Target { get; }

	#endregion

	#region Fields

	// the render target to target. this what every IRenderable is drawing to

	// the IRenderables in this mothafuckin' pipeline!
	readonly List<IRenderable> _renderables;

	// stacks to determine what IRenderables to add and remove
	readonly Stack<IRenderable> _renderablesToAdd;
	readonly Stack<IRenderable> _renderablesToRemove;

	// called through Update(), if true then we need to use IRenderableComparer to sort our list of IRenderables
	bool _needToSort;

	// compares IRenderables
	readonly RenderableComparer _depthCompare;

	// when we can't sort by depth, we sort by IRenderable ids. this is determined when a IRenderable is added
	readonly Dictionary<IRenderable, int> _renderableIds;

	// how many IRenderables this pipeline is rendering rendering 
	int _renderableCount;

	FloatRect _viewRect;
	FloatRect _renderableRect;
	View _view;

	#endregion
	/// <summary>
    ///     Creates a new RenderPipeline using a render target
    /// </summary>
    /// <param name="target">The render target to render to</param>
    public RenderPipeline (RenderTarget target) {
		// set target
		Target = target;

		// create list of IRenderables
		_renderables = new List<IRenderable>();

		// create stack of IRenderables (wowie!)
		_renderablesToAdd = new Stack<IRenderable>();
		_renderablesToRemove = new Stack<IRenderable>();

		_renderableIds = new Dictionary<IRenderable, int>();

		_depthCompare = new RenderableComparer(this);

		_viewRect = new FloatRect();
		_renderableRect = new FloatRect();
	}

    /// <summary>
    ///     Adds a IRenderable object to the stack of objects to render
    /// </summary>
    /// <param name="renderable">The IRenderable to add</param>
    public void Add (IRenderable renderable) {
		// if we don't already have this IRenderable in this pipeline
		if (_renderables.Contains(renderable)) {
			Outer.LogError("Tried to add IRenderable that already is already being rendered in the RenderPipeline.", null);
			return;
		}

		_renderablesToAdd.Push(renderable);
	}
    
    public void Add (BasicRenderableWrapper renderable) {
	    if (_renderables.Contains(renderable.Renderable)) {
		    Outer.LogError("Tried to add IRenderable that already is already being rendered in the RenderPipeline.", null);
		    return;
	    }
		Add(renderable.Renderable);
	}

    /// <summary>
    ///     Adds a list of IRenderables.
    /// </summary>
    /// <param name="renderablesToAdd">IRenderables to add</param>
    public void AddAll (List<IRenderable> renderablesToAdd) {
		renderablesToAdd.ForEach(Add);
	}

    /// <summary>
    ///     Adds a list of IRenderables.
    /// </summary>
    /// <param name="renderablesToAdd">IRenderables to add</param>
    public void AddAll (IRenderable[] renderablesToAdd) {
		for (int i = 0; i < renderablesToAdd.Length; i++) {
			Add(renderablesToAdd[i]);
		}
	}

    /// <summary>
    ///     Adds a list of IRenderables
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="addIRenderables">IRenderables to add</param>
    public void AddAll<T> (IList<T> addIRenderables) where T : IRenderable {
		int count = addIRenderables.Count;
		for (int i = 0; i < count; i++) {
			Add(addIRenderables[i]);
		}
	}

    /// <summary>
    ///     Removes a IRenderable.
    /// </summary>
    /// <param name="renderable">The IRenderable to remove.</param>
    public void Remove (IRenderable renderable) {
		if (renderable != null) {
			_renderablesToRemove.Push(renderable);
		}
	}

    /// <summary>
    ///     Forces the render pipeline to sort again
    /// </summary>
    public void ForceSort () {
		_needToSort = true;
	}

	void DoAdditions () {
		while (_renderablesToAdd.Count > 0) {
			// remove the thing from the top of this
			IRenderable key = _renderablesToAdd.Pop();

			// add it to the list
			_renderables.Add(key);

			// determine its IRenderable ID
			_renderableIds.Add(key, _renderableCount);

			// force our render pipeline to sort IRenderables after adding
			_needToSort = true;

			++_renderableCount;
		}
	}

	void DoRemovals () {
		while (_renderablesToRemove.Count > 0) {
			IRenderable key = _renderablesToRemove.Pop();
			_renderables.Remove(key);
			_renderableIds.Remove(key);

			// unlike DoAdditions, we don't need to force sort our IRenderables again
			// this is pretty obvious, but you don't need to sort again if something was removed 
		}
	}

    /// <summary>
    ///     Executes a function for every IRenderable with a IRenderable parameter
    /// </summary>
    /// <param name="forEachFunc">The function to use on each IRenderable</param>
    public void Each (Action<IRenderable> forEachFunc) {
		_renderables.ForEach(forEachFunc);
	}

    public void Clear (bool dispose = true) {
		if (dispose) {
			foreach (IRenderable renderable in _renderablesToRemove) {
				renderable.Dispose();
			}
		}
		_renderablesToRemove.Clear();
		if (dispose) {
			foreach (IRenderable renderable in _renderables) {
				renderable.Dispose();
			}
		}

		_renderables.Clear();

		if (dispose) {
			while (_renderablesToAdd.Count > 0) {
				_renderablesToAdd.Pop().Dispose();
			}
		}

		_renderablesToAdd.Clear();
		_renderableIds.Clear();
	}

	public void Draw () {
		DoAdditions();
		DoRemovals();
		if (_needToSort) {
			_renderables.Sort(_depthCompare);
			_needToSort = false;
		}

		_view = Target.GetView();

		_viewRect.Left = _view.Center.X - _view.Size.X/2f;
		_viewRect.Top = _view.Center.Y - _view.Size.Y/2f;
		_viewRect.Width = _view.Size.X;
		_viewRect.Height = _view.Size.Y;

		int count = _renderables.Count;
		// go through each IRenderable
		for (int index = 0; index < count; ++index) {

			// get IRenderable at index
			IRenderable iRenderable = _renderables[index];

			// if the IRenderable is visible, allow it to draw
			if (iRenderable.Visible) {

				// fancy code to determine if a IRenderable is in the view of the game

				// basically, you can think of the origin as an offset from renderable's position
				_renderableRect.Left = iRenderable.RenderPosition.x - iRenderable.Origin.x;
				_renderableRect.Top = iRenderable.RenderPosition.y - iRenderable.Origin.y;
				_renderableRect.Width = iRenderable.Size.x;
				_renderableRect.Height = iRenderable.Size.y;

				// if it's in the view of the game, allow that shit to draw baby!
				if (_renderableRect.Intersects(_viewRect) || iRenderable.DrawRegardlessOfVisibility) {
					iRenderable.Draw(Target);
					iRenderable.IsBeingDrawn = true;
				} else {
					iRenderable.IsBeingDrawn = false;
				}
			} else {
				iRenderable.IsBeingDrawn = false;
			}
		}
	}
}
