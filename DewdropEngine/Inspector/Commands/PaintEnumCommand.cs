using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class PaintEnumCommand : ICommand
{
	private MemberInfo _memberInfo;
	private object _entity;
	private object _previousValue;
	private object _newValue;

	public PaintEnumCommand(MemberInfo memberInfo, object newValue, object entity)
	{
		_memberInfo = memberInfo;
		_newValue = newValue;
		_entity = entity;
		_previousValue = GetValue(_memberInfo, _entity);
	}

	public void Execute()
	{
		SetValue(_memberInfo, _entity, _newValue);
	}

	public void Undo()
	{
		SetValue(_memberInfo, _entity, _previousValue);
	}

	public void Redo()
	{
		Execute();
	}

	private object GetValue(MemberInfo memberInfo, object entity)
	{
		switch (memberInfo)
		{
		case FieldInfo fieldInfo:
			return fieldInfo.GetValue(entity);
		case PropertyInfo propertyInfo:
			return propertyInfo.GetValue(entity);
		default:
			throw new ArgumentException("Invalid member type");
		}
	}

	private void SetValue(MemberInfo memberInfo, object entity, object value)
	{
		switch (memberInfo)
		{
		case FieldInfo fieldInfo:
			fieldInfo.SetValue(entity, value);
			break;
		case PropertyInfo propertyInfo:
			propertyInfo.SetValue(entity, value);
			break;
		default:
			throw new ArgumentException("Invalid member type");
		}
	}
}
