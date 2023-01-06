using DewDrop.Utilities;
using System.Reflection;

namespace DewDrop.Resources
{
    /// <summary>
    /// Allows you to easily acquire a resource stream of any resource.
    /// </summary>
    public static class EmbeddedResourcesHandler
    {
        private static Dictionary<string, Stream> streams;
        public static void GetStreams() {
            
            // create dict 
            streams = new Dictionary<string, Stream>();

            // get current assembly
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            // get all manifest resource names
            string[] allResourceNames = currentAssembly.GetManifestResourceNames();

            for (int i = 0; i < allResourceNames.Length; i++)
            {
                string currentStreamName = allResourceNames[i];

                // trim the DewDrop.Resources part
                Stream? resourceStream = currentAssembly.GetManifestResourceStream(currentStreamName);
                if (resourceStream != null)
                {
                    // we can do this safely because we know the name of the project will never change
                    streams.Add(currentStreamName.Replace("DewDrop.Resources.", ""), resourceStream);
                }
                else
                {
                    Debug.LogError($"Can't get resource stream of resource '{currentStreamName}'", new Exception("Resource stream is null!"));
                }
            }
        }

        /// <summary>
        /// Adds all embedded resources from an assembly. Embedded resources must be in a folder called "Resources" at the root of the project for this to function properly.
        /// </summary>
        /// <param name="asm">The assembly you want to get embedded resource from</param>
        /// <param name="trimName">To remove the path to the embedded resource, the name of the assembly is required./</param>
        public static void AddEmbeddedResources(Assembly asm, string trimName)  {

            // get all manifest resource names
            string[] allResourceNames = asm.GetManifestResourceNames();
            
            for (int i = 0; i < allResourceNames.Length; i++)
            {
                string currentStreamName = allResourceNames[i];

                Stream? resourceStream = asm.GetManifestResourceStream(currentStreamName);

                if (resourceStream != null)
                {
                    streams.Add(currentStreamName.Replace($"{trimName}.Resources.", ""), resourceStream);
                }
                else
                {
                    Debug.LogError($"Can't get resource stream of resource '{currentStreamName}'", new Exception("Resource stream is null!"));
                }
            }
        }

        /// <summary>
        /// Gets a resource stream by name.
        /// </summary>
        /// <param name="name">The name of the resource you want to get.</param>
        /// <returns>The resource stream of the resource. Can be null.</returns>
        public static Stream? GetResourceStream(string name) { 

            if (streams.TryGetValue(name, out Stream? stream))
            {
                return stream;
            }

            Debug.LogError($"Couldn't find stream '{name}'!", null);
            return null;
        }
    }
}
