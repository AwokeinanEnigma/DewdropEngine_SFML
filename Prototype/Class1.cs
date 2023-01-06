using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.Scenes;
using DewDrop;
using DewDrop.Utilities;
using DewDrop.GUI.Fonts;
using SFML.System;
using SFML.Graphics;

namespace Prototype.Scenes
{
    public class ErrorScene : Scene
    {
        private RenderPipeline pipeline;

        private GenericText title;
        private GenericText message;
        private GenericText pressenter;
        private GenericText exceptionDetails;
        private GenericText additionalUserDetails;

        public ErrorScene()
        {

            //  Engine.ClearColor = Color.Blue;
            FontData DefaultFont = new FontData();

            this.title = new GenericText(new Vector2(3f, 8f), 0, DefaultFont, "John Lemon..");
            title.Color = Color.Yellow;

            //todo - change this to a nonpersistant path
            //IndexedColorGraphic graphic = new IndexedColorGraphic($"C:\\Users\\Tom\\source\\repos\\SunsetRhapsody\\SunsetRhapsody\\bin\\Release\\Resources\\Graphics\\whoops.dat", "whoops", new Vector2(160, 90), 100);
            this.pipeline = new RenderPipeline(Engine.RenderTexture);
            //pipeline.Add(graphic);


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
