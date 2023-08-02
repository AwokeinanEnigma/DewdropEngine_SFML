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
            SceneManager.Instance.Push(new TestScene());
            DDDebug.Log("1");
            //SceneManager.Instance.Push(new TileScene());
            Engine.DebugMode = true;
           // EmbeddedResourcesHandler.AddEmbeddedResources(Assembly.GetExecutingAssembly(), "Prototype");
           TextRenderer ADA = new TextRenderer(new Vector2(143, 14), 100, new FontData(), "AAAAAAAAAAAAAAAAAAAAA");
            ADA.Color = Color.Blue;
            TextRenderer ADAA = new TextRenderer(new Vector2(146, 14), 90, new FontData(), "AAAAAAAAAAAAAAAAAAAAA");
            ADAA.Color = Color.Red;
            TextRenderer ADAAA = new TextRenderer(new Vector2(149, 14), 80, new FontData(), "AAAAAAAAAAAAAAAAAAAAA");
            ADAAA.Color = Color.Cyan;
            //GenericText ADA = new GenericText("BALLS ITCH MY BALLS ITCH ITCHY ITCHY!!!", 10, new Vector2(143, 14), new FontData());
            DDDebug.Log("2");
            Engine.DebugPipeline.Add(ADA);
            Engine.DebugPipeline.Add(ADAA);
            Engine.DebugPipeline.Add(ADAAA);
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