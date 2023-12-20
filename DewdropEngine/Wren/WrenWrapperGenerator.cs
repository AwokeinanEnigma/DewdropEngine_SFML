#region

using DewDrop.Graphics;
using DewDrop.Utilities;
using DewDrop.Wren;
using SFML.Graphics;
using System.ComponentModel.Design;
using System.Reflection;

#endregion
public class WrenWrapperGenerator {
	public static List<string> BlacklistedMethods = new List<string> {
		"GetType",
	};
	public static string? GenerateWrapper (Type originalType) {
		// check if type has WrenBlackList
		if (originalType.GetCustomAttributes(typeof(WrenBlackList), true).Length > 0) {
			Outer.LogError("Cannot generate wrapper for blacklisted type: " + originalType.Name, null);
			return null;
		}
		
		string wrapperClassName = GetWrapperClassName(originalType);
		var wrapperCode = "using DewDrop.Wren;" + Environment.NewLine;
		wrapperCode += "using DewDrop.Utilities;" + Environment.NewLine;
		wrapperCode += "using IronWren;" + Environment.NewLine;
		wrapperCode += "using IronWren.AutoMapper;" + Environment.NewLine;
		wrapperCode += "using SFML.Graphics;" + Environment.NewLine;
		wrapperCode += "using DewDrop.Graphics;" + Environment.NewLine;
		wrapperCode += Environment.NewLine;
		bool isStatic = originalType.IsAbstract && originalType.IsSealed;
		

		wrapperCode += $"[WrenClass(\"{originalType.Name}\")]" + Environment.NewLine;
		if (originalType.GetInterface("IRenderable") != null) {
			wrapperCode += $"public class {wrapperClassName} : BasicRenderableWrapper" + Environment.NewLine;
			wrapperCode += "{" + Environment.NewLine;
			wrapperCode += "public override IRenderable Renderable => Stored;" + Environment.NewLine;
		} else {
			if (isStatic) {
				wrapperCode += $"public static class {wrapperClassName}" + Environment.NewLine;
			} else {
				wrapperCode += $"public class {wrapperClassName}" + Environment.NewLine;
			}

			wrapperCode += "{" + Environment.NewLine;
		}
		
		//if the type is static, we don't need to generate a constructor
		wrapperCode += "private const string constructorCode = \"\";" + Environment.NewLine;
		string storedName = "Stored";
		if (!isStatic) {
			wrapperCode += $"public {originalType.FullName} {storedName};" + Environment.NewLine;

			wrapperCode += $"public {wrapperClassName}({originalType.FullName} original)" + Environment.NewLine;
			wrapperCode += "{" + Environment.NewLine;
			wrapperCode += $"{storedName} = original;" + Environment.NewLine;
			wrapperCode += "}" + Environment.NewLine;


			// Generate constructor
			var constructorParams = originalType.GetConstructors()
				.FirstOrDefault()?.GetParameters() ?? Array.Empty<ParameterInfo>();

			var validParams = new List<ParameterInfo>();
			foreach (var parameter in constructorParams) {
				string type = GetWrenTypeSuffix(parameter.ParameterType, false, out string cast);
				if (type.Contains("Unknown")) {
					//Outer.Log("Skipping unknown type: " + parameter.ParameterType);
					continue;
				}
				validParams.Add(parameter);
			}

			string code = "\"" + string.Join("\", \"", validParams
				.Select(p => $"{p.Name}")) + "\"";

			if (constructorParams.Length > 0) {
				wrapperCode += $"[WrenConstructor({code}, Code = \"field:constructorCode\")]" + Environment.NewLine;
			} else {
				wrapperCode += "[WrenConstructor(Code = \"field:constructorCode\")]" + Environment.NewLine;
			}
			wrapperCode += $"public {wrapperClassName}(WrenVM vm)" + Environment.NewLine;
			wrapperCode += "{" + Environment.NewLine;

			if (constructorParams.Length > 0) {
				wrapperCode += $"vm.EnsureSlots({validParams.Count});" + Environment.NewLine;
			}
			string constructoCode = "";
			foreach (var parameter in constructorParams) {
				string type = GetWrenTypeSuffix(parameter.ParameterType, false, out string cast);
				if (type.Contains("Unknown")) {
					//Outer.Log("Skipping unknown type: " + parameter.ParameterType);
					constructoCode += "null, ";
					continue;
				}
				wrapperCode += $"var {parameter.Name} = {cast}vm.GetSlot{type}({validParams.IndexOf(parameter) + 1});" + Environment.NewLine;
				if (type.Contains("Foreign")) {
					//Outer.Log("hey");
					constructoCode += $"{parameter.Name}.{GetForeignField(parameter.ParameterType)}, ";
				} else {
					//Outer.Log(parameter.Name);
					constructoCode += $"{parameter.Name}, ";
				}
			}
			if (constructoCode.Length > 2) {
				constructoCode = constructoCode.Substring(0, constructoCode.Length - 2);
			}
			
			// remove the last comma and space
			wrapperCode += $"{storedName} = new {originalType.FullName}({constructoCode});" + Environment.NewLine;
			wrapperCode += "}" + Environment.NewLine;
			
			// also generate static constructor
			wrapperCode += $"[WrenMethod(\"New\", {code})]" + Environment.NewLine;
			wrapperCode += $"public static void New(WrenVM vm)" + Environment.NewLine;
			wrapperCode += "{" + Environment.NewLine;
			if (constructorParams.Length > 0) {
				wrapperCode += $"vm.EnsureSlots({validParams.Count});" + Environment.NewLine;
			}
			foreach (var parameter in constructorParams) {
				string type = GetWrenTypeSuffix(parameter.ParameterType, false, out string cast);
				if (type.Contains("Unknown")) {
					//Outer.Log("Skipping unknown type: " + parameter.ParameterType);
					continue;
				}
				wrapperCode += $"var {parameter.Name} = {cast}vm.GetSlot{type}({validParams.IndexOf(parameter) + 1});" + Environment.NewLine;
			}


			string foreignConstructoCode = constructoCode;
				// remove all instances of "null," from the string
			foreignConstructoCode = foreignConstructoCode.Replace("null,", "");
			foreignConstructoCode = foreignConstructoCode.Replace(", null)", "");
			if (foreignConstructoCode.Contains("null")) {
				foreignConstructoCode = foreignConstructoCode.Substring(0, foreignConstructoCode.Length - 6);
			}

			wrapperCode += $"vm.SetSlotNewForeign(0, new {wrapperClassName}({foreignConstructoCode})";
			wrapperCode +=  ");" + Environment.NewLine;
			wrapperCode += "}" + Environment.NewLine;
			
			// now generate a constructor that takes constructorparams as arguments and generates a new instance
			wrapperCode += $"public {wrapperClassName}({string.Join(", ", validParams.Select(p => $"{p.ParameterType} {p.Name}"))})" + Environment.NewLine;
			wrapperCode += "{" + Environment.NewLine;
			constructoCode = constructoCode.Replace(".Vector", "");
			//wrapperCode += $"{storedName} = new {originalType.FullName}({string.Join(", ", validParams.Select(p => p.Name))});" + Environment.NewLine;
			wrapperCode += $"{storedName} = new {originalType.FullName}({constructoCode});" + Environment.NewLine;
			wrapperCode += "}" + Environment.NewLine;
			


		} else {
			storedName = originalType.FullName;
		}

		// Generate field wrapper
		if (originalType.GetFields().Length > 0) {
			wrapperCode += "// Field wrappers" + Environment.NewLine;
		}
		foreach (var field in originalType.GetFields()) {
			//check if field is blacklisted
			if (field.GetCustomAttributes(typeof(WrenBlackList), true).Length > 0) {
				continue;
			}
			string declarationParameters = "public void";
			if (isStatic) {
				declarationParameters = "public static void";
			}
			
			string type = GetWrenTypeSuffix(field.FieldType, false, out string cast);
			if (type.Contains("Unknown")) {
				//Outer.Log("Skipping unknown type: " + field.FieldType);
				continue;
			}
			wrapperCode += $"[WrenProperty(PropertyType.Set, \"{field.Name}\")]" + Environment.NewLine;
			wrapperCode += $"{declarationParameters} Set{field.Name}(WrenVM vm)" + Environment.NewLine;
			wrapperCode += "{" + Environment.NewLine;
			wrapperCode += "vm.EnsureSlots(1);" + Environment.NewLine;
			
			if (type.Contains("Foreign")) {
				wrapperCode += $"{storedName}.{field.Name} = {cast}vm.GetSlot{type}(1).{GetForeignField(field.FieldType)};" + Environment.NewLine;

			} else {
				wrapperCode += $"{storedName}.{field.Name} = {cast}vm.GetSlot{type}(1);" + Environment.NewLine;
			}
			wrapperCode += "}" + Environment.NewLine;
			wrapperCode += Environment.NewLine;

			wrapperCode += $"[WrenProperty(PropertyType.Get, \"{field.Name}\")]" + Environment.NewLine;
			wrapperCode += $"{declarationParameters} Get{field.Name}(WrenVM vm)" + Environment.NewLine;
			wrapperCode += "{" + Environment.NewLine;
			wrapperCode += "vm.EnsureSlots(1);" + Environment.NewLine;
			if (type.Contains("Foreign")) {
				wrapperCode += $"vm.SetSlot{GetWrenTypeSuffix(field.FieldType, true, out _)}(0, new {GetWrapperClassName(field.FieldType)}({storedName}.{field.Name}));" + Environment.NewLine;
			} else {
				wrapperCode += $"vm.SetSlot{GetWrenTypeSuffix(field.FieldType, true, out _)}(0, {storedName}.{field.Name});" + Environment.NewLine;
			}
			wrapperCode += "}" + Environment.NewLine;
			wrapperCode += Environment.NewLine;
			
		}

		if (originalType.GetProperties().Length > 0) {
			wrapperCode += "// Property wrappers" + Environment.NewLine;
		}
		foreach (var field in originalType.GetProperties()) {
			//check if field is blacklisted
			if (field.GetCustomAttributes(typeof(WrenBlackList), true).Length > 0) {
				continue;
			}
			string type = GetWrenTypeSuffix(field.PropertyType, false, out string cast);
			if (type.Contains("Unknown")) {
				//Outer.Log("Skipping unknown type: " + field.PropertyType);
				continue;
			}
			string declarationParameters = "public void";
			if (isStatic) {
				declarationParameters = "public static void";
			}
			// only make setters for properties that have them
			if (field.GetSetMethod() != null) {


				wrapperCode += $"[WrenProperty(PropertyType.Set, \"{field.Name}\")]" + Environment.NewLine;
				wrapperCode += $"{declarationParameters} Set{field.Name}(WrenVM vm)" + Environment.NewLine;
				wrapperCode += "{" + Environment.NewLine;
				wrapperCode += "vm.EnsureSlots(1);" + Environment.NewLine;

				if (type.Contains("Foreign")) {
					wrapperCode += $"{storedName}.{field.Name} = {cast}vm.GetSlot{type}(1).{GetForeignField(field.PropertyType)};" + Environment.NewLine;

				} else {
					wrapperCode += $"{storedName}.{field.Name} = {cast}vm.GetSlot{type}(1);" + Environment.NewLine;
				}
				wrapperCode += "}" + Environment.NewLine;
				wrapperCode += Environment.NewLine;
			}
			wrapperCode += $"[WrenProperty(PropertyType.Get, \"{field.Name}\")]" + Environment.NewLine;
			wrapperCode += $"{declarationParameters} Get{field.Name}(WrenVM vm)" + Environment.NewLine;
			wrapperCode += "{" + Environment.NewLine;
			wrapperCode += "vm.EnsureSlots(1);" + Environment.NewLine;
			if (type.Contains("Foreign")) {
				wrapperCode += $"vm.SetSlot{GetWrenTypeSuffix(field.PropertyType, true, out _)}(0, new {GetWrapperClassName(field.PropertyType)}({storedName}.{field.Name}));" + Environment.NewLine;
			} else {
				wrapperCode += $"vm.SetSlot{GetWrenTypeSuffix(field.PropertyType, true, out _)}(0, {storedName}.{field.Name});" + Environment.NewLine;
			}
			wrapperCode += "}" + Environment.NewLine;
			wrapperCode += Environment.NewLine;
		}


		// Generate method wrappers
		foreach (var method in originalType.GetMethods()) {
			//check if field is blacklisted
			if (method.GetCustomAttributes(typeof(WrenBlackList), true).Length > 0) {
				continue;
			}
			var parameters = method.GetParameters();
			if (method.ReturnType == typeof(void) && parameters.Length == 0) {
				continue;
			}
			// make sure we haven't already generated properties for this
			bool move = false;
			if (originalType.GetFields().Length > 0) {
				foreach (var originalField in originalType.GetFields()) {
					if (method.Name.Contains(originalField.Name)) {
						move = true;
						break;
					}
				}
			}
			foreach (var originalProperty in originalType.GetProperties()) {
				if (method.Name.Contains(originalProperty.Name)) {
					move = true;
					break;
				}
			}
			if (move) {
				continue;
			}


			if (parameters.Length > 0) {
				//Outer.Log(parameters[0].ParameterType);
				string parameterType = GetWrenTypeSuffix(parameters[0].ParameterType, false, out _);
				if (parameterType.Contains("Unknown")) {
					//Outer.Log("Skipping unknown type: " + method.ReturnType);
					continue;
				}
			}

			if (BlacklistedMethods.Contains(method.Name)) {
				continue;
			}


			string declaredParameters = "public void";
			if (isStatic) {
				declaredParameters = "public static void";
			}
			if (parameters.Length > 0) {
				wrapperCode += $"[WrenMethod(\"{method.Name}\"";
				foreach (var parameter in parameters) {
					wrapperCode += $", \"{parameter.Name}\"";
				}

				wrapperCode += ")]" + Environment.NewLine;
				string methodName = method.Name;

				//check if we've already generated a method with this name
				if (wrapperCode.Contains($"{declaredParameters} {method.Name}(WrenVM vm)")) {
					methodName += parameters.Length;
				}

				wrapperCode += $"{declaredParameters} {methodName}(WrenVM vm)" + Environment.NewLine;
				wrapperCode += "{" + Environment.NewLine;
				wrapperCode += $"vm.EnsureSlots({parameters.Length});" + Environment.NewLine;
				var parameterList = "";
				for (int i = 0; i < parameters.Length; i++) {
					var parameter = parameters[i];
					string type = GetWrenTypeSuffix(parameter.ParameterType, false, out string cast);

					if (type.Contains("Foreign")) {
						parameterList += $"{cast}vm.GetSlot{type}({i + 1}).{GetForeignField(parameter.ParameterType)}";
					} else {
						parameterList += $"{cast}vm.GetSlot{type}({i + 1})";
					}
					if (i < parameters.Length - 1) {
						parameterList += ", ";
					}
				}
				if (method.ReturnType != typeof(void)) {
					var type = GetWrenTypeSuffix(method.ReturnType, false, out string cast);
					if (type.Contains("Foreign")) {
						wrapperCode += $"vm.SetSlot{GetWrenTypeSuffix(method.ReturnType, true, out _)}(0, new {GetWrapperClassName(method.ReturnType)}({storedName}.{method.Name}({parameterList})));" + Environment.NewLine;
					} else {
						wrapperCode += $"vm.SetSlot{GetWrenTypeSuffix(method.ReturnType, true, out _)}(0, {storedName}.{method.Name}({parameterList}));" + Environment.NewLine;
					}
				} else {
					wrapperCode += $"{storedName}.{method.Name}({parameterList});" + Environment.NewLine;
				}
				//wrapperCode += $"Stored.{method.Name}({parameterList});" + Environment.NewLine;
				wrapperCode += "}" + Environment.NewLine;
				wrapperCode += Environment.NewLine;
			} else {
				wrapperCode += $"[WrenMethod(\"{method.Name}\"";
				foreach (var parameter in parameters) {
					wrapperCode += $", \"{parameter.Name}\"";
				}

				wrapperCode += ")]" + Environment.NewLine;
				string methodName = method.Name;
				//check if we've already generated a method with this name
				if (wrapperCode.Contains($"{declaredParameters} {method.Name}(WrenVM vm)")) {
					methodName += parameters.Length;
				}

				if (method.ReturnType != typeof(void)) {
					wrapperCode += $"{declaredParameters} {methodName}(WrenVM vm)" + Environment.NewLine;
					wrapperCode += "{" + Environment.NewLine;
					wrapperCode += "vm.EnsureSlots(1);" + Environment.NewLine;
					var type = GetWrenTypeSuffix(method.ReturnType, false, out string cast);
					if (type.Contains("Foreign")) {
						wrapperCode += $"vm.SetSlot{GetWrenTypeSuffix(method.ReturnType, true, out _)}(0, new {GetWrapperClassName(method.ReturnType)}({storedName}.{method.Name}()));" + Environment.NewLine;
					} else {
						wrapperCode += $"vm.SetSlot{GetWrenTypeSuffix(method.ReturnType, true, out _)}(0, {storedName}.{method.Name}());" + Environment.NewLine;
					}
					wrapperCode += "}" + Environment.NewLine;
				}
			}
		}

		wrapperCode += "}" + Environment.NewLine;

		return wrapperCode;
	}

	public static string GetWrapperClassName (Type type) {
		return $"Wren{type.Name}Wrapper";
	}

	public static string GetForeignField (Type type) {
		if (type == typeof(Vector2)) {
			return "Vector";
		}
		if (type == typeof(Color)) {
			return "Color";
		}
		return "Stored";
	}
	


	static string GetWrenTypeSuffix (Type type, bool set, out string cast) {
		cast = "";
		if (type == typeof(float)) {
			cast = "(float)";
			return "Double";
		}
		if (type == typeof(double)) {
			cast = "";
			return "Double";
		}
		if (type == typeof(int)) {
			cast = "(int)";
			return "Double";
		}
		if (type == typeof(uint)) {
			cast = "(uint)";
			return "Double";
		}
		if (type == typeof(ushort)) {
			cast = "(ushort)";
			return "Double";
		}
		if (type == typeof(byte)) {
			cast = "(byte)";
			return "Double";
		}
		if (type == typeof(sbyte)) {
			cast = "(sbyte)";
			return "Double";
		}
		if (type == typeof(ushort)) {
			cast = "(ushort)";
			return "Double";

		}
		if (type == typeof(string)) {
			return "String";
		}
		if (type == typeof(bool)) {
			return "Bool";
		}
		if (type == typeof(Vector2)) {
			if (set) {
				cast = "WrenVector2Wrapper";
				return "NewForeign";
			}
			return "Foreign<WrenVector2Wrapper>";
		}
		if (type == typeof(Color)) {
			if (set) {
				cast = "WrenColorWrapper";
				return "NewForeign";
			}
			return "Foreign<WrenColorWrapper>";
		}
		if (type == typeof(SpriteGraphic)) {
			if (set) {
				cast = "WrenSpriteGraphicWrapper";
				return "NewForeign";
			}
			return "Foreign<WrenSpriteGraphicWrapper>";

		}
		if (type == typeof(SpriteDefinition)) {
			if (set) {
				cast = "WrenSpriteDefinitionWrapper";
				return "NewForeign";
			}
			return "Foreign<WrenSpriteDefinitionWrapper>";
		}
		if (type == typeof(AsepriteGraphic)) {
			if (set) {
				cast = "WrenAsepriteGraphicWrapper";
				return "NewForeign";
			}
			return "Foreign<WrenAsepriteGraphicWrapper>";

		}
		return "Unknown";
	}
}
