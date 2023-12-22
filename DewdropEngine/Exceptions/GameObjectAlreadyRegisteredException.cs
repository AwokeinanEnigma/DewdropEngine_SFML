namespace DewDrop.Exceptions; 

public class GameObjectAlreadyRegisteredException : Exception {
	public GameObjectAlreadyRegisteredException (string message) : base(message) {}
}
