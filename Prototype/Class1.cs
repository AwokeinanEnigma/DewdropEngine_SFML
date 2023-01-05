using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.Scenes;
using DewDrop;
using DewDrop.Utilities;
using DewDrop.GUI.Fonts;
using SFML.System;

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

            this.title = new GenericText(new Vector2(3f, 8f), 0, DefaultFont, "An unhandled exception has occurred.");
            this.message = new GenericText(new Vector2(3f, 32f), 0, DefaultFont, "Enigma is obviously an incompetent programmer.");
            this.pressenter = new GenericText(new Vector2(3f, 48f), 0, DefaultFont, "Press Enter/Start to exit.");
            this.exceptionDetails = new GenericText(new Vector2(3f, 80f), 0, DefaultFont, string.Format("{0}\nSee error.log for more details.", "a"));
            this.additionalUserDetails = new GenericText(new Vector2(3f, 110), 0, DefaultFont, "Additionally, files detailing the state of the " +
                "\nTextureManager and all logs prior to the error have " +
                "\nbeen dumped.");

            //todo - change this to a nonpersistant path
            //IndexedColorGraphic graphic = new IndexedColorGraphic($"C:\\Users\\Tom\\source\\repos\\SunsetRhapsody\\SunsetRhapsody\\bin\\Release\\Resources\\Graphics\\whoops.dat", "whoops", new Vector2(160, 90), 100);
            this.pipeline = new RenderPipeline(Engine.RenderTexture);
            //pipeline.Add(graphic);


            this.pipeline.Add(this.title);
            this.pipeline.Add(this.message);
            this.pipeline.Add(this.pressenter);
            this.pipeline.Add(this.exceptionDetails);
            this.pipeline.Add(this.additionalUserDetails);
            Debug.DumpLogs();
        }

        public override void Focus()
        {
            base.Focus();
            ViewManager.Instance.FollowActor = null;
            ViewManager.Instance.Center = new Vector2f(160f, 90f);
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
