namespace DewDrop.Updatable;

public interface IUpdateable {

	int Priority { get; }
	void Update ();
}
