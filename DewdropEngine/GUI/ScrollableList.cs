#region

using DewDrop.Graphics;
using DewDrop.GUI.Fonts;
using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.GUI;

public class ScrollableList : Renderable
{
    public string SelectedItem
    {
        get => _options[_selectedIndex];
        set => _options[_selectedIndex] = value;
    }

    public override bool Visible
    {
        get => _visible;
        set
        {
            for (int i = 0; i < _texts.Length; i++)
            {
                if (_texts[i].Visible)
                {
                    _texts[i].Visible = value;
                }
            }

            _visible = value;
        }
    }

    public int ItemCount => _options.Length;

    private int _selectedIndex;
    private TextRenderer[] _texts;
    private string[] _options;

    public ScrollableList(Vector2 position, int depthToRenderAt, string[] options, int displayableOptions, FontData data)
    {
        _options = options;

        int num = Math.Min(_options.Length, displayableOptions);
        _texts = new TextRenderer[num];
        for (int j = 0; j < num; j++)
        {
            //    _texts[j] = new TextRenderer(new Vector2(position.X, position.Y + lineHeight * j), depthToRenderAt, new FontData(), _options[j]);
        }
    }

    public override void Draw(RenderTarget target)
    {
        for (int i = 0; i < _texts.Length; i++)
        {
            if (_texts[i].Visible)
                _texts[i].Draw(target);
        }
    }
}