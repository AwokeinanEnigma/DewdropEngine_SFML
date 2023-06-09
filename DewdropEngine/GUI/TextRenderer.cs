using DewDrop.Graphics;
using DewDrop.GUI.Fonts;
using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;

namespace DewDrop.GUI
{
    /// <summary>
    /// This is a renderable which simply displays text on the screen.
    /// </summary>
    public class TextRenderer : Renderable
    {
        public override Vector2 Position
        {
            get => this._position;
            set
            {
                this._position = value;
                this.drawText.Position = new Vector2f(this._position.x + (float)this.font.XCompensation, this._position.y + (float)this.font.YCompensation);
            }
        }

        public string Text
        {
            get => this.text;

            set
            {
                this.text = value;
                this.UpdateText();
            }
        }

        public Color Color
        {
            get => this.drawText.FillColor;
            set
            {
                this.drawText.FillColor = value;
                _colorDirty = true;
            }
        }

        public FontData FontData
        {
            get => this.font;

        }

        private bool _colorDirty = false;

        private RenderStates renderStates;
        private Text drawText;
        private Shader shader;

        private FontData font;
        private string text;


        public TextRenderer(Vector2 position, int depth, FontData font, string text) : this(position, depth, font, (text != null) ? text : string.Empty, 0, (text != null) ? text.Length : 0) { }

        public TextRenderer(Vector2 position, int depth, FontData font, string text, int index, int length)
        {
            this._position = position;
            this.text = text;


            this._depth = depth;
            this.font = font;

            this.drawText = new Text(string.Empty, this.font.Font, this.font.Size);
            this.drawText.Position = new Vector2f(position.x + (float)this.font.XCompensation, position.y + (float)this.font.YCompensation);
            this.UpdateText();

            shader = new Shader(EmbeddedResourcesHandler.GetResourceStream("text.vert"), null, EmbeddedResourcesHandler.GetResourceStream("text.frag"));
            this.shader.SetUniform("color", new SFML.Graphics.Glsl.Vec4(this.drawText.FillColor));
            this.shader.SetUniform("threshold", font.AlphaThreshold);
            this.renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, this.shader);
        }

        public Vector2f FindCharacterPosition(uint index)
        {
            uint num = Math.Max(0U, Math.Min((uint)this.text.Length, index));
            return this.drawText.FindCharacterPos(num);
        }

        private void UpdateText()
        {
            this.drawText.DisplayedString = this.text;
            FloatRect localBounds = this.drawText.GetLocalBounds();
            
            float width = Math.Max(1f, localBounds.Width);
            float height = Math.Max(1f, localBounds.Height);
            this._size = new Vector2(width, height);
        }

        public override void Draw(RenderTarget target)
        {
            if (_colorDirty)
            {
                this.shader.SetUniform("color", new SFML.Graphics.Glsl.Vec4(this.drawText.FillColor));
                _colorDirty = false;
            }

            target.Draw(this.drawText, this.renderStates);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                this.drawText.Dispose();
            }
            this._disposed = true;
        }
    }
}
