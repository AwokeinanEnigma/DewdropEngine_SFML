using DewDrop.Wren;
using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using SFML.Graphics;

[WrenClass("SpriteDefinition")]
public class WrenSpriteDefinitionWrapper {
	private const string constructorCode = "";
	public DewDrop.Graphics.SpriteDefinition Stored;
	public WrenSpriteDefinitionWrapper (DewDrop.Graphics.SpriteDefinition original) {
		Stored = original;
	}
	[WrenConstructor("name", "coords", "bounds", "origin", "frames", "flipX", "flipY", "mode", Code = "field:constructorCode")]
	public WrenSpriteDefinitionWrapper (WrenVM vm) {
		vm.EnsureSlots(8);
		var name = vm.GetSlotString(1);
		var coords = vm.GetSlotForeign<WrenVector2Wrapper>(2);
		var bounds = vm.GetSlotForeign<WrenVector2Wrapper>(3);
		var origin = vm.GetSlotForeign<WrenVector2Wrapper>(4);
		var frames = (int)vm.GetSlotDouble(5);
		var flipX = vm.GetSlotBool(6);
		var flipY = vm.GetSlotBool(7);
		var mode = (int)vm.GetSlotDouble(8);
		Stored = new DewDrop.Graphics.SpriteDefinition(name, coords.Vector, bounds.Vector, origin.Vector, frames, null, flipX, flipY, mode, null);
	}
	[WrenProperty(PropertyType.Get, "IsValid")]
	public void GetIsValid (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotBool(0, Stored.IsValid);
	}

	[WrenProperty(PropertyType.Get, "Name")]
	public void GetName (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotString(0, Stored.Name);
	}

	[WrenProperty(PropertyType.Get, "Coords")]
	public void GetCoords (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(Stored.Coords));
	}

	[WrenProperty(PropertyType.Get, "Bounds")]
	public void GetBounds (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(Stored.Bounds));
	}

	[WrenProperty(PropertyType.Get, "Offset")]
	public void GetOffset (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(Stored.Offset));
	}

	[WrenProperty(PropertyType.Get, "Frames")]
	public void GetFrames (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Stored.Frames);
	}

	[WrenProperty(PropertyType.Get, "FlipX")]
	public void GetFlipX (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotBool(0, Stored.FlipX);
	}

	[WrenProperty(PropertyType.Get, "FlipY")]
	public void GetFlipY (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotBool(0, Stored.FlipY);
	}

	[WrenMethod("GetHashCode")]
	public void GetHashCode (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Stored.GetHashCode());
	}

	[WrenMethod("ToString")]
	public void ToString (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotString(0, Stored.ToString());
	}
}