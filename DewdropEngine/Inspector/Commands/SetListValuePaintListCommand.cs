using System.Collections;
using System.Collections.Generic;

public class SetListValuePaintListCommand : ICommand
{
	private IList _list;
	private object _previousValue;
	private object _newValue;
	private int _index;

	public SetListValuePaintListCommand(IList list, object newValue, int index)
	{
		_list = list;
		_newValue = newValue;
		_index = index;
		_previousValue = _list[_index];
	}

	public void Execute()
	{
		_list[_index] = _newValue;
	}

	public void Undo()
	{
		_list[_index] = _previousValue;
	}

	public void Redo()
	{
		Execute();
	}
}
