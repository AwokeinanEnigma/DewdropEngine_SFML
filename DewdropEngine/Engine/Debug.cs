using DewDrop.Graphics;
using DewDrop.Utilities;
using SFML.Graphics;
using System.Runtime.CompilerServices;
using ImGuiNET;

namespace DewDrop
{
    public static partial class Engine
    {
        public static RenderPipeline DebugPipeline
        {
            get => debugPipeline;
        }

        public static bool DebugMode {
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
        private static bool debugMode = false;

        public static void CreateDebugPipeline()
        {
            debugPipeline = new RenderPipeline(frameBuffer);
            ImGuiSfml.Init(window);
            ImGui.LoadIniSettingsFromDisk("imgui.ini");
        }
    }
}