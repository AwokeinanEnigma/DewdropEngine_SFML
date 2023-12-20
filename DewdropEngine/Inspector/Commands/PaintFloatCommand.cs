using DewDrop.Entities;
using System.Reflection;

namespace DewDrop.Inspector.Commands; 

public class PaintFloatCommand : InspectorCommand
{
	 readonly float _value;
	 float _previousValue;

	public PaintFloatCommand(MemberInfo member, float value, Entity entity)
	{
		_member = member;
		_value = value;
		_entity = entity;
	}

	public override void Execute()
	{
		// Save the previous state for undo
		_previousValue = (float)GetValue(_entity);

		// Execute the action
		SetValue(_entity, _value);
	}

	public override void Undo()
	{
		// Revert the action
		SetValue(_entity, _previousValue);
	}

	public override void Redo()
	{
		// Reapply the action
		SetValue(_entity, _value);
	}
}
