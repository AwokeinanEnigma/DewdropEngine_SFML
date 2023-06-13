#region

using System.Runtime.InteropServices;
using System.Security;
using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Graphics;

/// <summary>
///     Holds information about a spritesheet's sprite definitions, its grayscale image, and its palette.
/// </summary>
public class SpritesheetTexture : ITexture
{
    [SuppressUnmanagedCodeSecurity]
    [DllImport("csfml-graphics", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void sfTexture_updateFromPixels(IntPtr texture, byte* pixels, uint width, uint height, uint x, uint y);

    #region Properties

    public Texture Image
    {
        get => _imageTex;
        set => _imageTex = value;
    }

    public Texture Palette => _paletteTex;

    public uint CurrentPalette
    {
        get => _currentPal;
        set => _currentPal = Math.Min(_totalPals, value);
    }

    public float CurrentPaletteFloat => _currentPal / (float)_totalPals;

    public uint PaletteCount => _totalPals;

    public uint PaletteSize => _palSize;

    #endregion

    #region Sprite Definitions

    private readonly SpriteDefinition _defaultDefinition;
    private readonly Dictionary<int, SpriteDefinition> _definitions;

    public SpriteDefinition GetSpriteDefinition(string name)
    {

        int hashCode = name.GetHashCode();
        return GetSpriteDefinition(hashCode);
    }

    public SpriteDefinition GetSpriteDefinition(int hash)
    {
        SpriteDefinition result;
        if (!_definitions.TryGetValue(hash, out result))
        {
            result = default;
        }

        return result;
    }

    public ICollection<SpriteDefinition> GetSpriteDefinitions()
    {
        return _definitions.Values;
    }

    public SpriteDefinition GetDefaultSpriteDefinition()
    {
        return _defaultDefinition;
    }

    #endregion

    #region Textures

    private readonly Texture _paletteTex;
    private Texture _imageTex;

    #endregion

    #region Palette

    private uint _currentPal;
    private readonly uint _totalPals;
    private readonly uint _palSize;

    #endregion


    private bool _disposed;

    public unsafe SpritesheetTexture(uint imageWidth, int[][] palettes, byte[] image, Dictionary<int, SpriteDefinition> definitions, SpriteDefinition defaultDefinition)
    {
        // create palette
        _totalPals = (uint)palettes.Length;
        _palSize = (uint)palettes[0].Length;
        _paletteTex = new Texture(_palSize, _totalPals);


        uint imageHeight = (uint)(image.Length / (int)imageWidth);
        _imageTex = new Texture(imageWidth, imageHeight);

        Color[] totalColors = new Color[_palSize * _totalPals];
        for (uint allPalettes = 0; allPalettes < _totalPals; allPalettes++)
        {
            uint colors = 0;
            while (colors < palettes[allPalettes].Length)
            {
                totalColors[allPalettes * _palSize + colors] = ColorHelper.FromInt(palettes[allPalettes][colors]);
                colors++;
            }
        }

        Color[] uncoloredPixels = new Color[imageWidth * imageHeight];
        uint pixels = 0;
        while (pixels < image.Length)
        {
            uncoloredPixels[pixels].A = byte.MaxValue;
            uncoloredPixels[pixels].R = image[pixels];
            uncoloredPixels[pixels].G = image[pixels];
            uncoloredPixels[pixels].B = image[pixels];
            pixels++;
        }

        fixed (Color* ptr = totalColors)
        {
            byte* b_pixels = (byte*)ptr;
            sfTexture_updateFromPixels(_paletteTex.CPointer, b_pixels, _palSize, _totalPals, 0, 0);
        }

        fixed (Color* ptr2 = uncoloredPixels)
        {
            byte* pixels2 = (byte*)ptr2;
            sfTexture_updateFromPixels(_imageTex.CPointer, pixels2, imageWidth, imageHeight, 0, 0);
        }

        _definitions = definitions;
        _defaultDefinition = defaultDefinition;
    }

    ~SpritesheetTexture()
    {
        Dispose(false);
    }

    public void ToFullColorTexture()
    {
        uint x1 = _imageTex.Size.X;
        uint y1 = _imageTex.Size.Y;
        Image image1 = new(x1, y1);
        Image image2 = _imageTex.CopyToImage();
        Image image3 = _paletteTex.CopyToImage();
        for (uint y2 = 0; y2 < y1; ++y2)
        {
            for (uint x2 = 0; x2 < x1; ++x2)
            {
                uint x3 = (uint)(image2.GetPixel(x2, y2).R / (double)byte.MaxValue * _palSize);
                Color pixel = image3.GetPixel(x3, _currentPal);
                image1.SetPixel(x2, y2, pixel);
            }
        }

        image1.SaveToFile("combinedEngineSprite.png");
        image2.SaveToFile("baseSpritesheet.png");
        image3.SaveToFile("palette.png");
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _imageTex.Dispose();
            _paletteTex.Dispose();
        }

        _disposed = true;
    }
}