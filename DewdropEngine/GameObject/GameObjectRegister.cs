using DewDrop.Exceptions;
using DewDrop.Scenes;
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
			return Ids[y] - Ids[x];
		}
	}
	class GameObjectZComparer : IComparer<GameObject> {
		public int Compare(GameObject x, GameObject y) {
			return (int)(x.Z != y.Z ? x.Z - y.Z : Ids[y] - Ids[x]); 
		}
	}

	#endregion

	public struct SceneBoundObjects {
		public List<GameObject> GameObjects;
		public List<GameObject> DrawableGameObjects;
		public SortedSet<GameObject> UpdateableGameObjects;
		public Dictionary<GameObject, int> Ids;
		public SceneBoundObjects() {
			//Outer.Log("Creating new scene bound objects");
			GameObjects = new List<GameObject>();
			DrawableGameObjects = new List<GameObject>();
			UpdateableGameObjects = new SortedSet<GameObject>(new GameObjectComparer());
			Ids = new Dictionary<GameObject, int>();
		}
	}
	 public static List<GameObject> GameObjects => _CurrentSceneBoundObjects.GameObjects;
	 static FloatRect _ViewRect;
	static FloatRect _RenderableRect;
	static View _View;
	static RenderTarget _Target;
	static Stack<GameObject> _GameObjectsToAdd;
	static Stack<GameObject> _GameObjectsToRemove;
	static bool _Sort;
	static int _IdCounter;
	static GameObjectZComparer _GameObjectZComparer;
	static Dictionary<SceneBase, SceneBoundObjects> _SceneBoundObjects;
	static SceneBoundObjects _CurrentSceneBoundObjects;
	public static bool Initialized;
	static List<GameObject> DrawableGameObjects=> _CurrentSceneBoundObjects.DrawableGameObjects;
	static SortedSet<GameObject> UpdateableGameObjects=> _CurrentSceneBoundObjects.UpdateableGameObjects;
	static Dictionary<GameObject, int> Ids => _CurrentSceneBoundObjects.Ids;
	
	public static void Initialize(RenderTarget target) {
		_Target = target;
		_SceneBoundObjects = new Dictionary<SceneBase, SceneBoundObjects>();
		//Outer.Log("Initializing GameObjectRegister");
		
		_CurrentSceneBoundObjects = new SceneBoundObjects();
		SceneBase initScene = SceneManager.CurrentScene();
		_SceneBoundObjects.Add(initScene, _CurrentSceneBoundObjects);
		
		SceneManager.OnSceneChange += () => {
			SceneBase scene = SceneManager.CurrentScene();
		//	Outer.Log($"Scene changed to {scene}");
			if (_SceneBoundObjects.TryGetValue(scene, out SceneBoundObjects o)) {
				_CurrentSceneBoundObjects = o;
			} else {
				_CurrentSceneBoundObjects = new SceneBoundObjects();
				_SceneBoundObjects.Add(scene, _CurrentSceneBoundObjects);
			}
		};
		Outer.Log("Waiting for scene change");
		_GameObjectsToAdd = new Stack<GameObject>();
		_GameObjectsToRemove = new Stack<GameObject>();
		_GameObjectZComparer = new GameObjectZComparer();
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
		// We do our additions and removals before updating.
		// It's okay to do it here, because Draw() is called after Update(), so when we Draw, we'll already have the updated lists
		DoRemovals();
		DoAdditions();
		if (_Sort) {
			DrawableGameObjects.Sort(_GameObjectZComparer);
			_Sort = false;
		}
		
		foreach (GameObject gameObject in UpdateableGameObjects) {
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

			Ids.Add(key, _IdCounter);

			// if it's only drawable, add it to the drawable list
			if (key.OnlyDraw) {
				DrawableGameObjects.Add(key);
			}
			// if it's only updateable, add it to the updateable list
			else if (key.OnlyUpdate) {
				UpdateableGameObjects.Add(key);
			}
			// otherwise, add it to both
			else {
				DrawableGameObjects.Add(key);
				UpdateableGameObjects.Add(key);
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
			DrawableGameObjects.Remove(key);
			UpdateableGameObjects.Remove(key);
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
		
		
		foreach (GameObject gameObject in DrawableGameObjects) {
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
		DrawableGameObjects.Clear();
		UpdateableGameObjects.Clear();
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