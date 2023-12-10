using DewDrop.Wren;
using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using SFML.Graphics;
using DewDrop.Graphics;

[WrenClass("TextRenderer")]
public class WrenTextRendererWrapper : BasicRenderableWrapper {
	public override IRenderable Renderable => Stored;
	private const string constructorCode = "";
	public DewDrop.GUI.TextRenderer Stored;
	public WrenTextRendererWrapper (DewDrop.GUI.TextRenderer original) {
		Stored = original;
	}
	[WrenConstructor("position", "depth", "text", Code = "field:constructorCode")]
	public WrenTextRendererWrapper (WrenVM vm) {
		vm.EnsureSlots(3);
		var position = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		var depth = (int)vm.GetSlotDouble(2);
		var text = vm.GetSlotString(3);
		Stored = new DewDrop.GUI.TextRenderer(position.Vector, depth, text);
	}
	[WrenMethod("New", "position", "depth", "text")]
	public static void New (WrenVM vm) {
		vm.EnsureSlots(3);
		var position = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		var depth = (int)vm.GetSlotDouble(2);
		var text = vm.GetSlotString(3);
		vm.SetSlotNewForeign(0, new WrenTextRendererWrapper(position.Vector, depth, text));
	}
	public WrenTextRendererWrapper (DewDrop.Utilities.Vector2 position, System.Int32 depth, System.String text) {
		Stored = new DewDrop.GUI.TextRenderer(position, depth, text);
	}
// Property wrappers
	[WrenProperty(PropertyType.Set, "RenderPosition")]
	public void SetRenderPosition (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.RenderPosition = vm.GetSlotForeign<WrenVector2Wrapper>(1).Vector;
	}

	[WrenProperty(PropertyType.Get, "RenderPosition")]
	public void GetRenderPosition (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(Stored.RenderPosition));
	}

	[WrenProperty(PropertyType.Set, "Text")]
	public void SetText (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.Text = vm.GetSlotString(1);
	}

	[WrenProperty(PropertyType.Get, "Text")]
	public void GetText (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotString(0, Stored.Text);
	}

	[WrenProperty(PropertyType.Set, "Color")]
	public void SetColor (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.Color = vm.GetSlotForeign<WrenColorWrapper>(1).Color;
	}

	[WrenProperty(PropertyType.Get, "Color")]
	public void GetColor (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenColorWrapper(Stored.Color));
	}

	[WrenProperty(PropertyType.Set, "Origin")]
	public void SetOrigin (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.Origin = vm.GetSlotForeign<WrenVector2Wrapper>(1).Vector;
	}

	[WrenProperty(PropertyType.Get, "Origin")]
	public void GetOrigin (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(Stored.Origin));
	}

	[WrenProperty(PropertyType.Set, "Size")]
	public void SetSize (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.Size = vm.GetSlotForeign<WrenVector2Wrapper>(1).Vector;
	}

	[WrenProperty(PropertyType.Get, "Size")]
	public void GetSize (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(Stored.Size));
	}

	[WrenProperty(PropertyType.Set, "Depth")]
	public void SetDepth (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.Depth = (int)vm.GetSlotDouble(1);
	}

	[WrenProperty(PropertyType.Get, "Depth")]
	public void GetDepth (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Stored.Depth);
	}

	[WrenProperty(PropertyType.Set, "Visible")]
	public void SetVisible (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.Visible = vm.GetSlotBool(1);
	}

	[WrenProperty(PropertyType.Get, "Visible")]
	public void GetVisible (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotBool(0, Stored.Visible);
	}

	[WrenProperty(PropertyType.Set, "Rotation")]
	public void SetRotation (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.Rotation = (float)vm.GetSlotDouble(1);
	}

	[WrenProperty(PropertyType.Get, "Rotation")]
	public void GetRotation (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Stored.Rotation);
	}

	[WrenProperty(PropertyType.Set, "IsBeingDrawn")]
	public void SetIsBeingDrawn (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.IsBeingDrawn = vm.GetSlotBool(1);
	}

	[WrenProperty(PropertyType.Get, "IsBeingDrawn")]
	public void GetIsBeingDrawn (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotBool(0, Stored.IsBeingDrawn);
	}

	[WrenMethod("FindCharacterPosition", "index")]
	public void FindCharacterPosition (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(Stored.FindCharacterPosition((uint)vm.GetSlotDouble(1))));
	}

	[WrenMethod("ToString")]
	public void ToString (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotString(0, Stored.ToString());
	}
	[WrenMethod("GetHashCode")]
	public void GetHashCode (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Stored.GetHashCode());
	}
}