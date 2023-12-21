using DewDrop.GameObject;
namespace Prototype; 

public class MyComponent : Component {
	public override int Importance => 10;
	public override void Awake () {
		
	}
	public override void Start () { }
	public override void Update () { }
	public override void Draw () { }
	public override void Destroy () { }
}
