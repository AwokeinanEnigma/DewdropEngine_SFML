#region

using DewDrop.GUI;
using DewDrop.Resources;
using DewDrop.Scenes;
using DewDrop.UserInput;
using DewDrop.Utilities;
using DewDrop.Wren;
using fNbt;
using SFML.System;
using System.Reflection;

#endregion

namespace DewDrop;

/// <summary>
/// The main class for managing the DewDrop engine.
/// </summary>
/// <remarks>
///  "You must be ahead to quit. Too many people quit when they’re behind instead of attempting to get ahead. Failure!"
/// </remarks>
public static partial class Engine {
	static bool _Initialized;
	
	// ReSharper disable once MemberCanBePrivate.Global
	public static Clock SessionTimer;
    internal static EngineConfigurationData.ApplicationData ApplicationData;
    static EngineConfigurationData _ConfigurationData;
    public static Assembly Assembly => typeof(Engine).Assembly; 
    /// <summary>
    /// Initializes the DewDrop engine with the provided configuration data.
    /// </summary>
    /// <param name="config">The configuration data for the DewDrop engine.</param>
    public static void Initialize (EngineConfigurationData config) {
	    ApplicationData = config.Application;
	    _ConfigurationData = config;
	    
	    // if we haven't initialized yet
		if (!_Initialized) {
			Outer.Initialize();
			WrenManager.Initialize(config);	
			EmbeddedResourcesHandler.FillStreams();
			
			// ReSharper disable once ObjectCreationAsStatement
			// We just want to create an instance of Input to initialize it.
			new Input();

			// get the config from appdata
			if (File.Exists(ApplicationData.ConfigPath)) {
				GlobalData.LoadFromNbt(new NbtFile(ApplicationData.ConfigPath).RootTag);
			} else {
				//create folder if it doesn't exist
				Directory.CreateDirectory(ApplicationData.ConfigPath.Replace("/config.nconf", ""));
				Outer.LogError("Config file doesn't exist.", null);
			}
			
			ScreenSize = config.ScreenSize;
			HalfScreenSize = ScreenSize / 2;
			
			// get em' graphics going!!!
			// this is located in EngineGraphics.cs
			InitializeGraphics(config);
			CreateDebugPipeline(config);
			
			// ReSharper disable once ObjectCreationAsStatement
			new ViewManager();
			// initialize the view manager after the graphics are initialized but before the scenes are initialized
			
			SceneManager.Initialize(config.StartScene);
			_Initialized = true;

			SessionTimer = new Clock();
			SessionTimer.Restart();

			StartGameLoop();
		}
		else {
			Outer.LogError("Engine already initialized.", new Exception());
		}
	}
}
