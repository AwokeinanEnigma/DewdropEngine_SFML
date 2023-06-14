using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.Scenes;
using DewDrop;
using DewDrop.Entities;
using DewDrop.Utilities;
using DewDrop.GUI.Fonts;
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
        ShapeEntity swagity;
        public EntityManager EntityManager { get; private set; }
        public TestScene()
        {
            this.pipeline = new RenderPipeline(Engine.RenderTexture);
            EntityManager = new EntityManager();
 
            swagity = new(
                new RectangleShape(new Vector2f(10,10)), 
                new Vector2(160, 90), 
                new Vector2(10,10), 
                new Vector2(5,5), 5000, Color.Green, Color.Green);
 EntityManager.AddEntity(swagity);
 pipeline.Add(swagity);

            //  Engine.ClearColor = Color.Blue;
            FontData DefaultFont = new FontData();

            this.title = new TextRenderer(new Vector2(3f, 8f), 555, DefaultFont, "John Lemon..");
            title.Color = Color.Yellow;

            //todo - change this to a nonpersistant path
            //texture = new SpriteGraphic($"C:\\Users\\Tom\\Documents\\bear.dat", "walk north", new Vector2(160, 90), 100);
            texture2 = new SpriteGraphic($"C:\\Users\\Tom\\Documents\\tree2.dat", "default", new Vector2(65, 90), 100);
           //belringtreehead
           texture = new SpriteGraphic($"C:\\Users\\Tom\\Documents\\belringtreehead.dat", "default", new Vector2(160, 90), 100);
              Input.Instance.OnMouseClick += (button, position) =>
            {
                Debug.Log($"Click button: {position.Button} at position: {Input.GetMousePosition() }");
            };
            
            NbtCompound rootTag = new NbtFile("C:\\Users\\Tom\\Documents\\Mother 4\\Union\\Resources\\Maps\\AAA.dat").RootTag;
            ((SpritesheetTexture)texture.Texture).ToFullColorTexture();
            pipeline.AddAll(MakeTileChunks(0, LoadTileChunks(rootTag)));
            //pipeline.Add(graphic);
         //
            //ViewManager.  Instance.View.Zoom(10);
         shape = new ShapeGraphic(
             new RectangleShape(new Vector2f(10,10)), 
             new Vector2(160, 90), 
             new Vector2(1,1), 
             new Vector2(5,5), 500, Color.Green, Color.Green);//);
         pipeline.Add(shape);   
         pipeline.Add(texture);
         pipeline.Add(texture2);
         pipeline.Add(new SpriteGraphic($"C:\\Users\\Tom\\Documents\\travis_oddity.dat", "idle", new Vector2(190, 90), 90));
         pipeline.Add(new SpriteGraphic($"C:\\Users\\Tom\\Documents\\stump.dat", "default", new Vector2(65, 95 + (82/2)), 90));
            pipeline.Add(new SpriteGraphic($"C:\\Users\\Tom\\Documents\\stump2.dat", "default", new Vector2(160, 115 + (28/2)), 90));

            Input.Instance.OnKeyPressed += (key, key2) =>
            {
                Debug.Log(key2);
            };
            Engine.RenderImGUI += () =>
            {
                ImGui.Begin("Dewdrop Debug Utilities");
                
                if (ImGui.Button("down"))
                {
                    ViewManager.Instance.Center += new Vector2(0,50);
                }
                
                if (ImGui.Button("right"))
                {
                    ViewManager.Instance.Center += new Vector2(50,0
                    );
                }
                if (ImGui.Button("left"))
                {
                    ViewManager.Instance.Center += new Vector2(-50,0);
                }
                
                if (ImGui.Button("up"))
                {
                    ViewManager.Instance.Center += new Vector2(0,-50);
                }
                
               // ImGui.SliderFloat("Amplitude", ref amplitude, 0, 500);
               // ImGui.SliderFloat("Frequency", ref frequency, 0, 500);

      
                ImGui.InputFloat("Amplitude", ref amplitude);
                ImGui.InputFloat("Frequency", ref frequency);
                ImGui.InputFloat("Division", ref division);
                
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
                Debug.Log(key2);
                if (key2 == Keyboard.Key.F1)
                {
                    EntityManager.ClearEntities();

                }
            };
            
            //this.pipeline.Add(this.title);
            Debug.DumpLogs();
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
            texture.Rotation = /*(int)*/amplitude * (float)Math.Sin(frequency * MathF.PI *  Engine.SessionTimer.ElapsedTime.AsSeconds()/ division);

            texture2.Rotation = amplitude - 1 * (float)Math.Sin(frequency - 1* MathF.PI * Engine.SessionTimer.ElapsedTime.AsSeconds() + 1 /* for random offset!! */ / division);
            //texture.Rotation = -955 * Engine.SessionTimer.ElapsedTime.AsSeconds();
            //title.Text =$"MGC: {(GC.GetTotalMemory(false) / 1024L) * 0.001}MB\n";
                
            if (Input.Instance[Keyboard.Key.A])
            {
                Debug.Log("hit breakpoint");
            }
            EntityManager.Update();
        }
 
        
        public IList<TileChunk>  MakeTileChunks(uint palette, List<Group> groups)
        {
            string arg = "default";

            string resource = "C:\\Users\\Tom\\Documents\\Mother 4\\Union\\Resources\\Graphics\\echogulch.dat";// string.Format("{0}{1}.mtdat", graphicDirectory, arg);
            IList<TileChunk> list = new List<TileChunk>(groups.Count);
            long ticks = DateTime.Now.Ticks;
            for (int i = 0; i < groups.Count; i++)
            {
                Group mapGroups = groups[i];
                IList<Tile> tileList = new List<Tile>(mapGroups.Tiles.Length / 2);
                int o = 0;
                int index = 0;
                bool Tree = false;
                while (o < mapGroups.Tiles.Length)
                {

                    int intTile = mapGroups.Tiles[o] - 1;

                    if (intTile >= 0)
                    {
                        ushort tileData;
                        if (o + 1 < mapGroups.Tiles.Length)
                        {
                            tileData = mapGroups.Tiles[o + 1];
                        }
                        else
                        {
                            tileData = 0;
                        }
                        int width = mapGroups.Width * 8;
                        Vector2f position = new Vector2f(index * 8L % width, index * 8L / width * 8L);
                        bool flipHoriz = (tileData & 1) > 0;
                        bool flipVert = (tileData & 2) > 0;
                        bool flipDiag = (tileData & 4) > 0;
                        ushort animId = (ushort)(tileData >> 3);
                        
                        Tile item = new Tile((uint)intTile, position, flipHoriz, flipVert, flipDiag, animId);
                        tileList.Add(item);
                    }
                    o += 2;
                    index++;
                }
                
                TileChunk item2 = new TileChunk(tileList.ToArray(), resource, mapGroups.Depth, new Vector2f(mapGroups.X, mapGroups.Y), palette);
                list.Add(item2);
            }
            Debug.LogInfo($"Created tile groups in {(DateTime.Now.Ticks - ticks) / 10000L}ms");
            return list;
        }

        private List<Group> LoadTileChunks(NbtCompound mapTag)
        {
            // Check if the mapTag has a "tiles" property
            NbtTag tilesTag = mapTag.Get("tiles");

            // Return if the tilesTag is not a collection
            if (!(tilesTag is ICollection<NbtTag>))
            {
                return null;
            }

            List<Group> Groups = new();

            // Iterate through each tile compound in the tilesTag collection
            foreach (NbtTag tileTag in (IEnumerable<NbtTag>)tilesTag)
            {
                // Check if the tileTag is a compound
                if (tileTag is NbtCompound tileCompound)
                {
                    // Get the values from the tile compound
                    uint depth = (uint)tileCompound.Get<NbtInt>("depth").Value;
                    int x = tileCompound.Get<NbtInt>("x").Value;
                    int y = tileCompound.Get<NbtInt>("y").Value;
                    int width = tileCompound.Get<NbtInt>("w").Value;
                    int tree = tileCompound.Get<NbtInt>("tree").Value;

                    Debug.Log($"WE MIGHT HAVE A TREE GROUP: {tree}");
                    if (tree > 0)
                    {
                        Debug.Log("WE GOT A TREE GROUP");
                    }

                    // Get the tiles as a byte array and convert it to a ushort array
                    byte[] src = tileCompound.Get<NbtByteArray>("tiles").Value;
                    ushort[] dst = new ushort[src.Length / 2];
                    Buffer.BlockCopy(src, 0, dst, 0, src.Length);

                    // Create a new Group object with the values
                    Group group = new Group()
                    {
                        Depth = depth,
                        X = x,
                        Y = y,
                        Width = width,
                        Height = dst.Length / 2 / width,
                        Tiles = dst,
                        Tree = tree == 1 ? true : false,
                    };

                    // Add the Group object to the map's Groups list
                    Groups.Add(group);
                }
            }

            return Groups;
        }
                public struct Group
                {
                    public ushort[] Tiles;

                    public uint Depth;

                    public int X;

                    public int Y;

                    public int Width;

                    public int Height;

                    public bool Tree;
                }
        
        public override void Draw()
        {
            this.pipeline.Draw();
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
