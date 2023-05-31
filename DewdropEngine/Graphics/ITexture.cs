using SFML.Graphics;

namespace DewDrop.Graphics
{
    public interface ITexture : IDisposable
    {
        public Texture Image { get; set; }
    }
}
