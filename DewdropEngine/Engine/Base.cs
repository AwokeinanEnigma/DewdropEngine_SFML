#region

using DewDrop.Resources;
using DewDrop.Scenes;
using DewDrop.UserInput;
using DewDrop.Utilities;
using DewDrop.Wren;
using fNbt;
using SFML.System;
using System.Runtime.CompilerServices;

#endregion

namespace DewDrop;

/// <summary>
///  "You must be ahead to quit. Too many people quit when they’re behind instead of attempting to get ahead. Failure!"
/// </summary>
public static partial class Engine {
	static bool initialized;

    /// <summary>
    ///     Clock that's started when the game starts.
    /// </summary>
    public static Clock SessionTimer;
    internal static EngineConfigurationData.ApplicationData ApplicationData;
    public static void Initialize (EngineConfigurationData config) {
	    ApplicationData = config.Application;

		// if we haven't initialized yet
		if (!initialized) {
			Outer.Initialize();
			WrenManager.Initialize(config);	
			EmbeddedResourcesHandler.GetStreams();
			new Input();

			// get the config from appdata
			if (File.Exists(ApplicationData.ConfigPath)) {
				GlobalData.LoadFromNbt(new NbtFile(ApplicationData.ConfigPath).RootTag);
			} else {
				//create folder if it doesn't exist
				Directory.CreateDirectory(ApplicationData.ConfigPath.Replace("/config.nbt", ""));
				Outer.LogError("Config file doesn't exist.", null);
			}
			
			ScreenSize = config.ScreenSize;
			HalfScreenSize = ScreenSize / 2;
			
			// get em' graphics going!!!
			// this is located in EngineGraphics.cs
			InitializeGraphics(config);
			CreateDebugPipeline(config);
			
			SceneManager.Initialize(config.StartScene);
			initialized = true;

			SessionTimer = new Clock();
			SessionTimer.Restart();

			StartGameLoop();
		}
	}

	public static void CleanseCollect () {
		GC.Collect();
	}
}
