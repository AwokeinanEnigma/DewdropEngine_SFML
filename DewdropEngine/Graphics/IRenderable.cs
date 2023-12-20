#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Graphics;

/// <summary>
/// Represents a renderable object.
/// Extends the IDisposable interface, which means any class implementing IRenderable should provide a method to free, release, or reset unmanaged resources.
/// </summary>
public interface IRenderable : IDisposable {
	public Vector2 RenderPosition { get; set; }

	/// <summary>
	/// The origin of the renderable object. Acts as an offset.
	/// </summary>
	public Vector2 Origin { get; set; }

	/// <summary>
	/// The size of the renderable object.
	/// </summary>
	public Vector2 Size { get; set; }

	/// <summary>
	/// The depth of the renderable object.
	/// </summary>
	public int Depth { get; set; }

	/// <summary>
	///     Determines whether or not the renderable is visible. 
	/// </summary>
	public bool Visible { get; set; }

	/// <summary>
	///     Is this graphic being drawn by a renderpipeline?
	/// </summary>
	public bool IsBeingDrawn { get; set; }
	
	public bool DrawRegardlessOfVisibility { get; set; }
	public void Draw (RenderTarget target);
}
