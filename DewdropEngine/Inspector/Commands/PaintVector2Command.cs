using DewDrop.Entities;
using System.Reflection;
using DewDrop.Utilities;

public class PaintVector2Command : ICommand
{
	private readonly MemberInfo _member;
	private readonly Vector2 _vector;
	private readonly Entity _entity;
	private Vector2 _previousVector;

	public PaintVector2Command(MemberInfo member, Vector2 vector, Entity entity)
	{
		_member = member;
		_vector = vector;
		_entity = entity;
	}

	public void Execute()
	{
		// Save the previous state for undo
		_previousVector = (Vector2)GetValue(_entity);

		// Execute the action
		SetValue(_entity, _vector);
	}

	public void Undo()
	{
		// Revert the action
		SetValue(_entity, _previousVector);
	}

	public void Redo()
	{
		// Reapply the action
		SetValue(_entity, _vector);
	}

	private void SetValue(Entity entity, Vector2 value)
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
