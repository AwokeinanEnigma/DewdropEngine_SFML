#region

using DewDrop;
using DewDrop.Collision;
using DewDrop.Entities;
using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.GUI.Fonts;
using DewDrop.Inspector;
using DewDrop.Maps;
using DewDrop.Maps.MapData;
using DewDrop.Scenes;
using DewDrop.Tiles;
using DewDrop.Updatable;
using DewDrop.UserInput;
using DewDrop.Utilities;
using DewDrop.Wren;
using ImGuiNET;
using Mother4.GUI;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics;
using Color = SFML.Graphics.Color;

#endregion

namespace Prototype.Scenes; 

public class TestScene : SceneBase
{
    private Player _playerEntity;
    public Text funnyText;
    private ShapeEntity2 overlayEntity;
    //private Wrentity wren;

    private RenderPipeline pipeline;
    LineRenderer line;
    private TextRenderer title;
    public EntityManager EntityManager { get; private set; }
    public CollisionManager CollisionManager { get; private set; }
    public List<TriggerArea> Triggers { get; private set; }
    public TextBox TextBox;

    Inspector _inspector;
    public TestScene()
    {
        #region Initialize

        funnyText = new Text("swag", new FontData().Font);
        funnyText.FillColor = Color.Red;

        pipeline = new RenderPipeline(Engine.RenderTexture);
        EntityManager = new EntityManager();
        WrenPipelineWrapper.Pipeline = pipeline;
            
        #endregion

        #region  Tiles
            
        MapLoader loader = new("testmapg.dat");
        Map mapFile = loader.Load();
        Triggers = new List<TriggerArea>();
        CollisionManager = new CollisionManager(mapFile.Width, mapFile.Height);
        pipeline.AddAll(MakeTileChunks(0, mapFile.TileChunkData));
        foreach (var trigger in mapFile.Triggers)
        {
            Triggers.Add(new TriggerArea(trigger.Position, trigger.Points, trigger.Flag, trigger.Script));
        }
        CollisionManager.AddAll(Triggers);

        mapFile.Collisions.ForEach(x => CollisionManager.Add( new StaticCollider(x)));
        #endregion

        TextBox = new TextBox(pipeline, 0);
        EntityManager.AddEntity(TextBox);
        TextBox.OnTextboxComplete += () => TextBox.Hide();
            
        TextBox.Hide();
        WrenContext.TextBox = TextBox;
        #region Create Entities
        TextureManager.Instance.DumpLoadedTextures();

        _playerEntity = new Player(
            new RectangleShape(new Vector2f(11, 20)),
            new Vector2(160, 90),
            new Vector2(11, 20),
            new Vector2(0, 0), 90000, pipeline, CollisionManager, Color.Green, Color.Green);
        EntityManager.AddEntity(_playerEntity);
        pipeline.Add(_playerEntity);
        CollisionManager.Add(_playerEntity);

        Wrentity wren = new Wrentity(File.ReadAllText(Directory.GetCurrentDirectory() + "/wrenity.wren"), new RectangleShape(new Vector2f(11, 20)),
            new Vector2(160, 90),
            new Vector2(11, 20),
            new Vector2(0, 0), 90000, pipeline, CollisionManager, Color.Blue, Color.Blue);
        EntityManager.AddEntity(wren);
        pipeline.Add(wren);
        CollisionManager.Add(wren);
            
        overlayEntity = new ShapeEntity2(
            new RectangleShape(new Vector2f(1, 1)),
            new Vector2(160, 90),
            new Vector2(20, 20),
            new Vector2(0, 0), 90000, pipeline, Color.Blue, Color.Blue);
            
            
            
        EntityManager.AddEntity(overlayEntity);
        pipeline.Add(overlayEntity);
        _inspector = new Inspector();
        _inspector.Initialize(EntityManager, CollisionManager);
            
        #endregion
        line = new LineRenderer(_playerEntity.Position, _playerEntity.Position, new Vector2(3000,3000), new Vector2(0, 0),10000, Color.Yellow);
        pipeline.Add(line);

        Engine.OnRenderImGui += EngineOnRenderImGUI;
        Input.OnKeyPressed += InstanceOnOnKeyPressed;

    }
        
    public ICollidable[] results = new ICollidable[8];

    private void InstanceOnOnKeyPressed(object? sender, Keyboard.Key key)
    {
        if (key == Keyboard.Key.Tilde) 
            Engine.DebugMode = !Engine.DebugMode;
        if (key == Keyboard.Key.F1 && !SceneManager.IsTransitioning)
        {
            SceneManager.Push(new DebugPlayground(false), true);
        }
        if (!TextBox.Visible && key == Keyboard.Key.E) {
            RenderPNG();
            var lino = new LineRenderer(_playerEntity.Position, _playerEntity.Position + Vector2.Normalize(_playerEntity.CheckVector)*25, new Vector2(3000, 3000), new Vector2(0, 0), 10000, Color.Magenta);
            pipeline.Add(lino);
            List<RaycastHit> intersectedCollidables = CollisionManager.RaycastAll(
                _playerEntity.Position,
                Vector2.Normalize(_playerEntity.CheckVector),
                25);

            if (intersectedCollidables.Count > 0) {

                Outer.Log($"""Found {intersectedCollidables.Count} collidables""");
                for (int i = 0; i < intersectedCollidables.Count; i++) {
                    ICollidable collidable = intersectedCollidables[i].Collider;
                    Outer.Log("Found collidable: " + collidable.GetType().FullName);
                    if (collidable is Wrentity wrentity) {
                        line.SetPosition(0, intersectedCollidables[i].Point);
                        wrentity.Interact();
                        break;
                    }
                    if (collidable is StaticCollider) {
                        line.SetPosition(0, intersectedCollidables[i].Point);
                        break;
                    }
                }
            }
        }
        if (key == Keyboard.Key.G){
            Outer.Log("Checking collisions");
            //draw floatrect
            FloatRect rect = new FloatRect( -15f, -15f, 30, 30);
            FloatRectDrawer drawer = new FloatRectDrawer(rect, _playerEntity.Position);
            pipeline.Add(drawer);
                
            List<ICollidable> overlap = CollisionManager.OverlapBoxAll(_playerEntity.Position, rect);
            Outer.Log(overlap.Count - 1);
            overlap.ForEach(x => {
                if (x is Wrentity wrentity) {
                    Outer.Log("Found wrentity");
                    Outer.Log(wrentity.Position);
                }
            });
        }
        
        //SceneManager.Pop();
    }


    private void EngineOnRenderImGUI () {
        try {
            ImGui.Begin("entities");
            for (int i = 0; i < EntityManager.Entities.Count; i++) {
                ImGui.Text(EntityManager.Entities[i].Name);
            }
            ImGui.End();
        }
        catch (Exception e) {
            Outer.LogError(e.Message, null);
        }
    }
    public static void RenderPNG()
    {
        SFML.Graphics.Image image3 = Engine.RenderTexture.Texture.CopyToImage();
        string text = string.Format("screenshot{0}.png", Directory.GetFiles("./", "Screenshot*.png").Length);
        image3.SaveToFile(text);
        Outer.LogInfo("Screenshot saved as \"{0}\"", text);
    }
        
    public IList<TileChunk>  MakeTileChunks(uint palette, List<TileChunkData> groups)
    {
        string arg = "default";

        //string resource = "C:\\Users\\Tom\\Documents\\Mother 4\\Union\\Resources\\Graphics\\cave2.dat";// string.Format("{0}{1}.mtdat", graphicDirectory, arg);
        string resource = "testmap.dat";
        List<TileChunk> list = new(groups.Count);
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
            TileChunk item2 = new(tiles, resource, (int)group.Depth, new Vector2f(group.X, group.Y), palette);
            list.Add(item2);
        }
        Console.WriteLine("Created tile groups in {0}ms", (DateTime.UtcNow.Ticks - ticks) / 10000L);
        return list; 
    }

    public override void Focus()
    {
        base.Focus();
 
        ViewManager.Instance.Reset();
        ViewManager.Instance.EntityFollow = _playerEntity;
        ViewManager.Instance.Center = new Vector2(160f, 90f);
        ViewManager.Instance.Offset = new Vector2(0, (float)(-(float)((int)_playerEntity.AABB.Size.Y)/2));
    }


    public override void Update()
    {
        base.Update();
        EntityManager.Update();
        CollisionManager.UpdateTriggers();
        line.SetPosition(1, _playerEntity.Position);
  
    }

    public override void TransitionIn () {
        base.TransitionIn();
        Outer.Log("Created wreno.");
        /*  wren = new Wrentity(File.ReadAllText(Directory.GetCurrentDirectory() + "/wrentity.wren"), new RectangleShape(new Vector2f(11, 20)),
              new Vector2(160, 90),
              new Vector2(11, 20),
              new Vector2(0, 0), 90000, pipeline, CollisionManager, Color.Blue, Color.Blue);*/
        //  EntityManager.AddEntity(wren);
        //   pipeline.Add(wren);
        //  CollisionManager.Add(wren);
    }

    public override void Draw()
    {
        pipeline.Draw();
        Engine.RenderTexture.Draw(funnyText);
        if (Engine.DebugMode)
        {
            CollisionManager.Draw(pipeline.Target);
        }      base.Draw();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            CollisionManager.Clear();
            EntityManager.ClearEntities();
            pipeline.Clear(true);
            TextureManager.Instance.Purge();
            Triggers.ForEach(x => x.Dispose());
            Triggers.Clear();
            ViewManager.Instance.EntityFollow = null;
            _inspector.Dispose();
            disposed = true;
            // dispose here
        }
        Engine.OnRenderImGui -= EngineOnRenderImGUI;
        Input.OnKeyPressed -= InstanceOnOnKeyPressed;
        ViewManager.Instance.Center = new Vector2(0, 0);

        base.Dispose(disposing);
    }
}