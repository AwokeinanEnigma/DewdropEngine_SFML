using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using System.Reflection;
namespace DewDrop.Wren;

public static class WrenManager {
	static List<Type> _TypesList;
	static Type[] _TypesArray;
	public static IEnumerable<Type> FindWrenTypes(Assembly assembly) {
		foreach(Type type in assembly.GetTypes()) {
			if (type.GetCustomAttributes(typeof(WrenClassAttribute), true).Length > 0) {
				yield return type;
			}
		}
	}
	
	static WrenConfig _Config;
	internal static void Initialize (EngineConfigurationData configurationData) {
		_TypesList = new();
		_TypesList.AddRange(FindWrenTypes(Assembly.GetExecutingAssembly()));
		
		if (configurationData.WrenTypes != null) 
			_TypesList.AddRange(configurationData.WrenTypes);
		
		CollectTypes?.Invoke(_TypesList);
		_TypesArray = _TypesList.ToArray();
		
		_Config = new WrenConfig();
	}

	public static Wreno MakeWreno(string script) {
		WrenConfig config = new WrenConfig();
		Wreno wreno = new Wreno(config, script);
		wreno.Automap(_TypesArray);
		return wreno;
	}

	public static Action<List<Type>> CollectTypes;
}
