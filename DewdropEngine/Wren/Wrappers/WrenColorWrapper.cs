using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using SFML.Graphics;
namespace DewDrop.Wren;

[WrenClass("Color")]
public class WrenColorWrapper {
	public Color Color;
	private const string constructorCode = $"System.print(\"hello\")";

	public WrenColorWrapper(Color color) {
		Color = color;
	}
	
	[WrenProperty(PropertyType.Get, "r")]
	void GetR(WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Color.R);
	}
	
	[WrenProperty(PropertyType.Get, "g")]
	void GetG(WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Color.G);
	}
	
	[WrenProperty(PropertyType.Get, "b")]
	void GetB(WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Color.B);
	}
	
	[WrenProperty(PropertyType.Get, "a")]
	void GetA(WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Color.A);
	}
	
	[WrenProperty(PropertyType.Set, "r")]
	void SetR(WrenVM vm) {
		vm.EnsureSlots(1);
		Color.R = (byte)vm.GetSlotDouble(1);
	}
	
	[WrenProperty(PropertyType.Set, "g")]
	void SetG(WrenVM vm) {
		vm.EnsureSlots(1);
		Color.G = (byte)vm.GetSlotDouble(1);
	}
	
	[WrenProperty(PropertyType.Set, "b")]
	void SetB(WrenVM vm) {
		vm.EnsureSlots(1);
		Color.B = (byte)vm.GetSlotDouble(1);
	}
	
	[WrenProperty(PropertyType.Set, "a")]
	void SetA(WrenVM vm) {
		vm.EnsureSlots(1);
		Color.A = (byte)vm.GetSlotDouble(1);
	}
	
	[WrenMethod("toString", "color")]
	void ToString(WrenVM vm) {
		vm.SetSlotString(0, Color.ToString());
	}
	
	[WrenMethod("fromString", "r", "g", "b", "a")]
	void FromString(WrenVM vm) {
		vm.EnsureSlots(1);
		Color = new Color((byte)vm.GetSlotDouble(1), (byte)vm.GetSlotDouble(2), (byte)vm.GetSlotDouble(3), (byte)vm.GetSlotDouble(4));
	}
	
	[WrenConstructor("r", "g", "b")]
	WrenColorWrapper(WrenVM vm) {
		vm.EnsureSlots(3);
		Color = new Color((byte)vm.GetSlotDouble(1), (byte)vm.GetSlotDouble(2), (byte)vm.GetSlotDouble(3));
	}

	[WrenMethod("New", "r", "g", "b")]
	static void New (WrenVM vm) {
		vm.EnsureSlots(3);
		vm.SetSlotNewForeign(0, new WrenColorWrapper(new Color((byte)vm.GetSlotDouble(1), (byte)vm.GetSlotDouble(2), (byte)vm.GetSlotDouble(3))));
	}	
}
