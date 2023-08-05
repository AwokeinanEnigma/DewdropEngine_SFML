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
            Engine.DebugMode = true;
            Engine.Window.SetMouseCursorVisible(false);
            
            new AxisManager();
 
            Console.WriteLine("Press ESC key to close window");
            Engine.StartGameLoop();
        }
    }
}