using DewDrop.Wren;
using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using SFML.Graphics;
using DewDrop.Graphics;

[WrenClass("ShapeGraphic")]
public class WrenShapeGraphicWrapper : BasicRenderableWrapper {
	public override IRenderable Renderable => Stored;
	private const string constructorCode = "";
	public DewDrop.Graphics.ShapeGraphic Stored;
	public WrenShapeGraphicWrapper (DewDrop.Graphics.ShapeGraphic original) {
		Stored = original;
	}	
	[WrenConstructor("position", "size", "origin", "depth", "fillColor", "outlineColor", Code = "field:constructorCode")]
	public WrenShapeGraphicWrapper (WrenVM vm) {
		vm.EnsureSlots(6);
		var position = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		var size = vm.GetSlotForeign<WrenVector2Wrapper>(2);
		var origin = vm.GetSlotForeign<WrenVector2Wrapper>(3);
		var depth = (int)vm.GetSlotDouble(4);
		var fillColor = vm.GetSlotForeign<WrenColorWrapper>(5);
		var outlineColor = vm.GetSlotForeign<WrenColorWrapper>(6);
		Stored = new DewDrop.Graphics.ShapeGraphic(null, position.Vector, size.Vector, origin.Vector, depth, fillColor.Color, outlineColor.Color);
	}
	[WrenProperty(PropertyType.Set, "OutlineColor")]
	public void SetOutlineColor (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.OutlineColor = vm.GetSlotForeign<WrenColorWrapper>(1).Color;
	}

	[WrenProperty(PropertyType.Get, "OutlineColor")]
	public void GetOutlineColor (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenColorWrapper(Stored.OutlineColor));
	}

	[WrenProperty(PropertyType.Set, "FillColor")]
	public void SetFillColor (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.FillColor = vm.GetSlotForeign<WrenColorWrapper>(1).Color;
	}

	[WrenProperty(PropertyType.Get, "FillColor")]
	public void GetFillColor (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenColorWrapper(Stored.FillColor));
	}

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
