#region

using DewDrop.Graphics;
using DewDrop.GUI.Fonts;
using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;

#endregion

namespace DewDrop.GUI;

/// <summary>
///     This is a renderable which simply displays text on the screen.
/// </summary>
public class TextRenderer : Renderable
{
    public override Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            drawText.Position = new Vector2f(_position.x + font.XCompensation, _position.y + font.YCompensation);
        }
    }

    public string Text
    {
        get => text;

        set
        {
            text = value;
            UpdateText();
        }
    }

    public Color Color
    {
        get => drawText.FillColor;
        set
        {
            drawText.FillColor = value;
            _colorDirty = true;
        }
    }

    public FontData FontData => font;

    private bool _colorDirty;

    private RenderStates renderStates;
    private Text drawText;
    private Shader shader;

    private FontData font;
    private string text;


    public TextRenderer(Vector2 position, int depth, FontData font, string text) : this(position, depth, font, text != null ? text : string.Empty, 0, text != null ? text.Length : 0)
    {
    }

    public TextRenderer(Vector2 position, int depth, FontData font, string text, int index, int length)
    {
        _position = position;
        this.text = text;


        _depth = depth;
        this.font = font;

        drawText = new Text(string.Empty, this.font.Font, this.font.Size);
        drawText.Position = new Vector2f(position.x + this.font.XCompensation, position.y + this.font.YCompensation);
        UpdateText();

        shader = new Shader(EmbeddedResourcesHandler.GetResourceStream("text.vert"), null, EmbeddedResourcesHandler.GetResourceStream("text.frag"));
        shader.SetUniform("color", new Vec4(drawText.FillColor));
        shader.SetUniform("threshold", font.AlphaThreshold);
        renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, shader);
    }

    public Vector2f FindCharacterPosition(uint index)
    {
        uint num = Math.Max(0U, Math.Min((uint)text.Length, index));
        return drawText.FindCharacterPos(num);
    }

    private void UpdateText()
    {
        drawText.DisplayedString = text;
        FloatRect localBounds = drawText.GetLocalBounds();

        float width = Math.Max(1f, localBounds.Width);
        float height = Math.Max(1f, localBounds.Height);
        _size = new Vector2(width, height);
    }

    public override void Draw(RenderTarget target)
    {
        if (_colorDirty)
        {
            shader.SetUniform("color", new Vec4(drawText.FillColor));
            _colorDirty = false;
        }

        target.Draw(drawText, renderStates);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            drawText.Dispose();
        }

        _disposed = true;
    }
}