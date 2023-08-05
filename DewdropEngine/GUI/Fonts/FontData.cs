#region

using DewDrop.Resources;
using SFML.Graphics;

#endregion

namespace DewDrop.GUI.Fonts;

public class FontData : IDisposable
{
    private bool _disposed;
    private readonly Font _font;
    private readonly int _xComp;
    private readonly int _yComp;
    private readonly int _lineHeight;
    private readonly int _wHeight;
    private readonly uint _fontSize;
    private readonly float _alphaThreshold;

    public Font Font => _font;

    public int XCompensation => _xComp;

    public int YCompensation => _yComp;

    public int LineHeight => _lineHeight;

    public int WHeight => _wHeight;

    public uint Size => _fontSize;

    public float AlphaThreshold => _alphaThreshold;

    public FontData()
    {
        _font = new Font(EmbeddedResourcesHandler.GetResourceStream("openSansPX.ttf"));

        _fontSize = 16U;
        _wHeight = (int)_font.GetGlyph(41U, _fontSize, false, 1).Bounds.Height;
        _lineHeight = (int)(_wHeight * 1.20000004768372);
        _alphaThreshold = 0.0f;
    }

    public FontData(Font font, uint fontSize, int lineHeight, int xComp, int yComp)
    {
        _font = font;
        _fontSize = fontSize;
        _lineHeight = lineHeight;
        _xComp = xComp;
        _yComp = yComp;
        _wHeight = (int)_font.GetGlyph(41U, _fontSize, false, 1).Bounds.Height;
        // Console.WriteLine($"wHeight = {wHeight}");
        _alphaThreshold = 0.8f;
    }

    ~FontData()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _font.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}