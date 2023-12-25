using DewDrop.Collision;
using DewDrop.Inspector.Attributes;
using DewDrop.Inspector.Commands;
using DewDrop.Internal;
using DewDrop.UserInput;
using DewDrop.Utilities;
using ImGuiNET;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections;
using System.Numerics;
using System.Reflection;
using Vector2 = DewDrop.Utilities.Vector2;

namespace DewDrop.Inspector;

/// <summary>
/// The Inspector class is responsible for providing a user interface for inspecting and modifying entity properties in real-time.
/// </summary>
/// <remarks>
/// This class uses ImGui for rendering the user interface and provides functionality for inspecting and modifying various types of properties including integers, floats, booleans, strings, colors, enums, and vectors.
/// It also supports inspecting and modifying properties of lists and arrays.
/// </remarks>
public class Inspector : IDisposable {
	CollisionManager _collisionManager;
	GameObject _selectedGameobject;
	FieldInfo[] _fields;
	MethodInfo[] _methods;
	PropertyInfo[] _properties;
	// needed so that we can store the parameters for a method
	// if this didn't exist, the parameter values would be lost the millisecond after the player clicked off of a imgui button
	Dictionary<Component, List<AssociatedEnumData>> _eMd;
	Dictionary<Component, List<AssociatedMethodParameter>> _aMp;
	// this is a struct that holds the parameters and name for a method
	struct AssociatedMethodParameter {
		public object[] Parameters;
		public MethodInfo Method;
	}
	static readonly List<string> _Blacklist = new List<string> {
		"Position"
	};
	// Contains all the types that the inspector supports
	static readonly List<Type> _SupportedTypes = new List<Type> {
		typeof(int),
		typeof(float),
		typeof(bool),
		typeof(string),
		typeof(Color),
		typeof(Vector2),
		typeof(Enum)
	};
	CommandHistory _commandHistory = new CommandHistory();
	List<ICustomPainter> _customPainters;
	struct AssociatedEnumData {
		public string[] EnumOptions;
		public string EnumName;
	}
	Dictionary<Component, FieldInfo[]> _fieldInfo;
	Dictionary<Component, PropertyInfo[]> _propertyInfo;
	Dictionary<Component, MethodInfo[]> _methodInfo;
	Dictionary<Component, Type> _componentType;
	Component[] _components;
	Clock _clock;
	/// <summary>
	/// Initializes the Inspector with the given EntityManager and CollisionManager.
	/// </summary>
	/// <param name="entityManager">The EntityManager to use for entity management.</param>
	/// <param name="collisionManager">The CollisionManager to use for collision detection.</param>
	public void Initialize (CollisionManager collisionManager) {
		_aMp = new();
		_eMd = new();
		_customPainters = new List<ICustomPainter>();
		_collisionManager = collisionManager;
		_clock = new Clock();
		
		_methodInfo = new Dictionary<Component, MethodInfo[]>();
		_fieldInfo = new Dictionary<Component, FieldInfo[]>();
		_propertyInfo = new Dictionary<Component, PropertyInfo[]>();
		_componentType = new Dictionary<Component, Type>();
		
		Input.OnMouseClick += OnMouseClick;
		Engine.OnRenderImGui += Paint;

	}

	/// <summary>
	/// Adds a custom painter to the Inspector.
	/// </summary>
	/// <param name="painter">The CustomPainter object to be added to the Inspector.</param>
	/// <remarks>
	/// This method allows the addition of custom painters to the Inspector. Custom painters are used to customize the way certain fields or properties are displayed and edited in the Inspector. Once added, the custom painter will be used whenever a field or property of the type it supports is encountered.
	/// </remarks>
	public void AddCustomPainter (ICustomPainter painter) {
		if (painter == null) {
			throw new ArgumentNullException(nameof(painter));
		}
		if (_customPainters.Contains(painter)) {
			throw new ArgumentException("This painter has already been added to the inspector.", nameof(painter));
		}
		if (painter.Type == null) {
			throw new ArgumentException("The painter's type cannot be null.", nameof(painter));
		}
		_customPainters.Add(painter);
	}

	public void Select (GameObject gameObject) {
		_selectedGameobject = gameObject;
		_components = gameObject.ComponentHolder.GetComponentsArray();
	}
	void OnMouseClick (object? sender, MouseButtonEventArgs e) {
		/*foreach (var entity in _collisionManager.ObjectsAtPosition(Input.GetMouseWorldPosition())) {
			Component localEntity = null;
			; //_entityManager.Find(x => x == entity);
			if (localEntity != null) {
				if (_selectedEntity != null && _selectedEntity != localEntity) {
					_selectedEntity = localEntity;
					_fields = localEntity.GetType().GetFields();
					_methods = localEntity.GetType().GetMethods();
					_properties = localEntity.GetType().GetProperties();

					_eMd.TryAdd(localEntity, new List<AssociatedEnumData>());
					_aMp.TryAdd(localEntity, new List<AssociatedMethodParameter>());
				} else if (_selectedEntity == null) {
					_selectedEntity = localEntity;
					_fields = localEntity.GetType().GetFields();
					_methods = localEntity.GetType().GetMethods();
					_properties = localEntity.GetType().GetProperties();

					_eMd.TryAdd(localEntity, new List<AssociatedEnumData>());
					_aMp.TryAdd(localEntity, new List<AssociatedMethodParameter>());
				}
			}
		}*/
	}

	//Component _selectedComponent; 
	/// <summary>
	/// Renders the Inspector user interface.
	/// </summary>
	void Paint () {
		if (_selectedGameobject != null) {
			if (Keyboard.IsKeyPressed(Keyboard.Key.LControl) || Keyboard.IsKeyPressed(Keyboard.Key.RControl)) {
				if (Keyboard.IsKeyPressed(Keyboard.Key.Z) && _clock.ElapsedTime.AsMilliseconds() > 150) {
					_clock.Restart();
					_commandHistory.Undo();
				}

				// Check if Ctrl + Y is pressed for Redo
				if (Keyboard.IsKeyPressed(Keyboard.Key.Y) && _clock.ElapsedTime.AsMilliseconds() > 150) {
					_clock.Restart();
					_commandHistory.Redo();
				}
			}

			ImGui.Begin("Inspector");
			if (ImGui.CollapsingHeader("Transform")) {
				ImGui.Text($"Position: {_selectedGameobject.Transform.Position}");
				ImGui.Text($"Rotation: {_selectedGameobject.Transform.Rotation}");
				ImGui.Text($"Size: {_selectedGameobject.Transform.Size}");
				bool visible = _selectedGameobject.Active;
				if (ImGui.Checkbox("Active", ref visible)) {
					_selectedGameobject.Active = visible;
				}
				ImGui.Text($"Can Have Children: {_selectedGameobject.Transform.CanHaveChildren}");
				ImGui.Text($"Update Slot: {_selectedGameobject.UpdateSlot}");
				ImGui.Text($"Parent: {_selectedGameobject.Transform.Parent?.GameObject.Name ?? "None"}");
			}
			ImGui.Separator();
			foreach (Component component in _components) {
				if (component == null) {
					continue;
				}


				Type type = null;
				bool cache = false;
				if (!_componentType.ContainsKey(component)) {
					type = component.GetType();
					Outer.Log("Caching component info for " + type.Name);
					_componentType.Add(component, type);
					cache = true;
				} else {
					type = _componentType[component];
				} 
					
				if (cache) {
					Outer.Log("Cached component info for " + type.Name);
					_componentType.TryAdd(component, type);
					_fieldInfo.TryAdd(component, type.GetFields());
					_propertyInfo.TryAdd(component, type.GetProperties());
					_methodInfo.TryAdd(component, type.GetMethods());
					_eMd.TryAdd(component, new List<AssociatedEnumData>());
					_aMp.TryAdd(component, new List<AssociatedMethodParameter>());
				}
				if (ImGui.CollapsingHeader("Component: " + type.Name)) {
					_fields = _fieldInfo[component];
					_methods = _methodInfo[component];
					_properties = _propertyInfo[component];
					
					ImGui.Indent(5);
					if (ImGui.CollapsingHeader("Fields: " + type.Name)) {
						// go through each field and draw it
						ImGui.Indent(5);
						foreach (var field in _fields) {
							PaintField(field, component);
						}
						if (_fields.Length == 0) {
							ImGui.Text("No fields :(");
						}
						ImGui.Unindent(5);
					}
					ImGui.Separator();
					if (ImGui.CollapsingHeader("Properties: " + type.Name)) {
						// go through each property and draw it
						ImGui.Indent(5);
						foreach (var property in _properties) {
							PaintProperty(property, component);
						}
						if (_properties.Length == 0) {
							ImGui.Text("No properties :(");
						}
						ImGui.Unindent(5);
					}
					ImGui.Separator();
					if (ImGui.CollapsingHeader("Methods: " + type.Name)) {
						// go through each method and draw it
						ImGui.Indent(5);
						foreach (var method in _methods) {
							PaintMethod(method, component);
						}
						if (_methods.Any(x => x.GetCustomAttribute<ButtonMethodAttribute>() != null) == false) {
							ImGui.Text("No methods :(");
						}
						ImGui.Unindent(5);
					}
					ImGui.Unindent(5);
				}
				ImGui.Separator();
	
			}
			if (ImGui.CollapsingHeader("Inspector Info ")) {
				ImGui.Indent(5);
				if (ImGui.CollapsingHeader("Command History")) {
					// start at 1 so it makes sense to the user
					int i = 1;
					foreach (var command in _commandHistory.UndoStack) {
						ImGui.Text($"Command {i}: {command}");
						i++;
					}
					if (_commandHistory.UndoStack.Count == 0) {
						ImGui.Text("No undo commands have been added :(");
					}
				}
				if (ImGui.CollapsingHeader("Custom Painters")) {
					// go through each custom painter and draw it
					foreach (var painter in _customPainters) {
						ImGui.Text(painter.ToString());
					}
					if (_customPainters.Count == 0) {
						ImGui.Text("No custom painters have been added :(");
					}
				}
				ImGui.Unindent(5);
			}

			ImGui.End();
		}
	}
	void PaintField (FieldInfo field, Component component) {
		object value = field.GetValue(component);
		// Daily reminder never to use GetType() in a loop. It may not cause a System Access Violation, but it'll still stall the program and crash it.
		if (_Blacklist.Contains(field.Name)) {
			return;
		}
		switch (value) {
		case Color color: PaintColor(field, color, component);
			break;
		case int integer: PaintIntegers(field, integer, component);
			break;
		case float floatValue: PaintFloat(field, floatValue, component);
			break;
		case bool boolean: PaintBool(field, boolean, component);
			break;
		case string str: PaintString(field, str, component);
			break;
		case Enum: PaintEnum(field, value, component);
			break;
		case Vector2 vector2: PaintVector2(field, vector2, component);
			break;
		case IList list: PaintList(field, list, component);
			break;
		}
		if (_customPainters.Count > 0) {
			foreach (var painter in _customPainters) {
				if (painter.Type == field.FieldType) {
					painter.PaintField(field, value, component, this, _commandHistory);
				}
			}
		}
	}
	void PaintProperty (PropertyInfo property, Component component) {
		// Don't use GetType() here, it'll cause a System Access Violation.
		object value = property.GetValue(component);
		bool canWrite = property.GetSetMethod() != null;
		if (_Blacklist.Contains(property.Name)) {
			return;
		}
		switch (value) {
		case Color color:
			PaintColor(property, color, component, canWrite);
			break;
		case int integer:
			PaintIntegers(property, integer, component, canWrite);
			break;
		case float floatValue:
			PaintFloat(property, floatValue, component, canWrite);
			break;
		case bool boolean:
			PaintBool(property, boolean, component, canWrite);
			break;
		case string str:
			PaintString(property, str, component, canWrite);
			break;
		case Enum enumValue:
			PaintEnum(property, enumValue, component, canWrite);
			break;
		case Vector2 vector2:
			PaintVector2(property, vector2, component, canWrite);
			break;
		case IList list:
			PaintList(property, list, component);
			break;
		}
		if (_customPainters.Count > 0) {
			foreach (var painter in _customPainters) {
				if (painter.Type == property.PropertyType) {
					painter.PaintProperty(property, value, component, this, _commandHistory);
				}
			}
		}
		//ImGui.Separator();
	}
	void PaintMethod (MethodInfo info, Component component) {
		ButtonMethodAttribute button = info.GetCustomAttribute<ButtonMethodAttribute>();
		if (button != null) {
			PaintTooltip(info);
			ParameterInfo[] parameters = info.GetParameters();
			AssociatedMethodParameter aMp = _aMp[component].Find(x => x.Method == info);
			if (aMp.Method == null) {
				Outer.Log($"AMP for method '{info.Name}' not found, creating new one");
				aMp.Method = info;
				aMp.Parameters = new object[parameters.Length];
				for (int i = 0; i < parameters.Length; i++) {
					if (parameters[i].ParameterType == typeof(int)) {
						aMp.Parameters[i] = 0;
					}
					if (parameters[i].ParameterType == typeof(float)) {
						aMp.Parameters[i] = 0f;
					}
					if (parameters[i].ParameterType == typeof(bool)) {
						aMp.Parameters[i] = false;
					}
					if (parameters[i].ParameterType == typeof(string)) {
						aMp.Parameters[i] = "default";
					}
					if (parameters[i].ParameterType == typeof(Color)) {
						aMp.Parameters[i] = Color.White;
					}
					if (parameters[i].ParameterType.IsEnum) {
						aMp.Parameters[i] = Enum.GetValues(parameters[i].ParameterType).GetValue(0);
					}
					if (parameters[i].ParameterType == typeof(Vector2)) {
						aMp.Parameters[i] = Vector2.Zero;
					}
				}
				_aMp[component].Add(aMp);
			}
			if (parameters.Length > 0) {
				// go through each parameter and display it
				// also check if the parameter is null, if it is, set it to the default value
				for (int i = 0; i < parameters.Length; i++) {
					ParameterInfo parameter = parameters[i];
					if (parameter.ParameterType == typeof(int)) {
						int value = (int)aMp.Parameters[i];
						if (ImGui.InputInt("Parameter: " + parameter.Name + " : " + _componentType[component].Name, ref value)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMp.Parameters, value, i));
						}
					}
					if (parameter.ParameterType == typeof(float)) {
						float value = (float)aMp.Parameters[i];
						if (ImGui.InputFloat("Parameter: " + parameter.Name + " : " + _componentType[component].Name, ref value)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMp.Parameters, value, i));
						}
					}
					if (parameter.ParameterType == typeof(bool)) {
						bool value = (bool)aMp.Parameters[i];
						if (ImGui.Checkbox("Parameter: " + parameter.Name + " : " + _componentType[component].Name, ref value)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMp.Parameters, value, i));
						}
					}
					if (parameter.ParameterType == typeof(string)) {
						string value = (string)aMp.Parameters[i];
						if (ImGui.InputText( "Parameter: " + parameter.Name + " : " + _componentType[component].Name, ref value, 100)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMp.Parameters, value, i));
						}
					}
					if (parameter.ParameterType == typeof(Color)) {
						Color value = (Color)aMp.Parameters[i];
						Vector4 numericalColor = ColorHelper.ToNumericVector4(value);
						if (ImGui.ColorPicker4("Parameter: " + parameter.Name + " : " + _componentType[component].Name, ref numericalColor)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMp.Parameters, ColorHelper.ToSfmlColor(numericalColor), i));
						}
					}
					if (parameter.ParameterType.IsEnum) {
						AssociatedEnumData aEd = _eMd[component].Find(x => x.EnumName == parameter.ParameterType.Name);
						object enumValue = aMp.Parameters[i];
						if (aEd.EnumName == null) {
							aEd.EnumOptions = Enum.GetNames(enumValue.GetType());
							aEd.EnumName = enumValue.GetType().Name;
							_eMd[component].Add(aEd);
						}
						var index = Array.IndexOf(aEd.EnumOptions, enumValue.ToString());
						if (ImGui.Combo("Parameter: "+ aEd.EnumName + " : " + _componentType[component].Name, ref index, aEd.EnumOptions, aEd.EnumOptions.Length)) {
							enumValue = Enum.Parse(enumValue.GetType(), aEd.EnumOptions[index]);
							Outer.Log($"Setting enum value to {enumValue}");
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMp.Parameters, enumValue, i));
						}
					}
					if (parameter.ParameterType == typeof(Vector2)) {
						Vector2 value = (Vector2)aMp.Parameters[i];
						var vector2 = (System.Numerics.Vector2)value;
						if (ImGui.InputFloat2("Parameter: " + parameter.Name + " : " + _componentType[component].Name, ref vector2)) {
							aMp.Parameters[i] = (Vector2)vector2;
						}
					}
				}
				if (ImGui.Button(button.MethodName + " : " + _componentType[component].Name)) {
					Outer.Log($"Invoking method '{info.Name}'");
					info.Invoke(component, aMp.Parameters);
				}
				ImGui.Separator();
			}
			// else we have no parameters, so just invoke the method with null params
			else {
				if (ImGui.Button(button.MethodName)) {
					info.Invoke(component, null);
				}
			}
		}
	}
	#region Paint Fields
	void PaintList (MemberInfo info, IList list, Component component) {
		bool isArray = list is Array;
		Type elementType = isArray ? list.GetType().GetElementType() : list.GetType().GetGenericArguments()[0];
		if (!_SupportedTypes.Contains(elementType)) {
			// We don't support this type, so just return.
			return;
		}
		PaintTooltip(info);
		
		// generate header for the list
		if (ImGui.CollapsingHeader($"{info.Name} [{list.Count}]###{info.Name}", ImGuiTreeNodeFlags.FramePadding)) {
			ImGui.Indent();

			if (!isArray) {
				// if it's not an array, add a button to add an element to the list 
				if (ImGui.Button("Add Element : " + info.Name)) {
					// check if our custom painter supports this type
					if (_customPainters.Count > 0) {
						foreach (var painter in _customPainters) {
							if (painter.Type == elementType) {
								painter.AddListElement(list, component, this, _commandHistory);
								return;
							}
						}
					}
					_commandHistory.ExecuteCommand(
						elementType == typeof(string) ? new AddListCommand(list, "") 
							: new AddListCommand(list, Activator.CreateInstance(elementType)));
				}

				ImGui.SameLine(ImGui.GetWindowWidth() - ImGui.GetItemRectSize().X -
					ImGui.GetStyle().ItemInnerSpacing.X - ImGui.GetStyle().IndentSpacing);


				if (ImGui.Button("Clear Data: " + info.Name)) {
					_commandHistory.ExecuteCommand(new ClearListCommand(list));
				}
			}
			ImGui.PushItemWidth(-ImGui.GetStyle().IndentSpacing);
			for (var i = 0; i < list.Count; i++) {
				switch (list[i]) {
				case int value:
					if (ImGui.DragInt($"{i} : {info.Name}" + " : "+ _componentType[component].Name, ref value)) {
						_commandHistory.ExecuteCommand(new SetListValueCommand(list, value, i));
					}
					break;
				case float value:
					if (ImGui.DragFloat($"{i} : {info.Name}" + " : "+ _componentType[component].Name, ref value)) {
						_commandHistory.ExecuteCommand(new SetListValueCommand(list, value, i));
					}
					break;
				case string value:
					if (ImGui.InputText($"{i} : {info.Name}" + " : "+ _componentType[component].Name, ref value, 200)) {
						_commandHistory.ExecuteCommand(new SetListValueCommand(list, value, i));
					}
					break;
				case Vector2 value:
					var vector2 = (System.Numerics.Vector2)value;
					if (ImGui.DragFloat2($"{i} : {info.Name}" + " : "+ _componentType[component].Name, ref vector2)) {
						_commandHistory.ExecuteCommand(new SetListValueCommand(list, (Vector2)vector2, i));
					}
					break;
				case Color value:
					Vector4 numericalColor = ColorHelper.ToNumericVector4(value);
					if (ImGui.ColorPicker4($"{i} : {info.Name}" + " : "+ _componentType[component].Name, ref numericalColor)) {
						_commandHistory.ExecuteCommand(new SetListValueCommand(list, ColorHelper.ToSfmlColor(numericalColor), i));
					}
					break;
				}
			}

			ImGui.PopItemWidth();
			ImGui.Unindent();
		}
	}
	void PaintFloat (MemberInfo info, float floatValue, Component component, bool canWrite = true) {
		ImGui.Text(info.Name + " : Float");
		if (!canWrite) {
			ImGui.Text("Value: " + floatValue);
			ImGui.Text("Can't set this property.");
			return;
		}
		FloatRangeAttribute range = info.GetCustomAttribute<FloatRangeAttribute>();
		PaintTooltip(info);
		if (range != null) {
			if (ImGui.SliderFloat(info.Name + " : "+ _componentType[component].Name, ref floatValue, range.Min, range.Max)) {
				_commandHistory.ExecuteCommand(new PaintFloatCommand(info, floatValue, component));
			}
		} else {
			if (ImGui.InputFloat(info.Name + " : "+ _componentType[component].Name, ref floatValue)) {
				_commandHistory.ExecuteCommand(new PaintFloatCommand(info, floatValue, component));
			}
		}
	}
	void PaintColor (MemberInfo info, Color color, Component component, bool canWrite = true) {
		ImGui.Text(info.Name + " : Color");
		Vector4 numericalColor = ColorHelper.ToNumericVector4(color);
		if (!canWrite) {
			// i don't think there's any way to display a color without using a color picker
			ImGui.ColorPicker4(info.Name + " : "+ _componentType[component].Name, ref numericalColor);
			ImGui.Text("Can't set this property.");
			return;
		}
		PaintTooltip(info);
		if (ImGui.ColorPicker4(info.Name + " : "+ _componentType[component].Name, ref numericalColor)) {
			_commandHistory.ExecuteCommand(new PaintColorCommand(info, ColorHelper.ToSfmlColor(numericalColor), component));
		}
	}
	void PaintIntegers (MemberInfo info, int integer, Component component, bool canWrite = true) {
		ImGui.Text(info.Name + " : Integer");;
		if (!canWrite) {
			ImGui.Text("Value: " + integer);
			ImGui.Text("Can't set this property.");
			return;
		}
		IntegerRangeAttribute range = info.GetCustomAttribute<IntegerRangeAttribute>();
		PaintTooltip(info);
		if (range != null) {
			if (ImGui.SliderInt(info.Name + " : "+ _componentType[component].Name, ref integer, range.Min, range.Max)) {
				_commandHistory.ExecuteCommand(new PaintIntegerCommand(info, integer, component));
			}
		} else {
			if (ImGui.InputInt(info.Name + " : "+ _componentType[component].Name, ref integer)) {
				_commandHistory.ExecuteCommand(new PaintIntegerCommand(info, integer, component));
			}
		}
	}
	void PaintString (MemberInfo info, string str, Component component, bool canWrite = true) {
		ImGui.Text(info.Name + " : String");
		if (!canWrite) {
			ImGui.Text("Value: " + str);
			ImGui.Text("Can't set this property.");
			return;
		}
		PaintTooltip(info);
		if (ImGui.InputText(info.Name + " : "+ _componentType[component].Name, ref str, 100)) {
			_commandHistory.ExecuteCommand(new PaintStringCommand( info, str, component));
		}
	}
	void PaintBool (MemberInfo info, bool boolean, Component component, bool canWrite = true) {
		ImGui.Text(info.Name + " : Boolean");
		if (!canWrite) {
			ImGui.Text("Value: " + boolean);
			ImGui.Text("Can't set this property.");
			return;
		}
		PaintTooltip(info);
		if (ImGui.Checkbox(info.Name + " : "+ _componentType[component].Name, ref boolean)) {
			_commandHistory.ExecuteCommand(new PaintBoolCommand(info, boolean, component));
		}
	}
	void PaintEnum (MemberInfo info, object enumValue, Component component, bool canWrite = true) {
		ImGui.Text(info.Name + " : Enum");
		if (!canWrite) {
			ImGui.Text("Value: " + enumValue);
			ImGui.Text("Can't set this property.");
			return;
		}
		PaintTooltip(info);
		AssociatedEnumData aEd =  _eMd[component].Find(x => x.EnumName == enumValue.GetType().Name);
		if (aEd.EnumName == null) {
			aEd.EnumOptions = Enum.GetNames(enumValue.GetType());
			aEd.EnumName = enumValue.GetType().Name;
			_eMd[component].Add(aEd);
		}
		var index = Array.IndexOf(aEd.EnumOptions, enumValue.ToString());
		if (ImGui.Combo(aEd.EnumName + " : "+ _componentType[component].Name, ref index, aEd.EnumOptions, aEd.EnumOptions.Length)) {
			enumValue = Enum.Parse(enumValue.GetType(), aEd.EnumOptions[index]);
			_commandHistory.ExecuteCommand(new PaintEnumCommand(info, enumValue, component));
			//info.SetValue(entity, enumValue);
		}
	}
	void PaintVector2 (MemberInfo info, Vector2 vector, Component component, bool canWrite = true) {
		ImGui.Text(info.Name + " : Vector2");
		var vector2 = (System.Numerics.Vector2)vector;
		if (!canWrite) {
			// like the color, i don't think there's any way to display a vector without using inputfloat2	
			ImGui.InputFloat2(info.Name + " : "+ _componentType[component].Name, ref vector2);
			ImGui.Text("Can't set this property.");
			return;
		}
		PaintTooltip(info);
		if (ImGui.InputFloat2(info.Name + " : "+ _componentType[component].Name, ref vector2)) {
			_commandHistory.ExecuteCommand(new PaintVector2Command( info, vector2, component));
			//info.SetValue(entity,  (Utilities.Vector2)vector2);
		}
	}
	#endregion

	void PaintTooltip (MemberInfo info) {
		TooltipAttribute tooltip = info.GetCustomAttribute<TooltipAttribute>();
		if (tooltip != null) {
			ImGui.Separator();
			ImGui.Text("TOOLTIP: "+tooltip.Tooltip);
		}
	}
	/// <summary>
	/// Disposes the Inspector, cleaning up any resources it is using.
	/// </summary>
	public void Dispose () {
		Input.OnMouseClick -= OnMouseClick;
		Engine.OnRenderImGui -= Paint;
		_customPainters.ForEach(x => x.Dispose());
		_customPainters = null;
		_selectedGameobject = null;
		_fields = null;
		_methods = null;
		_properties = null;
		_collisionManager = null;
		_clock.Dispose();
		_clock = null;
		//_commandHistory.Dispose();
		_commandHistory = null;
		_aMp = null;
		_eMd = null;
	}
}
