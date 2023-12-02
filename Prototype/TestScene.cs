#region

using DewDrop;
using DewDrop.Collision;
using DewDrop.Entities;
using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.GUI.Fonts;
using DewDrop.Maps;
using DewDrop.Scenes;
using DewDrop.Tiles;
using DewDrop.UserInput;
using DewDrop.Utilities;
using ImGuiNET;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Color = SFML.Graphics.Color;

#endregion

namespace Prototype.Scenes
{
    public class TestScene : SceneBase
    {
        private Player _playerEntity;
        public Text funnyText;
        private ShapeEntity2 overlayEntity;

        private RenderPipeline pipeline;

        private TextRenderer title;
        public EntityManager EntityManager { get; private set; }
        public CollisionManager CollisionManager { get; private set; }

        public TestScene()
        {
            #region Initialize

            funnyText = new Text("swag", new FontData().Font);
            funnyText.FillColor = Color.Red;

            pipeline = new RenderPipeline(Engine.RenderTexture);
            EntityManager = new EntityManager();
            #endregion

            #region  Tiles
            
            MapLoader loader = new("testmap.dat");
            Map mapFile = loader.Load();
            CollisionManager = new CollisionManager(mapFile.Width, mapFile.Height);
            pipeline.AddAll(MakeTileChunks(0, mapFile.TileChunkData));

            mapFile.Collisions.ForEach(x => CollisionManager.Add( new StaticCollider(x)));
            #endregion
            
            
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


            overlayEntity = new ShapeEntity2(
                new RectangleShape(new Vector2f(1, 1)),
                new Vector2(160, 90),
                new Vector2(20, 20),
                new Vector2(0, 0), 90000, pipeline, Color.Blue, Color.Blue);
            EntityManager.AddEntity(overlayEntity);
            pipeline.Add(overlayEntity);

            #endregion

            Engine.RenderImGUI += EngineOnRenderImGUI;
            Input.OnKeyPressed += InstanceOnOnKeyPressed;

        }

        private void InstanceOnOnKeyPressed(object? sender, Keyboard.Key key)
        {
            if (key == Keyboard.Key.Tilde) 
                Engine.DebugMode = !Engine.DebugMode;
            if (key == Keyboard.Key.F1 && !SceneManager.Instance.IsTransitioning)
            {
                SceneManager.Instance.Push(new DebugPlayground(false), true);
            }

            //SceneManager.Instance.Pop();
        }


        private void EngineOnRenderImGUI()
        {

        }
        
        public IList<TileChunk>  MakeTileChunks(uint palette, List<TileChunkData> groups)
        {
            string arg = "default";

            //string resource = "C:\\Users\\Tom\\Documents\\Mother 4\\Union\\Resources\\Graphics\\cave2.dat";// string.Format("{0}{1}.mtdat", graphicDirectory, arg);
            string resource = "testmapg.dat";
            List<TileChunk> list = new(groups.Count);
            long ticks = DateTime.Now.Ticks;
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
                        if (tileIndex + 1 < group.Tiles.Length)
                        {
                            tileModifier = group.Tiles[tileIndex + 1];
                        }
                        else
                        {
                            tileModifier = 0;
                        }
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
            Console.WriteLine("Created tile groups in {0}ms", (DateTime.Now.Ticks - ticks) / 10000L);
            return list; 
        }

        public override void Focus()
        {
            base.Focus();
            ViewManager.Instance.EntityFollow = _playerEntity;
            ViewManager.Instance.Center = new Vector2(160f, 90f);
        }


        public override void Update()
        {
            base.Update();
            EntityManager.Update();
  
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
                
                ViewManager.Instance.EntityFollow = null;
                disposed = true;
                // dispose here
            }
            Engine.RenderImGUI -= EngineOnRenderImGUI;
            Input.OnKeyPressed -= InstanceOnOnKeyPressed;
            ViewManager.Instance.Center = new Vector2(0, 0);

            base.Dispose(disposing);
        }
    }
}