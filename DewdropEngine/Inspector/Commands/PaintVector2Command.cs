using DewDrop.Entities;
using System.Reflection;
using DewDrop.Utilities;

namespace DewDrop.Inspector.Commands; 

public class PaintVector2Command : InspectorCommand {
	readonly Vector2 _vector;
	Vector2 _previousVector;

	public PaintVector2Command (MemberInfo member, Vector2 vector, Entity entity) {
		_member = member;
		_vector = vector;
		_entity = entity;
	}

	public override void Execute () {
		// Save the previous state for undo
		_previousVector = (Vector2)GetValue(_entity);

		// Execute the action
		SetValue(_entity, _vector);
	}

	public override void Undo () {
		// Revert the action
		SetValue(_entity, _previousVector);
	}

	public override void Redo () {
		// Reapply the action
		SetValue(_entity, _vector);
	}
}