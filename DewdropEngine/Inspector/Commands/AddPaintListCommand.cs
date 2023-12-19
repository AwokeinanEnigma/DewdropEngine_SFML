using System.Collections;
using System.Collections.Generic;

public class AddPaintListCommand : ICommand
{
	private IList _list;
	private object _item;
	private int _index;

	public AddPaintListCommand(IList list, object item)
	{
		_list = list;
		_item = item;
		_index = _list.Count; // The item will be added at the end of the list
	}

	public void Execute()
	{
		_list.Add(_item);
	}

	public void Undo()
	{
		if (_list.Count > _index)
		{
			_list.RemoveAt(_index);
		}
	}

	public void Redo()
	{
		Execute();
	}
}
