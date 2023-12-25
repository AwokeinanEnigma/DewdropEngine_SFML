using DewDrop.GUI;
using DewDrop.Internal; using DewDrop.UserInput;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.Window;
namespace Prototype; 

public class PlayerComponent : Component{
	public override int UpdateSlot { get; set; }
	Shape _shape;
	public void Awake () {
		_shape = new RectangleShape(new Vector2(11, 20));
		_shape.FillColor = Color.Green;
		_shape.Position = transform.Position;	
		_shape.Origin = new Vector2(0, 20);
		ViewManager.Instance.EntityFollow = GameObject;
		ViewManager.Instance .Center = new Vector2(160f, 90f);
		ViewManager.Instance.Offset = new Vector2(0, (float)(-(float)((int)11)/2));

	}
	public void Start () {
		
	}
	public void Update () {
		if (ViewManager.Instance.EntityFollow != GameObject) {
			ViewManager.Instance.EntityFollow = GameObject;
		}
		Vector3 nuposition = ((Input.Instance.Axis) * 2) + position;
		nuposition.Z = nuposition.Y;
		base.position = nuposition;

		if (Input.Instance[Keyboard.Key.M]) {
			ViewManager.Instance.SetZoom(1.1f);
		}
		Vector2 positionCopy = base.position;
		//positionCopy.y -= size.y;
		//positionCopy.x -= size.x / 2;
		_shape.Position = positionCopy;
		_shape.Rotation = rotation;
		
		
	}
	public  void Draw (RenderTarget target) {
		target.Draw(_shape);
	}
	public override void Destroy () {
		base.Destroy(); 
		ViewManager.Instance.EntityFollow = null;
	}
	protected override void ReleaseUnmanagedResources () { base.ReleaseUnmanagedResources(); }
	protected override void ReleaseManagedResources () { base.ReleaseManagedResources(); }
}