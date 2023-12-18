#region

using DewDrop.Scenes;
using DewDrop.Utilities;

#endregion

namespace DewDrop;

/// <summary>
///     The engine is able to have certain aspects of it manipulated to fit your needs.
///     This struct contains values used by the engine to determine certain aspects of itself.
///     Such as the size of the screen, and if VSync should be enabled.
/// </summary>
public struct EngineConfigurationData {
	public struct ApplicationData {
		public string Name;
		public string Version;
		public string Developer;
		public string ConfigPath;
	}
	
	public ApplicationData Application;
	public Vector2 ScreenSize;
	public bool VSync;
	public bool Fullscreen;
	public SceneBase StartScene;
	public bool DebugMode;
	public List<Type> WrenTypes;
	public int DefaultBufferScale;
	public EngineConfigurationData (ApplicationData data, Vector2 screenSize, bool fullscreen, bool vSync, bool debugMode, SceneBase startScene, int defaultBufferScale, List<Type> wrenTypes) {
		Application = data;
		ScreenSize = screenSize;
		Fullscreen = fullscreen;
		VSync = vSync;
		DefaultBufferScale = defaultBufferScale;
		DebugMode = debugMode;
		StartScene = startScene;
		WrenTypes = wrenTypes;
	}
}
