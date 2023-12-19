#region

using DewDrop.GUI;
using DewDrop.Scenes;
using DewDrop.Scenes.Transitions;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics;

#endregion

namespace DewDrop;

public static partial class Engine {
	static Time _DeltaTime;
	static Clock _FrameTimer;
	static Clock _DeltaTimeClock;
	static Stopwatch _FrameStopwatch;

	static float _DeltaTimeFloat = 1;
	static int _FrameLoops;
	const float MaxDeltaTime = 0.25f;
	static float _Accumulator;
	const float SixtyFps = 0.016666668F;
	const float TechnicallySixtyFps = 0.016949153F;
	static float _Time;
	static float _LastTime;
	public static event Action OnRenderImGui;
	static double _Fps;
	public static void StartGameLoop () {
		_DeltaTimeClock = new Clock();
		_FrameTimer = new Clock();

		_FrameStopwatch = Stopwatch.StartNew();
		_Time = _FrameTimer.ElapsedTime.AsSeconds();
		_LastTime = _Time;

			while (Window.IsOpen) {
				
				_Time = _FrameTimer.ElapsedTime.AsSeconds();
				_DeltaTimeFloat = _Time - _LastTime;
				_LastTime = _Time;


				if (_DeltaTimeFloat > MaxDeltaTime) {
					Outer.LogWarning($"Passed the threshold for max deltaTime, deltaTime is {_DeltaTime}, lastTime is {_LastTime}");
					_DeltaTimeFloat = MaxDeltaTime;
				}

				_Accumulator += _DeltaTimeFloat;
				
				/*deltaText.DisplayedString = $"d {_deltaTime}" + Environment.NewLine +
				                            $"a {_accumulator}";*/
				_FrameLoops = 0;

				while (_Accumulator >= TechnicallySixtyFps) {
					if (_FrameLoops >= 5) {
						/*
						 * Here's possible causes as to why this would be triggered:
						 *
						 * The user tabbed in and back out of the game. SFML gets weird when you lose focus.
						 * The game is taking a frame or two to load something
						 * An error has occurred
						 * The user's computer cannot keep up with the game
						 * 
						*/

						Outer.LogWarning($"Resyncing, accumulator is {_Accumulator}, and loop count is {_FrameLoops}. See comments above this line in Program.cs for more info.");
						_Accumulator = 0.0f;
						break;
					}

					_FrameStopwatch.Restart();
					try {
						Update();
						Render();
					}
					catch (Exception value) {
						SceneManager.AbortTransition();
						SceneManager.Clear();
						SceneManager.Transition = new InstantTransition();
						SceneManager.Push(new ErrorScene(value), true);
						
						StreamWriter streamWriter = new StreamWriter("error.log", true);
						streamWriter.WriteLine("At {0}:", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
						streamWriter.WriteLine(value);
						streamWriter.WriteLine();
						streamWriter.Close();
					}
					_FrameStopwatch.Stop();
					_Fps = 1.0f / _FrameStopwatch.Elapsed.Ticks * Stopwatch.Frequency;
					_Accumulator -= SixtyFps;
					_FrameLoops++;

				}
			}

		// GameLoop();
	}

	public static event Action  OnFocusLost;
	public static event Action  OnFocusGained;
	static bool _HadFocus = true;
	static bool _HasFocus = false;
	static Stopwatch FrameTimer = new Stopwatch();
	public static void Update () {

		// Update our audio as soon as possible
		//AudioManager.Instance.Update();

		// This makes input and other events from the window work
		Window.DispatchEvents();

		// Update crucial game instances
		SceneManager.Update();

		ViewManager.Instance.Update();
		ViewManager.Instance.UseView();

		// OpenGL shit, we have to clear our frame buffer before we can draw to it
		RenderTexture.Clear(Color.Black);
		//Finally, draw our scene.
		SceneManager.Draw();
		// Draw over our scene.
		if (_debugMode) {
			DebugPipeline.Draw();
		}

		if (_debugMode) {
			ImGuiSfml.Update(Window, _DeltaTimeFloat);
		}

		ViewManager.Instance.UseDefault();

		_HasFocus = Window.HasFocus();
		if (_HasFocus != _HadFocus) {
			if (!_HasFocus) {
				OnFocusLost?.Invoke();
			} else {
				OnFocusGained?.Invoke();
			}
		}
		_HadFocus = Window.HasFocus();
	}

	public static void Render () {


		RenderTexture.Display();
		Window.Clear(Color.Black);
		if (_debugMode) {
			OnRenderImGui?.Invoke();
		}

		Window.Draw(frameBufferVertexArray, frameBufferState);

		if (_debugMode) {
			ImGuiSfml.Render();
		}

		Window.Display();

		Frame++;
		// += 1;
	}
}
