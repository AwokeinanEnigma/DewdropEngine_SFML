#region

using DewDrop.Graphics;
using DewDrop.Utilities;
using ImGuiNET;

#endregion

namespace DewDrop;

public static partial class Engine
{
    public static RenderPipeline DebugPipeline => debugPipeline;

    public static bool DebugMode
    {
        get => debugMode;
        set
        {
            debugMode = value;
            if (value)
                Debug.LogWarning("Enabling debug mode!");
            else
                Debug.LogWarning("Disabling debug mode!");
        }
    }


    private static RenderPipeline debugPipeline;
    private static bool debugMode;

    public static void CreateDebugPipeline()
    {
        debugPipeline = new RenderPipeline(frameBuffer);
        ImGuiSfml.Init(window);
        ImGui.LoadIniSettingsFromDisk("imgui.ini");
    }
}