using DewDrop.GUI;
using DewDrop.Internal; using DewDrop.UserInput;
using DewDrop.Utilities;
using SFML.Graphics;
namespace Prototype; 

public class PlayerComponent : Component{
	public override int Importance { get; set; }
	Shape _shape;
	public override void Awake () {
		base.Awake();
		_shape = new RectangleShape(new Vector2(11, 20));
		_shape.FillColor = Color.Green;
		_shape.Position = transform.Position;	
		_shape.Origin = new Vector2(0, 20);
		ViewManager.Instance.EntityFollow = GameObject;
		ViewManager.Instance .Center = new Vector2(160f, 90f);
		ViewManager.Instance.Offset = new Vector2(0, (float)(-(float)((int)11)/2));

	}
	public override void Start () { base.Start(); }
	public override void Update () {
		base.Update();
		Vector3 nuposition = ((Input.Instance.Axis) * 2) + position;
		nuposition.Z = nuposition.Y;
		base.position = nuposition;

		Vector2 positionCopy = base.position;
		//positionCopy.y -= size.y;
		//positionCopy.x -= size.x / 2;
		_shape.Position = positionCopy;
		_shape.Rotation = rotation;
		
		
	}
	public override void Draw (RenderTarget target) {
		base.Draw(target); 
		target.Draw(_shape);
	}
	public override void Destroy () {
		base.Destroy(); 
		ViewManager.Instance.EntityFollow = null;
	}
	protected override void ReleaseUnmanagedResources () { base.ReleaseUnmanagedResources(); }
	protected override void ReleaseManagedResources () { base.ReleaseManagedResources(); }
}