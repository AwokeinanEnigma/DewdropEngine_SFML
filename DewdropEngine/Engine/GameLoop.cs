using DewDrop.GUI;
using DewDrop.Scenes.Transitions;
using DewDrop.Scenes;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;

namespace DewDrop
{
    public static partial class Engine
    {

        private static Time _deltaTime;
        private static Clock _frameTimer;
        private static Clock _deltaTimeClock;
    
        private static float _deltaTimeFloat = 1;
        private static int _frameLoops = 0;
        private static float _maxDeltaTime = 0.25f;
        private static float _accumulator = 0;
        private static float _sixty_fps = 1.0f / 60.0f;
        private static float _technically_sixty_fps = 1.0f / 59.0f;
        private static float time;
        private static float lastTime;
        public static event Action RenderImGUI;
        
        public static void StartGameLoop()
        {
            _deltaTimeClock = new Clock();
            _frameTimer = new Clock();
            
            time = _frameTimer.ElapsedTime.AsSeconds();
            lastTime = time;

            try
            {
             
                while (window.IsOpen)
                {
                    time = _frameTimer.ElapsedTime.AsSeconds();
                    _deltaTimeFloat = time - lastTime;
                    lastTime = time;
                    
                    
                    if (_deltaTimeFloat > _maxDeltaTime)
                    { 
                        Debug.LogWarning($"Passed the threshold for max deltaTime, deltaTime is {_deltaTime}, lastTime is {lastTime}");
                        _deltaTimeFloat = _maxDeltaTime;
                    }
                    
                    _accumulator += _deltaTimeFloat;
                    /*deltaText.DisplayedString = $"d {_deltaTime}" + Environment.NewLine +
                                                $"a {_accumulator}";*/
                    _frameLoops = 0;



                    while (_accumulator >= _technically_sixty_fps)
                    {
                        if (_frameLoops >= 5)
                        {
                            /*
                             * Here's possible causes as to why this would be triggered:
                             *
                             * The user tabbed in and back out of the game. SFML gets weird when you lose focus.
                             * The game is taking a frame or two to load something
                             * An error has occurred
                             * The user's computer cannot keep up with the game
                             * 
                            */

                            Debug.LogWarning($"Resyncing, accumulator is {_accumulator}, and loop count is {_frameLoops}. See comments above this line in Program.cs for more info.");
                            _accumulator = 0.0f;
                            break;
                        }

                        Update();
                        Render();          
                        
                        _accumulator -= _sixty_fps;
                        _frameLoops++;
                        
                    }
                }
            }
            catch (Exception value)
            {

                StreamWriter streamWriter = new StreamWriter("error.log", true);
                streamWriter.WriteLine("At {0}:", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                streamWriter.WriteLine(value);
                streamWriter.WriteLine();
                streamWriter.Close();
                
                SceneManager.Instance.AbortTransition();
                SceneManager.Instance.Clear();
            }
            // GameLoop();
        }

        public static void GameLoop()
        {
            
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
                frameBuffer.Clear(Color.Black);
                //Finally, draw our scene.
                SceneManager.Instance.Draw();
                // Draw over our scene.
                if (debugMode)
                {
                    debugPipeline.Draw();
                }
                ImGuiSfml.Update(window, _maxDeltaTime);
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

        public static void Render()
        {


            frameBuffer.Display();
            window.Clear(SFML.Graphics.Color.Black);
            if (debugMode)
            {
                RenderImGUI?.Invoke();
            }
            window.Draw(frameBufferVertexArray, frameBufferState);
            ImGuiSfml.Render();
            window.Display();
        }
    }
}