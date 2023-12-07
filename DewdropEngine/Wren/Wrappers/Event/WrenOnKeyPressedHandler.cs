using DewDrop.UserInput;
using DewDrop.Utilities;
namespace DewDrop.Wren; 

public class WrenOnKeyPressedHandler : WrenEventHandler {
	Wreno _wren;
	public WrenOnKeyPressedHandler (Wreno wren) {
		_wren = wren;
		Input.OnKeyPressed += OnKeyPressed;
	}
	public void OnKeyPressed (object sender, SFML.Window.Keyboard.Key e) {
		_wren.CallFunction ("e_OnKeyPressed", e.ToString ());
	}
	protected override void Dispose (bool disposing) {
		Input.OnKeyPressed -= OnKeyPressed;
	}
}
