using System.Numerics;
using DewDrop;
using DewDrop.Graphics;
using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.Scenes;
using DewDrop.UserInput;
using DewDrop.Utilities;
using ImGuiNET;
using Prototype.Scenes;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Vector2 = DewDrop.Utilities.Vector2;

namespace Prototype;

public class TileScene : SceneBase
{
    public bool TileGridInitialized { get; private set; }
    public int TileGridX;
    public int TileGridY;
    public Tile[,] TileGrid;
    public const int tileSize = 8;
    
    public struct Tile
    {
        /// <summary>
        /// The position of this tile
        /// </summary>
        public readonly Vector2 Position;

        /// <summary>
        /// Is this tile flipped horizontally?
        /// </summary>
        public readonly bool FlipHorizontal;
        
        /// <summary>
        /// Is this tile flipped vertically?
        /// </summary>
        public readonly bool FlipVertical;
    }
    
    private RenderPipeline pipeline;
    public SpriteGraphic texture;
    public TileScene()
    {       
        Input.Instance.OnKeyPressed += (key, key2) =>
        {
            if (key2 == Keyboard.Key.Down)
            {
                ViewManager.Instance.Center += new Vector2(0, 8);
                // SceneManager.Instance.Push(new TestScene());
            }
        };
        Texture texture = new Texture("C:\\Users\\Tom\\Documents\\Mother 4\\Maple Knob\\MapleKnob.png");
        texture.GenerateMipmap();
        uint ySize = texture.Size.Y;
        uint xSize = texture.Size.X;
        Sprite sprite = new Sprite(texture);
        bool[,] selectedTiles = new bool[xSize, ySize];  
        Engine.RenderImGUI += () =>
        {
            ImGui.Begin("Tile Scene", ImGuiWindowFlags.HorizontalScrollbar); //if (ImGui.Begin("Tile Scene"))
            {
                var textureId = ImGuiSfml.ConvertGlTextureHandleToImTextureId(texture.NativeHandle);
                
                //ImGuiSfml.Image(texture, Color.White, Color.Blue);

                // The image might be larger than the ImGui window, so we want to add the current scroll value to the 
                // mouse position for the selected image 
                float scrollY = ImGui.GetScrollY();
                float scrollX = ImGui.GetScrollX();

                int imageWidth = (int)xSize * 2;
                int imageHeight = (int)ySize * 2;

                ImGui.Image(textureId , new Vector2(imageWidth, imageHeight));

                int mousePosX = (int)(ImGui.GetMousePos().X - (int)ImGui.GetWindowPos().X + scrollX);
                int mousePosY = (int)(ImGui.GetMousePos().Y - (int)ImGui.GetWindowPos().Y + scrollY);

                int rows = imageHeight / 8 ;
                int cols = imageWidth / 8;
                
                for (int i = 0; i < cols; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

                        int tilePosX = i * 8;
                        int tilePosY = j * 8;

                        // Check to see if the mouse position is within the tile's area
                        if (mousePosX >= tilePosX && mousePosX <= tilePosX + 8 &&
                            mousePosY >= tilePosY && mousePosY <= tilePosY + 8)
                        {
                            // Draw a border around the selected tile
                            drawList.AddRect(new Vector2(tilePosX, tilePosY), new Vector2(tilePosX + 8, tilePosY + 8), ImGui.GetColorU32(ImGuiCol.Border), 0, ImDrawCornerFlags.All, 16);
            
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.SetTooltip($"We are in the area of tile {i}, {j}");
                                if (ImGui.IsMouseClicked(0))
                                {
                                    // Handle tile selection
                                    // mTileProps.srcRectX = i * 8;
                                    // mTileProps.srcRectY = j * 8;
                                }
                            }
                        }
                    }
                }
                
                
                ImGui.InputInt("X", ref TileGridX);
                ImGui.InputInt("Y", ref TileGridY);
                if (ImGui.Button("Generate Tile Grid"))
                {

                    if (!TileGridInitialized
                        && TileGridX > 0 && TileGridY > 0)
                    {
                        TileGridInitialized = true;
                        DDDebug.Log("Tilegrid successfully initialized!");
                        TileGrid = new Tile[TileGridX, TileGridY];
                        // loop through every tile and add an sprite graphic to the render pipeline
                    }
                    else
                    {
                        if (TileGridInitialized)
                        {
                            DDDebug.Log("Tile Grid already initialized!");
                        }

                        if (TileGridX <= 0 || TileGridY <= 0)
                        {
                            DDDebug.Log("Tile Grid X and Y must be greater than 0!");
                        }
                    }
                }
            }
            ImGui.End();
        };
        pipeline = new RenderPipeline(Engine.RenderTexture);
        this.texture = new SpriteGraphic($"C:\\Users\\Tom\\Documents\\bear.dat", "walk north", new Vector2(160, 90), 100);
        pipeline.Add(this.texture);
    }
    public override void Focus()
    {
        base.Focus();
        ViewManager.Instance.EntityFollow = null;
        //ViewManager.Instance.Center = new Vector2(160f, 90f);    
    }

    public override void Unfocus()
    {
        base.Unfocus();

    }

    public override void Unload()
    {
        base.Unload();
    }

    public override void Update()
    {
        base.Update();
        //texture.Visible = false;

        Vector2 pos = Engine.Window.MapPixelToCoords( Mouse.GetPosition());
        texture.RenderPosition = pos;            //ViewManager.Instance.Center = new Vector2( 160,(ViewManager.Instance.Center.y + 90) * (float)MathF.Sin((2 * MathF.PI * Engine.SessionTimer.ElapsedTime.AsSeconds()) / 2));
        if (Input.MouseDown )// && TileGridInitialized)
        {
            Vector2 newPosition = Input.GetMousePosition() - ViewManager.Instance.Center /2;
            int tileX = (int)newPosition.X / 8 ;
            int tileY = (int)newPosition.Y / 8;
            DDDebug.Log($"tileX: {tileX}, tileY: {tileY}");
            //if (tileX >= 0 && tileX < TileGridX && tileY >= 0 && tileY < TileGridY)
            {
                pipeline.Add(new SpriteGraphic($"C:\\Users\\Tom\\Documents\\block.dat", "base", new Vector2(tileX* 8, tileY * 8), 100));
                // Place tile at tileX, tileY
                // Example: tileGrid[tileX, tileY] = tileID;
            }
        }
    }

    public override void Draw()
    {
        base.Draw();
        pipeline.Draw();
    }
}