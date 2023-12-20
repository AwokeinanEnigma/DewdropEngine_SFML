using DewDrop.Entities;
using System.Reflection;

namespace DewDrop.Inspector.Commands; 

public class PaintEnumCommand : InspectorCommand
{
	readonly object _previousValue;
	readonly object _newValue;

	public PaintEnumCommand(MemberInfo memberInfo, object newValue, Entity entity)
	{
		_member = memberInfo;
		_newValue = newValue;
		_entity = entity;
		_previousValue = GetValue(_entity);
	}

	public override void Execute()
	{
		SetValue(_entity, _newValue);
	}

	public override void Undo()
	{
		SetValue(_entity, _previousValue);
	}

	public override void Redo()
	{
		Execute();
	}
}
