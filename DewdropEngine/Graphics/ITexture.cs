#region

using SFML.Graphics;

#endregion

namespace DewDrop.Graphics;

public interface ITexture : IDisposable {
	public Texture Image { get; set; }
	
	public void Reload ();
}
