using DewDrop.Entities;
using System.Reflection;

public class PaintFloatCommand : ICommand
{
	private readonly MemberInfo _member;
	private readonly float _value;
	private readonly Entity _entity;
	private float _previousValue;

	public PaintFloatCommand(MemberInfo member, float value, Entity entity)
	{
		_member = member;
		_value = value;
		_entity = entity;
	}

	public void Execute()
	{
		// Save the previous state for undo
		_previousValue = (float)GetValue(_entity);

		// Execute the action
		SetValue(_entity, _value);
	}

	public void Undo()
	{
		// Revert the action
		SetValue(_entity, _previousValue);
	}

	public void Redo()
	{
		// Reapply the action
		SetValue(_entity, _value);
	}

	private void SetValue(Entity entity, float value)
	{
		switch (_member)
		{
		case PropertyInfo property:
			property.SetValue(entity, value);
			break;
		case FieldInfo field:
			field.SetValue(entity, value);
			break;
		}
	}

	private object GetValue(Entity entity)
	{
		return _member switch
		{
			PropertyInfo property => property.GetValue(entity),
			FieldInfo field => field.GetValue(entity),
			_ => null
		};
	}
}
