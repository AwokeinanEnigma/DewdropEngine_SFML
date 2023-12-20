using System.Collections;

namespace DewDrop.Inspector.Commands;

public class SetListValueCommand : InspectorCommand
{
	 readonly IList _list; 
	 readonly object _previousValue;
	 readonly object _newValue;
	 readonly int _index;

	public SetListValueCommand(IList list, object newValue, int index)
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
