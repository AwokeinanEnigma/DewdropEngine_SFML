/// <summary>
/// Defines a command that can be executed, undone, and redone.
/// </summary>
/// <remarks>
/// This interface is used to implement the Command pattern, which allows actions to be encapsulated as objects.
/// These command objects can then be executed, undone, and redone as needed, providing a mechanism for implementing undo/redo functionality.
/// </remarks>
public interface ICommand
{
	/// <summary>
	/// Executes the command.
	/// </summary>
	void Execute();

	/// <summary>
	/// Undoes the command.
	/// </summary>
	void Undo();

	/// <summary>
	/// Redoes the command.
	/// </summary>
	void Redo();
}
