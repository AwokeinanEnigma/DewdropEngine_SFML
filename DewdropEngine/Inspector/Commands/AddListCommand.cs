using System.Collections;

namespace DewDrop.Inspector.Commands;

public class AddListCommand : InspectorCommand, ICommand {
	readonly IList _list;
	readonly object _item;
	readonly int _index;

	public AddListCommand (IList list, object item) {
		_list = list;
		_item = item;
		_index = _list.Count; // The item will be added at the end of the list
	}

	public override void Execute () {
		_list.Add(_item);
	}

	public override void Undo () {
		if (_list.Count > _index) {
			_list.RemoveAt(_index);
		}
	}

	public override void Redo () {
		Execute();
	}
}