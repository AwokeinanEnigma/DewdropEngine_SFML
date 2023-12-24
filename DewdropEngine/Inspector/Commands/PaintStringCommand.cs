using DewDrop.Internal;
using System.Reflection;

namespace DewDrop.Inspector.Commands; 

public class PaintStringCommand : InspectorCommand {
	readonly string _str;
	string _previousStr;

	public PaintStringCommand (MemberInfo member, string str, Component entity) {
		_member = member;
		_str = str;
		_entity = entity;
	}

	public override void Execute () {
		// Save the previous state for undo
		_previousStr = (string)GetValue(_entity);

		// Execute the action
		SetValue(_entity, _str);
	}

	public override void Undo () {
		// Revert the action
		SetValue(_entity, _previousStr);
	}

	public override void Redo () {
		// Reapply the action
		SetValue(_entity, _str);
	}
}
