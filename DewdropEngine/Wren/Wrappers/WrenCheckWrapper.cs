using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
namespace DewDrop.Wren;

[WrenClass("CheckWrapper")]
public class WrenCheckWrapper {
	//IronWren needs this to be here or else it'll throw an exception
	// You can put wren code here if you want
	private const string constructorCode = $"System.print(\"hello\")";

	// this is a wren method
	[WrenCode]
	private const string print = "print() {\nSystem.print(\"Vector (%(x), %(y))\")\n}";

	float stored;

	[WrenConstructor("x", Code = "field:constructorCode")]
	private WrenCheckWrapper (WrenVM vm) {
		vm.EnsureSlots(1);
		stored = (float)vm.GetSlotDouble(1);
	}

	[WrenProperty(PropertyType.Get, "x")]
	private void GetX (WrenVM vm) {
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, stored);
	}

	[WrenProperty(PropertyType.Set, "x")]
	private void SetY (WrenVM vm) {
		vm.EnsureSlots(1);
		stored = (float)vm.GetSlotDouble(1);
	}

	[WrenMethod("check")]
	private void Add (WrenVM vm) {
		vm.EnsureSlots(1);
		Outer.Log($"check {stored}");

	}
}
