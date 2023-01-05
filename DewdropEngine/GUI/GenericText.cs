using DewDrop.Graphics;
using DewDrop.GUI.Fonts;
using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;
using System;

namespace DewDrop.GUI
{
    public class GenericText : Renderable
    {
        public override Vector2 Position
        {
            get => this.position;
            set
            {
                this.position = value;
                this.drawText.Position = new Vector2f(this.position.x + (float)this.font.XCompensation, this.position.y + (float)this.font.YCompensation);
            }
        }

        public string Text
        {
            get => this.text;

            set
            {
                this.text = value;
            }
        }

        public Color Color
        {
            get => this.drawText.FillColor;
            set
            {
                this.drawText.FillColor = value;
            }
        }

        public FontData FontData
        {
            get => this.font;

        }

        private RenderStates renderStates;
        private Text drawText;

        private FontData font;
        private string text;

        public GenericText(Vector2 position, int depth, FontData font, string text) : this(position, depth, font, (text != null) ? text : string.Empty, 0, (text != null) ? text.Length : 0) { }

        public GenericText(Vector2 position, int depth, FontData font, string text, int index, int length)
        {
            this.position = position;
            this.text = text;


            this.depth = depth;
            this.font = font;

            this.drawText = new Text(string.Empty, this.font.Font, this.font.Size);
            this.drawText.Position = new Vector2f(position.x + (float)this.font.XCompensation, position.y + (float)this.font.YCompensation);
            this.UpdateText();

            shader = new Shader(EngineResources.GetResourceStream("text.vert"), null, EngineResources.GetResourceStream("text.frag"));
            this.shader.SetUniform("color", new SFML.Graphics.Glsl.Vec4(this.drawText.FillColor));
            this.shader.SetUniform("threshold", font.AlphaThreshold);
            this.renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, this.shader);
        }

        public Vector2f FindCharacterPosition(uint index)
        {
            uint num = Math.Max(0U, Math.Min((uint)this.text.Length, index));
            return this.drawText.FindCharacterPos(num);
        }

        public void Reset(string text, int index, int length)
        {

            this.UpdateText();
        }

        private void UpdateText()
        {
            this.drawText.DisplayedString = this.text;
            FloatRect localBounds = this.drawText.GetLocalBounds();
            this.size = new Vector2((int)Math.Max(1f, localBounds.Width), (int)Math.Max(1f, localBounds.Height));
        }

        public override void Draw(RenderTarget target)
        {

            this.shader.SetParameter("color", this.drawText.Color);

            target.Draw(this.drawText, this.renderStates);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                this.drawText.Dispose();
            }
            this.disposed = true;
        }

        private Shader shader;


    }
}
