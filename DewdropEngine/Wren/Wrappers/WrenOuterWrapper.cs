using DewDrop.Wren;
using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using SFML.Graphics;
using DewDrop.Graphics;

[WrenClass("Outer")]
public static class WrenOuterWrapper {
	private const string constructorCode = $"System.print(\"hello\")";
	[WrenMethod("Log", "message")]
	public static void Log (WrenVM vm) {
		vm.EnsureSlots(1);
		DewDrop.Utilities.Outer.sLog(vm.GetSlotString(1));
	}

	[WrenMethod("LogAssertion", "condition", "message")]
	public static void LogAssertion (WrenVM vm) {
		vm.EnsureSlots(2);
		DewDrop.Utilities.Outer.sLogAssertion(vm.GetSlotBool(1), vm.GetSlotString(2));
	}

	[WrenMethod("LogError", "message")]
	public static void LogError (WrenVM vm) {
		vm.EnsureSlots(1);
		DewDrop.Utilities.Outer.sLogError(vm.GetSlotString(1));
	}

	[WrenMethod("LogWarning", "message")]
	public static void LogWarning (WrenVM vm) {
		vm.EnsureSlots(1);
		DewDrop.Utilities.Outer.sLogWarning(vm.GetSlotString(1));
	}

	[WrenMethod("LogInfo", "message")]
	public static void LogInfo (WrenVM vm) {
		vm.EnsureSlots(1);
		DewDrop.Utilities.Outer.sLogInfo(vm.GetSlotString(1));
	}

	[WrenMethod("LogESL", "message")]
	public static void LogESL (WrenVM vm) {
		vm.EnsureSlots(1);
		DewDrop.Utilities.Outer.sLogESL(vm.GetSlotString(1));
	}

	[WrenMethod("LogDebug", "message")]
	public static void LogDebug (WrenVM vm) {
		vm.EnsureSlots(1);
		DewDrop.Utilities.Outer.sLogDebug(vm.GetSlotString(1));
	}
}
