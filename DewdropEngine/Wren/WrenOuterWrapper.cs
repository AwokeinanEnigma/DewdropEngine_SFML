using DewDrop.Wren;
using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using SFML.Graphics;
using DewDrop.Graphics;

[WrenClass("Outer")]
public static class WrenOuterWrapper {
	private const string constructorCode = "";
	[WrenMethod("Log", "message")]
	public static void Log (WrenVM vm) {
		vm.EnsureSlots(1);
		DewDrop.Utilities.Outer.Log(vm.GetSlotString(1));
	}

	[WrenMethod("LogAssertion", "condition", "message")]
	public static void LogAssertion (WrenVM vm) {
		vm.EnsureSlots(2);
		DewDrop.Utilities.Outer.LogAssertion(vm.GetSlotBool(1), vm.GetSlotString(2));
	}

	[WrenMethod("LogError", "message")]
	public static void LogError (WrenVM vm) {
		vm.EnsureSlots(1);
		DewDrop.Utilities.Outer.LogError(vm.GetSlotString(1));
	}

	[WrenMethod("LogWarning", "message")]
	public static void LogWarning (WrenVM vm) {
		vm.EnsureSlots(1);
		DewDrop.Utilities.Outer.LogWarning(vm.GetSlotString(1));
	}

	[WrenMethod("LogInfo", "message")]
	public static void LogInfo (WrenVM vm) {
		vm.EnsureSlots(1);
		DewDrop.Utilities.Outer.LogInfo(vm.GetSlotString(1));
	}

	[WrenMethod("LogESL", "message")]
	public static void LogESL (WrenVM vm) {
		vm.EnsureSlots(1);
		DewDrop.Utilities.Outer.LogESL(vm.GetSlotString(1));
	}

	[WrenMethod("LogDebug", "message")]
	public static void LogDebug (WrenVM vm) {
		vm.EnsureSlots(1);
		DewDrop.Utilities.Outer.LogDebug(vm.GetSlotString(1));
	}
}
