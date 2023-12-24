using DewDrop.Utilities;
using SFML.Graphics;
using System.Collections;
namespace DewDrop.Internal; 

public class GameObject : IDisposable {
	public Transform? Parent {
		get => Transform.Parent;
		set => Transform.Parent = value;
	}
	public long FrameRegistered { get; set; }
	public float X { get => Transform.Position.X;}
	public float Y { get => Transform.Position.Y;  }
	public float Z { get => Transform.Position.Z;  }
	public float Rotation { get => Transform.Rotation; }
	public int UpdateSlot { get; set; }
	public Transform Transform { get; private set; }
	public bool Awakened;
	public bool OnlyDraw;
	public bool OnlyUpdate;
	public ComponentHolder ComponentHolder {
		get;
		private set;
	}

	public bool Active {
		get => _active;
		set {
			_active = value;
			Transform.SetActive(value);
		}
	}
	bool _active = true;
	bool _destroyed;
	public GameObject (bool canHaveChildren = true) {
		Transform = new Transform(this, canHaveChildren);
		ComponentHolder = new ComponentHolder(this);
		Name = "GameObject";
		UpdateSlot = 0;																					
	}
	
	public string Name { get; set; }
	public void Awake () {
		//Awakened = true;
		ComponentHolder.Awake();
	}
	public void Start () {
		ComponentHolder.Start();
	}
	public void Update () {
		ComponentHolder.Update();
	}
	public void Draw (RenderTarget target) {
		ComponentHolder.Draw(target);
	}
	public void Destroy (bool sceneWipe = false) {
		if (_destroyed) {
			return;
		}
		_destroyed = true;
		if (!sceneWipe) {
		
			GameObjectRegister.RemoveGameObject(this);
		}
		//
		Transform.Destroy(sceneWipe);
		ComponentHolder.Destroy(sceneWipe);
		Dispose();
	}

	#region Add Component
	
	public T? AddComponent<T> (T component) where T : Component {
		if (_destroyed) 
			return null;
		
		ComponentHolder.AddComponent(component);
		return component;
	}
	public void AddComponent (Component component) {
		ComponentHolder.AddComponent(component);
	}
	public T AddComponent<T> () where T : Component, new() {
		return _destroyed ? null : ComponentHolder.AddComponent<T>();
	}
	public T AddOrGetComponent<T> () where T : Component, new() {
		return _destroyed ? null : ComponentHolder.AddOrGetComponent<T>();
	}
	
	#endregion
	public T GetComponent<T> () where T : Component {
		return _destroyed ? null : ComponentHolder.GetComponent<T>();
	}

	#region Remove Component

	public void RemoveComponent (Component component) {
		ComponentHolder.RemoveComponent(component);
	}
	public void RemoveComponent<T> () where T : Component {
		ComponentHolder.RemoveComponent<T>();
	}
	public void RemoveComponent<T> (T component) where T : Component {
		ComponentHolder.RemoveComponent(component);
	}	

	#endregion

	public GameObject Clone () {
		if (_destroyed) {
			throw new ObjectDisposedException("Cannot clone a destroyed GameObject");
		}
		GameObject clone = new GameObject();
		Transform.Clone(clone.Transform);
		ComponentHolder.Clone(clone.ComponentHolder);
		clone.UpdateSlot = UpdateSlot;
		clone.Name = Name;
		return clone;
	}
	
	void ReleaseUnmanagedResources () {
		// release unmanaged resources here
	} 
	void Dispose (bool disposing) {
		ReleaseUnmanagedResources();
		if (disposing) {
			Transform = null;
			ComponentHolder = null;
		}
	}
	public void Dispose () {
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	~GameObject () {
		Dispose(false);
	}
}
