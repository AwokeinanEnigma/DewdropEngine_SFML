using DewDrop.Utilities;
using SFML.Graphics;
namespace DewDrop.Internal; 

public class Component : IDisposable {
	public virtual int Importance { get; set; }
	public GameObject GameObject { get; set; }
	protected Transform transform => GameObject.Transform;
	protected Vector3 position {
		get => transform.Position;
		set => transform.Position = value;
	}
	protected float rotation {
		get => transform.Rotation;
		set => transform.Rotation = value;
	}
	protected Vector2 size {
		get => transform.Size;
		set => transform.Size = value;
	}
	protected Vector2 origin {
		get => transform.Origin;
		set => transform.Origin = value;
	}
	
	public string Name { get; set; }
	public bool Active = true;
	protected bool _destroyed;
	
	public virtual void Awake (){}
	public virtual void Start (){}
	public virtual void Update (){}
	public virtual void Draw (RenderTarget target){}
	public virtual void Destroy () {
		if (_destroyed) {
			return;
		}
		GameObject.RemoveComponent(this);
		Dispose();
	}

	protected virtual void ReleaseUnmanagedResources () {
		// release unmanaged resources here
	}
	protected virtual void ReleaseManagedResources () {
		// release managed resources here
	}
	protected void Dispose (bool disposing) {
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
