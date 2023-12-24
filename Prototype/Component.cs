using DewDrop;
using DewDrop.Internal; using DewDrop.GUI;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;
using Transform = DewDrop.Internal.Transform;
namespace Prototype; 

public class MyComponent : Component {
	public override int Importance => 10;
	private Shape _shape;
	public float offset;
	public bool dos = false;
	public  void Awake () {
		_shape = new RectangleShape(new Vector2(32, 32));
		_shape.Position = new Vector2(transform.Position.X, transform.Position.Y);
		_shape.FillColor = Color.Red;
		_shape.Origin = new Vector2(16, 16);
		transform.Size = new Vector2(32, 32);
		//Outer.Log("Awake on frame " + GameObject.FrameRegistered);
	}
	public  void Start () {
		//Outer.Log("Start on frame " + Engine.Frame);
	}
	public void Update (){
		if (dos) {
			transform.Position = new Vector3(0 + offset, (ViewManager.Instance.Center.y + 90 + offset)*(float)MathF.Sin((2*MathF.PI*Engine.SessionTimer.ElapsedTime.AsSeconds())/2), 0);
			transform.Rotation = (float)MathF.Sin((2*MathF.PI*Engine.SessionTimer.ElapsedTime.AsSeconds())/2) * 360
			; 
		}
		_shape.Rotation = transform.Rotation; 
		_shape.Position = new Vector2(transform.Position.X, transform.Position.Y);
		//Outer.Log("Update");
	}
	public void Draw (RenderTarget target) {
		target.Draw(_shape);
	//	Outer.Log("Draw");
	}

	protected override void ReleaseManagedResources () {
		base.ReleaseManagedResources();
		_shape.Dispose();
	}

	public override void Destroy () {
		Outer.Log("Destroy");
	}
}
