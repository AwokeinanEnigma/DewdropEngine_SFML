using DewDrop.Collision;
using DewDrop.Entities;
using DewDrop.GUI;
using DewDrop.UserInput;
using DewDrop.Utilities;
using ImGuiNET;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata;
namespace DewDrop.Inspector;

public class Inspector : IDisposable{
	EntityManager _entityManager;
	CollisionManager _collisionManager;
	Entity _selectedEntity;
	FieldInfo[] _fields;
	MethodInfo[] _methods;
	public void Initialize (EntityManager entityManager, CollisionManager collisionManager) {
		_entityManager = entityManager;
		_collisionManager = collisionManager;
		Input.OnMouseClick += OnMouseClick;
		Engine.RenderImGUI += Paint;
	}
	public void OnMouseClick (object? sender, MouseButtonEventArgs e) {
		foreach (var entity in _collisionManager.ObjectsAtPosition(Input.GetMouseWorldPosition())) {
			Entity localEntity = _entityManager.Find(x => x == entity);
			if (localEntity != null) {
				Outer.Log(localEntity.Name);
				Outer.Log(localEntity.Position);
				
				_fields = localEntity.GetType().GetFields();
				_methods = localEntity.GetType().GetMethods();
				if (_selectedEntity != null && _selectedEntity != localEntity)
					_selectedEntity = localEntity;
				else if (_selectedEntity == null)
					_selectedEntity = localEntity;
			}
		}
	}

	public void Paint () {
		if (_selectedEntity != null) {
			ImGui.Begin("Entity Inspector");
			//ImGui.ColorPicker4("Color Picker", ref Color);
			ImGui.Text("Selected Entity: " + _selectedEntity.Name);
			ImGui.Text("Position: " + _selectedEntity.Position.ToString());
			// go through each field and display it
			ImGui.Separator();
			ImGui.Text("Fields");
			foreach (var field in _fields) {
				object value = field.GetValue(_selectedEntity);
				if (value is Color color) {
					PaintColor(field, color, _selectedEntity);
					continue;
				}
				if (value is int integer) {
					PaintIntegers(field, integer, _selectedEntity);
					continue;
				}
				if (value is float floatValue) {
					PaintFloat(field, floatValue, _selectedEntity);
					continue;
				}
				if (value is bool boolean) {
					PaintBool(field, boolean, _selectedEntity);
					continue;
				}
			}
			ImGui.Separator();
			ImGui.Text("Methods");
			foreach (var method in _methods) {
				PaintMethod(method, _selectedEntity);
			}
			ImGui.End();
		}
	}

	void PaintFloat (FieldInfo info, float floatValue, Entity entity) {
		ImGui.Text(info.Name + " : Float");
		FloatRangeAttribute range = info.GetCustomAttribute<FloatRangeAttribute>();
		PaintTooltip(info);
		if (range != null) {
			if (ImGui.SliderFloat(info.Name, ref floatValue, range.Min, range.Max)) {
				info.SetValue(entity, floatValue);
			}
		} else {
			if (ImGui.InputFloat(info.Name, ref floatValue)) {
				info.SetValue(entity, floatValue);
			}
		}
	}
	void PaintColor (FieldInfo info, Color color, Entity entity) {
		ImGui.Text(info.Name + " : Color");
		Vector4 numericalColor = ToNumerics(color);
		PaintTooltip(info);
		if (ImGui.ColorPicker4(info.Name, ref numericalColor)) {
			info.SetValue(entity, ToXNAColor(numericalColor));
		}
	}

	void PaintIntegers (FieldInfo info, int integer, Entity entity) {
		;
		ImGui.Text(info.Name + " : Integer");
		IntegerRangeAttribute range = info.GetCustomAttribute<IntegerRangeAttribute>();
		PaintTooltip(info);
		if (range != null) {
			if (ImGui.SliderInt(info.Name, ref integer, range.Min, range.Max)) {
				info.SetValue(entity, integer);
			}
		} else {
			if (ImGui.InputInt(info.Name, ref integer)) {
				info.SetValue(entity, integer);
			}
		}
	}

	void PaintBool (FieldInfo info, bool boolean, Entity entity) {
		ImGui.Text(info.Name + " : Boolean");
		PaintTooltip(info);
		if (ImGui.Checkbox(info.Name, ref boolean)) {
			info.SetValue(entity, boolean);
		}
	}
	
	// needed so that we can store the parameters for a method
	// if this didn't exist, the parameter values would be lost the millisecond after the player clicked off of a imgui button
	List<AssociatedMethodParameter> _aMP = new List<AssociatedMethodParameter>();
	// this is a struct that holds the parameters for a method
	struct AssociatedMethodParameter {
		public object[] Parameters;
		public MethodInfo Method;
	}
	void PaintMethod (MethodInfo info, Entity entity) {
		ButtonMethodAttribute button = info.GetCustomAttribute<ButtonMethodAttribute>();
		if (button != null) {
			PaintTooltip(info);
			ParameterInfo[] parameters = info.GetParameters();
			AssociatedMethodParameter aMP = _aMP.Find(x => x.Method == info);
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
				}
				_aMP.Add(aMP);
			}
			if (parameters.Length > 0) {
				// go through each parameter and display it
				// also check if the parameter is null, if it is, set it to the default value
				for (int i = 0; i < parameters.Length; i++) {
					ParameterInfo parameter = parameters[i];
					if (parameter.ParameterType == typeof(int)) {
						int value = (int)aMP.Parameters[i];
						if (ImGui.InputInt("Parameter: " + parameter.Name, ref value)) {
							aMP.Parameters[i] = value;
						}
					}
					if (parameter.ParameterType == typeof(float)) {
						float value = (float)aMP.Parameters[i];
						if (ImGui.InputFloat("Parameter: " + parameter.Name, ref value)) {
							aMP.Parameters[i] = value;
						}
					}
					if (parameter.ParameterType == typeof(bool)) {
						bool value = (bool)aMP.Parameters[i];
						if (ImGui.Checkbox("Parameter: " + parameter.Name, ref value)) {
							aMP.Parameters[i] = value;
						}
					}
					if (parameter.ParameterType == typeof(string)) {
						string value = (string)aMP.Parameters[i];
						if (ImGui.InputText( "Parameter: " + parameter.Name, ref value, 100)) {
							aMP.Parameters[i] = value;
						}
					}
					if (parameter.ParameterType == typeof(Color)) {
						Color value = (Color)aMP.Parameters[i];
						Vector4 numericalColor = ToNumerics(value);
						if (ImGui.ColorPicker4("Parameter: " + parameter.Name, ref numericalColor)) {
							aMP.Parameters[i] = ToXNAColor(numericalColor);
						}
					}
				}
				if (ImGui.Button(button.MethodName)) {
					info.Invoke(entity, aMP.Parameters);
				}
			}
			else {
				if (ImGui.Button(button.MethodName)) {
					info.Invoke(entity, null);
				}
			}
		}
	}
	
	void PaintTooltip (FieldInfo info) {
		TooltipAttribute tooltip = info.GetCustomAttribute<TooltipAttribute>();
		if (tooltip != null) {
			ImGui.Separator();
			ImGui.Text("TOOLTIP: "+tooltip.Tooltip);
		}
	}
	void PaintTooltip (MethodInfo info) {
		TooltipAttribute tooltip = info.GetCustomAttribute<TooltipAttribute>();
		if (tooltip != null) {
			ImGui.Separator();
			ImGui.Text("TOOLTIP: "+tooltip.Tooltip);
		}
	}
	
	public static Vector4 ToNumerics (Color self) => new System.Numerics.Vector4(self.R/255.0f, self.G/255.0f, self.B/255.0f, self.A/255.0f);
	public static Color ToXNAColor (Vector4 self) => new Color((byte)(self.X*255f), (byte)(self.Y*255f), (byte)(self.Z*255f), (byte)(self.W*255f));
	public void Dispose () {
		Input.OnMouseClick -= OnMouseClick;
		Engine.RenderImGUI -= Paint;
		_selectedEntity = null;
		_fields = null;
	}
}
