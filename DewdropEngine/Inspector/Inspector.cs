using DewDrop.Collision;
using DewDrop.Entities;
using DewDrop.Inspector.Attributes;
using DewDrop.Inspector.Commands;
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
	EntityManager _entityManager;
	CollisionManager _collisionManager;
	Entity _selectedEntity;
	FieldInfo[] _fields;
	MethodInfo[] _methods;
	PropertyInfo[] _properties;
	// needed so that we can store the parameters for a method
	// if this didn't exist, the parameter values would be lost the millisecond after the player clicked off of a imgui button
	Dictionary<Entity, List<AssociatedEnumData>> _eMd;
	Dictionary<Entity, List<AssociatedMethodParameter>> _aMp;
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
	List<CustomPainter> _customPainters;
	struct AssociatedEnumData {
		public string[] EnumOptions;
		public string EnumName;
	}
	Clock _clock;
	/// <summary>
	/// Initializes the Inspector with the given EntityManager and CollisionManager.
	/// </summary>
	/// <param name="entityManager">The EntityManager to use for entity management.</param>
	/// <param name="collisionManager">The CollisionManager to use for collision detection.</param>
	public void Initialize (EntityManager entityManager, CollisionManager collisionManager) {
		_aMp = new();
		_eMd = new();
		_customPainters =new List<CustomPainter>();
		_entityManager = entityManager;
		_collisionManager = collisionManager;
		_clock = new Clock();
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
	public void AddCustomPainter (CustomPainter painter) {
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
	
	void OnMouseClick (object? sender, MouseButtonEventArgs e) {
		foreach (var entity in _collisionManager.ObjectsAtPosition(Input.GetMouseWorldPosition())) {
			Entity localEntity = _entityManager.Find(x => x == entity);
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
		}
	}

	/// <summary>
	/// Renders the Inspector user interface.
	/// </summary>
	 void Paint () {
		if (_selectedEntity != null) {
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

			ImGui.Begin("Entity Inspector");
			if (ImGui.CollapsingHeader("Entity Data")) {
				ImGui.Text("Selected Entity: " + _selectedEntity.Name);
				ImGui.Text("Position: " + _selectedEntity.Position);
				var vector2 = (System.Numerics.Vector2)_selectedEntity.Position;
				if (ImGui.InputFloat2("Position", ref vector2)) {
					_commandHistory.ExecuteCommand(new PaintVector2Command( _selectedEntity.GetType().GetProperty("Position"), vector2, _selectedEntity));
				}
			}
			ImGui.Separator();
			if (ImGui.CollapsingHeader("Fields")) {
				// go through each field and draw it
				foreach (var field in _fields) {
					PaintField(field);
				}
			}
			ImGui.Separator();
			if (ImGui.CollapsingHeader("Properties")) {
				// go through each property and draw it
				foreach (var property in _properties) {
					PaintProperty(property);
				}
			}
			ImGui.Separator();
			if (ImGui.CollapsingHeader("Methods")) {
				// go through each method and draw it
				foreach (var method in _methods) {
					PaintMethod(method);
				}
			}
			ImGui.Separator();
			if (ImGui.CollapsingHeader("Inspector Info ")) {
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
			}

			ImGui.End();
		}
	}
	void PaintField (FieldInfo field) {
		object value = field.GetValue(_selectedEntity);
		// Daily reminder never to use GetType() in a loop. It may not cause a System Access Violation, but it'll still stall the program and crash it.
		if (_Blacklist.Contains(field.Name)) {
			return;
		}
		if (value is Color color) {
			PaintColor(field, color, _selectedEntity);
		}
		if (value is int integer) {
			PaintIntegers(field, integer, _selectedEntity);
		}
		if (value is float floatValue) {
			PaintFloat(field, floatValue, _selectedEntity);
		}
		if (value is bool boolean) {
			PaintBool(field, boolean, _selectedEntity);
		}
		if (value is string str) {
			PaintString(field, str, _selectedEntity);
		}
		if (value is Enum) {
			PaintEnum(field, value, _selectedEntity);
		}
		if (value is Vector2 vector2) {
			PaintVector2(field, vector2, _selectedEntity);
		}
		if (value is IList list) {
			PaintList(field, list, _selectedEntity);
		}
		if (_customPainters.Count > 0) {
			foreach (var painter in _customPainters) {
				if (painter.Type == field.FieldType) {
					painter.PaintField(field, value, _selectedEntity, this, _commandHistory);
				}
			}
		}
		ImGui.Separator();
	}
	void PaintProperty (PropertyInfo property) {
		// Don't use GetType() here, it'll cause a System Access Violation.
		object value = property.GetValue(_selectedEntity);
		bool canWrite = property.GetSetMethod() != null;
		if (_Blacklist.Contains(property.Name)) {
			return;
		}
		if (value is Color color) {
			PaintColor(property, color, _selectedEntity, canWrite);
		}
		if (value is int integer) {
			PaintIntegers(property, integer, _selectedEntity, canWrite);
		}
		if (value is float floatValue) {
			PaintFloat(property, floatValue, _selectedEntity,canWrite);
		}
		if (value is bool boolean) {
			PaintBool(property, boolean, _selectedEntity, canWrite);
		}
		if (value is string str) {
			PaintString(property, str, _selectedEntity, canWrite);
		}
		if (value is Enum enumValue) {
			PaintEnum(property, enumValue, _selectedEntity, canWrite);
		}
		if (value is Vector2 vector2) {
			PaintVector2(property, vector2, _selectedEntity, canWrite);
		}
		// check if list contains a supported type
		if (value is IList list) {
			PaintList(property, list, _selectedEntity);
		}
		if (_customPainters.Count > 0) {
			foreach (var painter in _customPainters) {
				if (painter.Type == property.PropertyType) {
					painter.PaintProperty(property, value, _selectedEntity, this, _commandHistory);
				}
			}
		}
		ImGui.Separator();
	}
	void PaintMethod (MethodInfo info) {
		ButtonMethodAttribute button = info.GetCustomAttribute<ButtonMethodAttribute>();
		if (button != null) {
			PaintTooltip(info);
			ParameterInfo[] parameters = info.GetParameters();
			AssociatedMethodParameter aMp = _aMp[_selectedEntity].Find(x => x.Method == info);
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
				_aMp[_selectedEntity].Add(aMp);
			}
			if (parameters.Length > 0) {
				// go through each parameter and display it
				// also check if the parameter is null, if it is, set it to the default value
				for (int i = 0; i < parameters.Length; i++) {
					ParameterInfo parameter = parameters[i];
					if (parameter.ParameterType == typeof(int)) {
						int value = (int)aMp.Parameters[i];
						if (ImGui.InputInt("Parameter: " + parameter.Name, ref value)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMp.Parameters, value, i));
						}
					}
					if (parameter.ParameterType == typeof(float)) {
						float value = (float)aMp.Parameters[i];
						if (ImGui.InputFloat("Parameter: " + parameter.Name, ref value)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMp.Parameters, value, i));
						}
					}
					if (parameter.ParameterType == typeof(bool)) {
						bool value = (bool)aMp.Parameters[i];
						if (ImGui.Checkbox("Parameter: " + parameter.Name, ref value)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMp.Parameters, value, i));
						}
					}
					if (parameter.ParameterType == typeof(string)) {
						string value = (string)aMp.Parameters[i];
						if (ImGui.InputText( "Parameter: " + parameter.Name, ref value, 100)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMp.Parameters, value, i));
						}
					}
					if (parameter.ParameterType == typeof(Color)) {
						Color value = (Color)aMp.Parameters[i];
						Vector4 numericalColor = ToNumericVector4(value);
						if (ImGui.ColorPicker4("Parameter: " + parameter.Name, ref numericalColor)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMp.Parameters, ToSfmlColor(numericalColor), i));
						}
					}
					if (parameter.ParameterType.IsEnum) {
						AssociatedEnumData aEd = _eMd[_selectedEntity].Find(x => x.EnumName == parameter.ParameterType.Name);
						object enumValue = aMp.Parameters[i];
						if (aEd.EnumName == null) {
							aEd.EnumOptions = Enum.GetNames(enumValue.GetType());
							aEd.EnumName = enumValue.GetType().Name;
							_eMd[_selectedEntity].Add(aEd);
						}
						var index = Array.IndexOf(aEd.EnumOptions, enumValue.ToString());
						if (ImGui.Combo("Parameter: "+ aEd.EnumName, ref index, aEd.EnumOptions, aEd.EnumOptions.Length)) {
							enumValue = Enum.Parse(enumValue.GetType(), aEd.EnumOptions[index]);
							Outer.Log($"Setting enum value to {enumValue}");
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMp.Parameters, enumValue, i));
						}
					}
					if (parameter.ParameterType == typeof(Vector2)) {
						Vector2 value = (Vector2)aMp.Parameters[i];
						var vector2 = (System.Numerics.Vector2)value;
						if (ImGui.InputFloat2("Parameter: " + parameter.Name, ref vector2)) {
							aMp.Parameters[i] = (Vector2)vector2;
						}
					}
				}
				if (ImGui.Button(button.MethodName)) {
					Outer.Log($"Invoking method '{info.Name}'");
					info.Invoke(_selectedEntity, aMp.Parameters);
				}
				ImGui.Separator();
			}
			// else we have no parameters, so just invoke the method with null params
			else {
				if (ImGui.Button(button.MethodName)) {
					info.Invoke(_selectedEntity, null);
				}
			}
		}
	}
	#region Paint Fields
	void PaintList (MemberInfo info, IList list, Entity entity) {
		bool isArray = list is Array;
		Type elementType = isArray ? list.GetType().GetElementType() : list.GetType().GetGenericArguments()[0];
		if (!_SupportedTypes.Contains(elementType)) {
			return;
		}
		//	ImGui.Text(info.Name + (isArray ? " : Array" : " : List" ));
		PaintTooltip(info);
		if (ImGui.CollapsingHeader($"{info.Name} [{list.Count}]###{info.Name}", ImGuiTreeNodeFlags.FramePadding)) {
			ImGui.Indent();

			if (!isArray) {
				if (ImGui.Button("Add Element : " + info.Name)) {
					// check if our custom painter supports this type
					if (_customPainters.Count > 0) {
						foreach (var painter in _customPainters) {
							if (painter.Type == elementType) {
								painter.AddListElement(list, entity, this, _commandHistory);
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
				if (elementType == typeof(int)) {
					int value = (int)(list[i] ?? 0);
					if (ImGui.DragInt($"{i} : {info.Name}", ref value)) {
						_commandHistory.ExecuteCommand(new SetListValueCommand(list, value, i));
					}
				}
				else if (elementType == typeof(float)) {
					float value = (float)(list[i] ?? 0);
					if (ImGui.DragFloat($"{i} : {info.Name}", ref value)) 
					{
						_commandHistory.ExecuteCommand(new SetListValueCommand(list, value, i));
					}
				}
				else if (elementType == typeof(string)) {
					string value = (string)(list[i] ?? string.Empty);
					if (ImGui.InputText($"{i} : {info.Name}", ref value, 200))
					{
						_commandHistory.ExecuteCommand(new SetListValueCommand(list, value, i));
					}
				}
				else if (elementType == typeof(Vector2)) {
					Vector2 value = (Vector2)(list[i] ?? Vector2.Zero);
					var vector2 = (System.Numerics.Vector2)value;
					if (ImGui.DragFloat2($"{i} : {info.Name}", ref vector2)) 					{
						_commandHistory.ExecuteCommand(new SetListValueCommand(list, (Vector2)vector2, i));
					}
				} 
				else if (elementType == typeof(Color)) {
					Color value = (Color)(list[i] ?? Color.White);
					Vector4 numericalColor = ToNumericVector4(value);
					if (ImGui.ColorPicker4($"{i} : {info.Name}", ref numericalColor)) {
						_commandHistory.ExecuteCommand(new SetListValueCommand(list, ToSfmlColor(numericalColor), i));
					}
				}
				if (_customPainters.Count > 0) {
					foreach (var painter in _customPainters) {
						if (painter.Type == elementType) {
							painter.PaintList(list, i, entity, this, _commandHistory);
						}
					}
				}
			}

			ImGui.PopItemWidth();
			ImGui.Unindent();
		}
	}
	void PaintFloat (MemberInfo info, float floatValue, Entity entity, bool canWrite = true) {
		ImGui.Text(info.Name + " : Float");
		if (!canWrite) {
			ImGui.Text("Value: " + floatValue);
			ImGui.Text("Can't set this property.");
			return;
		}
		FloatRangeAttribute range = info.GetCustomAttribute<FloatRangeAttribute>();
		PaintTooltip(info);
		if (range != null) {
			if (ImGui.SliderFloat(info.Name, ref floatValue, range.Min, range.Max)) {
				_commandHistory.ExecuteCommand(new PaintFloatCommand(info, floatValue, entity));
			}
		} else {
			if (ImGui.InputFloat(info.Name, ref floatValue)) {
				_commandHistory.ExecuteCommand(new PaintFloatCommand(info, floatValue, entity));
			}
		}
	}
	void PaintColor (MemberInfo info, Color color, Entity entity, bool canWrite = true) {
		ImGui.Text(info.Name + " : Color");
		Vector4 numericalColor = ToNumericVector4(color);
		if (!canWrite) {
			// i don't think there's any way to display a color without using a color picker
			ImGui.ColorPicker4(info.Name, ref numericalColor);
			ImGui.Text("Can't set this property.");
			return;
		}
		PaintTooltip(info);
		if (ImGui.ColorPicker4(info.Name, ref numericalColor)) {
			_commandHistory.ExecuteCommand(new PaintColorCommand(info, ToSfmlColor(numericalColor), entity));
		}
	}
	void PaintIntegers (MemberInfo info, int integer, Entity entity, bool canWrite = true) {
		ImGui.Text(info.Name + " : Integer");
		if (!canWrite) {
			ImGui.Text("Value: " + integer);
			ImGui.Text("Can't set this property.");
			return;
		}
		IntegerRangeAttribute range = info.GetCustomAttribute<IntegerRangeAttribute>();
		PaintTooltip(info);
		if (range != null) {
			if (ImGui.SliderInt(info.Name, ref integer, range.Min, range.Max)) {
				_commandHistory.ExecuteCommand(new PaintIntegerCommand(info, integer, entity));
			}
		} else {
			if (ImGui.InputInt(info.Name, ref integer)) {
				_commandHistory.ExecuteCommand(new PaintIntegerCommand(info, integer, entity));
			}
		}
	}
	void PaintString (MemberInfo info, string str, Entity entity, bool canWrite = true) {
		ImGui.Text(info.Name + " : String");
		if (!canWrite) {
			ImGui.Text("Value: " + str);
			ImGui.Text("Can't set this property.");
			return;
		}
		PaintTooltip(info);
		if (ImGui.InputText(info.Name, ref str, 100)) {
			_commandHistory.ExecuteCommand(new PaintStringCommand( info, str, entity));
		}
	}
	void PaintBool (MemberInfo info, bool boolean, Entity entity, bool canWrite = true) {
		ImGui.Text(info.Name + " : Boolean");
		if (!canWrite) {
			ImGui.Text("Value: " + boolean);
			ImGui.Text("Can't set this property.");
			return;
		}
		PaintTooltip(info);
		if (ImGui.Checkbox(info.Name, ref boolean)) {
			_commandHistory.ExecuteCommand(new PaintBoolCommand(info, boolean, entity));
		}
	}
	void PaintEnum (MemberInfo info, object enumValue, Entity entity, bool canWrite = true) {
		ImGui.Text(info.Name + " : Enum");
		if (!canWrite) {
			ImGui.Text("Value: " + enumValue);
			ImGui.Text("Can't set this property.");
			return;
		}
		PaintTooltip(info);
		AssociatedEnumData aEd =  _eMd[entity].Find(x => x.EnumName == enumValue.GetType().Name);
		if (aEd.EnumName == null) {
			aEd.EnumOptions = Enum.GetNames(enumValue.GetType());
			aEd.EnumName = enumValue.GetType().Name;
			_eMd[entity].Add(aEd);
		}
		var index = Array.IndexOf(aEd.EnumOptions, enumValue.ToString());
		if (ImGui.Combo(aEd.EnumName, ref index, aEd.EnumOptions, aEd.EnumOptions.Length)) {
			enumValue = Enum.Parse(enumValue.GetType(), aEd.EnumOptions[index]);
			_commandHistory.ExecuteCommand(new PaintEnumCommand(info, enumValue, entity));
			//info.SetValue(entity, enumValue);
		}
	}
	void PaintVector2 (MemberInfo info, Vector2 vector, Entity entity, bool canWrite = true) {
		ImGui.Text(info.Name + " : Vector2");
		var vector2 = (System.Numerics.Vector2)vector;
		if (!canWrite) {
			// like the color, i don't think there's any way to display a vector without using inputfloat2	
			ImGui.InputFloat2(info.Name, ref vector2);
			ImGui.Text("Can't set this property.");
			return;
		}
		PaintTooltip(info);
		if (ImGui.InputFloat2(info.Name, ref vector2)) {
			_commandHistory.ExecuteCommand(new PaintVector2Command( info, vector2, entity));
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
	static Vector4 ToNumericVector4 (Color self) => new Vector4(self.R/255.0f, self.G/255.0f, self.B/255.0f, self.A/255.0f);
	static Color ToSfmlColor (Vector4 self) => new Color((byte)(self.X*255f), (byte)(self.Y*255f), (byte)(self.Z*255f), (byte)(self.W*255f));
	/// <summary>
	/// Disposes the Inspector, cleaning up any resources it is using.
	/// </summary>
	public void Dispose () {
		Input.OnMouseClick -= OnMouseClick;
		Engine.OnRenderImGui -= Paint;
		_customPainters.ForEach(x => x.Dispose());
		_customPainters = null;
		_selectedEntity = null;
		_fields = null;
		_methods = null;
		_properties = null;
		_entityManager = null;
		_collisionManager = null;
		_clock.Dispose();
		_clock = null;
		//_commandHistory.Dispose();
		_commandHistory = null;
		_aMp = null;
		_eMd = null;
	}
}
