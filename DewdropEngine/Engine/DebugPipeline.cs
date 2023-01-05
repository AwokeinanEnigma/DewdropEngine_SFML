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

        private static RenderPipeline debugPipeline;
        public static void CreateDebugPipeline()
        {
            debugPipeline = new RenderPipeline(frameBuffer);
        }
    }
}