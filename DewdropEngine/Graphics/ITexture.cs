#region

using SFML.Graphics;

#endregion

namespace DewDrop.Graphics;

/// <summary>
/// ITexture is an interface that represents a texture.
/// It extends the IDisposable interface, which means any class implementing ITexture should provide a method to dispose of its texture.
/// </summary>
public interface ITexture : IDisposable {
	/// <summary>
	/// Gets or sets the Image property, which is of type Texture from the SFML.Graphics namespace.
	/// </summary>
	Texture Image { get; set; }

	/// <summary>
	/// The Reload method is responsible for handling the reloading of the texture.
	/// Any class implementing ITexture should provide an implementation for this method.
	/// </summary>
	void Reload ();
}

