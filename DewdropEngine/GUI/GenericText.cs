using DewDrop.Graphics;
using DewDrop.GUI.Fonts;
using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;

namespace DewDrop.GUI
{
    public class GenericText : Renderable
    {
        private Text textLine;
        private string textToDraw;
        private Shader shader;

        public override Vector2 Position
        {
            get => this.position;
            set
            {
                this.position = value;
                this.textLine.Position = new Vector2f(this.position.x + (float)this.data.XCompensation, this.position.y + (float)this.data.YCompensation);
            }
        }
        private RenderStates renderStates;
        private FontData data;

        public GenericText(string text, int depth, Vector2 position, FontData data) 
        {

            textToDraw = text;


            base.depth = depth;
            base.position = position;


            textLine = new Text(textToDraw, data.Font, data.Size);
            textLine.Position = this.Position.TranslateToV2F();


            shader = new Shader(EngineResources.GetResourceStream("text.vert"), null, EngineResources.GetResourceStream("text.frag"));


            this.renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, this.shader);
            
        }

        public override void Draw(RenderTarget target)
        {
            this.shader.SetUniform("color", new SFML.Graphics.Glsl.Vec4( Color.Blue ));
                
            target.Draw(textLine, renderStates);
        }

        protected override void Dispose(bool disposing)
        {

            // only get rid of unmanaged resources if we're actually disposing
            if (disposing && !disposed)
            {
                // cya!
                textLine.Dispose();
                data.Dispose();
            }

            disposed = true; 
        }
    }
}
