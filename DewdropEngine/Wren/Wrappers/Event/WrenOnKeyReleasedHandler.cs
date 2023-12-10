using DewDrop.UserInput;
namespace DewDrop.Wren; 

public class WrenOnKeyReleasedHandler : WrenEventHandler{
	Wreno _wren;
	public WrenOnKeyReleasedHandler (Wreno wren) {
		_wren = wren;
		Input.OnKeyReleased += OnKeyReleased;
	}
	public void OnKeyReleased (object sender, SFML.Window.Keyboard.Key e) {
		_wren.Call ("e_OnKeyReleased", e.ToString ());
	}
	protected override void Dispose (bool disposing) {
		Input.OnKeyReleased -= OnKeyReleased;
	}
	
	
}
