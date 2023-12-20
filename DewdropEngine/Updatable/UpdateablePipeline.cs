namespace DewDrop.Updatable;

/// <summary>
/// Manages a pipeline of IUpdateable objects.
/// </summary>
/// <remarks>
/// This class maintains a list of IUpdateable objects and provides methods to add, remove, and update them.
/// The Update method updates all IUpdateable objects in the pipeline in the order of their priority.
/// </remarks>
public class UpdateablePipeline
{
    /// <summary>
    /// List of IUpdateable objects.
    /// </summary>
    readonly List<IUpdateable> _updateables;

    /// <summary>
    /// Indicates whether the list of IUpdateable objects needs to be sorted.
    /// </summary>
    bool _sort;

    /// <summary>
    /// Initializes a new instance of the UpdateablePipeline class.
    /// </summary>
    public UpdateablePipeline()
    {
        _updateables = new List<IUpdateable>();
    }

    /// <summary>
    /// Adds an IUpdateable object to the pipeline.
    /// </summary>
    /// <param name="updateable">The IUpdateable object to add.</param>
    public void Add(IUpdateable updateable)
    {
        _sort = true;
        _updateables.Add(updateable);
    }

    /// <summary>
    /// Adds a collection of IUpdateable objects to the pipeline.
    /// </summary>
    /// <param name="updateables">The collection of IUpdateable objects to add.</param>
    public void AddAll(IEnumerable<IUpdateable> updateables)
    {
        _sort = true;
        _updateables.AddRange(updateables);
    }

    /// <summary>
    /// Removes a collection of IUpdateable objects from the pipeline.
    /// </summary>
    /// <param name="updateables">The collection of IUpdateable objects to remove.</param>
    public void RemoveAll(IEnumerable<IUpdateable> updateables)
    {
        _updateables.RemoveAll(updateables.Contains);
    }

    /// <summary>
    /// Removes an IUpdateable object from the pipeline.
    /// </summary>
    /// <param name="updateable">The IUpdateable object to remove.</param>
    public void Remove(IUpdateable updateable)
    {
        _updateables.Remove(updateable);
    }

    /// <summary>
    /// Updates all IUpdateable objects in the pipeline.
    /// </summary>
    /// <remarks>
    /// If the list of IUpdateable objects has been modified since the last update, it is sorted by priority before the update.
    /// </remarks>
    public void Update()
    {
        if (_sort)
        {
            _updateables.Sort((x, y) => x.Priority.CompareTo(y.Priority));
            _sort = false;
        }
        foreach (IUpdateable updateable in _updateables)
        {
            updateable.Update();
        }
    }
}