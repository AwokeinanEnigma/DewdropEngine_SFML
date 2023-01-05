using DewDrop.Utilities;
using SFML.Graphics;
using System.Runtime.CompilerServices;

namespace DewDrop
{
    public static partial class Engine
    {
        public static Vector2 Screen_Size
        {

            // this is in EngineGraphics.cs
            get => screen_size;
        }

        private static bool initialized = false;



        public static void Initialize()
        {

            // if we haven't initialized yet
            if (!initialized)
            {
                Debug.Initialize();

                // get em' graphics going!!!
                // this is located in EngineGraphics.cs
                InitializeGraphics();
                CreateDebugPipeline();
                StartGameLoop();
            }
        }
    }
}