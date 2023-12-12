#region

using DewDrop.Utilities;
using SFML.Graphics;
using System.ComponentModel.Design;
using System.Data.SqlTypes;

#endregion
namespace DewDrop.Collision;

public class CollisionManager {
	SpatialHash _spatialHash;
	readonly Stack<ICollidable> resultStack;
	readonly List<ICollidable> resultList;
	readonly Dictionary<ICollidable, Vector2> positions;
	public CollisionManager (int width, int height) {
		_spatialHash = new SpatialHash(width, height);
		resultStack = new Stack<ICollidable>(512);
		resultList = new List<ICollidable>(4);
		_collidables = new List<ICollidable>();
		positions = new Dictionary<ICollidable, Vector2>();
		
	}

    /// <summary>
    ///     Adds a collider
    /// </summary>
    /// <param name="collidable">Collider to add</param>
    public void Add (ICollidable collidable) {
		_spatialHash.Insert(collidable);
		_collidables.Add(collidable);
		positions.Add(collidable, collidable.Position);
	}

    /// <summary>
    ///     Adds a list of colliders
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collidables"></param>
    public void AddAll<T> (ICollection<T> collidables) where T : ICollidable {
		foreach (T t in collidables) {
			ICollidable collidable = t;
			Add(collidable);

		}
	}

    /// <summary>
    ///     Removes a collider
    /// </summary>
    /// <param name="collidable"></param>
    public void Remove (ICollidable collidable) {
		_collidables.Remove(collidable);
		_spatialHash.Remove(collidable);
		positions.Remove(collidable);

	}

	public void Purge () {
		_collidables.ForEach(x => _spatialHash.Remove(x));
		//collidables.ForEach(x => x.Mesh.Destroy());
		_collidables.Clear();
		_collidables = null;
		_spatialHash = null;
	}

	public void Filter () {
		List<ICollidable> itemToRemove = _collidables.FindAll(r => r == null);
		itemToRemove.ForEach(x => _spatialHash.Remove(x));
		List<ICollidable> result = _collidables.Except(itemToRemove).ToList();
		_collidables = result;

	}

	 List<ICollidable> _collidables;

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

	 public void Update (ICollidable collidable, Vector2 oldPosition, Vector2 newPosition) {
		_spatialHash.Update(collidable, oldPosition, newPosition);
	}
	

	public bool PlaceFree (ICollidable obj, Vector2 position) {
		return PlaceFree(obj, position, null);
	}

	public bool PlaceFree (ICollidable obj, Vector2 position, ICollidable[] collisionResults) {
		return PlaceFree(obj, position, collisionResults, null);
	}

	public bool PlaceFree (ICollidable obj, Vector2 position, ICollidable[] collisionResults, Type[] ignoreTypes) {
		if (collisionResults != null) {
			Array.Clear(collisionResults, 0, collisionResults.Length);
		}
		bool flag = false;

		Vector2 offset = obj.Position - position;
		resultList.Clear();
		_spatialHash.Query(obj, offset, resultStack);
		int num = 0;
		while (resultStack.Count > 0) {
			ICollidable collidable = resultStack.Pop();

			if (PlaceFreeBroadPhase(obj, position, collidable)) {
				if (CheckPositionCollision(obj, position, collidable)) {

					bool cannotPlace = false;
					//Console.WriteLine($"Moving collider '{obj}' to position '{position}' ");
					if (ignoreTypes != null) {
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
		resultStack.Clear();
		return !flag;
	}
	
	public RaycastHit Raycast (Vector2 position, Vector2 direction, float distance) {
		return _spatialHash.Raycast(position, direction, distance);
	}
	public List<RaycastHit> RaycastAll (Vector2 position, Vector2 direction, float distance) {
		return _spatialHash.RaycastAll(position, direction, distance);
	}
	
	public List<ICollidable> OverlapBoxAll (Vector2 position, FloatRect size) {
		return _spatialHash.OverlapBoxAll(position, size);
	}
	
	public ICollidable OverlapBoxTarrget (Vector2 position, FloatRect size) {
		return _spatialHash.OverlapBoxTarget(position, size);
	}
	
	public bool OverlapBox (Vector2 position, FloatRect size) {
		return _spatialHash.OverlapBox(position, size);
	}
	
	
	
	public bool GetDoorStuck (Vector2 position) {
		return _spatialHash.CheckPosition(position);
	}

	public IEnumerable<ICollidable> ObjectsAtPosition (Vector2 position) {
		resultList.Clear();
		_spatialHash.Query(position, resultStack);
		while (resultStack.Count > 0) {
			ICollidable collidable = resultStack.Pop();
			if (position.X >= collidable.Position.X + collidable.AABB.Position.X && position.X < collidable.Position.X + collidable.AABB.Position.X + collidable.AABB.Size.X && position.Y >= collidable.Position.Y + collidable.AABB.Position.Y && position.Y < collidable.Position.Y + collidable.AABB.Position.Y + collidable.AABB.Size.Y) {
				resultList.Add(collidable);
			}
		}
		return resultList;
	}

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

	bool CheckPositionCollision (ICollidable objA, Vector2 position, ICollidable objB) {
		int count = objA.Mesh.Edges.Count;
		int count2 = objB.Mesh.Edges.Count;
		for (int i = 0; i < count + count2; i++) {
			Vector2 Vector2;
			if (i < count) {
				Vector2 = objA.Mesh.Normals[i];
			} else {
				Vector2 = objB.Mesh.Normals[i - count];
			}
			Vector2 = Vector2.Normalize(Vector2);
			float minA = 0f;
			float minB = 0f;
			float maxA = 0f;
			float maxB = 0f;
			ProjectPolygon(Vector2, objA.Mesh, position, ref minA, ref maxA);
			ProjectPolygon(Vector2, objB.Mesh, objB.Position, ref minB, ref maxB);
			if (IntervalDistance(minA, maxA, minB, maxB) > 0f) {
				return false;
			}
		}
		return true;
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

	public void Clear () {
		_spatialHash.Clear();
	}

	public void Draw (RenderTarget target) {
		_spatialHash.DebugDraw(target);
	}
}
