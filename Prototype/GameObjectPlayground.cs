using DewDrop;
using DewDrop.GameObject;
using DewDrop.Scenes;
using DewDrop.UserInput;
using DewDrop.Utilities;
using SFML.Window;
using System.Diagnostics;
namespace Prototype; 

public class GameObjectPlayground : SceneBase{
	public GameObjectPlayground () {
		
	}

	public GameObject obj;
	public GameObject obj2;
	public override void Focus () {
		base.Focus();
		GameObjectRegister.Initialize(Engine.RenderTexture);
		obj = new GameObject();
		obj.Name = "Test Object";
		obj.Transform.DrawRegardlessOfVisibility = true;
		obj.Transform.Position = new Vector3(160, 90, 0);
		obj.AddComponent<MyComponent>().dos = true;
		GameObjectRegister.AddGameObject(obj);

		
		obj2 = obj.Clone();
		obj2.GetComponent<MyComponent>().dos = false;
		obj2.Transform.Position = new Vector3(90,90,0);
		obj2.Name = "Test Object 2";
		obj2.Transform.DrawRegardlessOfVisibility = true;
		//Debug.Assert(gameObject.GetComponent<MyComponent>() == obj2.GetComponent<MyComponent>());
		obj2.Transform.SetParent(obj.Transform);
		GameObjectRegister.AddGameObject(obj2);
		
	}
	public override void TransitionIn () { base.TransitionIn(); }
	public override void Unfocus () { base.Unfocus(); }
	bool dunnit;
	public override void Update () {
		base.Update(); 
		if (GameObjectRegister.Initialized) 
			GameObjectRegister.Update();
		
		if (Input.Instance[Keyboard.Key.A] && !dunnit) {
			obj2.Destroy();
		}
		
		if (Input.Instance[Keyboard.Key.D] && !dunnit) {
			obj.Destroy();
			dunnit = true;
		}
	}
	public override void Draw () {
		base.Draw();
		if (GameObjectRegister.Initialized) 
			GameObjectRegister.Draw();
	}
	protected override void Dispose (bool disposing) { base.Dispose(disposing); }
}
