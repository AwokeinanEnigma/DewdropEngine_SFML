using DewDrop.Graphics;
using DewDrop.Utilities;
using SFML.Graphics;
using System.Runtime.CompilerServices;

namespace DewDrop
{
    public static partial class Engine
    {
        public static RenderPipeline DebugPipeline
        {
            get => debugPipeline;
        }

        public static bool DebugMode {
            get => debugMode;
            set 
            {
                debugMode = value;
                Debug.LogWarning("Enabling debug mode!");
            }
        }


        private static RenderPipeline debugPipeline;
        private static bool debugMode = false;

        public static void CreateDebugPipeline()
        {
            debugPipeline = new RenderPipeline(frameBuffer);
        }
    }
}