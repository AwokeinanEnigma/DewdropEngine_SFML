using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using System.Reflection;
namespace DewDrop.Wren;

public static class WrenManager {
	static List<Type> _Types;
	public static IEnumerable<Type> FindWrenTypes(Assembly assembly) {
		foreach(Type type in assembly.GetTypes()) {
			if (type.GetCustomAttributes(typeof(WrenClassAttribute), true).Length > 0) {
				yield return type;
			}
		}
	}
	
	static WrenConfig _Config;
	public static void Initialize () {
		_Types = new();
		_Types.AddRange(FindWrenTypes(Assembly.GetExecutingAssembly()));
		CollectTypes?.Invoke(_Types);
		_Config = new WrenConfig();
	}

	public static Wreno MakeWreno(string script) {
		WrenConfig config = new WrenConfig();
		Wreno wreno = new Wreno(config, script);
		wreno.Automap(_Types.ToArray());
		return wreno;
	}

	public static Action<List<Type>> CollectTypes;
}
