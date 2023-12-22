using DewDrop.Exceptions;
using DewDrop.Utilities;
using SFML.Graphics;
namespace DewDrop.GameObject; 

public static class GameObjectRegister {
	#region Comparers

	class GameObjectComparer : IComparer<GameObject> {
		public int Compare(GameObject x, GameObject y) {
			// Compare based on Importance
			int importanceComparison = x.Importance.CompareTo(y.Importance);
			if (importanceComparison != 0) {
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
	//		Outer.Log(x);
		//	Outer.Log(y);
			int zComparison = x.Transform.Position.Z.CompareTo(y.Transform.Position.Z);
			if (zComparison != 0) {
				return zComparison;
			}

			// If Z is equal, compare based on IDs to ensure uniqueness
			return x.GetHashCode().CompareTo(y.GetHashCode());
		}
	}

	#endregion

	static SortedSet<GameObject> _GameObjects;
	static SortedSet<GameObject> _GameObjectsSortedByZ;
	static FloatRect _ViewRect;
	static FloatRect _RenderableRect;
	static View _View;
	public static bool Initialized;
	static RenderTarget _Target;

	public static void Initialize(RenderTarget target) {
		_Target = target;
		_GameObjects = new SortedSet<GameObject>(new GameObjectComparer());
		_GameObjectsSortedByZ = new SortedSet<GameObject>(new GameObjectZComparer());
		Initialized = true;
	}

	public static void AddGameObject(GameObject gameObject) {
		if (_GameObjects.Contains(gameObject)) {
			throw new GameObjectAlreadyRegisteredException($"GameObject '{gameObject.Name}' already registered");
		}
		gameObject.Awake();
		gameObject.FrameRegistered = Engine.Frame;
		_GameObjects.Add(gameObject);
		_GameObjectsSortedByZ.Add(gameObject);
	}

	public static void RemoveGameObject(GameObject gameObject) {
		_GameObjects.Remove(gameObject);
		_GameObjectsSortedByZ.Remove(gameObject);
	}
	
	public static void Update() {
		foreach (GameObject gameObject in _GameObjects) {
			if (Engine.Frame - gameObject.FrameRegistered == 1) {
				gameObject.Start();
			}
			
			if (gameObject.Active) 
				gameObject.Update();
		}
	}
	
	public static void Draw() {
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
		foreach (GameObject gameObject in _GameObjects) {
			gameObject.Destroy();
			RemoveGameObject(gameObject);
		}
	}

	public static GameObject? GetGameObjectByName(string name) {
		foreach (GameObject gameObject in _GameObjects) {
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
		newGameObject.Importance = importance;
		AddGameObject(newGameObject);
		return newGameObject;
	}
	
	public static GameObject Instantiate(GameObject gameObject, Vector3 position, float rotation, Vector2 size, int importance, string name) {
		GameObject newGameObject = gameObject.Clone();
		newGameObject.Transform.Position = position;
		newGameObject.Transform.Rotation = rotation;
		newGameObject.Transform.Size = size;
		newGameObject.Importance = importance;
		newGameObject.Name = name;
		AddGameObject(newGameObject);
		return newGameObject;
	}
	
	public static GameObject Instantiate(GameObject gameObject, Vector3 position, float rotation, Vector2 size, int importance, string name, Transform parent) {
		GameObject newGameObject = gameObject.Clone();
		newGameObject.Transform.Position = position;
		newGameObject.Transform.Rotation = rotation;
		newGameObject.Transform.Size = size;
		newGameObject.Importance = importance;
		newGameObject.Name = name;
		newGameObject.Transform.SetParent(parent);
		AddGameObject(newGameObject);
		return newGameObject;
	}
	#endregion
}