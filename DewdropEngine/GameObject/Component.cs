using SFML.Graphics;
namespace DewDrop.GameObject; 

public class Component : IDisposable {
	public virtual int Importance { get; set; }
	public GameObject GameObject { get; set; }
	protected Transform transform => GameObject.Transform;
	public string Name { get; set; }
	public bool Active = true;
	protected bool _destroyed;
	
	public virtual void Awake (){}
	public virtual void Start (){}
	public virtual void Update (){}
	public virtual void Draw (RenderTarget target){}
	public virtual void Destroy () {
		GameObject.RemoveComponent(this);
		Dispose();
	}

	protected virtual void ReleaseUnmanagedResources () {
		// release unmanaged resources here
	}
	protected virtual void ReleaseManagedResources () {
		// release managed resources here
	}
	protected  void Dispose (bool disposing) {
		if (_destroyed) {
			return;
		}
		ReleaseUnmanagedResources();
		if (disposing) {
			ReleaseManagedResources();
			// dispose managed resources

		}
	}
	public void Dispose () {
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	~Component () {
		Dispose(false);
	}
}
