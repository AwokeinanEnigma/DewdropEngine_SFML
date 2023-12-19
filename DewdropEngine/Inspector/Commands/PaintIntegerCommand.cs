using DewDrop.Entities;
using System.Reflection;

public class PaintIntegerCommand : ICommand
{
	private readonly MemberInfo _member;
	private readonly int _value;
	private readonly Entity _entity;
	private int _previousValue;

	public PaintIntegerCommand(MemberInfo member, int value, Entity entity)
	{
		_member = member;
		_value = value;
		_entity = entity;
	}

	public void Execute()
	{
		// Save the previous state for undo
		_previousValue = (int)GetValue(_entity);

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

	private void SetValue(Entity entity, int value)
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
