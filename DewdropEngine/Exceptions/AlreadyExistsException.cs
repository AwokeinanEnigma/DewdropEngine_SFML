namespace DewDrop.Exceptions; 

public class AlreadyExistsException : Exception {
	public AlreadyExistsException (Type type, string message) : base (message) {}
}
