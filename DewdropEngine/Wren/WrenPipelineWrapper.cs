using DewDrop.Graphics;
using IronWren;
using IronWren.AutoMapper;
namespace DewDrop.Wren; 

[WrenClass("RenderPipeline")]
public static class WrenPipelineWrapper {
	public static RenderPipeline Pipeline;
	private const string constructorCode = "";

	[WrenMethod("Add", "renderable")]
	public static void Add(WrenVM vm)
	{
		vm.EnsureSlots(1); 
		Pipeline.Add(vm.GetSlotForeign<BasicRenderableWrapper>(1).Renderable);
	}
	
}
