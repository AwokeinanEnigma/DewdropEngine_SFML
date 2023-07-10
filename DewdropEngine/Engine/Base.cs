#region

using DewDrop.Resources;
using DewDrop.UserInput;
using DewDrop.Utilities;
using SFML.System;

#endregion

namespace DewDrop;

/// <summary>
/// "You must be ahead to quit. Too many people quit when they’re behind instead of attempting to get ahead. Failure!"
/// </summary>
public static partial class Engine
{
    private static bool initialized;

    /// <summary>
    ///     Clock that's started when the game starts.
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

            //TODO: Engine should REALLY be calling its own loop
        }
    }
}