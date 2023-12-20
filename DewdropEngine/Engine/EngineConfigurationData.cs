#region

using DewDrop.Scenes;
using DewDrop.Utilities;

#endregion

namespace DewDrop;

/// <summary>
/// Contains configuration data for the DewDrop engine.
/// </summary>
public struct EngineConfigurationData {
	/// <summary>
	/// Contains application-specific data.
	/// </summary>
	public struct ApplicationData {
		/// <summary>
		/// The name of the application.
		/// </summary>
		public string Name;

		/// <summary>
		/// The version of the application.
		/// </summary>
		public string Version;

		/// <summary>
		/// The developer of the application.
		/// </summary>
		public string Developer;

		/// <summary>
		/// The path to the application's configuration file.
		/// </summary>
		public string ConfigPath;
	}

	/// <summary>
	/// The application-specific data.
	/// </summary>
	public ApplicationData Application;

	/// <summary>
	/// The size of the screen.
	/// </summary>
	public Vector2 ScreenSize;

	/// <summary>
	/// Whether vertical sync should be enabled.
	/// </summary>
	public bool VSync;

	/// <summary>
	/// Whether the application should run in fullscreen mode.
	/// </summary>
	public bool Fullscreen;

	/// <summary>
	/// The initial scene of the application.
	/// </summary>
	public SceneBase StartScene;

	/// <summary>
	/// Whether the application should run in debug mode.
	/// </summary>
	public bool DebugMode;

	/// <summary>
	/// The list of Wren types.
	/// </summary>
	public List<Type> WrenTypes;

	/// <summary>
	/// The default scale of the buffer.
	/// </summary>
	public int DefaultBufferScale;
	
	/// <summary>
	/// Whether ImGui should be enabled.
	/// </summary>
	// ReSharper disable once FieldCanBeMadeReadOnly.Global
	public bool EnableImGui;
	
	/// <summary>
	/// Initializes a new instance of the EngineConfigurationData struct.
	/// </summary>
	/// <param name="data">The application-specific data.</param>
	/// <param name="screenSize">The size of the screen.</param>
	/// <param name="fullscreen">Whether the application should run in fullscreen mode.</param>
	/// <param name="vSync">Whether vertical sync should be enabled.</param>
	/// <param name="debugMode">Whether the application should run in debug mode.</param>
	/// <param name="startScene">The initial scene of the application.</param>
	/// <param name="defaultBufferScale">The default scale of the buffer.</param>
	/// <param name="wrenTypes">The list of Wren types.</param>
	public EngineConfigurationData (ApplicationData data, Vector2 screenSize, bool fullscreen, bool vSync, bool debugMode, SceneBase startScene, int defaultBufferScale, List<Type> wrenTypes, bool useImGui) {
		Application = data;
		ScreenSize = screenSize;
		Fullscreen = fullscreen;
		VSync = vSync;
		DefaultBufferScale = defaultBufferScale;
		DebugMode = debugMode;
		StartScene = startScene;
		WrenTypes = wrenTypes;
		EnableImGui = useImGui;
	}
}
