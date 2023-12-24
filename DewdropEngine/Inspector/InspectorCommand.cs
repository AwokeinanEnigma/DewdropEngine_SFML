using DewDrop.Internal;
using System.Reflection;

namespace DewDrop.Inspector.Commands; 

public abstract class InspectorCommand : ICommand {
	protected MemberInfo _member;
	protected Component _entity;
	
	protected void SetValue (Component entity, object value) {
		if (_member is PropertyInfo property) {
			property.SetValue(entity, value);
		} else if (_member is FieldInfo field) {
			field.SetValue(entity, value);
		}
	}

	protected object? GetValue (Component entity) {
		if (_member is PropertyInfo property) {
			return property.GetValue(entity);
		} 
		if (_member is FieldInfo field) {
			return field.GetValue(entity);
		}
		return null;
	}
	public abstract void Execute ();
	public abstract void Undo ();
	public abstract void Redo ();
}
