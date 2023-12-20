/// <summary>
/// Manages the history of executed commands for undo and redo operations.
/// </summary>
/// <remarks>
/// This class maintains two stacks of ICommand objects, one for undo operations and one for redo operations.
/// When a command is executed, it is added to the undo stack and the redo stack is cleared.
/// When an undo operation is performed, the command is moved from the undo stack to the redo stack.
/// When a redo operation is performed, the command is moved from the redo stack back to the undo stack.
/// </remarks>
public class CommandHistory
{
    /// <summary>
    /// Stack of commands that can be undone.
    /// </summary>
    private readonly Stack<ICommand> _undoStack = new Stack<ICommand>();

    /// <summary>
    /// Stack of commands that can be redone.
    /// </summary>
    private readonly Stack<ICommand> _redoStack = new Stack<ICommand>();

    /// <summary>
    /// Gets the undo stack.
    /// </summary>
    public Stack<ICommand> UndoStack => _undoStack;

    /// <summary>
    /// Gets a value indicating whether there are any commands that can be undone.
    /// </summary>
    public bool CanUndo => _undoStack.Count > 0;

    /// <summary>
    /// Gets a value indicating whether there are any commands that can be redone.
    /// </summary>
    public bool CanRedo => _redoStack.Count > 0;

    /// <summary>
    /// Gets a value indicating whether there are any commands in the history.
    /// </summary>
    public bool HasCommands => _undoStack.Count > 0 || _redoStack.Count > 0;

    /// <summary>
    /// Executes a command and adds it to the history.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear(); // Once a new command is executed, the redo stack clears
    }

    /// <summary>
    /// Undoes the last executed command.
    /// </summary>
    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
        }
    }

    /// <summary>
    /// Redoes the last undone command.
    /// </summary>
    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            var command = _redoStack.Pop();
            command.Redo();
            _undoStack.Push(command);
        }
    }

    /// <summary>
    /// Clears the command history.
    /// </summary>
    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }
}