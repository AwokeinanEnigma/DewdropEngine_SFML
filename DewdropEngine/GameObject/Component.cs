using DewDrop.Utilities;
using SFML.Graphics;
using System.Reflection;
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

	readonly MethodInfo? _awake;
	readonly MethodInfo? _start;
	readonly MethodInfo? _update;
	readonly MethodInfo? _draw;
	object[] _parameters;
	
	public Component () {
		MethodInfo[] methods = GetType().GetMethods();
		_awake = methods.FirstOrDefault(m => m.Name == "Awake") ?? null;
		_start = methods.FirstOrDefault(m => m.Name == "Start") ?? null;
		_update = methods.FirstOrDefault(m => m.Name == "Update") ?? null;
		_draw = methods.FirstOrDefault(m => m.Name == "Draw");
		methods = null;
		_parameters = new object[1];
	}
	
	public void InvokeAwake () {
		_awake?.Invoke(this, null);
	}
	public void InvokeStart () {
		_start?.Invoke(this, null);
	}
	public void InvokeUpdate () {
		_update?.Invoke(this, null);
	}
	public void InvokeDraw (RenderTarget target) {
		_parameters[0] = target;
		_draw?.Invoke(this, _parameters);
	}

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
