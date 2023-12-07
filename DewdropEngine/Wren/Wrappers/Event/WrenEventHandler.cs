namespace DewDrop.Wren; 

public abstract class WrenEventHandler : IDisposable {
	protected abstract void Dispose (bool disposing);
	public void Dispose () {
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
