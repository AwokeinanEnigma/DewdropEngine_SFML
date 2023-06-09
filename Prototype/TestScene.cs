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

        private TextRenderer title;
        private TextRenderer message;
        private TextRenderer pressenter;
        private TextRenderer exceptionDetails;
        private TextRenderer additionalUserDetails;
        public SpriteGraphic texture;
        private ShapeGraphic shape;
        public TestScene()
        {

            //  Engine.ClearColor = Color.Blue;
            FontData DefaultFont = new FontData();

            this.title = new TextRenderer(new Vector2(3f, 8f), 555, DefaultFont, "John Lemon..");
            title.Color = Color.Yellow;

            //todo - change this to a nonpersistant path
            texture = new SpriteGraphic($"C:\\Users\\Tom\\Documents\\bear.dat", "walk north", new Vector2(160, 90), 100);
            
            Input.Instance.OnMouseClick += (button, position) =>
            {
                Debug.Log($"Click button: {position.Button} at position: {Input.GetMousePosition() }");
            };
            
            
            ((SpritesheetTexture)texture.Texture).ToFullColorTexture();
            this.pipeline = new RenderPipeline(Engine.RenderTexture);
            //pipeline.Add(graphic);
         //
         shape = new ShapeGraphic(
                    new RectangleShape(new Vector2f(10,10)), 
                    new Vector2(160, 90), 
                    new Vector2(1,1), 
                    new Vector2(5,5), 500, Color.Green, Color.Green);//);
         pipeline.Add(shape);   
         pipeline.Add(texture);
            Input.Instance.OnKeyPressed += (key, key2) =>
            {
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
            shape.Position =new Vector2(160, (title.Position.y + 90) * (float)MathF.Sin((2 * MathF.PI * Engine.SessionTimer.ElapsedTime.AsSeconds()) / 2));
            texture.Position =  Input.GetMousePosition(); // new Vector2(160, (title.Position.y + 90) * (float)MathF.Sin((2 * MathF.PI * Engine.SessionTimer.ElapsedTime.AsSeconds()) / 2));
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
