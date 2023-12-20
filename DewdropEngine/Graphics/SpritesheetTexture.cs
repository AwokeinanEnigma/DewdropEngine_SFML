#region

using DewDrop.Utilities;
using SFML.Graphics;
using System.Runtime.InteropServices;
using System.Security;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable MemberCanBePrivate.Global

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
	/// <summary>
	/// Gets or sets the image associated with the texture.
	/// </summary>
	public Texture Image { get; set; }
	/// <summary>
	/// Reloads the texture.
	/// </summary>
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
			byte* bPixels = (byte*)ptr;
			sfTexture_updateFromPixels(Palette.CPointer, bPixels, PaletteSize, PaletteCount, 0, 0);
		}

		fixed (Color* ptr2 = uncoloredPixels) {
			byte* pixels2 = (byte*)ptr2;
			sfTexture_updateFromPixels(Image.CPointer, pixels2, (uint)_size.x, (uint)_size.y, 0, 0);
		}
	}
	/// <summary>
	/// Gets the palette of the texture.
	/// </summary>
	public Texture Palette { get; private set;}
	/// <summary>
	/// Gets or sets the current palette.
	/// </summary>
	public uint CurrentPalette {
		get => _currentPal;
		set => _currentPal = Math.Min(PaletteCount, value);
	}
	/// <summary>
	/// Gets the current palette as a float.
	/// </summary>
	public float CurrentPaletteFloat => _currentPal/(float)PaletteCount;
	/// <summary>
	/// Gets the count of palettes.
	/// </summary>
	public uint PaletteCount { get; private set; }
	/// <summary>
	/// Gets the size of the palette.
	/// </summary>
	public uint PaletteSize { get;private set; }

	#endregion

	#region Sprite Definitions

	readonly SpriteDefinition _defaultDefinition;
	readonly Dictionary<int, SpriteDefinition> _definitions;

	/// <summary>
	/// Retrieves a sprite definition by name.
	/// </summary>
	/// <param name="name">The name of the sprite definition.</param>
	/// <returns>The sprite definition.</returns>
	public SpriteDefinition GetSpriteDefinition (string name) {

		int hashCode = name.GetHashCode();
		return GetSpriteDefinition(hashCode);
	}

	/// <summary>
	/// Retrieves a sprite definition by hash.
	/// </summary>
	/// <param name="hash">The hash of the sprite definition.</param>
	/// <returns>The sprite definition.</returns>
	public SpriteDefinition GetSpriteDefinition (int hash) {
		if (!_definitions.TryGetValue(hash, out SpriteDefinition result)) {
			result = default;
		}

		return result;
	}
	
	/// <summary>
	/// Retrieves all sprite definitions.
	/// </summary>
	/// <returns>A collection of sprite definitions.</returns>
	public ICollection<SpriteDefinition> GetSpriteDefinitions () {
		return _definitions.Values;
	}

	/// <summary>
	/// Retrieves the default sprite definition.
	/// </summary>
	/// <returns>The default sprite definition.</returns>
	public SpriteDefinition GetDefaultSpriteDefinition () {
		return _defaultDefinition;
	}

	#endregion

	#region Palette

	uint _currentPal;

	#endregion


	bool _disposed;
	readonly string _fileName;
	readonly Vector2 _size;

	/// <summary>
	/// Initializes a new instance of the SpritesheetTexture class with specified image width, palettes, image, sprite definitions, default definition, and file name.
	/// </summary>
	/// <param name="imageWidth">The width of the image.</param>
	/// <param name="palettes">The palettes of the image.</param>
	/// <param name="image">The image data.</param>
	/// <param name="definitions">The sprite definitions.</param>
	/// <param name="defaultDefinition">The default sprite definition.</param>
	/// <param name="fileName">The file name of the sprite.</param>
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
			byte* bPixels = (byte*)ptr;
			sfTexture_updateFromPixels(Palette.CPointer, bPixels, PaletteSize, PaletteCount, 0, 0);
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

	/// <summary>
	/// Converts the spritesheet texture to an image.
	/// </summary>
	public void ConvertToImage () {
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

	/// <summary>
	/// Disposes of the SpritesheetTexture and its resources.
	/// </summary>
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
