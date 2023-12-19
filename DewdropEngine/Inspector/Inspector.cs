using DewDrop.Collision;
using DewDrop.Entities;
using DewDrop.GUI;
using DewDrop.UserInput;
using DewDrop.Utilities;
using ImGuiNET;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata;
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
	static List<string> Blacklist = new() {
		"Position"
	};
	static List<Type> supportedTypes = new() {
		typeof(int),
		typeof(float),
		typeof(bool),
		typeof(string),
		typeof(Color),
		typeof(Vector2),
		typeof(Enum)
	};
	CommandHistory _commandHistory = new CommandHistory();
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
		_entityManager = entityManager;
		_collisionManager = collisionManager;
		_clock = new Clock();
		Input.OnMouseClick += OnMouseClick;
		Engine.OnRenderImGui += Paint;
	}
	public void OnMouseClick (object? sender, MouseButtonEventArgs e) {
		foreach (var entity in _collisionManager.ObjectsAtPosition(Input.GetMouseWorldPosition())) {
			Entity localEntity = _entityManager.Find(x => x == entity);
			if (localEntity != null) {
				if (_selectedEntity != null && _selectedEntity != localEntity) {
					_selectedEntity = localEntity;
					_fields = localEntity.GetType().GetFields();
					_methods = localEntity.GetType().GetMethods();
					_properties = localEntity.GetType().GetProperties();
					if (!_eMd.ContainsKey(localEntity)) {
						_eMd.Add(localEntity, new());
					}
					if (!_aMp.ContainsKey(localEntity)) {
						_aMp.Add(localEntity, new());
					}
				} else if (_selectedEntity == null) {
					_selectedEntity = localEntity;
					_fields = localEntity.GetType().GetFields();
					_methods = localEntity.GetType().GetMethods();
					_properties = localEntity.GetType().GetProperties();
					if (!_eMd.ContainsKey(localEntity)) {
						_eMd.Add(localEntity, new());
					}
					if (!_aMp.ContainsKey(localEntity)) {
						_aMp.Add(localEntity, new());
					}
				}
			}
		}
	}

	/// <summary>
	/// Renders the Inspector user interface.
	/// </summary>
	public void Paint () {
		if (_selectedEntity != null) {
			if (Keyboard.IsKeyPressed(Keyboard.Key.LControl) || Keyboard.IsKeyPressed(Keyboard.Key.RControl)) {
				if (Keyboard.IsKeyPressed(Keyboard.Key.Z) && _clock.ElapsedTime.AsMilliseconds() > 300) {
					_clock.Restart();
					_commandHistory.Undo();
				}

				// Check if Ctrl + Y is pressed for Redo
				if (Keyboard.IsKeyPressed(Keyboard.Key.Y) && _clock.ElapsedTime.AsMilliseconds() > 300) {
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
					_selectedEntity.Position = vector2;
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
			ImGui.End();
		}
	}
		void PaintField (FieldInfo field) {
		object value = field.GetValue(_selectedEntity);
		// Daily reminder never to use GetType() in a loop. It may not cause a System Access Violation, but it'll still stall the program and crash it.
		if (Blacklist.Contains(field.Name)) {
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
		if (value is Utilities.Vector2 vector2) {
			PaintVector2(field, vector2, _selectedEntity);
		}
		if (value is IList list) {
			
			PaintList(field, list, _selectedEntity);
		}
	}
	void PaintProperty (PropertyInfo property) {
		// Don't use GetType() here, it'll cause a System Access Violation.
		object value = property.GetValue(_selectedEntity);
		bool canWrite = property.GetSetMethod() != null;
		if (!canWrite || Blacklist.Contains(property.Name)) {
			return;
		}
		if (value is Color color) {
			PaintColor(property, color, _selectedEntity);
		}
		if (value is int integer) {
			PaintIntegers(property, integer, _selectedEntity);
		}
		if (value is float floatValue) {
			PaintFloat(property, floatValue, _selectedEntity);
		}
		if (value is bool boolean) {
			PaintBool(property, boolean, _selectedEntity);
		}
		if (value is string str) {
			PaintString(property, str, _selectedEntity);
		}
		if (value is Enum enumValue) {
			PaintEnum(property, enumValue, _selectedEntity);
		}
		if (value is Utilities.Vector2 vector2) {
			PaintVector2(property, vector2, _selectedEntity);
		}
		// check if list contains a supported type
		if (value is IList list) {
			Outer.Log($"Painting list {property.Name}");
			PaintList(property, list, _selectedEntity);
		}
	}
	void PaintMethod (MethodInfo info) {
		ButtonMethodAttribute button = info.GetCustomAttribute<ButtonMethodAttribute>();
		if (button != null) {
			PaintTooltip(info);
			ParameterInfo[] parameters = info.GetParameters();
			AssociatedMethodParameter aMP = _aMp[_selectedEntity].Find(x => x.Method == info);
			if (aMP.Method == null) {
				Outer.Log($"AMP for method '{info.Name}' not found, creating new one");
				aMP.Method = info;
				aMP.Parameters = new object[parameters.Length];
				for (int i = 0; i < parameters.Length; i++) {
					if (parameters[i].ParameterType == typeof(int)) {
						aMP.Parameters[i] = 0;
					}
					if (parameters[i].ParameterType == typeof(float)) {
						aMP.Parameters[i] = 0f;
					}
					if (parameters[i].ParameterType == typeof(bool)) {
						aMP.Parameters[i] = false;
					}
					if (parameters[i].ParameterType == typeof(string)) {
						aMP.Parameters[i] = "default";
					}
					if (parameters[i].ParameterType == typeof(Color)) {
						aMP.Parameters[i] = Color.White;
					}
					if (parameters[i].ParameterType.IsEnum) {
						aMP.Parameters[i] = Enum.GetValues(parameters[i].ParameterType).GetValue(0);
					}
					if (parameters[i].ParameterType == typeof(Vector2)) {
						aMP.Parameters[i] = Vector2.Zero;
					}
				}
				_aMp[_selectedEntity].Add(aMP);
			}
			if (parameters.Length > 0) {
				// go through each parameter and display it
				// also check if the parameter is null, if it is, set it to the default value
				for (int i = 0; i < parameters.Length; i++) {
					ParameterInfo parameter = parameters[i];
					if (parameter.ParameterType == typeof(int)) {
						int value = (int)aMP.Parameters[i];
						if (ImGui.InputInt("Parameter: " + parameter.Name, ref value)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMP.Parameters, value, i));
						}
					}
					if (parameter.ParameterType == typeof(float)) {
						float value = (float)aMP.Parameters[i];
						if (ImGui.InputFloat("Parameter: " + parameter.Name, ref value)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMP.Parameters, value, i));
						}
					}
					if (parameter.ParameterType == typeof(bool)) {
						bool value = (bool)aMP.Parameters[i];
						if (ImGui.Checkbox("Parameter: " + parameter.Name, ref value)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMP.Parameters, value, i));
						}
					}
					if (parameter.ParameterType == typeof(string)) {
						string value = (string)aMP.Parameters[i];
						if (ImGui.InputText( "Parameter: " + parameter.Name, ref value, 100)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMP.Parameters, value, i));
						}
					}
					if (parameter.ParameterType == typeof(Color)) {
						Color value = (Color)aMP.Parameters[i];
						Vector4 numericalColor = ToNumericVector4(value);
						if (ImGui.ColorPicker4("Parameter: " + parameter.Name, ref numericalColor)) {
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMP.Parameters, ToSFMLColor(numericalColor), i));
						}
					}
					if (parameter.ParameterType.IsEnum) {
						AssociatedEnumData aEd = _eMd[_selectedEntity].Find(x => x.EnumName == parameter.ParameterType.Name);
						object enumValue = aMP.Parameters[i];
						if (aEd.EnumName == null) {
							aEd.EnumOptions = Enum.GetNames(enumValue.GetType());
							aEd.EnumName = enumValue.GetType().Name;
							_eMd[_selectedEntity].Add(aEd);
						}
						var index = Array.IndexOf(aEd.EnumOptions, enumValue.ToString());
						if (ImGui.Combo("Parameter: "+ aEd.EnumName, ref index, aEd.EnumOptions, aEd.EnumOptions.Length)) {
							enumValue = Enum.Parse(enumValue.GetType(), aEd.EnumOptions[index]);
							Outer.Log($"Setting enum value to {enumValue}");
							_commandHistory.ExecuteCommand(new SetParameterValueCommand(aMP.Parameters, enumValue, i));
						}
					}
					if (parameter.ParameterType == typeof(Vector2)) {
						Vector2 value = (Vector2)aMP.Parameters[i];
						var vector2 = (System.Numerics.Vector2)value;
						if (ImGui.InputFloat2("Parameter: " + parameter.Name, ref vector2)) {
							aMP.Parameters[i] = (Vector2)vector2;
						}
					}
				}
				if (ImGui.Button(button.MethodName)) {
					Outer.Log($"Invoking method '{info.Name}'");
					info.Invoke(_selectedEntity, aMP.Parameters);
				}
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
		if (!supportedTypes.Contains(elementType)) {
			return;
		}
		//	ImGui.Text(info.Name + (isArray ? " : Array" : " : List" ));
		PaintTooltip(info);
		if (ImGui.CollapsingHeader($"{info.Name} [{list.Count}]###{info.Name}", ImGuiTreeNodeFlags.FramePadding)) {
			ImGui.Indent();

			if (!isArray) {
				if (ImGui.Button("Add Element : " + info.Name)) {
					if (elementType == typeof(string)) {
						_commandHistory.ExecuteCommand(new AddPaintListCommand(list, ""));
					} else {
						_commandHistory.ExecuteCommand(new AddPaintListCommand(list, Activator.CreateInstance(elementType)));
					}
				}

				ImGui.SameLine(ImGui.GetWindowWidth() - ImGui.GetItemRectSize().X -
					ImGui.GetStyle().ItemInnerSpacing.X - ImGui.GetStyle().IndentSpacing);


				if (ImGui.Button("Clear Data: " + info.Name)) {
					_commandHistory.ExecuteCommand(new ClearPaintListCommand(list));
				}
			}
			ImGui.PushItemWidth(-ImGui.GetStyle().IndentSpacing);
			for (var i = 0; i < list.Count; i++) {
				if (elementType == typeof(int)) {
					int value = (int)(list[i] ?? 0);
					if (ImGui.DragInt($"{i} : {info.Name}", ref value)) {
						_commandHistory.ExecuteCommand(new SetListValuePaintListCommand(list, value, i));
					}
				}
				else if (elementType == typeof(float)) {
					float value = (float)(list[i] ?? 0);
					if (ImGui.DragFloat($"{i} : {info.Name}", ref value)) 
					{
						_commandHistory.ExecuteCommand(new SetListValuePaintListCommand(list, value, i));
					}
				}
				else if (elementType == typeof(string)) {
					string value = (string)(list[i] ?? string.Empty);
					if (ImGui.InputText($"{i} : {info.Name}", ref value, 200))
					{
						_commandHistory.ExecuteCommand(new SetListValuePaintListCommand(list, value, i));
					}
				}
				else if (elementType == typeof(Vector2)) {
					Vector2 value = (Vector2)(list[i] ?? Vector2.Zero);
					var vector2 = (System.Numerics.Vector2)value;
					if (ImGui.DragFloat2($"{i} : {info.Name}", ref vector2)) 					{
						_commandHistory.ExecuteCommand(new SetListValuePaintListCommand(list, (Vector2)vector2, i));
					}
				} 
				else if (elementType == typeof(Color)) {
					Color value = (Color)(list[i] ?? Color.White);
					Vector4 numericalColor = ToNumericVector4(value);
					if (ImGui.ColorPicker4($"{i} : {info.Name}", ref numericalColor)) {
						_commandHistory.ExecuteCommand(new SetListValuePaintListCommand(list, ToSFMLColor(numericalColor), i));
					}
				}
			}

			ImGui.PopItemWidth();
			ImGui.Unindent();
		}
	}
	void PaintFloat (MemberInfo info, float floatValue, Entity entity) {
		ImGui.Text(info.Name + " : Float");
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
	void PaintColor (MemberInfo info, Color color, Entity entity) {
		ImGui.Text(info.Name + " : Color");
		Vector4 numericalColor = ToNumericVector4(color);
		PaintTooltip(info);
		if (ImGui.ColorPicker4(info.Name, ref numericalColor)) {
			_commandHistory.ExecuteCommand(new PaintColorCommand(info, color, entity));
		}
	}
	void PaintIntegers (MemberInfo info, int integer, Entity entity) {
		;
		ImGui.Text(info.Name + " : Integer");
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
	void PaintString (MemberInfo info, string str, Entity entity) {
		ImGui.Text(info.Name + " : String");
		PaintTooltip(info);
		if (ImGui.InputText(info.Name, ref str, 100)) {
			_commandHistory.ExecuteCommand(new PaintStringCommand( info, str, entity));
		}
	}
	void PaintBool (MemberInfo info, bool boolean, Entity entity) {
		ImGui.Text(info.Name + " : Boolean");
		PaintTooltip(info);
		if (ImGui.Checkbox(info.Name, ref boolean)) {
			_commandHistory.ExecuteCommand(new PaintBoolCommand(info, boolean, entity));
		}
	}
	void PaintEnum (MemberInfo info, object enumValue, Entity entity) {
		ImGui.Text(info.Name + " : Enum");
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
	void PaintVector2 (MemberInfo info, Utilities.Vector2 vector, Entity entity) {
		ImGui.Text(info.Name + " : Vector2");
		PaintTooltip(info);
		var vector2 = (System.Numerics.Vector2)vector;
		if (ImGui.InputFloat2(info.Name, ref vector2)) {
			_commandHistory.ExecuteCommand(new PaintVector2Command( info, (Utilities.Vector2)vector2, entity));
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
	public static Vector4 ToNumericVector4 (Color self) => new Vector4(self.R/255.0f, self.G/255.0f, self.B/255.0f, self.A/255.0f);
	public static Color ToSFMLColor (Vector4 self) => new Color((byte)(self.X*255f), (byte)(self.Y*255f), (byte)(self.Z*255f), (byte)(self.W*255f));
	/// <summary>
	/// Disposes the Inspector, cleaning up any resources it is using.
	/// </summary>
	public void Dispose () {
		Input.OnMouseClick -= OnMouseClick;
		Engine.OnRenderImGui -= Paint;
		_selectedEntity = null;
		_fields = null;
	}
}
