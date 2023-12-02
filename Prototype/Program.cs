using DewDrop;
using DewDrop.Utilities;
using DewDrop.Scenes;
using DewDrop.UserInput;
using DewDrop.Wren;
using Prototype;
using System.Reflection;

namespace RotatingHelloWorldSfmlDotNetCoreCSharp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            WrenManager.CollectTypes += (types) => {
                types.AddRange(
                    WrenManager.FindWrenTypes(Assembly.GetExecutingAssembly()).ToArray()
                    );
            };
            Engine.Initialize();

            SceneManager.Instance.Push(new DebugPlayground(true));
            Engine.DebugMode = true;
            Engine.Window.SetMouseCursorVisible(false);
            
            new AxisManager();
 
            Console.WriteLine("Press ESC key to close window");
            Engine.StartGameLoop();
        }
    }
}