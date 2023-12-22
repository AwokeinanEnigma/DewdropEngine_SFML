using DewDrop;
using DewDrop.Internal; using DewDrop.Scenes;
using DewDrop.Scenes.Transitions;
using DewDrop.UserInput;
using DewDrop.Utilities;
using ImGuiNET;
using Prototype.Scenes;
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
		obj.Transform.Position = new Vector3(160, 90, 1);
		obj.UpdateSlot = 1;
		obj.AddComponent<MyComponent>().dos = true;
		GameObjectRegister.AddGameObject(obj);

		
		obj2 = obj.Clone();
		obj2.GetComponent<MyComponent>().dos = false;
		obj2.Transform.Position = new Vector3(90,90,2);
		obj2.Name = "Test Object 2";
		obj2.UpdateSlot = 2;
		obj2.Transform.DrawRegardlessOfVisibility = true;
		//Debug.Assert(gameObject.GetComponent<MyComponent>() == obj2.GetComponent<MyComponent>());
		obj2.Transform.SetParent(obj.Transform);
		GameObjectRegister.AddGameObject(obj2);
		
		var obj3 = obj.Clone();
		obj3.RemoveComponent<MyComponent>();

		obj3.UpdateSlot = 3;
		PlayerComponent playerComponent = obj3.AddComponent<PlayerComponent>();
		obj3.Transform.Position = new Vector3(21,50,3);
		obj3.Name = "Test Object 3";
		obj3.Transform.DrawRegardlessOfVisibility = true;
		GameObjectRegister.AddGameObject(obj3);

		Engine.OnRenderImGui += EngineOnRenderImGUI;
	}
	
	Dictionary<GameObject, bool> _drawn = new Dictionary<GameObject, bool>();
	void DrawGameObject (GameObject gameObject) {
		ImGui.Text($"Position: {gameObject.Transform.Position}");
		ImGui.Text($"Rotation: {gameObject.Transform.Rotation}");
		ImGui.Text($"Size: {gameObject.Transform.Size}");
		ImGui.Text($"Active: {gameObject.Active}");
		ImGui.Text($"Awakened: {gameObject.Awakened}");
		ImGui.Text($"Update Slot: {gameObject.UpdateSlot}");
		ImGui.Text($"Parent: {gameObject.Transform.Parent?.GameObject.Name ?? "None"}");
		ImGui.Text($"Children: {gameObject.Transform.ChildCount}");
	}
	void EngineOnRenderImGUI () {
		if (ImGui.CollapsingHeader("Hierarchy")) {
			// go through all gameobjects and draw them in a tree
			// if they have children, draw them as well
			// and then don't draw them again
			foreach (GameObject gameObject in GameObjectRegister.GameObjects) {
				//Outer.Log(gameObject.UpdateSlot);
				if (!_drawn.ContainsKey(gameObject)) {
					if (gameObject.Parent != null && !_drawn.ContainsKey(gameObject.Parent.GameObject)) {
						continue;
					}
					_drawn.Add(gameObject, true);
					if (ImGui.TreeNode(gameObject.Name)) {
						DrawGameObject(gameObject);
						if (gameObject.Transform.ChildCount > 0) {
							ImGui.Indent();
							for (int i = 0; i < gameObject.Transform.ChildCount; i++) {
								GameObject child = gameObject.Transform.Children[i].GameObject;
								if (!_drawn.ContainsKey(child)) {
									_drawn.Add(child, true);
									if (ImGui.TreeNode(child.Name)) {
										DrawGameObject(child);
										ImGui.TreePop();
									}
								}
							}
							ImGui.Unindent();
						}
						ImGui.TreePop();
					} 
				}
			}
		}
		_drawn.Clear();
	}
	public override void TransitionIn () { base.TransitionIn(); }
	public override void Unfocus () { base.Unfocus(); }
	bool dunnit;
	public override void Update () {
		base.Update(); 
		if (GameObjectRegister.Initialized) 
			GameObjectRegister.Update();
		
		if (Input.Instance[Keyboard.Key.P] && !dunnit) {
			obj2.Destroy();
		}
		
		if (Input.Instance[Keyboard.Key.O] && !dunnit) {
			obj.Destroy();
		}
		
		if (Input.Instance[Keyboard.Key.K] && !dunnit) {
			SceneManager.Transition = new InstantTransition();
			SceneManager.Push(new TestScene(), true);
		}
	}
	public override void Draw () {
		base.Draw();
		if (GameObjectRegister.Initialized) 
			GameObjectRegister.Draw();
	}
	protected override void Dispose (bool disposing) {
		Outer.Log("HEY");

		
		if (disposing && !disposed) {
			obj2 = null;
			obj = null;
			GameObjectRegister.Destroy();
		}
		base.Dispose(disposing); 
	}
}
