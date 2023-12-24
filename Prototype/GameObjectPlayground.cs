using DewDrop;
using DewDrop.Internal;
using DewDrop.Maps;
using DewDrop.Maps.MapData;
using DewDrop.Scenes;
using DewDrop.Scenes.Transitions;
using DewDrop.Tiles;
using DewDrop.UserInput;
using DewDrop.Utilities;
using ImGuiNET;
using Prototype.Scenes;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
namespace Prototype; 

public class GameObjectPlayground : SceneBase{
	public GameObjectPlayground () {
		
	}

	public GameObject obj;
	public GameObject obj2;
	bool init;
	long frameCount = 0;
	public override void Focus () {
		base.Focus();
		/*obj = new GameObject();
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
			GameObjectRegister.AddGameObject(obj2);*/
		frameCount = Engine.Frame;
		if (!init) {
			var obj3 = new GameObject();
			obj3.RemoveComponent<MyComponent>();

			obj3.UpdateSlot = -1;
			PlayerComponent playerComponent = obj3.AddComponent<PlayerComponent>();
			obj3.Transform.Position = new Vector3(21, 50, 3);
			obj3.Name = "Test Object 3";
			obj3.Transform.DrawRegardlessOfVisibility = true;
			GameObjectRegister.AddGameObject(obj3);

			MapLoader loader = new("railwaycave1.mdat");
			Map mapFile = loader.Load();
			GameObject mapParent = new();
			mapParent.Name = "Map Parent: " + mapFile.Title;
			GameObjectRegister.AddGameObject(mapParent);
			foreach (GameObject tileChunkData in MakeTileChunks(0, mapFile.TileChunkData)) {
				tileChunkData.Transform.SetParent(mapParent.Transform);
				tileChunkData.OnlyDraw = true;
				GameObjectRegister.AddGameObject(tileChunkData);
			}
			init = true;
		}
		Engine.OnRenderImGui += EngineOnRenderImGUI;
		Input.OnKeyPressed += InputOnKeyPressed;

	}

	Dictionary<GameObject, bool> _drawn = new Dictionary<GameObject, bool>();
	void DrawGameObject (GameObject gameObject) {
		ImGui.Text($"Position: {gameObject.Transform.Position}");
		ImGui.Text($"Rotation: {gameObject.Transform.Rotation}");
		ImGui.Text($"Size: {gameObject.Transform.Size}");
		bool visible = gameObject.Active;
		if (ImGui.Checkbox("Active", ref visible)) {
			gameObject.Active = visible;
		}
		ImGui.Text($"Can Have Children: {gameObject.Transform.CanHaveChildren}");
		ImGui.Text($"Update Slot: {gameObject.UpdateSlot}");
		ImGui.Text($"Parent: {gameObject.Transform.Parent?.GameObject.Name ?? "None"}");
	}
	void EngineOnRenderImGUI () {
		if (ImGui.CollapsingHeader("Hierarchy")) {
			// go through all gameobjects and draw them in a tree
			// if they have children, draw them as well
			// and then don't draw them again
			// go through in reverse order so that the root objects are drawn first
			
			for (int i = GameObjectRegister.GameObjects.Count - 1; i >= 0; i--) {
				GameObject gameObject = GameObjectRegister.GameObjects[i];
				//Outer.Log(gameObject.UpdateSlot);
				if (!_drawn.ContainsKey(gameObject)) {
					if (gameObject.Transform.Parent != null) {
						if (_drawn.ContainsKey(gameObject.Transform.Parent.GameObject)) {
							continue;
						}
					}
					_drawn.Add(gameObject, true);
					if (ImGui.TreeNode(gameObject.Name)) {
						DrawGameObject(gameObject);
						if (gameObject.Transform.ChildCount > 0) {
							ImGui.Indent();
							if (ImGui.CollapsingHeader($"Children: {gameObject.Transform.ChildCount}")) {
								for (int j = 0; j < gameObject.Transform.ChildCount; j++) {
									GameObject child = gameObject.Transform.Children[j].GameObject;
									if (!_drawn.ContainsKey(child)) {
										_drawn.Add(child, true);
										if (ImGui.TreeNode(child.Name)) {
											DrawGameObject(child);
											ImGui.TreePop();
										}
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
	
	public IList<GameObject>  MakeTileChunks(uint palette, List<TileChunkData> groups)
	{
		string arg = "default";

		//string resource = "C:\\Users\\Tom\\Documents\\Mother 4\\Union\\Resources\\Graphics\\cave2.dat";// string.Format("{0}{1}.mtdat", graphicDirectory, arg);
		string resource = "railwaycave1.gdat";
		List<GameObject> list = new(groups.Count);
		long ticks = DateTime.UtcNow.Ticks;

		for (int i = 0; i < groups.Count; i++)
		{
			TileChunkData group = groups[i];
                
			List<Tile> tiles = new(group.Tiles.Length / 2);
			int tileIndex = 0;
			int tileX = 0;
			while (tileIndex < group.Tiles.Length)
			{
				int tileID = group.Tiles[tileIndex] - 1;
				if (tileID >= 0)
				{
					ushort tileModifier;
					tileModifier = tileIndex + 1 < group.Tiles.Length ? group.Tiles[tileIndex + 1] : (ushort)0;
					int tileY = group.Width * 8;
					Vector2f position = new(tileX * 8L % tileY, tileX * 8L / tileY * 8L);
					bool flipHoriz = (tileModifier & 1) > 0;
					bool flipVert = (tileModifier & 2) > 0;
					bool flipDiag = (tileModifier & 4) > 0;
					ushort animId = (ushort)(tileModifier >> 3);
					Tile item = new((uint)tileID, position, flipHoriz, flipVert, flipDiag, animId);
					tiles.Add(item);
				}
				tileIndex += 2;
				tileX++;
			}
			// converting to array allocates extra memory, and it's just not needed
			GameObject gameObject = new(false);
			gameObject.Name = "TileChunk " + (i+ 1);
			TileChunk chunk = gameObject.AddComponent<TileChunk>();
			chunk.Setup(tiles, resource, (int)group.Depth, new Vector2(group.X, group.Y), palette);
			list.Add(gameObject);
		}
		Console.WriteLine("Created tile groups in {0}ms", (DateTime.UtcNow.Ticks - ticks) / 10000L);
		return list; 
	}
	
	public override void TransitionIn () { base.TransitionIn(); }
	public override void Unfocus () {
		base.Unfocus();
		Engine.OnRenderImGui -= EngineOnRenderImGUI;
		Input.OnKeyPressed -= InputOnKeyPressed;

	}
	bool dunnit;
	bool dunnit2;
	public override void Update () {
		base.Update();

	}
	void InputOnKeyPressed(object? sender, Keyboard.Key e) {
		if (e == Keyboard.Key.K && !SceneManager.IsTransitioning) {

			dunnit = true;
			SceneManager.Transition = new ColorFuckTransition(0.45f, new [] {
				Color.Red,
				Color.Blue,
				Color.Blue,
				Color.Red,
			});
			SceneManager.Push(new GameObjectPlayground(), false);
		}
		
		if (e == Keyboard.Key.Space && !SceneManager.IsTransitioning) 
		{
			dunnit2 = true;
			SceneManager.Transition = new ColorFuckTransition(0.45f, new [] {
				Color.Red,
				Color.Blue,
				Color.Blue,
				Color.Red,
			});			
			SceneManager.Pop();
		}
	}
	public override void Draw () {
		base.Draw();
	}
	protected override void Dispose (bool disposing) {
		Outer.Log("HEY");

		
		if (disposing && !disposed) {
			obj2 = null;
			obj = null;
			GameObjectRegister.Destroy(true);
		}
		base.Dispose(disposing); 
	}
}
