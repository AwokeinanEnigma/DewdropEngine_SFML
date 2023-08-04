using System;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using DewDrop;
using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.GUI.Fonts;
using DewDrop.Utilities;
using DewDrop.Scenes;
using Prototype.Scenes;
using DewDrop.Resources;
using System.Reflection;
using DewDrop.Scenes.Transitions;
using DewDrop.UserInput;
using ImGuiNET;
using Prototype;

namespace RotatingHelloWorldSfmlDotNetCoreCSharp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Engine.Initialize();
            SceneManager.Instance.Push(new GUIText());
            DDDebug.Log("1");
            //SceneManager.Instance.Push(new TileScene());
            Engine.DebugMode = false;
            Engine.Window.SetMouseCursorVisible(false);

            
            DDDebug.Log("3");
      
            new AxisManager();
            DDDebug.Log("5");
 
            Console.WriteLine("Press ESC key to close window");
            DDDebug.Log("7");
            Engine.StartGameLoop();
            DDDebug.Log("9");
        }
    }
}