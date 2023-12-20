using DewDrop.Entities;
using SFML.Graphics;
using System.Reflection;

namespace DewDrop.Inspector.Commands; 

public class PaintColorCommand : InspectorCommand
{
	 readonly Color _color;
	 Color _previousColor;

	public PaintColorCommand(MemberInfo member, Color color, Entity entity)
	{
		_member = member;
		_color = color;
		_entity = entity;
	}

	public override void Execute()
	{
		// Save the previous state for undo
		_previousColor = (Color)GetValue(_entity);

		// Execute the action
		SetValue(_entity, _color);
	}

	public override void Undo()
	{
		// Revert the action
		SetValue(_entity, _previousColor);
	}

	public override void Redo()
	{
		// Reapply the action
		SetValue(_entity, _color);
	}
}
