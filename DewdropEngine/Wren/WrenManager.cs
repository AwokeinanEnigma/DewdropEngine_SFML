using IronWren;
using IronWren.AutoMapper;
using System.Reflection;

namespace DewDrop.Wren;

/// <summary>
/// Manages the Wren scripting language integration in the DewDrop engine.
/// </summary>
/// <remarks>
/// This class is responsible for initializing the Wren scripting engine, 
/// finding and registering types that are exposed to the Wren scripts, 
/// and creating instances of the Wren scripting engine.
/// </remarks>
public static class WrenManager {
	static List<Type> _TypesList;
	static Type[] _TypesArray;
	static WrenConfig _Config;
	
	/// <summary>
	/// Initializes the WrenManager with the provided configuration data.
	/// </summary>
	/// <param name="configurationData">The configuration data for the WrenManager.</param>
	internal static void Initialize (EngineConfigurationData configurationData) {
		_TypesList = new();
		_TypesList.AddRange(FindWrenTypes(Engine.Assembly));
		
		if (configurationData.WrenTypes != null) 
			_TypesList.AddRange(configurationData.WrenTypes);
		
		_TypesArray = _TypesList.ToArray();
		
		_Config = new WrenConfig();
	}

	/// <summary>
	/// Creates a new Wreno instance with the provided script.
	/// </summary>
	/// <param name="script">The script to be executed by the Wreno instance.</param>
	/// <returns>A new Wreno instance.</returns>
	public static Wreno MakeWreno(string script) {
		Wreno wreno = new Wreno(_Config, script);
		wreno.Automap(_TypesArray);
		return wreno;
	}

	/// <summary>
	/// Searches for types in the provided assembly that have the WrenClassAttribute.
	/// </summary>
	/// <param name="assembly">The assembly to search for Wren types.</param>
	/// <returns>An IEnumerable of types that have the WrenClassAttribute.</returns>
	public static IEnumerable<Type> FindWrenTypes (Assembly assembly) {
		foreach (Type type in assembly.GetTypes()) {
			if (type.GetCustomAttributes(typeof(WrenClassAttribute), true).Length > 0) {
				yield return type;
			}
		}
	}
}
