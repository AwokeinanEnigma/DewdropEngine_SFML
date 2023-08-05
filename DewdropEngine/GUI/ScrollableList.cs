#region

using System.Net;
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
            UpdateVisibility(visible: value);
            _visible = value;
        }
    }

    public int ItemCount => _options.Length;

    private int _selectedIndex;
    private int _topIndex;
    
    private readonly TextRenderer[] _texts;
    private readonly string[] _options;
    private readonly int _displayCount;
    
    public ScrollableList(Vector2 position, int depthToRenderAt, string[] options, int displayableOptions, float lineHeight, FontData data)
    {
        _options = options;

        _position = position;
        _origin = Vector2.Zero;
        _depth = depthToRenderAt;
        _size = new Vector2(320, 180);
        _displayCount = displayableOptions;
        _visible = true;
        
        int renderers = Math.Min(_options.Length, _displayCount);
        _texts = new TextRenderer[renderers];
        for (int i = 0; i < renderers; i++)
        {
            _texts[i] = new TextRenderer(new Vector2(position.X, position.Y + lineHeight * i), depthToRenderAt, data, _options[i]);
            _texts[i].Color = Color.White;
        }
        UpdateIndicators();
        UpdateVisibility(true);
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
            DDDebug.LogError("Index out of range!", new IndexOutOfRangeException("You can't update a text that doesn't exist!"));
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

    public void Hide()
    {
        UpdateVisibility(false);
    }

    public void Show()
    {
        UpdateVisibility(true);
    }
    
    public void SelectItem(int index)
    {
        if (index < 0 || index >= _texts.Length)
        {
            DDDebug.LogError("Index out of range!", new IndexOutOfRangeException("You can't select a text that doesn't exist!"));
            return;
        }
        
        // make sure we don't go out of bounds with the min
        // and make sure we don't go below 0 with the max
        _selectedIndex = Math.Min(_options.Length - 1, Math.Max(0, index)); 
    }

    public void UpdateDisplayTexts()
    {
        for (int i = 0; i < _displayCount; i++)
        {
            int num = _topIndex + i;
            if (num < _options.Length)
            {
                _texts[i].Text = _options[num];
            }
            else if (i < _texts.Length)
            {
                _texts[i].Text = string.Empty;
            }
        }
    }
    
    public void UpdateIndicators()
    {
        foreach (var text in _texts)
        {
            text.Color = Color.White;
        }
        _texts[_selectedIndex - _topIndex].Color = new Color(255, 89, 209);
    }

    public bool SelectPrevious()
    {
        if ( _selectedIndex - 1 >= 0)
        {
            _selectedIndex--;
            if (_selectedIndex < _topIndex)
            {
                _topIndex--;
                UpdateDisplayTexts();
            }
            UpdateIndicators();
            return true;
        }
        return false;
    }
    public bool SelectNext()
    {
        if ( _selectedIndex + 1 < _options.Length)
        {
            _selectedIndex++;
            if (_selectedIndex > _topIndex + _displayCount - 1)
            {
                _topIndex++;
                UpdateDisplayTexts();
            }
            UpdateIndicators();
            return true;
        }
        return false;
    }
    
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!_disposed && disposing)
        {
            // dispose of each text
            for (int i = 0; i < _texts.Length; i++)
            {
                _texts[i].Dispose();
            }
        }
    }
}