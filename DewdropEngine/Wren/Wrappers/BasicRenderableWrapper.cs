using DewDrop.Graphics;
namespace DewDrop.Wren; 

public class BasicRenderableWrapper  {
	public virtual IRenderable Renderable { get; }
	private const string constructorCode = $"System.print(\"hello\")";
}
