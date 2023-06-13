#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Graphics;

public interface IRenderable : IDisposable
{
    public Vector2 Position { get; set; }
    /// <summary>
    ///     The origin of the renderable object.
    /// </summary>
    public Vector2 Origin { get; set; }
    /// <summary>
    ///     The size of the renderable object.
    /// </summary>
    public Vector2 Size { get; set; }
    /// <summary>
    ///     The depth of the renderable object.
    /// </summary>
    public int Depth { get; set; }

    /// <summary>
    ///     Determines whether or not the renderable is visible. Handled by the RenderPipeline
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    ///     Rotates the renderable object.
    /// </summary>
    public float Rotation { get; set; }

    public void Draw(RenderTarget target);
}