using DewDrop.UserInput;
namespace DewDrop.Wren; 

public class WrenOnButtonReleasedHandler : WrenEventHandler{
	Wreno _wren;
	public WrenOnButtonReleasedHandler (Wreno wren) {
		_wren = wren;
		Input.OnButtonReleased += OnButtonReleased;
	}
	public void OnButtonReleased (object sender, DButtons e) {
		_wren.Call ("e_OnButtonReleased", e.ToString ());
	}
	protected override void Dispose (bool disposing) {
		Input.OnButtonReleased -= OnButtonReleased;
	}
}
