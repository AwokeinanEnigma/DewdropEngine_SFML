#region

using DewDrop.Graphics;
using DewDrop.Utilities;
using ImGuiNET;

#endregion

namespace DewDrop;

public static partial class Engine {
	public static RenderPipeline DebugPipeline { get; private set; }

	public static bool DebugMode {
		get => _debugMode;
		set {
			_debugMode = value;
			if (value)
				Outer.LogWarning("Enabling debug mode!");
			else
				Outer.LogWarning("Disabling debug mode!");
		}
	}


	public const bool RenderImgui = true;
	static bool _debugMode;

	public static void CreateDebugPipeline (EngineConfigurationData config) {
		_debugMode = config.DebugMode;
		DebugPipeline = new RenderPipeline(RenderTexture);
		if (RenderImgui) {
			ImGuiSfml.Init(Window);
			ImGui.LoadIniSettingsFromDisk("imgui.ini");
			OnFocusGained += () => TextureManager.Instance.ReloadTextures();
			OnRenderImGui += OnRenderImGUI;
			//Rendersc
		}
	}

	static bool _showTextures;
	static void OnRenderImGUI () {
		ImGui.Begin("Dewdrop Debug Utilities");
		
		ImGui.Text("GC//");
		ImGui.Separator();
		// this is sort of a memory intensive thing but its ok for now
		ImGui.Text($"Memory Allocated: {GC.GetTotalMemory(false)/1024L}KB");
		if (ImGui.Button("Force GC Collection")) GC.Collect();
		ImGui.Separator();
		
		ImGui.Text("FPS//");
		ImGui.Separator();
		ImGui.Text($"FPS: {MathF.Round((float)_Fps)}");
		ImGui.Separator();

		//Outer.Log(GC.GetTotalMemory(false)/1024L);
		
		// go through each texturemanager active texture and display it's name
		ImGui.Text("Textures//");
		ImGui.Separator();
		ImGui.Text("Loaded Textures:");
		if (ImGui.Button("Show Textures"))_showTextures = !_showTextures;
		if (_showTextures) {
			foreach (var texture in TextureManager.Instance.ActiveTextures) {
				ImGui.Text($"{texture.Value} : {TextureManager.Instance.Instances[texture.Key]}");
			}
		}
		if (ImGui.Button("Purge Textures")) TextureManager.Instance.Purge();
		if (ImGui.Button("Reload Textures")) TextureManager.Instance.ReloadTextures();
		//ImGui.ShowStyleEditor();
		ImGui.End();

		//throw new NotImplementedException();
	}
}
