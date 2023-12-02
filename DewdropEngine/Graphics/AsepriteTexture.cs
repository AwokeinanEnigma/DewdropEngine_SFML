#region

using DewDrop.Graphics.Aseprite;
using DewDrop.Utilities;
using SFML.Graphics;
using System.Runtime.InteropServices;
using System.Security;
using Color = SFML.Graphics.Color;

#endregion

namespace DewDrop.Graphics;

/// <summary>
///     Holds information about a spritesheet's sprite definitions, its grayscale image, and its palette.
/// </summary>
public class AsepriteTexture : ITexture {
	[SuppressUnmanagedCodeSecurity]
	[DllImport("csfml-graphics", CallingConvention = CallingConvention.Cdecl)]
	static extern unsafe void sfTexture_updateFromPixels (IntPtr texture, byte* pixels, uint width, uint height, uint x, uint y);

	#region Properties

	public Texture Image { get; set; }
	public unsafe void Reload () { 		
		AsepriteImporter sprite = new AsepriteImporter(_path);
		;
		fixed (Color* ptr2 = sprite.Frames[0].Pixels) {
			byte* pixels2 = (byte*)ptr2;
			sfTexture_updateFromPixels(Image.CPointer, pixels2, (uint)sprite.Width,(uint) sprite.Height, 0, 0);
		}
		Outer.Log("reloaded");
		
	}

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

	bool _disposed;
	string _path;
	public unsafe AsepriteTexture (string path) {
		_path = path;
		//
		AsepriteImporter sprite = new AsepriteImporter(path);
		;
		//
		Image = new Texture((uint)sprite.Width, (uint)sprite.Height);
		fixed (Color* ptr2 = sprite.Frames[0].Pixels) {
			byte* pixels2 = (byte*)ptr2;
			sfTexture_updateFromPixels(Image.CPointer, pixels2, (uint)sprite.Width,(uint) sprite.Height, 0, 0);
		}
		_definitions = new Dictionary<int, SpriteDefinition>();
		_defaultDefinition = new SpriteDefinition("default", new Vector2(0,0), new Vector2(sprite.Width, sprite.Height), new Vector2(0,0), 0, Array.Empty<float>(), false, false, 0, Array.Empty<int>());
	}

	~AsepriteTexture () {
		Dispose(false);
	}

	public void ToFullColorTexture () {

		Image a = Image.CopyToImage();
		a.SaveToFile("test.png");
	}

	public virtual void Dispose () {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose (bool disposing) {
		if (!_disposed && disposing) {
			Image.Dispose();
		}

		_disposed = true;
	}
}
