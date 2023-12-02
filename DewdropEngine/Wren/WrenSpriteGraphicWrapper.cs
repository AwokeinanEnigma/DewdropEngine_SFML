using DewDrop.Wren;
using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using SFML.Graphics;
using DewDrop.Graphics;

[WrenClass("SpriteGraphic")]
public class WrenSpriteGraphicWrapper : BasicRenderableWrapper {
	public override IRenderable Renderable => Stored;
	private const string constructorCode = "";
	public DewDrop.Graphics.SpriteGraphic Stored;
	public WrenSpriteGraphicWrapper (DewDrop.Graphics.SpriteGraphic original) {
		Stored = original;
	}
	[WrenConstructor("resource", "spriteName", "position", "depth", Code = "field:constructorCode")]
	public WrenSpriteGraphicWrapper (WrenVM vm) {
		vm.EnsureSlots(4);
		var resource = vm.GetSlotString(1);
		var spriteName = vm.GetSlotString(2);
		var position = vm.GetSlotForeign<WrenVector2Wrapper>(3);
		var depth = (int)vm.GetSlotDouble(4);
		Stored = new DewDrop.Graphics.SpriteGraphic(resource, spriteName, position.Vector, depth);
	}
	[WrenProperty(PropertyType.Set, "CurrentPalette")]
	public void SetCurrentPalette (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.CurrentPalette = (uint)vm.GetSlotDouble(1);
	}

	[WrenProperty(PropertyType.Get, "CurrentPalette")]
	public void GetCurrentPalette (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Stored.CurrentPalette);
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

	[WrenProperty(PropertyType.Get, "PreviousPalette")]
	public void GetPreviousPalette (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Stored.PreviousPalette);
	}

	[WrenProperty(PropertyType.Set, "AnimationEnabled")]
	public void SetAnimationEnabled (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.AnimationEnabled = vm.GetSlotBool(1);
	}

	[WrenProperty(PropertyType.Get, "AnimationEnabled")]
	public void GetAnimationEnabled (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotBool(0, Stored.AnimationEnabled);
	}

	[WrenProperty(PropertyType.Get, "Frames")]
	public void GetFrames (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Stored.Frames);
	}

	[WrenProperty(PropertyType.Set, "Frame")]
	public void SetFrame (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.Frame = (float)vm.GetSlotDouble(1);
	}

	[WrenProperty(PropertyType.Get, "Frame")]
	public void GetFrame (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Stored.Frame);
	}

	[WrenProperty(PropertyType.Set, "SpeedModifier")]
	public void SetSpeedModifier (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.SpeedModifier = (float)vm.GetSlotDouble(1);
	}

	[WrenProperty(PropertyType.Get, "SpeedModifier")]
	public void GetSpeedModifier (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Stored.SpeedModifier);
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

	[WrenMethod("SetSprite", "name")]
	public void SetSprite (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.SetSprite(vm.GetSlotString(1));
	}

	[WrenMethod("SetSprite", "name", "reset")]
	public void SetSprite2 (WrenVM vm) {
		vm.EnsureSlots(2);
		Stored.SetSprite(vm.GetSlotString(1), vm.GetSlotBool(2));
	}

	[WrenMethod("GetSpriteDefinition", "sprite")]
	public void GetSpriteDefinition (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenSpriteDefinitionWrapper(Stored.GetSpriteDefinition(vm.GetSlotString(1))));
	}

	[WrenMethod("Clone")]
	public void Clone (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenSpriteGraphicWrapper(Stored.Clone()));
	}
	[WrenMethod("Translate", "x", "y")]
	public void Translate (WrenVM vm) {
		vm.EnsureSlots(2);
		Stored.Translate((float)vm.GetSlotDouble(1), (float)vm.GetSlotDouble(2));
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