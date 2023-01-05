using DewDrop.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DewDrop.Resources
{
    public static class EngineResources
    {
        private static bool haveStreams = false;
        private static Dictionary<string, Stream> streams;
        private static void GetStreams() {
            
            streams = new Dictionary<string, Stream>();

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string[] allResourceNames = currentAssembly.GetManifestResourceNames();   
            
            for (int i = 0; i < allResourceNames.Length; i++) {
                string currentStreamName = allResourceNames[i];

                // trim the DewDrop.Resources part
                streams.Add(currentStreamName.Replace("DewDrop.Resources.", ""), currentAssembly.GetManifestResourceStream(currentStreamName));
            }

            haveStreams = true; 
        }
        public static Stream GetResourceStream(string name) { 
            if (!haveStreams)
            {
                GetStreams();   
            }
            
            Stream stream = null;
            
            if (streams.TryGetValue(name, out stream)) {
                return stream;
            }

            Debug.LogError($"Couldn't find stream '{name}'!", null);
            return null;
        }
    }
}
