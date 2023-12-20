using DewDrop.Entities;
using System.Reflection;

namespace DewDrop.Inspector.Commands; 

public abstract class InspectorCommand : ICommand {
	protected MemberInfo _member;
	protected Entity _entity;
	
	protected void SetValue (Entity entity, object value) {
		switch (_member) {
		case PropertyInfo property:
			property.SetValue(entity, value);
			break;
		case FieldInfo field:
			field.SetValue(entity, value);
			break;
		}
	}

	protected object GetValue (Entity entity) {
		return _member switch {
			PropertyInfo property => property.GetValue(entity),
			FieldInfo field => field.GetValue(entity),
			_ => null
		};
	}
	public abstract void Execute ();
	public abstract void Undo ();
	public abstract void Redo ();
}
