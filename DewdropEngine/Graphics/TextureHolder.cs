using SFML.Graphics;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
namespace DewDrop.Graphics;

/// <summary>
/// Represents a texture holder that manages a texture with a full range of colors.
/// </summary>
public class TextureHolder : ITexture
{
	/// <summary>
	/// Gets the image associated with the texture.
	/// </summary>
	public Texture Image
	{
		get
		{
			return this._imageTex;
		}
		set
		{
			this._imageTex = value;
		}
	}

	Texture _imageTex;
	bool _disposed;
	

	/// <summary>
	/// Initializes a new instance of the TextureHolder class with a specified image.
	/// </summary>
	/// <param name="image">The image to create the texture image from.</param>
	public TextureHolder(Image image)
	{
		this._imageTex = new Texture(image);
	}

	/// <summary>
	/// Initializes a new instance of the TextureHolder class with a specified texture.
	/// </summary>
	/// <param name="tex">The texture to create the texture image from.</param>
	public TextureHolder(Texture tex)
	{
		this._imageTex = new Texture(tex);
	}

	/// <summary>
	/// Disposes of the TextureHolder and its resources.
	/// </summary>
	public virtual void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Reloads the texture.
	/// </summary>
	public void Reload()
	{
		// do nothing
	}
	
	/// <summary>
	/// Disposes of the TextureHolder and its resources.
	/// </summary>
	/// <param name="disposing">Indicates whether the TextureHolder is currently being disposed.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (!this._disposed && disposing)
		{
			this._imageTex.Dispose();
		}
		this._disposed = true;
	}
}