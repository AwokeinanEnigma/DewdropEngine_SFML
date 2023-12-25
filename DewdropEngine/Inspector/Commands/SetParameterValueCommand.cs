namespace DewDrop.Inspector.Commands; 

public class SetParameterValueCommand : InspectorCommand
{
	readonly object[] _list;
	readonly object _previousValue;
	readonly object _newValue;
	readonly int _index;

	public SetParameterValueCommand(object[] list, object newValue, int index)
	{
		_list = list;
		_newValue = newValue;
		_index = index;
		_previousValue = _list[_index];
	}

	public override void Execute()
	{
		
		_list[_index] = _newValue;
	}

	public override void Undo()
	{
		_list[_index] = _previousValue;
	}

	public override void Redo()
	{
		Execute();
	}
}
