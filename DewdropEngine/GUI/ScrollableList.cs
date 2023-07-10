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

    public ScrollableList(Vector2 position, int depthToRenderAt, string[] options, int displayableOptions, float lineHeight, FontData data)
    {
        _options = options;

        int renderers = Math.Min(_options.Length, displayableOptions);
        _texts = new TextRenderer[renderers];
        for (int i = 0; i < renderers; i++)
        {
            _texts[i] = new TextRenderer(new Vector2(position.X, position.Y + lineHeight * i), depthToRenderAt, new FontData(), _options[i]);
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
    
    public void UpdateText(int index, string newText)
    {
        if (index < 0 || index >= _texts.Length)
        {
            Debug.LogError("Index out of range!", new IndexOutOfRangeException("You can't update a text that doesn't exist!"));
            return;
        }
        _texts[index].Text = newText;
    }
    
    public void UpdateVisibility(bool visible)
    {
        for (int i = 0; i < _texts.Length; i++)
        {
            _texts[i].Visible = visible;
        }
    }
    
    public void SelectItem(int index)
    {
        if (index < 0 || index >= _texts.Length)
        {
            Debug.LogError("Index out of range!", new IndexOutOfRangeException("You can't select a text that doesn't exist!"));
            return;
        }
        
        // make sure we don't go out of bounds with the min
        // and make sure we don't go below 0 with the max
        _selectedIndex = Math.Min(_options.Length - 1, Math.Max(0, index)); 
    }
}