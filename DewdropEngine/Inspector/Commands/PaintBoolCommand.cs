using DewDrop.Internal;
using System.Reflection;

namespace DewDrop.Inspector.Commands; 

public class PaintBoolCommand : InspectorCommand {
	readonly bool _value;
	bool _previousValue;

	public PaintBoolCommand (MemberInfo member, bool value, Component entity) {
		_member = member;
		_value = value;
		_entity = entity;
	}

	public override void Execute () {
		// Save the previous state for undo
		_previousValue = (bool)GetValue(_entity);

		// Execute the action
		SetValue(_entity, _value);
	}

	public override void Undo () {
		// Revert the action
		SetValue(_entity, _previousValue);
	}

	public override void Redo () {
		// Reapply the action
		SetValue(_entity, _value);
	}
}