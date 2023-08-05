using DewDrop;
using DewDrop.Utilities;
using DewDrop.Scenes;
using DewDrop.UserInput;
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
            Engine.DebugMode = true;
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