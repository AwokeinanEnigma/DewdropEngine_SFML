using DewDrop.Wren;
using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using SFML.Graphics;
using DewDrop.Graphics;

[WrenClass("LineRenderer")]
public class WrenLineRendererWrapper : BasicRenderableWrapper {
	public override IRenderable Renderable => Stored;
	private const string constructorCode = "";
	public DewDrop.Graphics.LineRenderer Stored;
	public WrenLineRendererWrapper (DewDrop.Graphics.LineRenderer original) {
		Stored = original;
	}
	[WrenConstructor("positionA", "positionB", "size", "origin", "depth", "color", Code = "field:constructorCode")]
	public WrenLineRendererWrapper (WrenVM vm) {
		vm.EnsureSlots(6);
		var positionA = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		var positionB = vm.GetSlotForeign<WrenVector2Wrapper>(2);
		var size = vm.GetSlotForeign<WrenVector2Wrapper>(3);
		var origin = vm.GetSlotForeign<WrenVector2Wrapper>(4);
		var depth = (int)vm.GetSlotDouble(5);
		var color = vm.GetSlotForeign<WrenColorWrapper>(6);
		Stored = new DewDrop.Graphics.LineRenderer(positionA.Vector, positionB.Vector, size.Vector, origin.Vector, depth, color.Color);
	}
	[WrenMethod("New", "positionA", "positionB", "size", "origin", "depth", "color")]
	public static void New (WrenVM vm) {
		vm.EnsureSlots(6);
		var positionA = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		var positionB = vm.GetSlotForeign<WrenVector2Wrapper>(2);
		var size = vm.GetSlotForeign<WrenVector2Wrapper>(3);
		var origin = vm.GetSlotForeign<WrenVector2Wrapper>(4);
		var depth = (int)vm.GetSlotDouble(5);
		var color = vm.GetSlotForeign<WrenColorWrapper>(6);
		vm.SetSlotNewForeign(0, new WrenLineRendererWrapper(positionA.Vector, positionB.Vector, size.Vector, origin.Vector, depth, color.Color));
	}
	public WrenLineRendererWrapper (DewDrop.Utilities.Vector2 positionA, DewDrop.Utilities.Vector2 positionB, DewDrop.Utilities.Vector2 size, DewDrop.Utilities.Vector2 origin, System.Int32 depth, SFML.Graphics.Color color) {
		Stored = new DewDrop.Graphics.LineRenderer(positionA, positionB, size, origin, depth, color);
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

	[WrenProperty(PropertyType.Set, "DrawRegardlessOfVisibility")]
	public void SetDrawRegardlessOfVisibility (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.DrawRegardlessOfVisibility = vm.GetSlotBool(1);
	}

	[WrenProperty(PropertyType.Get, "DrawRegardlessOfVisibility")]
	public void GetDrawRegardlessOfVisibility (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotBool(0, Stored.DrawRegardlessOfVisibility);
	}

	[WrenMethod("SetPosition", "index", "position")]
	public void SetPosition (WrenVM vm) {
		vm.EnsureSlots(2);
		Stored.SetPosition((int)vm.GetSlotDouble(1), vm.GetSlotForeign<WrenVector2Wrapper>(2).Vector);
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