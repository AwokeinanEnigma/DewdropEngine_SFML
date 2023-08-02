using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.Scenes;
using DewDrop;
using DewDrop.Collision;
using DewDrop.Entities;
using DewDrop.Utilities;
using DewDrop.GUI.Fonts;
using DewDrop.Maps;
using DewDrop.Tiles;
using DewDrop.UserInput;
using fNbt;
using ImGuiNET;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace Prototype.Scenes
{
    public class TestScene : SceneBase
    {
        private RenderPipeline pipeline;

        private TextRenderer title;
        private TextRenderer message;
        private TextRenderer pressenter;
        private TextRenderer exceptionDetails;
        private TextRenderer additionalUserDetails;
        public SpriteGraphic texture;
        public SpriteGraphic texture2;
        private ShapeGraphic shape;
        Player swagity;
        public EntityManager EntityManager { get; private set; }
        
        public Text text;
        public CollisionManager manager;
        
        public TestScene()
        {
            this.pipeline = new RenderPipeline(Engine.RenderTexture);
            Map loader = new MapLoader("C:\\Users\\Tom\\Documents\\elurbanodesperadoworkbin\\Resources\\Maps\\testmap.dat").Load();
            pipeline.AddAll(MakeTileChunks(0, loader.TileChunkData));
            manager = new CollisionManager(900, 900);
            
            EntityManager = new EntityManager();
 text = new Text("swag", new FontData().Font);
 text.FillColor = Color.Red;
 loader.Collisions.ForEach(x => manager.Add(new StaticCollider(x)));// manager.Add();
 swagity = new(
                new RectangleShape(new Vector2f(11,20)), 
                new Vector2(160, 90), 
                new Vector2(11,20), 
                new Vector2(0,0), 90000, pipeline , manager, Color.Green, Color.Green);
            EntityManager.AddEntity(swagity);
            pipeline.Add(swagity);
            manager.Add(swagity);

            var aaa = new ShapeEntity2(
                new RectangleShape(new Vector2f(1,1)), 
                new Vector2(160, 90), 
                new Vector2(20,20), 
                new Vector2(0,0), 90000, pipeline , Color.Blue, Color.Blue);

            EntityManager.AddEntity(aaa);
            pipeline.Add(aaa);

            //  Engine.ClearColor = Color.Blue;
            FontData DefaultFont = new FontData();

            this.title = new TextRenderer(new Vector2(3f, 8f), 555, DefaultFont, "John Lemon..");
            title.Color = Color.Yellow;

            //todo - change this to a nonpersistant path
            //texture = new SpriteGraphic($"C:\\Users\\Tom\\Documents\\bear.dat", "walk north", new Vector2(160, 90), 100);
            //texture2 = new SpriteGraphic($"C:\\Users\\Tom\\Documents\\tree2.dat", "default", new Vector2(65, 90), 100);
           //belringtreehead
           //texture = new SpriteGraphic($"C:\\Users\\Tom\\Documents\\belringtreehead.dat", "default", new Vector2(160, 90), 100);
              Input.Instance.OnMouseClick += (button, position) =>
            {
                DDDebug.Log($"Click button: {position.Button} at position: {Input.GetMousePosition() }");
            };
              

            //pipeline.Add(graphic);
         //
            //ViewManager.  Instance.View.Zoom(10);
         shape = new ShapeGraphic(
             new RectangleShape(new Vector2f(10,10)), 
             new Vector2(160, 90), 
             new Vector2(1,1), 
             new Vector2(5,5), 500, Color.Green, Color.Green);//);
         /*pipeline.Add(shape);   
         pipeline.Add(texture);
         pipeline.Add(texture2);
         pipeline.Add(new SpriteGraphic($"C:\\Users\\Tom\\Documents\\travis_oddity.dat", "idle", new Vector2(190, 90), 90));
         pipeline.Add(new SpriteGraphic($"C:\\Users\\Tom\\Documents\\stump.dat", "default", new Vector2(65, 95 + (82/2)), 90));
            pipeline.Add(new SpriteGraphic($"C:\\Users\\Tom\\Documents\\stump2.dat", "default", new Vector2(160, 115 + (28/2)), 90));*/
         
         Input.Instance.OnKeyPressed += (key, key2) =>
            {
                if (key2 == Keyboard.Key.F1)
                {
                    pipeline.Remove(swagity);
                    pipeline.Remove(aaa);
                    EntityManager.ClearEntities();
                }
            };
            Engine.RenderImGUI += () =>
            {
                ImGui.Begin("Dewdrop Debug Utilities");
                ImGui.Text($"Garbage Allocated: {GC.GetTotalMemory(false) / 1024L}");
                
               // ImGui.SliderFloat("Amplitude", ref amplitude, 0, 500);
               // ImGui.SliderFloat("Frequency", ref frequency, 0, 500);
               ImGui.Separator();
                
               
               //ImGui.SetClipboardText("faggot");
               
               if (ImGui.Button("Force GC Collection"))
               {
                   GC.Collect();
               }

               /*ImGui.InputFloat("Amplitude", ref amplitude);
               ImGui.InputFloat("Frequency", ref frequency);
               ImGui.InputFloat("Division", ref division);*/
                
                /*ImGui.Text("I'm");
                ImGui.Text("going");
                ImGui.Text("to");
                ImGui.Text("have");
                ImGui.Text("back");
                ImGui.Text("breaking");
                ImGui.Text("sex");
                ImGui.Text("with");
                ImGui.Text("your");
                ImGui.Text("mother");*/

                ImGui.End();
            };
            
            Input.Instance.OnKeyPressed += (key, key2) =>
            {
                if (key2 == Keyboard.Key.F1)
                {
                    EntityManager.ClearEntities();

                }
            };
            
            //this.pipeline.Add(this.title);
            DDDebug.DumpLogs();
        }

        public override void Focus()
        {
            base.Focus();
            ViewManager.Instance.EntityFollow = null;
            ViewManager.Instance.Center = new Vector2(160f, 90f);
            //Engine.ClearColor = Color.Black;
        }

        // Adjust this value to change the wave intensity
        public float amplitude = 3.5f;
        // Adjust this value to change the wave speed
        public float frequency = 1.25f;
        public float division = 1.1f;
        public override void Update()
        {
            base.Update();
            //Engine.DebugMode = false;
           shape.Visible = false;
           ViewManager.Instance.EntityFollow = swagity;
            // shape.Position =new Vector2(160, (title.Position.y + 90) * (float)MathF.Sin((2 * MathF.PI * Engine.SessionTimer.ElapsedTime.AsSeconds()) / 2));
           //; texture.Position =  Input.GetMousePosition(); // new Vector2(160, (title.Position.y + 90) * (float)MathF.Sin((2 * MathF.PI * Engine.SessionTimer.ElapsedTime.AsSeconds()) / 2));
            //texture.Rotation = /*(int)*/amplitude * (float)Math.Sin(frequency * MathF.PI *  Engine.SessionTimer.ElapsedTime.AsSeconds()/ division);

            //texture2.Rotation = amplitude - 1 * (float)Math.Sin(frequency - 1* MathF.PI * Engine.SessionTimer.ElapsedTime.AsSeconds() + 1 /* for random offset!! */ / division);
            //texture.Rotation = -955 * Engine.SessionTimer.ElapsedTime.AsSeconds();
            //title.Text =$"MGC: {(GC.GetTotalMemory(false) / 1024L) * 0.001}MB\n";
            if (Input.Instance[Keyboard.Key.A])
            {
             //   Debug.Log("breakpoint");
            }
            ViewManager.Instance.Shake(new Vector2(1,1), 1);
            EntityManager.Update();
        }
 
        
        public IList<TileChunk>  MakeTileChunks(uint palette, List<TileChunkData> groups)
        {
            string arg = "default";

            //string resource = "C:\\Users\\Tom\\Documents\\Mother 4\\Union\\Resources\\Graphics\\cave2.dat";// string.Format("{0}{1}.mtdat", graphicDirectory, arg);
            string resource = "C:\\Users\\Tom\\Documents\\elurbanodesperadoworkbin\\Resources\\Graphics\\testmap.dat";
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
                TileChunk item2 = new(tiles, resource, (int)group.Depth, new Vector2f(group.X, group.Y), palette, true, Color.White);
                list.Add(item2);
            }
            Console.WriteLine("Created tile groups in {0}ms", (DateTime.Now.Ticks - ticks) / 10000L);
            return list; 
        }

        
        public override void Draw()
        {
            this.pipeline.Draw();
            Engine.RenderTexture.Draw(text);
            if (Engine.DebugMode)
            {
                manager.Draw(pipeline.Target);
            }
            base.Draw();
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                this.title.Dispose();
                this.message.Dispose();
                this.pressenter.Dispose();
                this.exceptionDetails.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
