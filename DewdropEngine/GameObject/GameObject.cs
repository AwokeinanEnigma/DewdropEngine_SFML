using DewDrop.Utilities;
using SFML.Graphics;
using System.Collections;
namespace DewDrop.GameObject; 

public class GameObject : IEnumerable<Component>, IDisposable {
	public Transform? Parent {
		get => Transform.Parent;
		set => Transform.Parent = value;
	}
	public long FrameRegistered { get; set; }
	public float X { get => Transform.Position.X;}
	public float Y { get => Transform.Position.Y;  }
	public float Z { get => Transform.Position.Z;  }
	public float Rotation { get => Transform.Rotation; }
	public int Importance { get; set; }
	public Transform Transform { get; private set; }
	public bool Awakened;
	public ComponentHolder ComponentHolder {
		get;
		private set;
	}

	public bool Active = true;
	bool _destroyed;
	public GameObject () {
		Transform = new Transform {
			GameObject = this
		};
		ComponentHolder = new ComponentHolder(this);
		Name = "GameObject";
	}
	
	public string Name { get; set; }
	public virtual void Awake () {
		//Awakened = true;
		ComponentHolder.Awake();
	}
	public virtual void Start () {
		if (_destroyed) 
			return;
		
		ComponentHolder.Start();
	}
	public virtual void Update () {
		if (_destroyed) 
			return;
		
		ComponentHolder.Update();
	}
	public virtual void Draw (RenderTarget target) {
		if (_destroyed) 
			return;
		
		ComponentHolder.Draw(target);
	}
	public virtual void Destroy () {
		if (_destroyed) {
			return;
		}
		_destroyed = true;
		//GameObjectRegister.RemoveGameObject(this);
		Transform.Destroy();
		ComponentHolder.Destroy();
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
		clone.Importance = Importance;
		clone.Name = Name;
		return clone;
	}
	
	protected virtual void ReleaseUnmanagedResources () {
		// release unmanaged resources here
	}
	protected virtual void Dispose (bool disposing) {
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
	public IEnumerator<Component> GetEnumerator () {
		return ComponentHolder.GetEnumerator();
	}
	IEnumerator IEnumerable.GetEnumerator () { return GetEnumerator(); }
	
	~GameObject () {
		Dispose(false);
	}
}
