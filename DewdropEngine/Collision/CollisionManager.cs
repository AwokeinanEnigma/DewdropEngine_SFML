#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Collision;

/// <summary>
/// Manages collision detection and response in the game.
/// </summary>
public class CollisionManager {
	/// <summary>
	/// The spatial hash used for efficient collision detection.
	/// </summary>
	SpatialHash _spatialHash;

	/// <summary>
	/// A stack used to store the result of spatial hash queries.
	/// </summary>
	readonly Stack<ICollidable> _resultStack;

	/// <summary>
	/// A list used to store the result of spatial hash queries.
	/// </summary>
	readonly List<ICollidable> _resultList;

	/// <summary>
	/// A list of all collidable objects.
	/// </summary>
	List<ICollidable> _collidables;

	/// <summary>
	/// Initializes a new instance of the CollisionManager class with specified width and height.
	/// </summary>
	/// <param name="width">The width of the spatial hash.</param>
	/// <param name="height">The height of the spatial hash.</param>
	public CollisionManager (int width, int height) {
		_spatialHash = new SpatialHash(width, height);
		_resultStack = new Stack<ICollidable>(512);
		_resultList = new List<ICollidable>(4);
		_collidables = new List<ICollidable>();
	}
	
	/// <summary>
	/// Adds a collidable object to the spatial hash.
	/// </summary>
	/// <param name="collidable">The collidable object to add.</param>
	public void Add (ICollidable collidable) {
		_spatialHash.Insert(collidable);
		_collidables.Add(collidable);
	}
    
	/// <summary>
	/// Adds a collection of collidable objects to the spatial hash.
	/// </summary>
	/// <param name="collidables">The collection of collidable objects to add.</param>
	public void AddAll<T> (ICollection<T> collidables) where T : ICollidable {
		foreach (T t in collidables) {
			ICollidable collidable = t;
			Add(collidable);

		}
	}
	
	/// <summary>
	/// Removes a collidable object from the spatial hash.
	/// </summary>
	/// <param name="collidable">The collidable object to remove.</param>
	public void Remove (ICollidable collidable) {
		_collidables.Remove(collidable);
		_spatialHash.Remove(collidable);
    }

	/// <summary>
	/// Removes all collidable objects from the spatial hash.
	/// </summary>
	public void Purge () {
		_collidables.ForEach(_spatialHash.Remove);
		//collidables.ForEach(x => x.Mesh.Destroy());
		_collidables.Clear();
		_collidables = null;
		_spatialHash = null;
	}

	/// <summary>
	/// Removes null collidable objects from the spatial hash.
	/// </summary>
	public void Filter () {
		List<ICollidable> itemToRemove = _collidables.FindAll(r => r == null);
		itemToRemove.ForEach(_spatialHash.Remove);
		List<ICollidable> result = _collidables.Except(itemToRemove).ToList();
		_collidables = result;

	}


	 public void UpdateTriggers() {
		 foreach (ICollidable collidable in _collidables) {
			 
			 if (collidable == null)
				 continue;
			 
			 if (collidable.IsTrigger) {
				 if (collidable.CollidingWith == null) {
					 Outer.LogError($"IsTrigger is true but CollidingWith list is null for '{collidable}'", null);
					 continue;
				 }


				 foreach (ICollidable possibleTrigger in _collidables) {
					 
					 if (possibleTrigger == null || collidable == possibleTrigger || !possibleTrigger.Solid)
						 continue;

					 if (PlaceFreeBroadPhase(collidable, collidable.Position, possibleTrigger)) {
						 if (CheckPositionCollision(collidable, collidable.Position, possibleTrigger)) {
							 if (!collidable.CollidingWith.Contains(possibleTrigger)) {
								 collidable.CollidingWith.Add(possibleTrigger);
								 collidable.OnTriggerEnter(possibleTrigger);
							 }
							 else  {
								 collidable.OnTriggerStay(possibleTrigger);
							 }
						 }
					 } else {
						 if (collidable.CollidingWith.Contains(possibleTrigger)) {
							 collidable.CollidingWith.Remove(possibleTrigger);
							 collidable.OnTriggerExit(possibleTrigger);
						 }
					 }
				 }
			 }
		 }
	 }

	 /// <summary>
	 /// Updates the position of a collidable object in the spatial hash.
	 /// </summary>
	 /// <param name="collidable">The collidable object to update.</param>
	 /// <param name="oldPosition">The old position of the collidable object.</param>
	 /// <param name="newPosition">The new position of the collidable object.</param>
	 public void Update (ICollidable collidable, Vector2 oldPosition, Vector2 newPosition) {
		_spatialHash.Update(collidable, oldPosition, newPosition);
	}

	 /// <summary>
	 /// Checks if a given position is free of collidable objects.
	 /// </summary>
	 /// <param name="obj">The collidable object to check.</param>
	 /// <param name="position">The position to check.</param>
	 /// <returns>True if the position is free of collidable objects, false otherwise.</returns>
	 public bool PlaceFree (ICollidable obj, Vector2 position, ICollidable[] collisionResults = null) {
		return PlaceFree(obj, position, collisionResults, null);
	}

	 /// <summary>
	 /// Checks if a given position is free of collidable objects.
	 /// </summary>
	 /// <param name="obj">The collidable object to check.</param>
	 /// <param name="position">The position to check.</param>
	 /// <param name="collisionResults">An array of collidable objects that the method fills with the collidable objects that were found at the position.</param>
	 /// <param name="ignoreTypes">An array of types to ignore during the check.</param>
	 /// <returns>True if the position is free of collidable objects, false otherwise.</returns>
	 // ReSharper disable once MemberCanBePrivate.Global
	 public bool PlaceFree (ICollidable obj, Vector2 position, ICollidable[] collisionResults, Type[] ignoreTypes) {
		if (collisionResults != null) {
			Array.Clear(collisionResults, 0, collisionResults.Length);
		}
		bool flag = false;

		Vector2 offset = obj.Position - position;
		_resultList.Clear();
		_spatialHash.Query(obj, offset, _resultStack);
		int num = 0;
		while (_resultStack.Count > 0) {
			ICollidable collidable = _resultStack.Pop();

			if (PlaceFreeBroadPhase(obj, position, collidable)) {
				bool cannotPlace = false;
				if (CheckPositionCollision(obj, position, collidable)) {

					if (ignoreTypes != null) {
						// ReSharper disable once ForCanBeConvertedToForeach
						// This is faster using a for loop than using LINQ or a foreach loop..
						for (int i = 0; i < ignoreTypes.Length; i++) {
							if (ignoreTypes[i] == collidable.GetType()) {
								cannotPlace = true;
								break;
							}
						}
					}
					if (!cannotPlace) {
						if (collidable.IsTrigger) {
							continue;
						}
						flag = true;
						if (collisionResults == null || num >= collisionResults.Length) {
							break;
						}
						collisionResults[num] = collidable;
						num++;
					}
				}
			}
		}
		_resultStack.Clear();
		return !flag;
	}
	
	/// <summary>
	/// Performs a raycast from the specified origin in the given direction for a maximum distance.
	/// Returns the first collidable object encountered along the raycast path, or null if no collidable objects were found.
	/// </summary>
	/// <param name="position">The starting point of the raycast.</param>
	/// <param name="direction">The direction of the raycast.</param>
	/// <param name="distance">The maximum distance the raycast should travel.</param>
	/// <returns>The first collidable object encountered along the raycast path, or null if no collidable objects were found.</returns>
	public RaycastHit Raycast (Vector2 position, Vector2 direction, float distance) {
		return _spatialHash.Raycast(position, direction, distance);
	}
	
	/// <summary>
	/// Performs a raycast and returns a list of all collidables intersected by the ray.
	/// </summary>
	/// <param name="position">The starting position of the ray.</param>
	/// <param name="direction">The direction of the ray.</param>
	/// <param name="distance">The maximum distance the ray can travel.</param>
	/// <returns>A list of collidables intersected by the ray.</returns>
	public List<RaycastHit> RaycastAll (Vector2 position, Vector2 direction, float distance) {
		return _spatialHash.RaycastAll(position, direction, distance);
	}
	
	/// <summary>
	/// Finds all collidables that overlap with a given box.
	/// </summary>
	/// <param name="position">The position of the box.</param>
	/// <param name="size">The dimensions of the box.</param>
	/// <returns>A list of collidables that overlap with the box.</returns>
	public List<ICollidable> OverlapBoxAll (Vector2 position, FloatRect size) {
		return _spatialHash.OverlapBoxAll(position, size);
	}
	
	/// <summary>
	/// Finds the first collidable that overlaps with a given box.
	/// </summary>
	/// <param name="position">The position of the box.</param>
	/// <param name="size">The dimensions of the box.</param>
	/// <returns>The collidable that overlaps with the box, or null if no overlap is found.</returns>
	public ICollidable OverlapBoxTarrget (Vector2 position, FloatRect size) {
		return _spatialHash.OverlapBoxTarget(position, size);
	}
	
	/// <summary>
	/// Checks if there is an overlap between a box and any collidable objects.
	/// </summary>
	/// <param name="position">The position of the box.</param>
	/// <param name="size">The dimensions of the box.</param>
	/// <returns>True if there is an overlap, false otherwise.</returns>
	public bool OverlapBox (Vector2 position, FloatRect size) {
		return _spatialHash.OverlapBox(position, size);
	}
	
	/// <summary>
	/// Checks if a given position is inside a collidable object.
	/// </summary>
	/// <param name="position">The position to check.</param>
	/// <returns>True if the position is inside a collidable object, false otherwise.</returns>
	public bool GetDoorStuck (Vector2 position) {
		return _spatialHash.CheckPosition(position);
	}

	/// <summary>
	/// Returns a list of all collidable objects at a given position.
	/// </summary>
	/// <param name="position">The position to check.</param>
	/// <returns>A list of collidable objects at the given position.</returns>
	public IEnumerable<ICollidable> ObjectsAtPosition (Vector2 position) {
		_resultList.Clear();
		_spatialHash.Query(position, _resultStack);
		while (_resultStack.Count > 0) {
			ICollidable collidable = _resultStack.Pop();
			if (position.X >= collidable.Position.X + collidable.AABB.Position.X && position.X < collidable.Position.X + collidable.AABB.Position.X + collidable.AABB.Size.X && position.Y >= collidable.Position.Y + collidable.AABB.Position.Y && position.Y < collidable.Position.Y + collidable.AABB.Position.Y + collidable.AABB.Size.Y) {
				_resultList.Add(collidable);
			}
		}
		return _resultList;
	}

	/// <summary>
	/// Checks if a given position is free of collidable objects in the broad phase.
	/// </summary>
	/// <param name="objA">The first collidable object.</param>
	/// <param name="position">The position to check.</param>
	/// <param name="objB">The second collidable object.</param>
	/// <returns>True if the position is free of collidable objects, false otherwise.</returns>
	bool PlaceFreeBroadPhase (ICollidable objA, Vector2 position, ICollidable objB) {
		if (objA == objB) {
			return false;
		}
		if (objA.AABB.OnlyPlayer && !objB.AABB.IsPlayer) {
			return false;
		}
		if (!objA.Solid || !objB.Solid) {
			return false;
		}
		FloatRect floatRect = objA.AABB.GetFloatRect();
		floatRect.Left += position.X;
		floatRect.Top += position.Y;
		FloatRect floatRect2 = objB.AABB.GetFloatRect();
		floatRect2.Left += objB.Position.X;
		floatRect2.Top += objB.Position.Y;
		return floatRect.Intersects(floatRect2);
	}

	/// <summary>
	/// Checks if a given position collides with a collidable object.
	/// </summary>
	/// <param name="objA">The first collidable object.</param>
	/// <param name="position">The position to check.</param>
	/// <param name="objB">The second collidable object.</param>
	/// <returns>True if the position collides with the collidable object, false otherwise.</returns>
	bool CheckPositionCollision (ICollidable objA, Vector2 position, ICollidable objB) {
		int count = objA.Mesh.Edges.Count;
		int count2 = objB.Mesh.Edges.Count;
		for (int i = 0; i < count + count2; i++) {
			Vector2 vector2 = i < count ? objA.Mesh.Normals[i] : objB.Mesh.Normals[i - count];
			vector2 = Vector2.Normalize(vector2);
			ProjectPolygon(vector2, objA.Mesh, position, out float minA, out float maxA);
			ProjectPolygon(vector2, objB.Mesh, objB.Position, out float minB, out float maxB);
			if (IntervalDistance(minA, maxA, minB, maxB) > 0f) {
				return false;
			}
		}
		return true;
	}
	
	/// <summary>
	/// Calculates the interval distance between two intervals.
	/// </summary>
	/// <param name="minA">The minimum value of the first interval.</param>
	/// <param name="maxA">The maximum value of the first interval.</param>
	/// <param name="minB">The minimum value of the second interval.</param>
	/// <param name="maxB">The maximum value of the second interval.</param>
	/// <returns>The interval distance between the two intervals.</returns>
	int IntervalDistance (float minA, float maxA, float minB, float maxB) {
		if (minA < minB) {
			return (int)(minB - maxA);
		}
		return (int)(minA - maxB);
	}

	/// <summary>
	/// Projects a mesh onto a given normal and calculates the minimum and maximum values of the projection.
	/// </summary>
	/// <param name="normal">The normal on which to project the mesh.</param>
	/// <param name="mesh">The mesh to project.</param>
	/// <param name="offset">The offset to apply to the mesh vertices before projection.</param>
	/// <param name="min">The minimum value of the projection.</param>
	/// <param name="max">The maximum value of the projection.</param>
	void ProjectPolygon (Vector2 normal, Mesh mesh, Vector2 offset, out float min, out float max) {
		float num = Vector2.DotProduct(normal, mesh.Vertices[0] + offset);
		min = num;
		max = num;
		// ReSharper disable once ForCanBeConvertedToForeach
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
	/// Clears all the touched cells in the spatial hash.
	/// </summary>
	public void Clear () {
		_spatialHash.Clear();
	}
	
	/// <summary>
	/// Draws the spatial hash grid and the debug vertices of the collidable objects in the spatial hash.
	/// </summary>
	/// <param name="target">The render target on which to draw.</param>
	public void Draw (RenderTarget target) {
		_spatialHash.DebugDraw(target);
	}
}
