namespace DewDrop.Entities;

public class EntityManager
{
    private readonly List<Entity> _entities;

    // entities we gotta add
    private readonly Stack<Entity> _entitiesToRemove = new();

    // entities we gotta remove
    private readonly Stack<Entity> _entitiesToAdd = new();

    public EntityManager()
    {
        // initialize lists
        _entities = new List<Entity>();
        _entitiesToRemove = new Stack<Entity>();
        _entitiesToAdd = new Stack<Entity>();
    }

    /// <summary>
    ///     Updates all entities. Adds entities waiting to be added and removes entities waiting to be removed.
    ///     Also calls Awake() on entities that have just been added.
    /// </summary>
    public void Update()
    {
        // prune entities marked for removal
        while (_entitiesToRemove.Count > 0)
        {
            Entity entity = _entitiesToRemove.Pop();
            _entities.Remove(entity);
        }

        _entitiesToRemove.Clear();

        // add entities marked for addition
        while (_entitiesToAdd.Count > 0)
        {
            Entity entity = _entitiesToAdd.Pop();
            _entities.Add(entity);
            entity.Awake();
        }

        _entitiesToAdd.Clear();

        foreach (Entity entity in _entities)
        {
            entity.Update();
        }
    }

    #region Addition

    /// <summary>
    ///     Adds an entity to the collection of entities to be processed.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public void AddEntity(Entity entity)
    {
        _entitiesToAdd.Push(entity);
    }

    /// <summary>
    ///     Adds multiple entities to the collection of entities to be processed.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    public void AddEntities(IEnumerable<Entity> entities)
    {
        foreach (Entity entity in entities)
        {
            _entitiesToAdd.Push(entity);
        }
    }

    #endregion

    #region Removal

    /// <summary>
    ///     Removes an entity from the collection of entities to be processed.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    public void RemoveEntity(Entity entity)
    {
        _entitiesToRemove.Push(entity);
    }

    /// <summary>
    ///     Removes multiple entities from the collection of entities to be processed.
    /// </summary>
    /// <param name="entities">The collection of entities to remove.</param>
    public void RemoveEntities(IEnumerable<Entity> entities)
    {
        foreach (Entity entity in entities)
        {
            _entitiesToRemove.Push(entity);
        }
    }

    #endregion


    #region Finding methods

    /// <summary>
    ///     Finds an entity by its name.
    /// </summary>
    /// <param name="name">The name of the entity to find.</param>
    /// <returns>The found Entity object, or null if not found.</returns>
    public Entity Find(string name)
    {
        return _entities.Find(x => x.Name == name);
    }

    /// <summary>
    ///     Finds an entity based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate used to determine the entity to find.</param>
    /// <returns>The found Entity object, or null if not found.</returns>
    public Entity Find(Predicate<Entity> predicate)
    {
        return _entities.Find(predicate);
    }

    /// <summary>
    ///     Finds entities of a specific type and populates the provided list.
    /// </summary>
    /// <typeparam name="T">The type of entities to find. Must be a derived class of Entity.</typeparam>
    /// <param name="entities">The list to populate with the found entities.</param>
    public void Find<T>(out List<T> entities) where T : Entity
    {
        entities = new List<T>();
        foreach (Entity entity in _entities)
        {
            if (entity is T t)
            {
                entities.Add(t);
            }
        }
    }

    #endregion


    public void ClearEntities()
    {
        foreach (Entity entity in _entities)
        {
            entity.Dispose();
        }

        _entitiesToRemove.Clear();
        _entitiesToAdd.Clear();
        _entities.Clear();
    }
}