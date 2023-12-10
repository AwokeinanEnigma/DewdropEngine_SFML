#region

using DewDrop.Utilities;
using SFML.Graphics;
using System.Runtime.InteropServices;
using System.Security;

#endregion

namespace DewDrop.Graphics;

/// <summary>
///     Holds information about a spritesheet's sprite definitions, its grayscale image, and its palette.
/// </summary>
public class SpritesheetTexture : ITexture {
	[SuppressUnmanagedCodeSecurity]
	[DllImport("csfml-graphics", CallingConvention = CallingConvention.Cdecl)]
	static extern unsafe void sfTexture_updateFromPixels (IntPtr texture, byte* pixels, uint width, uint height, uint x, uint y);

	#region Properties

	public Texture Image { get; set; }
	public unsafe void Reload () {
		Tuple<byte[], int[][]> result = TextureManager.Instance.GetRawSpritesheetData(_fileName);
		int[][] palettes = result.Item2;
		byte[] image = result.Item1;
		
		PaletteCount = (uint)palettes.Length;
		PaletteSize = (uint)palettes[0].Length;
		Palette = new Texture(PaletteSize, PaletteCount);
		Image = new Texture((uint)_size.x, (uint)_size.y);

		Color[] totalColors = new Color[PaletteSize*PaletteCount];
		for (uint allPalettes = 0; allPalettes < PaletteCount; allPalettes++) {
			uint colors = 0;
			while (colors < palettes[allPalettes].Length) {
				totalColors[allPalettes*PaletteSize + colors] = ColorHelper.FromInt(palettes[allPalettes][colors]);
				colors++;
			}
		}

		Color[] uncoloredPixels = new Color[(int)(_size.x*_size.y)];
		uint pixels = 0;
		while (pixels < image.Length) {
			uncoloredPixels[pixels].A = byte.MaxValue;
			uncoloredPixels[pixels].R = image[pixels];
			uncoloredPixels[pixels].G = image[pixels];
			uncoloredPixels[pixels].B = image[pixels];
			pixels++;
		}

		fixed (Color* ptr = totalColors) {
			byte* b_pixels = (byte*)ptr;
			sfTexture_updateFromPixels(Palette.CPointer, b_pixels, PaletteSize, PaletteCount, 0, 0);
		}

		fixed (Color* ptr2 = uncoloredPixels) {
			byte* pixels2 = (byte*)ptr2;
			sfTexture_updateFromPixels(Image.CPointer, pixels2, (uint)_size.x, (uint)_size.y, 0, 0);
		}
	}

	public Texture Palette { get; private set;}

	public uint CurrentPalette {
		get => _currentPal;
		set => _currentPal = Math.Min(PaletteCount, value);
	}

	public float CurrentPaletteFloat => _currentPal/(float)PaletteCount;

	public uint PaletteCount { get; private set; }

	public uint PaletteSize { get;private set; }

	#endregion

	#region Sprite Definitions

	readonly SpriteDefinition _defaultDefinition;
	readonly Dictionary<int, SpriteDefinition> _definitions;

	public SpriteDefinition GetSpriteDefinition (string name) {

		int hashCode = name.GetHashCode();
		return GetSpriteDefinition(hashCode);
	}

	public SpriteDefinition GetSpriteDefinition (int hash) {
		SpriteDefinition result;
		if (!_definitions.TryGetValue(hash, out result)) {
			result = default;
		}

		return result;
	}

	public ICollection<SpriteDefinition> GetSpriteDefinitions () {
		return _definitions.Values;
	}

	public SpriteDefinition GetDefaultSpriteDefinition () {
		return _defaultDefinition;
	}

	#endregion

	#region Textures

	#endregion

	#region Palette

	uint _currentPal;

	#endregion


	bool _disposed;
	string _fileName;
	Vector2 _size;
	public unsafe SpritesheetTexture (uint imageWidth, int[][] palettes, byte[] image, Dictionary<int, SpriteDefinition> definitions, SpriteDefinition defaultDefinition, string fileName) {
		// create palette
		PaletteCount = (uint)palettes.Length;
		PaletteSize = (uint)palettes[0].Length;
		Palette = new Texture(PaletteSize, PaletteCount);

		_fileName = fileName;

		uint imageHeight = (uint)(image.Length/(int)imageWidth);
		_size = new Vector2(imageWidth, imageHeight);
		Image = new Texture(imageWidth, imageHeight);

		Color[] totalColors = new Color[PaletteSize*PaletteCount];
		for (uint allPalettes = 0; allPalettes < PaletteCount; allPalettes++) {
			uint colors = 0;
			while (colors < palettes[allPalettes].Length) {
				totalColors[allPalettes*PaletteSize + colors] = ColorHelper.FromInt(palettes[allPalettes][colors]);
				colors++;
			}
		}

		Color[] uncoloredPixels = new Color[imageWidth*imageHeight];
		uint pixels = 0;
		while (pixels < image.Length) {
			uncoloredPixels[pixels].A = byte.MaxValue;
			uncoloredPixels[pixels].R = image[pixels];
			uncoloredPixels[pixels].G = image[pixels];
			uncoloredPixels[pixels].B = image[pixels];
			pixels++;
		}

		fixed (Color* ptr = totalColors) {
			byte* b_pixels = (byte*)ptr;
			sfTexture_updateFromPixels(Palette.CPointer, b_pixels, PaletteSize, PaletteCount, 0, 0);
		}

		fixed (Color* ptr2 = uncoloredPixels) {
			byte* pixels2 = (byte*)ptr2;
			sfTexture_updateFromPixels(Image.CPointer, pixels2, imageWidth, imageHeight, 0, 0);
		}

		_definitions = definitions;
		_defaultDefinition = defaultDefinition;
	}

	~SpritesheetTexture () {
		Dispose(false);
	}

	public void ToFullColorTexture () {
		uint x1 = Image.Size.X;
		uint y1 = Image.Size.Y;
		Image image1 = new Image(x1, y1);
		Image image2 = Image.CopyToImage();
		Image image3 = Palette.CopyToImage();
		for (uint y2 = 0; y2 < y1; ++y2) {
			for (uint x2 = 0; x2 < x1; ++x2) {
				uint x3 = (uint)(image2.GetPixel(x2, y2).R/(double)byte.MaxValue*PaletteSize);
				Color pixel = image3.GetPixel(x3, _currentPal);
				image1.SetPixel(x2, y2, pixel);
			}
		}

		image1.SaveToFile("combinedEngineSprite.png");
		image2.SaveToFile("baseSpritesheet.png");
		image3.SaveToFile("palette.png");
	}

	public virtual void Dispose () {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose (bool disposing) {
		if (!_disposed && disposing) {
			Image.Dispose();
			Palette.Dispose();
		}

		_disposed = true;
	}
}
