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

namespace RotatingHelloWorldSfmlDotNetCoreCSharp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Engine.Initialize();
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
            MyWindow window = new MyWindow();
            window.Show();

            Console.WriteLine("All done");
        }
    }

    class MyWindow
    {
        public RenderPipeline pipeline;
        private float wobble;

        private float wobbleSpeed;

        private int wobbleDamp;
        public void Show()
        {
            Font font = new Font("C:/Windows/Fonts/arial.ttf");
            SFML.Graphics.Text text = new SFML.Graphics.Text("Hello World!", font);
            text.CharacterSize = 40;
            float textWidth = text.GetLocalBounds().Width;
            float textHeight = text.GetLocalBounds().Height;
            float xOffset = text.GetLocalBounds().Left;
            float yOffset = text.GetLocalBounds().Top;
            text.Origin = new Vector2f(textWidth / 2f + xOffset, textHeight / 2f + yOffset);
            text.Position = new Vector2f(Engine.RenderTexture.Size.X / 2f, Engine.RenderTexture.Size.Y / 2f);

            Debug.Log("0");
            pipeline = new RenderPipeline(Engine.RenderTexture);
            Debug.Log("1");

            Debug.Log("3");

            SceneManager.Instance.Push(new ErrorScene());

            Clock clock = new Clock();
            float delta = 0f;
            float angle = 0f;
            float angleSpeed = 90f;

            while (Engine.Window.IsOpen)
            {
                delta = clock.Restart().AsSeconds();
                angle += angleSpeed * delta;

                Engine.GameLoop();


            }

        }

    }
}