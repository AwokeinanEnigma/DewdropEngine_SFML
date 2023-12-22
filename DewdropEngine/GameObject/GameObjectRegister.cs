using DewDrop.Exceptions;
using DewDrop.Utilities;
using SFML.Graphics;
namespace DewDrop.Internal; 

public static class GameObjectRegister {
	#region Comparers

	class GameObjectComparer : IComparer<GameObject> {
		public int Compare(GameObject x, GameObject y) {
			// Compare based on Importance
			int importanceComparison = x.UpdateSlot.CompareTo(y.UpdateSlot);
			//O/ter.Log(x.Name + " " + x.Importance);
			//Outer.Log(y.Name + " " + y.Importance);
			if (importanceComparison != 0) {
			//	Outer.Log(importanceComparison);
				return importanceComparison;
			}

			// If Importance is equal, compare based on IDs to ensure uniqueness
			return x.GetHashCode().CompareTo(y.GetHashCode());
		}
	}
	class GameObjectZComparer : IComparer<GameObject> {
		public int Compare(GameObject x, GameObject y) {
			// Compare based on Z
			if (x.Transform == null) {
				return (int)y.Transform.Position.Z;	
			}
			if (y.Transform == null) {
				return (int)x.Transform.Position.Z;	
			}
			return (int)(x.Transform.Position.Z != y.Transform.Position.Z ? x.Transform.Position.Z - y.Transform.Position.Z : x.GetHashCode().CompareTo(y.GetHashCode()));


			// If Z is equal, compare based on IDs to ensure uniqueness
			return x.GetHashCode().CompareTo(y.GetHashCode());
		}
	}

	#endregion

	public static SortedSet<GameObject> GameObjects { get; private set; }
	//static SortedSet<GameObject> _GameObjectsSortedByZ;
	static List<GameObject> _GameObjectsSortedByZ;
	static FloatRect _ViewRect;
	static FloatRect _RenderableRect;
	static View _View;
	static RenderTarget _Target;
	static Stack<GameObject> _GameObjectsToAdd;
	static Stack<GameObject> _GameObjectsToRemove;
	static bool _Sort;
	public static bool Initialized;
	public static void Initialize(RenderTarget target) {
		_Target = target;
		GameObjects = new SortedSet<GameObject>(new GameObjectComparer());
		_GameObjectsSortedByZ = new List<GameObject>(); //= new SortedSet<GameObject>(new GameObjectZComparer());
		_GameObjectsToAdd = new Stack<GameObject>();
		_GameObjectsToRemove = new Stack<GameObject>();
		Initialized = true;
	}

	public static void AddGameObject(GameObject gameObject) {
		if (GameObjects.Contains(gameObject)) {
			throw new GameObjectAlreadyRegisteredException($"GameObject '{gameObject.Name}' already registered");
		}
		gameObject.Awake();
		gameObject.FrameRegistered = Engine.Frame;
		GameObjects.Add(gameObject);
		_GameObjectsToAdd.Push(gameObject);
	}

	public static void AddAllGameObjects(IList<GameObject> gameObjects) {
		foreach (GameObject gameObject in gameObjects) {
			AddGameObject(gameObject);
		}
	}
	public static void RemoveGameObject(GameObject gameObject) {
		GameObjects.Remove(gameObject);
		_GameObjectsSortedByZ.Remove(gameObject);
	}
	
	public static void Update() {
		foreach (GameObject gameObject in GameObjects) {
			if (Engine.Frame - gameObject.FrameRegistered == 1) {
				gameObject.Start();
			}
			
			if (gameObject.Active) 
				gameObject.Update();
		}
	}
	
	static void DoAdditions () {
		while (_GameObjectsToAdd.Count > 0) {
			// remove the thing from the top of this
			GameObject key = _GameObjectsToAdd.Pop();

			// add it to the list
			_GameObjectsSortedByZ.Add(key);
			// force our render pipeline to sort IRenderables after adding
			_Sort = true;
		}
	}

	static void DoRemovals () {
		while (_GameObjectsToRemove.Count > 0) {
			GameObject key = _GameObjectsToRemove.Pop();
			_GameObjectsSortedByZ.Remove(key);

			// unlike DoAdditions, we don't need to force sort our IRenderables again
			// this is pretty obvious, but you don't need to sort again if something was removed 
		}
	}
	
	public static void ForceSort() {
		//Outer.Log("Forcing sort");
		_Sort = true;
	}
	public static void Draw() {
		DoRemovals();
		DoAdditions();
		if (_Sort) {
			_GameObjectsSortedByZ.Sort(new GameObjectZComparer());
			_Sort = false;
		}
		_View = Engine.Window.GetView();

		_ViewRect.Left = _View.Center.X - _View.Size.X/2f;
		_ViewRect.Top = _View.Center.Y - _View.Size.Y/2f;
		_ViewRect.Width = _View.Size.X;
		_ViewRect.Height = _View.Size.Y;
		
		
		foreach (GameObject gameObject in _GameObjectsSortedByZ) {

			if (gameObject.Transform.Visible) {
				
				_RenderableRect.Left = gameObject.Transform.Position.X - gameObject.Transform.Origin.x;
				_RenderableRect.Top = gameObject.Transform.Position.Y - gameObject.Transform.Origin.y;
				_RenderableRect.Width = gameObject.Transform.Size.X;
				_RenderableRect.Height = gameObject.Transform.Size.Y;
				
				// if it's in the view of the game, allow that shit to draw baby!
				if ((_RenderableRect.Intersects(_ViewRect) || gameObject.Transform.DrawRegardlessOfVisibility) && gameObject.Active) {
					gameObject.Draw(_Target);
					gameObject.Transform.IsBeingDrawn = true;
				} else {
					gameObject.Transform.IsBeingDrawn = false;
				}
			} else {
				gameObject.Transform.IsBeingDrawn = false;
			}
		}
	}
	
	
	public static void Destroy() {
		foreach (GameObject gameObject in GameObjects) {
			gameObject.Destroy(true);
		}
		GameObjects.Clear();
		_GameObjectsSortedByZ.Clear();
	}

	public static GameObject? GetGameObjectByName(string name) {
		foreach (GameObject gameObject in GameObjects) {
			if (gameObject.Name == name) {
				return gameObject; 
			}
		}
		return null;
	}
	
	#region Instantiate
	public static GameObject Instantiate(GameObject gameObject) {
		GameObject newGameObject = gameObject.Clone();
		AddGameObject(newGameObject);
		return newGameObject;
	}
	
	public static GameObject Instantiate(GameObject gameObject, Vector3 position) {
		GameObject newGameObject = gameObject.Clone();
		newGameObject.Transform.Position = position;
		AddGameObject(newGameObject);
		return newGameObject;
	}
	
	public static GameObject Instantiate(GameObject gameObject, Vector3 position, float rotation) {
		GameObject newGameObject = gameObject.Clone();
		newGameObject.Transform.Position = position;
		newGameObject.Transform.Rotation = rotation;
		AddGameObject(newGameObject);
		return newGameObject;
	}
	
	public static GameObject Instantiate(GameObject gameObject, Vector3 position, float rotation, Vector2 size) {
		GameObject newGameObject = gameObject.Clone();
		newGameObject.Transform.Position = position;
		newGameObject.Transform.Rotation = rotation;
		newGameObject.Transform.Size = size;
		AddGameObject(newGameObject);
		return newGameObject;
	}
	
	public static GameObject Instantiate(GameObject gameObject, Vector3 position, float rotation, Vector2 size, int importance) {
		GameObject newGameObject = gameObject.Clone();
		newGameObject.Transform.Position = position;
		newGameObject.Transform.Rotation = rotation;
		newGameObject.Transform.Size = size;
		newGameObject.UpdateSlot = importance;
		AddGameObject(newGameObject);
		return newGameObject;
	}
	
	public static GameObject Instantiate(GameObject gameObject, Vector3 position, float rotation, Vector2 size, int importance, string name) {
		GameObject newGameObject = gameObject.Clone();
		newGameObject.Transform.Position = position;
		newGameObject.Transform.Rotation = rotation;
		newGameObject.Transform.Size = size;
		newGameObject.UpdateSlot = importance;
		newGameObject.Name = name;
		AddGameObject(newGameObject);
		return newGameObject;
	}
	
	public static GameObject Instantiate(GameObject gameObject, Vector3 position, float rotation, Vector2 size, int importance, string name, Transform parent) {
		GameObject newGameObject = gameObject.Clone();
		newGameObject.Transform.Position = position;
		newGameObject.Transform.Rotation = rotation;
		newGameObject.Transform.Size = size;
		newGameObject.UpdateSlot = importance;
		newGameObject.Name = name;
		newGameObject.Transform.SetParent(parent);
		AddGameObject(newGameObject);
		return newGameObject;
	}
	#endregion
}