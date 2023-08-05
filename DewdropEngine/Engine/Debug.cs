#region

using DewDrop.Graphics;
using DewDrop.Utilities;
using ImGuiNET;

#endregion

namespace DewDrop;

public static partial class Engine
{
    public static RenderPipeline DebugPipeline => _debugPipeline;

    public static bool DebugMode
    {
        get => _debugMode;
        set
        {
            _debugMode = value;
            if (value)
                DDDebug.LogWarning("Enabling debug mode!");
            else
                DDDebug.LogWarning("Disabling debug mode!");
        }
    }


    private static RenderPipeline _debugPipeline;
    private static bool _debugMode;

    public static void CreateDebugPipeline()
    {
        _debugPipeline = new RenderPipeline(frameBuffer);
        ImGuiSfml.Init(window);
        ImGui.LoadIniSettingsFromDisk("imgui.ini");
        
        RenderImGUI += OnRenderImGUI;
        //Rendersc
    }

    private static void OnRenderImGUI()
    {
        ImGui.Begin("Dewdrop Debug Utilities");
        ImGui.Text($"Garbage Allocated: {GC.GetTotalMemory(false) / 1024L}KB");
        ImGui.Separator();

        if (ImGui.Button("Force GC Collection")) GC.Collect();
        ImGui.End();
        
        //throw new NotImplementedException();
    }
}