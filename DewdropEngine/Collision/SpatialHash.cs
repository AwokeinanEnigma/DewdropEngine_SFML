#region

using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;

#endregion
namespace DewDrop.Collision;

/// <summary>
///     Spatial hashing class for handling collision.
/// </summary>
class SpatialHash {
	internal const int CellSize = 256;
	internal const int InitialBucketSize = 4;
	internal const int MaxBucketSize = 512;
	readonly ICollidable[][] _buckets;
	VertexArray _debugGridVerts;
	readonly int _heightInCells;
	readonly bool[] _touches;
	readonly int _widthInCells;

    /// <summary>
    ///     Creates a new spatial hash
    /// </summary>
    /// <param name="width">The width of the space that this spatial hash handles collision in</param>
    /// <param name="height">The height of the space that this spatial hash handles collision in</param>
    public SpatialHash (int width, int height) {
		_widthInCells = (width - 1)/CellSize + 1;
		_heightInCells = (height - 1)/CellSize + 1;
		int num = _widthInCells*_heightInCells;
		_buckets = new ICollidable[num][];
		_touches = new bool[num];
		InitializeDebugGrid();
	}

    /// <summary>
    /// Initializes the debug grid for visualization.
    /// </summary>
    void InitializeDebugGrid()
    {
	    // Calculate the total number of vertices needed
	    uint vertexCount = (uint)((_widthInCells + _heightInCells + 2) * 2);

	    // Create a new VertexArray with the calculated vertex count
	    _debugGridVerts = new VertexArray(PrimitiveType.Lines, vertexCount);

	    // Calculate the width and height of the grid in pixels
	    int gridWidth = _widthInCells * CellSize;
	    int gridHeight = _heightInCells * CellSize;

	    // Add horizontal lines to the debug grid
	    for (uint i = 0; i <= (ulong)_widthInCells; i++)
	    {
		    // Calculate the x-position of the line
		    float x = i * CellSize;

		    // Create vertices for the line
		    _debugGridVerts[i * 2U] = new Vertex(new Vector2f(x, 0f), Color.Blue);
		    _debugGridVerts[i * 2U + 1U] = new Vertex(new Vector2f(x, gridHeight), Color.Blue);
	    }

	    // Calculate the number of vertices in the horizontal lines
	    uint horizontalLineVertexCount = (uint)((_widthInCells + 1) * 2);

	    // Add vertical lines to the debug grid
	    for (uint j = 0; j <= (ulong)_heightInCells; j++)
	    {
		    // Calculate the y-position of the line
		    float y = j * CellSize;

		    // Create vertices for the line
		    _debugGridVerts[horizontalLineVertexCount + j * 2U] = new Vertex(new Vector2f(0f, y), Color.Blue);
		    _debugGridVerts[horizontalLineVertexCount + j * 2U + 1U] = new Vertex(new Vector2f(gridWidth, y), Color.Blue);
	    }
    }
    
	void ClearTouches () {
		Array.Clear(_touches, 0, _touches.Length);
	}
	/// <summary>
	/// Gets the position hash for a given x and y coordinate.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <returns>The position hash.</returns>
	public int GetPositionHash(int x, int y)
	{
		// Calculate the x and y indices
		int xIndex = x / CellSize;
		int yIndex = y / CellSize;
		// Calculate the position hash
		return xIndex + yIndex * _widthInCells;
	}

	#region Buckets

		/// <summary>
	/// Inserts a collidable object into the spatial hash bucket
	/// </summary>
	/// <param name="hash">The hash value for the bucket</param>
	/// <param name="collidable">The collidable object to insert</param>
	void BucketInsert(int hash, ICollidable collidable)
	{
		int num = -1;
		ICollidable[] array = _buckets[hash];

		// If the bucket is empty, create a new array for the bucket
		if (array == null)
		{
			_buckets[hash] = new ICollidable[4];
			array = _buckets[hash];
		}

		// Check each element in the bucket
		for (int i = 0; i < array.Length; i++)
		{
			// If the collidable object is already in the bucket, return
			if (array[i] == collidable)
			{
				return;
			}

			// If an empty slot is found, record its index
			if (num < 0 && array[i] == null)
			{
				num = i;
			}
		}

		// If an empty slot was found, insert the collidable object
		if (num >= 0)
		{
			array[num] = collidable;
			return;
		}

		int num2 = array.Length;

		// If the array size can be doubled without exceeding the maximum bucket size, resize the array and insert the collidable object
		if (num2 * 2 <= MaxBucketSize)
		{
			Array.Resize(ref array, num2 * 2);
			array[num2] = collidable;
			_buckets[hash] = array;
			return;
		}

		// Otherwise, throw an exception
		string message = string.Format("Cannot insert more than {0} collidables into a single bucket.", MaxBucketSize);
		throw new InvalidOperationException(message);
	}

	/// <summary>
	/// Removes a collidable object from the bucket at the specified hash.
	/// </summary>
	/// <param name="hash">The hash of the bucket to remove from.</param>
	/// <param name="collidable">The collidable object to remove.</param>
	/// <param name="log">Flag indicating whether to log the removal.</param>
	void BucketRemove(int hash, ICollidable collidable, bool log)
	{
		// Get the array of collidables at the specified hash
		ICollidable[] array = _buckets[hash];

		// Check if the array is not null
		if (array != null)
		{
			// Iterate over the array
			for (int i = 0; i < array.Length; i++)
			{
				// Check if the current collidable is equal to the one to be removed
				if (array[i] == collidable)
				{
					// Check if logging is enabled
					if (log)
					{
						// Log the removal
						Console.WriteLine($"{collidable} removed!");
					}

					// Set the current array element to null
					array[i] = null;

					// Exit the loop and function
					return;
				}
			}
		}
	}

	/// <summary>
	/// Inserts a collidable object into the grid.
	/// </summary>
	/// <param name="collidable">The collidable object to insert.</param>
	public void Insert (ICollidable collidable) {
		// Clear the touches before inserting the new collidable
		ClearTouches();

		// Calculate the number of cells in the X and Y directions that the collidable occupies
		int numXCells = ((int)collidable.AABB.Size.X - 1)/CellSize + 1;
		int numYCells = ((int)collidable.AABB.Size.Y - 1)/CellSize + 1;

		// Iterate over each cell that the collidable occupies
		for (int i = 0; i <= numYCells; i++) {
			// Calculate the Y coordinate of the current cell
			int y = i == numYCells
				? (int)(collidable.Position.Y + collidable.AABB.Position.Y) + (int)collidable.AABB.Size.Y
				: (int)(collidable.Position.Y + collidable.AABB.Position.Y) + CellSize*i;

			for (int j = 0; j <= numXCells; j++) {
				// Calculate the X coordinate of the current cell
				int x = j == numXCells
					? (int)(collidable.Position.X + collidable.AABB.Position.X) + (int)collidable.AABB.Size.X
					: (int)(collidable.Position.X + collidable.AABB.Position.X) + CellSize*j;

				// Get the position hash for the current cell
				int positionHash = GetPositionHash(x, y);

				// Check if the position hash is within valid range and the cell has not been touched before
				if (positionHash >= 0 && positionHash < _buckets.Length && !_touches[positionHash]) {
					// Set the touch flag for the cell
					_touches[positionHash] = true;

					// Insert the collidable into the bucket corresponding to the cell
					BucketInsert(positionHash, collidable);
				}
			}
		}
	}

	#endregion



	/// <summary>
	/// Updates the position of a collidable object in the spatial hash.
	/// </summary>
	/// <param name="collidable">The collidable object to update.</param>
	/// <param name="oldPosition">The old position of the collidable object.</param>
	/// <param name="newPosition">The new position of the collidable object.</param>
	public void Update (ICollidable collidable, Vector2f oldPosition, Vector2f newPosition) {
		// Clear the touches
		ClearTouches();

		// Get the AABB of the collidable object
		AABB aabb = collidable.AABB;

		// Calculate the number of cells in the x and y directions
		int num = ((int)aabb.Size.X - 1)/CellSize + 1;
		int num2 = ((int)aabb.Size.Y - 1)/CellSize + 1;

		// Loop through each cell
		for (int i = 0; i <= num2; i++) {
			// Calculate the y position for the old and new positions
			int y = i == num2
				? (int)(oldPosition.Y + aabb.Position.Y) + (int)aabb.Size.Y
				: (int)(oldPosition.Y + aabb.Position.Y) + CellSize*i;

			int y2 = i == num2
				? (int)(newPosition.Y + aabb.Position.Y) + (int)aabb.Size.Y
				: (int)(newPosition.Y + aabb.Position.Y) + CellSize*i;

			// Loop through each cell in the x direction
			for (int j = 0; j <= num; j++) {
				// Calculate the x position for the old and new positions
				int x = j == num
					? (int)(oldPosition.X + aabb.Position.X) + (int)aabb.Size.X
					: (int)(oldPosition.X + aabb.Position.X) + CellSize*j;

				int x2 = j == num
					? (int)(newPosition.X + aabb.Position.X) + (int)aabb.Size.X
					: (int)(newPosition.X + aabb.Position.X) + CellSize*j;

				// Get the position hash for the old and new positions
				int positionHash = GetPositionHash(x, y);
				int positionHash2 = GetPositionHash(x2, y2);

				// Check if the position hashes are within the valid range
				bool flag = positionHash >= 0 && positionHash < _buckets.Length;
				bool flag2 = positionHash2 >= 0 && positionHash2 < _buckets.Length;

				// Check if the old or new position is within a valid cell in the spatial hash
				if ((flag && !_touches[positionHash]) || (flag2 && !_touches[positionHash2])) {
					// Check if the old and new position hashes are different
					if (flag && positionHash != positionHash2) {
						// Remove the collidable object from the old position hash
						BucketRemove(positionHash, collidable, false);
					}

					// Check if the new position hash is different from the old position hash
					if (flag2 && positionHash != positionHash2) {
						// Insert the collidable object into the new position hash
						BucketInsert(positionHash2, collidable);
					}

					// Set the touch flag for the old position hash
					if (flag) {
						_touches[positionHash] = true;
					}

					// Set the touch flag for the new position hash
					if (flag2) {
						_touches[positionHash2] = true;
					}
				}
			}
		}
	}

	/// <summary>
	/// Checks if a given position is colliding with any of the collidables in the buckets.
	/// </summary>
	/// <param name="position">The position to check for collision.</param>
	/// <returns>True if there is no collision, false otherwise.</returns>
	public bool CheckPosition(Vector2 position)
	{
		// Iterate through each bucket
		foreach (ICollidable[] collider0 in _buckets)
		{
			// Iterate through each collidable in the current bucket
			foreach (ICollidable collidable in collider0)
			{
				// Check if the position is within the collidable AABB
				if (!(position.X >= collidable.Position.X + collidable.AABB.Position.X &&
					    position.X < collidable.Position.X + collidable.AABB.Position.X + collidable.AABB.Size.X &&
					    position.Y >= collidable.Position.Y + collidable.AABB.Position.Y &&
					    position.Y < collidable.Position.Y + collidable.AABB.Position.Y + collidable.AABB.Size.Y))
				{
					// If there is a collision, return false
					return false;
				}
			}
		}

		// If no collision is found, return true
		return true;
	}

	/// <summary>
	/// Removes the given collidable from the spatial partitioning data structure.
	/// </summary>
	/// <param name="collidable">The collidable object to remove.</param>
	public void Remove(ICollidable collidable)
	{
		ClearTouches();

		AABB aabb = collidable.AABB;
		int num = ((int)aabb.Size.X - 1) / CellSize + 1;
		int num2 = ((int)aabb.Size.Y - 1) / CellSize + 1;

		// Iterate over each cell in the collidable's AABB
		for (int i = 0; i <= num2; i++)
		{
			int y = i == num2
				? (int)(collidable.Position.Y + aabb.Position.Y) + (int)aabb.Size.Y
				: (int)(collidable.Position.Y + aabb.Position.Y) + CellSize * i;

			for (int j = 0; j <= num; j++)
			{
				int x = j == num
					? (int)(collidable.Position.X + aabb.Position.X) + (int)aabb.Size.X
					: (int)(collidable.Position.X + aabb.Position.X) + CellSize * j;

				int positionHash = GetPositionHash(x, y);

				// Check if the positionHash is within the valid range and it hasn't already been touched
				if (positionHash >= 0 && positionHash < _buckets.Length && !_touches[positionHash])
				{
					_touches[positionHash] = true;
					BucketRemove(positionHash, collidable, false);
				}
			}
		}
	}
	
	/// <summary>
	/// Queries the spatial hash for collidable objects at a specific point.
	/// </summary>
	/// <param name="point">The point to query.</param>
	/// <param name="resultStack">The stack to store the results in.</param>
	public void Query(Vector2 point, Stack<ICollidable> resultStack)
	{
		ClearTouches();
		// Calculate the position hash for the given point
		int positionHash = GetPositionHash((int)point.X, (int)point.Y);

		// Check if the position hash is within valid range and the cell has not been touched before
		if (positionHash < 0 || positionHash >= _buckets.Length || _touches[positionHash])
		{
			return;
		}

		// Get the array of collidable objects at the position hash
		ICollidable[] array = _buckets[positionHash];

		// If the array is not null, add each collidable object to the result stack
		if (array != null)
		{
			foreach (ICollidable collidable in array)
			{
				if (collidable != null)
				{
					resultStack.Push(collidable);
				}
			}
		}
	}
	
	/// <summary>
	/// Queries the spatial hash for collidables that intersect with the given collidable.
	/// </summary>
	/// <param name="collidable">The collidable to query with.</param>
	/// <param name="offset">The offset to apply to the collidable's position.</param>
	/// <param name="resultStack">The stack to store the resulting collidables.</param>
	public void Query(ICollidable collidable, Vector2f offset, Stack<ICollidable> resultStack)
	{
		ClearTouches();

		AABB aabb = collidable.AABB;

		int numXCells = ((int)aabb.Size.X - 1) / CellSize + 1;
		int numYCells = ((int)aabb.Size.Y - 1) / CellSize + 1;

		int collidablePosX = (int)(collidable.Position.X + aabb.Position.X);
		int collidablePosY = (int)(collidable.Position.Y + aabb.Position.Y);

		for (int i = 0; i <= numYCells; i++)
		{
			int y = collidablePosY + CellSize * i;
			if (i == numYCells)
			{
				y += (int)aabb.Size.Y;
			}

			for (int j = 0; j <= numXCells; j++)
			{
				int x = collidablePosX + CellSize * j;
				if (j == numXCells)
				{
					x += (int)aabb.Size.X;
				}

				int positionHash = GetPositionHash(x, y);

				if (positionHash >= 0 && positionHash < _buckets.Length && !_touches[positionHash])
				{
					_touches[positionHash] = true;

					ICollidable[] array = _buckets[positionHash];

					if (array != null)
					{
						foreach (ICollidable t in array)
						{
							if (t != null && t != collidable)
							{
								resultStack.Push(t);
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Clears all collidable objects in the spatial hash.
	/// </summary>
	public void Clear()
	{
		// Iterate over each bucket in the spatial hash
		foreach (ICollidable[] array in _buckets)
		{
			// Check if the bucket is not null
			if (array != null)
			{
				// Iterate over each collidable object in the bucket
				for (int j = 0; j < array.Length; j++)
				{
					// Set the collidable object to null
					array[j] = null;
				}
			}
		}
	}

	#region Methods for checking collisions
	/// <summary>
	/// Performs a raycast from the specified origin in the given direction for a maximum distance.
	/// Returns the first collidable object encountered along the raycast path, or null if no collidable objects were found.
	/// </summary>
	/// <param name="origin">The starting point of the raycast.</param>
	/// <param name="direction">The direction of the raycast.</param>
	/// <param name="maxDistance">The maximum distance the raycast should travel.</param>
	/// <returns>The first collidable object encountered along the raycast path, or null if no collidable objects were found.</returns>
	public RaycastHit Raycast(Vector2 origin, Vector2 direction, float maxDistance)
	{
		Vector2 currentPos = origin;
		Vector2 normalizedDirection = Vector2.Normalize(direction);
		Stack <ICollidable> resultStack = new();
		float currentDistance = 0;

		while (currentDistance <= maxDistance)
		{
			Query(currentPos, resultStack);
			while (resultStack.Count > 0)
			{
				ICollidable collidable = resultStack.Pop();
				if (collidable != null && PointInRect(currentPos, collidable)) {
					// get the normal of the collidable
					Vector2 normal = collidable.AABB.GetNormal(currentPos);
					// return the raycast hit
					return new(currentPos, Vector2.Zero, currentDistance, collidable, true); }
			}
			currentPos += normalizedDirection * 1;
			currentDistance += 1;
		}

		return default;
	}
	
	/// <summary>
	/// Performs a raycast and returns a list of all collidables intersected by the ray.
	/// </summary>
	/// <param name="origin">The starting position of the ray.</param>
	/// <param name="direction">The direction of the ray.</param>
	/// <param name="maxDistance">The maximum distance the ray can travel.</param>
	/// <returns>A list of collidables intersected by the ray.</returns>
	public List<RaycastHit> RaycastAll(Vector2 origin, Vector2 direction, float maxDistance) {
		List<ICollidable> rayCollidables = new();
		List<RaycastHit> rayHits = new();
		Vector2 currentPos = origin;
		Vector2 normalizedDirection = Vector2.Normalize(direction);
		float currentDistance = 0;
		Stack <ICollidable> resultStack = new();

		while (currentDistance <= maxDistance)
		{
			Query(currentPos, resultStack);
			while (resultStack.Count > 0)
			{
				ICollidable collidable = resultStack.Pop();
				if (collidable != null && !rayCollidables.Contains(collidable) && PointInRect(currentPos, collidable)) {
					rayCollidables.Add(collidable);
					rayHits.Add(new(currentPos, Vector2.Zero, currentDistance, collidable, true));
				}
			}

			currentPos += normalizedDirection;
			currentDistance += 1;
		}
		return rayHits;
	}
	
	/// <summary>
	/// Checks if there is an overlap between a box and any collidable objects.
	/// </summary>
	/// <param name="position">The position of the box.</param>
	/// <param name="rect">The rectangle defining the size of the box.</param>
	/// <returns>True if there is an overlap, false otherwise.</returns>
	public bool OverlapBox(Vector2 position, FloatRect rect)
	{
		// Define the box's position
		FloatRect boxRect = new FloatRect(
			position.X + rect.Left,
			position.Y + rect.Top,
			rect.Width,
			rect.Height);

		// Iterate through each bucket
		foreach (ICollidable[] colliders in _buckets)
		{
			if (colliders == null)
			{
				continue;
			}
			// Iterate through each collidable in the current bucket
			foreach (ICollidable collidable in colliders)
			{
				if (collidable == null)
				{
					continue;
				}

				// Define the collidable's AABB
				FloatRect initialRect = collidable.AABB.GetFloatRect();
				FloatRect collidableRect = new FloatRect(
					collidable.Position.X + initialRect.Left, 
					collidable.Position.Y + initialRect.Top, 
					initialRect.Width, 
					initialRect.Height);

				// Check if the collidable's AABB intersects with the box's rectangle
				if (boxRect.Intersects(collidableRect, out _))
				{
					// If there is an overlap, return true
					return true;
				}
			}
		}

		// If no overlap is found, return false
		return false;
	}
	
	/// <summary>
	/// Finds all collidables that overlap with a given box.
	/// </summary>
	/// <param name="position">The position of the box.</param>
	/// <param name="rect">The dimensions of the box.</param>
	/// <returns>A list of collidables that overlap with the box.</returns>	
	public List<ICollidable> OverlapBoxAll(Vector2 position, FloatRect rect) {
		List<ICollidable> collidables = new();
		// Define the box's position
		FloatRect boxRect = new FloatRect(
			position.X + rect.Left,
			position.Y + rect.Top,
			rect.Width,
			rect.Height);

		// Iterate through each bucket
		foreach (ICollidable[] colliders in _buckets)
		{
			if (colliders == null)
			{
				continue;
			}
			// Iterate through each collidable in the current bucket
			foreach (ICollidable collidable in colliders)
			{
				if (collidable == null)
				{
					continue;
				}

				// Define the collidable's AABB
				FloatRect initialRect = collidable.AABB.GetFloatRect();
				FloatRect collidableRect = new FloatRect(
					collidable.Position.X + initialRect.Left, 
					collidable.Position.Y + initialRect.Top, 
					initialRect.Width, 
					initialRect.Height);

				// Check if the collidable's AABB intersects with the box's rectangle
				if (boxRect.Intersects(collidableRect, out _) && !collidables.Contains(collidable))
				{
					// If there is an overlap, return true
					collidables.Add(collidable);
				}
			}
		}

		// If no overlap is found, return false
		return collidables;
	}
	
	/// <summary>
	/// Finds the first collidable that overlaps with a given box.
	/// </summary>
	/// <param name="position">The position of the box.</param>
	/// <param name="rect">The dimensions of the box.</param>
	/// <returns>The collidable that overlaps with the box, or null if no overlap is found.</returns>
	public ICollidable OverlapBoxTarget(Vector2 position, FloatRect rect) {
		// Define the box's position
		FloatRect boxRect = new FloatRect(
			position.X + rect.Left,
			position.Y + rect.Top,
			rect.Width,
			rect.Height);

		// Iterate through each bucket
		foreach (ICollidable[] colliders in _buckets)
		{
			if (colliders == null)
			{
				continue;
			}
			// Iterate through each collidable in the current bucket
			foreach (ICollidable collidable in colliders)
			{
				if (collidable == null)
				{
					continue;
				}

				// Define the collidable's AABB
				FloatRect initialRect = collidable.AABB.GetFloatRect();
				FloatRect collidableRect = new FloatRect(
					collidable.Position.X + initialRect.Left, 
					collidable.Position.Y + initialRect.Top, 
					initialRect.Width, 
					initialRect.Height);

				// Check if the collidable's AABB intersects with the box's rectangle
				if (boxRect.Intersects(collidableRect, out _))
				{
					// If there is an overlap, return true
					return collidable;
				}
			}
		}

		// If no overlap is found, return false
		return null;
	}

	/// <summary>
	/// Checks if a given position is inside the bounding box of a collidable object.
	/// </summary>
	/// <param name="position">The position to check.</param>
	/// <param name="collidable">The collidable object.</param>
	/// <returns>True if the position is inside the bounding box, false otherwise.</returns>
	private bool PointInRect(Vector2 position, ICollidable collidable)
	{
		// Calculate the boundaries of the bounding box
		float minX = collidable.Position.X + collidable.AABB.Position.X;
		float maxX = collidable.Position.X + collidable.AABB.Position.X + collidable.AABB.Size.X;
		float minY = collidable.Position.Y + collidable.AABB.Position.Y;
		float maxY = collidable.Position.Y + collidable.AABB.Position.Y + collidable.AABB.Size.Y;

		// Check if the position is inside the bounding box
		if (position.X >= minX && position.X < maxX && position.Y >= minY && position.Y < maxY)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	#endregion

	bool CheckPositionCollision(ICollidable objA, Vector2 position, ICollidable objB)
	{
		foreach (var normal in objA.Mesh.Normals.Concat(objB.Mesh.Normals))
		{
			var normalizedNormal = Vector2.Normalize(normal);
			if (!CheckProjection(normalizedNormal, objA.Mesh, position, objB.Mesh, objB.Position))
			{
				return false;
			}
		}
		return true;
	}

	bool CheckProjection(Vector2 normal, Mesh meshA, Vector2 positionA, Mesh meshB, Vector2 positionB)
	{
		var minA = 0f;
		var minB = 0f;
		var maxA = 0f;
		var maxB = 0f;
		ProjectPolygon(normal, meshA, positionA, ref minA, ref maxA);
		ProjectPolygon(normal, meshB, positionB, ref minB, ref maxB);
		return IntervalDistance(minA, maxA, minB, maxB) <= 0f;
	}
	
	

	int IntervalDistance (float minA, float maxA, float minB, float maxB) {
		if (minA < minB) {
			return (int)(minB - maxA);
		}
		return (int)(minA - maxB);
	}

	void ProjectPolygon (Vector2 normal, Mesh mesh, Vector2 offset, ref float min, ref float max) {
		float num = Vector2.DotProduct(normal, mesh.Vertices[0] + offset);
		min = num;
		max = num;
		for (int i = 0; i < mesh.Vertices.Count; i++) {
			num = Vector2.DotProduct(mesh.Vertices[i] + offset, normal);
			if (num < min) {
				min = num;
			} else if (num > max) {
				max = num;
			}
		}
	}
	
	/// <summary>
	/// Draws the debug information for collidables on the specified render target.
	/// </summary>
	/// <param name="target">The render target to draw on.</param>
	public void DebugDraw(RenderTarget target)
	{
		// Create the render states for drawing
		RenderStates states = new RenderStates(BlendMode.Alpha, Transform.Identity, null, null);

		// Iterate over each bucket in the collection
		foreach (ICollidable[] array in _buckets)
		{
			// Check if the bucket is not null
			if (array != null)
			{
				// Iterate over each collidable in the bucket
				foreach (ICollidable collidable in array)
				{
					// Check if the collidable is not null and has debug vertices
					if (collidable != null && collidable.DebugVerts != null)
					{
						// Set the transform to the identity and translate it to the collidable's position
						states.Transform = Transform.Identity;
						states.Transform.Translate(collidable.Position);

						// Draw the debug vertices on the render target
						target.Draw(collidable.DebugVerts, states);
					}
				}
			}
		}

		// Draw the debug grid vertices on the render target
		target.Draw(_debugGridVerts);
	}
}
