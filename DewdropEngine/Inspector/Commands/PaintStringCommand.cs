using DewDrop.Entities;
using System.Reflection;

public class PaintStringCommand : ICommand
{
	private readonly MemberInfo _member;
	private readonly string _str;
	private readonly Entity _entity;
	private string _previousStr;

	public PaintStringCommand(MemberInfo member, string str, Entity entity)
	{
		_member = member;
		_str = str;
		_entity = entity;
	}

	public void Execute()
	{
		// Save the previous state for undo
		_previousStr = (string)GetValue(_entity);

		// Execute the action
		SetValue(_entity, _str);
	}

	public void Undo()
	{
		// Revert the action
		SetValue(_entity, _previousStr);
	}

	public void Redo()
	{
		// Reapply the action
		SetValue(_entity, _str);
	}

	private void SetValue(Entity entity, string value)
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
