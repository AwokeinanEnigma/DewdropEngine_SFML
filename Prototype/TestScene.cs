using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.Scenes;
using DewDrop;
using DewDrop.Utilities;
using DewDrop.GUI.Fonts;
using ImGuiNET;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace Prototype.Scenes
{
    public class TestScene : Scene
    {
        private RenderPipeline pipeline;

        private GenericText title;
        private GenericText message;
        private GenericText pressenter;
        private GenericText exceptionDetails;
        private GenericText additionalUserDetails;
        public SpriteGraphic texture;

        public TestScene()
        {

            //  Engine.ClearColor = Color.Blue;
            FontData DefaultFont = new FontData();

            this.title = new GenericText(new Vector2(3f, 8f), 555, DefaultFont, "John Lemon..");
            title.Color = Color.Yellow;

            //todo - change this to a nonpersistant path
            texture = new SpriteGraphic($"C:\\Users\\Tom\\Documents\\bear.dat", "walk north", new Vector2(160, 90), 100);
            

            ((SpritesheetTexture)texture.Texture).ToFullColorTexture();
            this.pipeline = new RenderPipeline(Engine.RenderTexture);
            //pipeline.Add(graphic);
            pipeline.Add(texture);
            for (int i = 0; i < 1500    ; i++)
            {
                pipeline.Add(new SpriteGraphic($"C:\\Users\\Tom\\Documents\\bear.dat", "walk north", new Vector2(new Random().Next(0,320), new Random().Next(0,90)), 100));
            }
            Input.Instance.OnKeyPressed += (key, key2) =>
            {
                if (key2 == Keyboard.Key.Escape)
                {
                    Input.RecieveInput = false;
                }
                Debug.Log(key2);
            };

            Engine.RenderImGUI += () =>
            {
                ImGui.Begin("Dewdrop Debug Utilities");
                
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

            
            this.pipeline.Add(this.title);
            Debug.DumpLogs();
        }

        /// <summary>
        /// Sets the position of the mouse relative to the game window.
        /// </summary>
        /// <param name="position">The position to set the mouse to.</param>
        public static void SetMousePosition(Vector2f position)
        {
            // This is stupid, let me explain:
            // We want a pixel location of where the mouse is relative to the game's window
            // Here's the problem: The scale of the screen
            float scaleFactor = Engine.FrameBufferScale;
            Mouse.SetPosition((Vector2i)(position * scaleFactor));// * scaleFactor;
        }

        /// <summary>
        /// Gets the position of the mouse relative to the game window.
        /// </summary>
        /// <returns>The position of the mouse relative to the game window.</returns>
        public static Vector2 GetMousePosition() {
            // had a really long winded thing written but i'll shorten it
            // the mouse position is not relative to the game's window
            // what is (69, 69) in game space is not the same in monitor space
            // this function is translating monitor space to window space.
            /*if (Engine.Fullscreen) {
                VideoMode desktopMode;
                desktopMode = VideoMode.DesktopMode;
                float fullScreenMin = Math.Min(desktopMode.Width / Engine.SCREEN_WIDTH, desktopMode.Height / Engine.SCREEN_HEIGHT);
                return (Vector2f)Mouse.GetPosition(Engine.Window) / fullScreenMin;
            }*/
            
            return (Vector2)Mouse.GetPosition(Engine.Window) / Engine.FrameBufferScale;
        }
        
        public override void Focus()
        {
            base.Focus();
            ViewManager.Instance.FollowActor = null;
            ViewManager.Instance.Center = new Vector2(160f, 90f);
            //Engine.ClearColor = Color.Black;
        }

        public override void Update()
        {
            base.Update();
            texture.Position = GetMousePosition(); // new Vector2(160, (title.Position.y + 90) * (float)MathF.Sin((2 * MathF.PI * Engine.SessionTimer.ElapsedTime.AsSeconds()) / 2));
            title.Text =$"MGC: {(GC.GetTotalMemory(true) / 1024L) * 0.001}MB\n";
            
            if (Input.Instance[Keyboard.Key.A])
            {
                Debug.Log("hit breakpoint");
            }
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
