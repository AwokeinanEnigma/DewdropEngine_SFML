using DewDrop.UserInput;
using DewDrop.Utilities;
using IronWren;
namespace DewDrop.Wren; 

public class WrenInputEventHandler : WrenEventHandler {
	Wreno _wren;
	public WrenInputEventHandler(Wreno wren) {
		_wren = wren;
		Input.OnButtonPressed += OnButtonPressed;
	}
	public void OnButtonPressed (object sender, DButtons e) {
		_wren.CallFunction("OnButtonPressed", e.ToString());
	}

	protected override void Dispose (bool disposing) {
		Input.OnButtonPressed -= OnButtonPressed;
	}
}
