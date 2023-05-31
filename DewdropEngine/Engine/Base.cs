using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;
using System.Runtime.CompilerServices;
using SFML.System;

namespace DewDrop
{
    public static partial class Engine
    {
        private static bool initialized = false;
        
        /// <summary>
        /// Clock that's started when the game starts.
        /// </summary>
        public static Clock SessionTimer;

        public static void Initialize()
        {

            // if we haven't initialized yet
            if (!initialized)
            {
                Debug.Initialize();
                EmbeddedResourcesHandler.GetStreams();
                new Input();
                
                // get em' graphics going!!!
                // this is located in EngineGraphics.cs
                InitializeGraphics();
                CreateDebugPipeline();
                initialized = true;
                
                SessionTimer = new Clock();
                SessionTimer.Restart();
                
            }
        }
    }
}