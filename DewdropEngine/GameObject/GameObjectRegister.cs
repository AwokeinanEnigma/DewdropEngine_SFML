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
			return _Ids[y] - _Ids[x];
		}
	}
	class GameObjectZComparer : IComparer<GameObject> {
		public int Compare(GameObject x, GameObject y) {
			return (int)(x.Transform.Position.Z != y.Transform.Position.Z ? x.Transform.Position.Z - y.Transform.Position.Z : _Ids[y] - _Ids[x]); 
		}
	}

	#endregion

	 public static List<GameObject> GameObjects { get; private set; }
	//static SortedSet<GameObject> _GameObjectsSortedByZ;
	static List<GameObject> _DrawableGameObjects;
	static SortedSet<GameObject> _UpdateableGameObjects;
	static Dictionary<GameObject, int> _Ids;
	static FloatRect _ViewRect;
	static FloatRect _RenderableRect;
	static View _View;
	static RenderTarget _Target;
	static Stack<GameObject> _GameObjectsToAdd;
	static Stack<GameObject> _GameObjectsToRemove;
	static bool _Sort;
	static int _IdCounter;
	public static bool Initialized;
	
	public static void Initialize(RenderTarget target) {
		_Target = target;
		GameObjects = new List<GameObject>();
		_DrawableGameObjects = new List<GameObject>(); //= new SortedSet<GameObject>(new GameObjectZComparer());
		_UpdateableGameObjects = new SortedSet<GameObject>(new GameObjectComparer());
		_GameObjectsToAdd = new Stack<GameObject>();
		_GameObjectsToRemove = new Stack<GameObject>();
		_Ids = new Dictionary<GameObject, int>();
		Initialized = true;
	}

	public static void AddGameObject(GameObject gameObject) {
		if (GameObjects.Contains(gameObject)) {
			throw new GameObjectAlreadyRegisteredException($"GameObject '{gameObject.Name}' already registered");
		}
		gameObject.Awake();
		gameObject.FrameRegistered = Engine.Frame;
		//GameObjects.Add(gameObject);
		_GameObjectsToAdd.Push(gameObject);
	}

	public static void AddAllGameObjects(IList<GameObject> gameObjects) {
		foreach (GameObject gameObject in gameObjects) {
			AddGameObject(gameObject);
		}
	}
	public static void RemoveGameObject(GameObject gameObject) {
		_GameObjectsToRemove.Push(gameObject);
	}
	
	public static void Update() {
		DoRemovals();
		DoAdditions();
		if (_Sort) {
			_DrawableGameObjects.Sort(new GameObjectZComparer());
			_Sort = false;
		}
		
		foreach (GameObject gameObject in _UpdateableGameObjects) {
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

			_Ids.Add(key, _IdCounter);

			// if it's only drawable, add it to the drawable list
			if (key.OnlyDraw) {
				_DrawableGameObjects.Add(key);
			}
			// if it's only updateable, add it to the updateable list
			else if (key.OnlyUpdate) {
				_UpdateableGameObjects.Add(key);
			}
			// otherwise, add it to both
			else {
				_DrawableGameObjects.Add(key);
				_UpdateableGameObjects.Add(key);
			}
			GameObjects.Add(key);

			// force our render pipeline to sort IRenderables after adding
			_Sort = true;
			++_IdCounter;
		}
	}

	static void DoRemovals () {
		while (_GameObjectsToRemove.Count > 0) {
			GameObject key = _GameObjectsToRemove.Pop();
			_DrawableGameObjects.Remove(key);
			_UpdateableGameObjects.Remove(key);
			GameObjects.Remove(key);
			// this is pretty obvious, but you don't need to sort again if something was removed 
		}
	}
	
	public static void ForceSort() {
		//Outer.Log("Forcing sort");
		_Sort = true;
	}
	public static void Draw() {
		_View = Engine.RenderTexture.GetView();

		_ViewRect.Left = _View.Center.X - _View.Size.X/2f;
		_ViewRect.Top = _View.Center.Y - _View.Size.Y/2f;
		_ViewRect.Width = _View.Size.X;
		_ViewRect.Height = _View.Size.Y;
		
		
		foreach (GameObject gameObject in _DrawableGameObjects) {
			if (Engine.Frame - gameObject.FrameRegistered == 1) {
				gameObject.Start();
			}
			if (gameObject.Transform.Visible) {
				
				_RenderableRect.Left = gameObject.Transform.Position.X - gameObject.Transform.Origin.X;
				_RenderableRect.Top = gameObject.Transform.Position.Y - gameObject.Transform.Origin.Y;
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
	
	
	public static void Destroy(bool sceneWipe) {
		foreach (GameObject gameObject in GameObjects) {
			gameObject.Destroy(sceneWipe);
		}
		GameObjects.Clear();
		_DrawableGameObjects.Clear();
		_UpdateableGameObjects.Clear();
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