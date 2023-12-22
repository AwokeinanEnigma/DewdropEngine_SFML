namespace DewDrop.Exceptions; 

public class TooManyChildrenException : Exception {
	public TooManyChildrenException (string message) : base(message) {}
}
