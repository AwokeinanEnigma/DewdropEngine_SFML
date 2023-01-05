using DewDrop.Graphics;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;
using System.Runtime.CompilerServices;

namespace DewDrop
{
    public static partial class Engine
    {
        private static float _sixty_fps = 1.0f / 60.0f;
        private static float _technically_sixty_fps = 1.0f / 59.0f;

        private static float _deltaTime = 1;
        private static int _frameLoops = 0;
        private static float _maxDeltaTime = 0.25f;
        private static float _accumulator = 0;

        private static Clock frameTimer;

        private static void StartGameLoop()
        {
            GameLoop();
        }

        private static void GameLoop()
        {
            frameTimer = new Clock();
            frameTimer.Restart();

            const int MAX_FRAMESKIP = 5;

            float time, lastTime;
            time = frameTimer.ElapsedTime.AsSeconds();
            lastTime = time;

            while (window.IsOpen)
            {
                time = frameTimer.ElapsedTime.AsSeconds();
                _deltaTime = time - lastTime;
                lastTime = time;


                if (_deltaTime > _maxDeltaTime)
                {
                    Debug.LogWarning($"Passed the threshold for max deltaTime, deltaTime is {_deltaTime}, lastTime is {lastTime}");
                    _deltaTime = _maxDeltaTime;
                }

                //_deltaTime = 0.01666666666F;

                _accumulator += _deltaTime;
                //deltaText.DisplayedString = $"d {_deltaTime}" + Environment.NewLine +
                //    $"a {_accumulator}";
                _frameLoops = 0;



                while (_accumulator >= _technically_sixty_fps)
                {
                    if (_frameLoops >= MAX_FRAMESKIP)
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

                        Debug.LogWarning($"Resyncing, accumulator is {_accumulator}, and loop count is {_frameLoops}. See comments above this line in GameLoop.cs for more info, Enigma.");
                        _accumulator = 0.0f;
                        break;
                    }

                    Debug.Log("roting");

                    debugPipeline.Draw();

                    frameBuffer.Display();
                    window.Clear(SFML.Graphics.Color.Black);
                    window.Draw(frameBufferVertexArray, frameBufferState);
                    window.Display();

                    _accumulator -= _sixty_fps;

                    _frameLoops++;
                }
            }
        }
    }
}