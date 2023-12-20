#region

using DewDrop.Utilities;
using System.Reflection;
// ReSharper disable ForCanBeConvertedToForeach

#endregion

namespace DewDrop.Resources;

/// <summary>
///     Allows you to easily acquire a resource stream of any resource.
/// </summary>
public static class EmbeddedResourcesHandler {
	static Dictionary<string, Stream> _Streams;

	public static void FillStreams () {

		// create dict 
		_Streams = new Dictionary<string, Stream>();

		// get current assembly
		Assembly currentAssembly = Engine.Assembly;

		// get all manifest resource names
		string[] allResourceNames = currentAssembly.GetManifestResourceNames();

		for (int i = 0; i < allResourceNames.Length; i++) {
			string currentStreamName = allResourceNames[i];

			// trim the DewDrop.Resources part
			Stream? resourceStream = currentAssembly.GetManifestResourceStream(currentStreamName);
			if (resourceStream != null) {
				// we can do this safely because we know the name of the project will never change
				_Streams.Add(currentStreamName.Replace("DewDrop.Resources.", ""), resourceStream);
			} else {
				Outer.LogError($"Can't get resource stream of resource '{currentStreamName}'", new Exception("Resource stream is null!"));
			}
		}
	}

    /// <summary>
    ///     Adds all embedded resources from an assembly. Embedded resources must be in a folder called "Resources" at the root
    ///     of the project for this to function properly.
    /// </summary>
    /// <param name="asm">The assembly you want to get embedded resource from</param>
    /// <param name="trimName">To remove the path to the embedded resource, the name of the assembly is required./</param>
    public static void AddEmbeddedResources (Assembly asm, string trimName) {

		// get all manifest resource names
		string[] allResourceNames = asm.GetManifestResourceNames();

		for (int i = 0; i < allResourceNames.Length; i++) {
			string currentStreamName = allResourceNames[i];

			Stream? resourceStream = asm.GetManifestResourceStream(currentStreamName);

			if (resourceStream != null) {
				_Streams.Add(currentStreamName.Replace($"{trimName}.Resources.", ""), resourceStream);
			} else {
				Outer.LogError($"Can't get resource stream of resource '{currentStreamName}'", new Exception("Resource stream is null!"));
			}
		}
	}

    /// <summary>
    ///     Gets a resource stream by name.
    /// </summary>
    /// <param name="name">The name of the resource you want to get.</param>
    /// <returns>The resource stream of the resource. Can be null.</returns>
    public static Stream? GetResourceStream (string name) {

		if (_Streams.TryGetValue(name, out Stream? stream)) {
			return stream;
		}

		Outer.LogError($"Couldn't find stream '{name}'!", null);
		return null;
	}
}
