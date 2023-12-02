#region

using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.GUI.Fonts;
using IronWren;
using IronWren.AutoMapper;

#endregion
namespace DewDrop.Wren;

[WrenClass("TextRenderer")]
public class WrenTextRendererWrapper : BasicRenderableWrapper {
	const string constructorCode = "";
	public TextRenderer _original;
	public override IRenderable Renderable => _original;
	WrenColorWrapper colorWrapper;

	[WrenConstructor("position", "depth", "text", Code = "field:constructorCode")]
	public WrenTextRendererWrapper (WrenVM vm) {
		vm.EnsureSlots(3);
		var position = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		var depth = (int)vm.GetSlotDouble(2);
		var text = vm.GetSlotString(3);
		_original = new DewDrop.GUI.TextRenderer(position.Vector, depth, text);
	}
	[WrenProperty(PropertyType.Set, "RenderPosition")]
	public void SetRenderPosition (WrenVM vm) {
		vm.EnsureSlots(1);
		_original.RenderPosition = vm.GetSlotForeign<WrenVector2Wrapper>(1).Vector;
	}

	[WrenProperty(PropertyType.Get, "RenderPosition")]
	public void GetRenderPosition (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(_original.RenderPosition));
	}

	[WrenProperty(PropertyType.Set, "Text")]
	public void SetText (WrenVM vm) {
		vm.EnsureSlots(1);
		_original.Text = vm.GetSlotString(1);
	}

	[WrenProperty(PropertyType.Get, "Text")]
	public void GetText (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotString(0, _original.Text);
	}

	[WrenProperty(PropertyType.Set, "Color")]
	public void SetColor (WrenVM vm) {
		vm.EnsureSlots(1);
		_original.Color = vm.GetSlotForeign<WrenColorWrapper>(1).Color;
	}

	[WrenProperty(PropertyType.Get, "Color")]
	public void GetColor (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenColorWrapper(_original.Color));
	}

	[WrenProperty(PropertyType.Set, "Origin")]
	public void SetOrigin (WrenVM vm) {
		vm.EnsureSlots(1);
		_original.Origin = vm.GetSlotForeign<WrenVector2Wrapper>(1).Vector;
	}

	[WrenProperty(PropertyType.Get, "Origin")]
	public void GetOrigin (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(_original.Origin));
	}

	[WrenProperty(PropertyType.Set, "Size")]
	public void SetSize (WrenVM vm) {
		vm.EnsureSlots(1);
		_original.Size = vm.GetSlotForeign<WrenVector2Wrapper>(1).Vector;
	}

	[WrenProperty(PropertyType.Get, "Size")]
	public void GetSize (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(_original.Size));
	}

	[WrenProperty(PropertyType.Set, "Depth")]
	public void SetDepth (WrenVM vm) {
		vm.EnsureSlots(1);
		_original.Depth = (int)vm.GetSlotDouble(1);
	}

	[WrenProperty(PropertyType.Get, "Depth")]
	public void GetDepth (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, _original.Depth);
	}

	[WrenProperty(PropertyType.Set, "Visible")]
	public void SetVisible (WrenVM vm) {
		vm.EnsureSlots(1);
		_original.Visible = vm.GetSlotBool(1);
	}

	[WrenProperty(PropertyType.Get, "Visible")]
	public void GetVisible (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotBool(0, _original.Visible);
	}

	[WrenProperty(PropertyType.Set, "Rotation")]
	public void SetRotation (WrenVM vm) {
		vm.EnsureSlots(1);
		_original.Rotation = (float)vm.GetSlotDouble(1);
	}

	[WrenProperty(PropertyType.Get, "Rotation")]
	public void GetRotation (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, _original.Rotation);
	}

	[WrenProperty(PropertyType.Set, "IsBeingDrawn")]
	public void SetIsBeingDrawn (WrenVM vm) {
		vm.EnsureSlots(1);
		_original.IsBeingDrawn = vm.GetSlotBool(1);
	}

	[WrenProperty(PropertyType.Get, "IsBeingDrawn")]
	public void GetIsBeingDrawn (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotBool(0, _original.IsBeingDrawn);
	}
}