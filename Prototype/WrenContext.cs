using DewDrop.Wren;
using IronWren;
using IronWren.AutoMapper;
using Mother4.GUI;
namespace Prototype; 

[WrenClass("Context")]
public static class WrenContext {
	public static TextBox TextBox { get; set; }
	private const string constructorCode = $"System.print(\"hello\")";

	[WrenMethod("ShowTextbox")]
	public static void ShowTextbox(WrenVM vm)
	{
		//vm.EnsureSlots(4); 
		TextBox.Show();
	}
	
	[WrenMethod("HideTextbox")]
	public static void HideTextbox(WrenVM vm)
	{
		//vm.EnsureSlots(4); 
		TextBox.Hide();;
	}

	[WrenMethod("SetTextboxText", "text", "name", "suppressShow", "suppressHide")]
	public static void ShowText (WrenVM vm) {
		vm.EnsureSlots(4);
		TextBox.Reset(vm.GetSlotString(1), vm.GetSlotString(2), vm.GetSlotBool(3), vm.GetSlotBool(4));
	}
	
	[WrenMethod("ShowTextboxText", "text", "name", "suppressShow", "suppressHide")]
	public static void StartText(WrenVM vm) {
		vm.EnsureSlots(4);
		TextBox.Show();
		TextBox.Reset(vm.GetSlotString(1), vm.GetSlotString(2), vm.GetSlotBool(3), vm.GetSlotBool(4));
	}

}

