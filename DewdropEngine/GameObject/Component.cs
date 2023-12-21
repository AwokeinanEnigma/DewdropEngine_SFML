namespace DewDrop.GameObject; 

public class Component : IDisposable {
	public virtual int Importance { get; set; }
	public GameObject GameObject { get; set; }
	public Transform Transform => GameObject.Transform;
	public string Name { get; set; }
	
	public virtual void Awake (){}
	public virtual void Start (){}
	public virtual void Update (){}
	public virtual void Draw (){}
	public virtual void Destroy (){}

	protected virtual void ReleaseUnmanagedResources () {
		// release unmanaged resources here
	}
	protected virtual void ReleaseManagedResources () {
		// release managed resources here
	}
	protected  void Dispose (bool disposing) {
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
