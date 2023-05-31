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
using ImGuiNET;

namespace RotatingHelloWorldSfmlDotNetCoreCSharp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Engine.Initialize();
            SceneManager.Instance.Push(new TestScene());
            Engine.DebugMode = true;
            EmbeddedResourcesHandler.AddEmbeddedResources(Assembly.GetExecutingAssembly(), "Prototype");
           GenericText ADA = new GenericText(new Vector2(143, 14), 100, new FontData(), "AAAAAAAAAAAAAAAAAAAAA");
            ADA.Color = Color.Blue;
            GenericText ADAA = new GenericText(new Vector2(146, 14), 90, new FontData(), "BBBBBBBBBBBBBBBBBBBB");
            ADAA.Color = Color.Red;
            GenericText ADAAA = new GenericText(new Vector2(149, 14), 80, new FontData(), "CCCCCCCCCCCCCCCCCCCCCC");
            ADAAA.Color = Color.Green;
            //GenericText ADA = new GenericText("BALLS ITCH MY BALLS ITCH ITCHY ITCHY!!!", 10, new Vector2(143, 14), new FontData());
            Debug.Log("2");
            Engine.DebugPipeline.Add(ADA);
            Engine.DebugPipeline.Add(ADAA);
            Engine.DebugPipeline.Add(ADAAA);
            Engine.Window.SetMouseCursorVisible(false);

            Console.WriteLine("Press ESC key to close window");
            Engine.StartGameLoop();
            

            Console.WriteLine("All done");
        }
    }
}