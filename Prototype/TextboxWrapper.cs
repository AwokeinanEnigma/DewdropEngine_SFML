using DewDrop.Wren;
using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using SFML.Graphics;
using DewDrop.Graphics;

[WrenClass("TextBox")]
public class WrenTextBoxWrapper {
	private const string constructorCode = "";
	public Mother4.GUI.TextBox Stored;
	public WrenTextBoxWrapper (Mother4.GUI.TextBox original) {
		Stored = original;
	}
	[WrenConstructor("colorIndex", Code = "field:constructorCode")]
	public WrenTextBoxWrapper (WrenVM vm) {
		vm.EnsureSlots(1);
		var colorIndex = (int)vm.GetSlotDouble(1);
		Stored = new Mother4.GUI.TextBox(WrenPipelineWrapper.Pipeline, colorIndex);
		
	}
	[WrenMethod("New", "colorIndex")]
	public static void New (WrenVM vm) {
		vm.EnsureSlots(1);
		var colorIndex = (int)vm.GetSlotDouble(1);
		vm.SetSlotNewForeign(0, new WrenTextBoxWrapper(colorIndex));
	}
	public WrenTextBoxWrapper (System.Int32 colorIndex) {
		Stored = new Mother4.GUI.TextBox(null, colorIndex);
	}
// Field wrappers

// Property wrappers
	[WrenProperty(PropertyType.Get, "Visible")]
	public void GetVisible (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotBool(0, Stored.Visible);
	}

	[WrenProperty(PropertyType.Get, "Name")]
	public void GetName (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotString(0, Stored.Name);
	}

	[WrenProperty(PropertyType.Set, "Position")]
	public void SetPosition (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.Position = vm.GetSlotForeign<WrenVector2Wrapper>(1).Vector;
	}

	[WrenProperty(PropertyType.Get, "Position")]
	public void GetPosition (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(Stored.Position));
	}

	[WrenMethod("Reset", "text", "namestring", "suppressSlideIn", "suppressSlideOut")]
	public void Reset (WrenVM vm) {
		vm.EnsureSlots(4);
		Stored.Reset(vm.GetSlotString(1), vm.GetSlotString(2), vm.GetSlotBool(3), vm.GetSlotBool(4));
	}

	[WrenMethod("SetDimmer", "dim")]
	public void SetDimmer (WrenVM vm) {
		vm.EnsureSlots(1);
		Stored.SetDimmer((float)vm.GetSlotDouble(1));
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