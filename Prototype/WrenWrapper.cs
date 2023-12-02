using DewDrop.Wren;
using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using SFML.Graphics;
using DewDrop.Graphics;

[WrenClass("WrenTest")]
public class WrenWrenTestWrapper
{
	private const string constructorCode = "";
	public WrenTest Stored;
	public WrenWrenTestWrapper(WrenTest original)
	{
		Stored = original;
	}
	[WrenConstructor(Code = "field:constructorCode")]
	public WrenWrenTestWrapper(WrenVM vm)
	{
		Stored = new WrenTest();
	}
	[WrenMethod("Log", "message")]
	public void Log(WrenVM vm)
	{
		vm.EnsureSlots(1);
		Stored.Log(vm.GetSlotString(1));
	}

	[WrenMethod("LogWarning", "message")]
	public void LogWarning(WrenVM vm)
	{
		vm.EnsureSlots(1);
		Stored.LogWarning(vm.GetSlotString(1));
	}

	[WrenMethod("LogError", "message")]
	public void LogError(WrenVM vm)
	{
		vm.EnsureSlots(1);
		Stored.LogError(vm.GetSlotString(1));
	}

	[WrenMethod("ToString")]
	public void ToString(WrenVM vm)
	{
		vm.EnsureSlots(1);
		vm.SetSlotString(0, Stored.ToString());
	}
	[WrenMethod("GetHashCode")]
	public void GetHashCode(WrenVM vm)
	{
		vm.EnsureSlots(1);
		vm.SetSlotDouble(0, Stored.GetHashCode());
	}
}
