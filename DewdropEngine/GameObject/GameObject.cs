using System.Collections;
namespace DewDrop.GameObject; 

public class GameObject : IEnumerable<Component>, IDisposable {
	public Transform Transform { get; private set; }
	public ComponentHolder ComponentHolder { get; private set; }

	public GameObject () {
		Transform = new Transform();
		ComponentHolder = new ComponentHolder(this);
		Name = "GameObject";
	}
	
	public string Name { get; set; }
	public virtual void Awake () {
		ComponentHolder.Awake();
	}
	public virtual void Start () {
		ComponentHolder.Start();
	}
	public virtual void Update () {
		ComponentHolder.Update();
	}
	public virtual void Draw () {
		ComponentHolder.Draw();
	}
	public virtual void Destroy () {
		Dispose();
	}
	
	public T GetComponent<T> () where T : Component {
		return ComponentHolder.GetComponent<T>();
	}
	public T AddComponent<T> (T component) where T : Component {
		ComponentHolder.AddComponent(component);
		return component;
	}
	public void AddComponent (Component component) {
		ComponentHolder.AddComponent(component);
	}
	public T AddComponent<T> () where T : Component, new() {
		return ComponentHolder.AddComponent<T>();
	}
	protected virtual void ReleaseUnmanagedResources () {
		// release unmanaged resources here
	}
	protected virtual void Dispose (bool disposing) {
		ReleaseUnmanagedResources();
		if (disposing) {
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
