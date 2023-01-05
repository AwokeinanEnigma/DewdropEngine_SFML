using DewDrop.GUI;
using DewDrop.Scenes.Transitions;
using DewDrop.Scenes;
using SFML.System;

namespace DewDrop
{
    public static partial class Engine
    {
        private static float _sixty_fps = 1.0f / 60.0f;
        private static float _technically_sixty_fps = 1.0f / 59.0f;

        private static Time _deltaTime;
        private static int _frameLoops = 0;
        private static float _maxDeltaTime = 0.25f;
        private static float _accumulator = 0;

        private static Clock frameTimer;

        private static Clock _deltaTimeClock;
        private static void StartGameLoop()
        {
            _deltaTimeClock = new Clock();

            // GameLoop();
        }

        public static void GameLoop()
        {
            Update();
            _deltaTime = _deltaTimeClock.Restart();
            Render();
        }

        public static void Update()
        {
            // This is wrapped in a try catch statement to detect errors and such.
            try
            {
                // Update our audio as soon as possible
                //AudioManager.Instance.Update();

                // This makes input and other events from the window work
                window.DispatchEvents();

                // Update cruciel game instances
                SceneManager.Instance.Update();
                ViewManager.Instance.Update();
                ViewManager.Instance.UseView();

                // OpenGL shit, we have to clear our frame buffer before we can draw to it
                //frameBuffer.Clear(Color.Blue);
                //Finally, draw our scene.
                SceneManager.Instance.Draw();
            }
            // If we catch an empty stack exception, we just quit. This is because there's no next scene to go to, the game is finished!
            catch (EmptySceneStackException)
            {
                //quit = true;
            }
            // If the exception is NOT an empty scene stack exception, we'll go to the error scene.
            catch (Exception ex)
            {
                SceneManager.Instance.AbortTransition();
                SceneManager.Instance.Clear();
                SceneManager.Instance.Transition = new InstantTransition();
                //SceneManager.Instance.Push(new ErrorScene(ex));
            }



            ViewManager.Instance.UseDefault();
        }

        public static int aaa;
        public static int aaaa;
        public static void Render()
        {


            frameBuffer.Display();
            window.Clear(SFML.Graphics.Color.Black);
            window.Draw(frameBufferVertexArray, frameBufferState);
            window.Display();

        }
    }
}