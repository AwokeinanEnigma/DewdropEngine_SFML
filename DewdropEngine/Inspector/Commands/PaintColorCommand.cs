using DewDrop.Entities;
using SFML.Graphics;
using System.Reflection;

public class PaintColorCommand : ICommand
{
	private readonly MemberInfo _member;
	private readonly Color _color;
	private readonly Entity _entity;
	private Color _previousColor;

	public PaintColorCommand(MemberInfo member, Color color, Entity entity)
	{
		_member = member;
		_color = color;
		_entity = entity;
	}

	public void Execute()
	{
		// Save the previous state for undo
		_previousColor = (Color)GetValue(_entity);

		// Execute the action
		SetValue(_entity, _color);
	}

	public void Undo()
	{
		// Revert the action
		SetValue(_entity, _previousColor);
	}

	public void Redo()
	{
		// Reapply the action
		SetValue(_entity, _color);
	}

	private void SetValue(Entity entity, Color value)
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
