public class CommandHistory
{
	readonly Stack<ICommand> _undoStack = new Stack<ICommand>();
	readonly Stack<ICommand> _redoStack = new Stack<ICommand>();

	public void ExecuteCommand(ICommand command)
	{
		command.Execute();
		_undoStack.Push(command);
		_redoStack.Clear(); // Once a new command is executed, the redo stack clears
	}

	public void Undo()
	{
		if (_undoStack.Count > 0)
		{
			var command = _undoStack.Pop();
			command.Undo();
			_redoStack.Push(command);
		}
	}

	public void Redo()
	{
		if (_redoStack.Count > 0)
		{
			var command = _redoStack.Pop();
			command.Redo();
			_undoStack.Push(command);
		}
	}
}
