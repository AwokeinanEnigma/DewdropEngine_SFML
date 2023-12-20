using System.Collections;

namespace DewDrop.Inspector.Commands; 

public class ClearListCommand : InspectorCommand {
	readonly IList _list;
	readonly List<object> _previousState;

	public ClearListCommand (IList list) {
		_list = list;
		_previousState = new List<object>(_list.Cast<object>());
	}

	public override void Execute () {
		_list.Clear();
	}

	public override void Undo () {
		foreach (var item in _previousState) {
			_list.Add(item);
		}
	}

	public override void Redo () {
		Execute();
	}

}
