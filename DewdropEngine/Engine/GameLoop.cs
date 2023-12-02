#region

using DewDrop.GUI;
using DewDrop.Scenes;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

#endregion

namespace DewDrop;

public static partial class Engine {
	static Time _deltaTime;
	static Clock _frameTimer;
	static Clock _deltaTimeClock;

	static float _deltaTimeFloat = 1;
	static int _frameLoops;
	static float _maxDeltaTime = 0.25f;
	static float _accumulator;
	static float _sixty_fps = 1.0f/60.0f;
	static float _technically_sixty_fps = 1.0f/59.0f;
	static float time;
	static float lastTime;
	public static event Action RenderImGUI;

	public static void StartGameLoop () {
		_deltaTimeClock = new Clock();
		_frameTimer = new Clock();

		time = _frameTimer.ElapsedTime.AsSeconds();
		lastTime = time;

		try {

			while (Window.IsOpen) {
				time = _frameTimer.ElapsedTime.AsSeconds();
				_deltaTimeFloat = time - lastTime;
				lastTime = time;


				if (_deltaTimeFloat > _maxDeltaTime) {
					Outer.LogWarning($"Passed the threshold for max deltaTime, deltaTime is {_deltaTime}, lastTime is {lastTime}");
					_deltaTimeFloat = _maxDeltaTime;
				}

				_accumulator += _deltaTimeFloat;
				/*deltaText.DisplayedString = $"d {_deltaTime}" + Environment.NewLine +
				                            $"a {_accumulator}";*/
				_frameLoops = 0;


				while (_accumulator >= _technically_sixty_fps) {
					if (_frameLoops >= 5) {
						/*
						 * Here's possible causes as to why this would be triggered:
						 *
						 * The user tabbed in and back out of the game. SFML gets weird when you lose focus.
						 * The game is taking a frame or two to load something
						 * An error has occurred
						 * The user's computer cannot keep up with the game
						 * 
						*/

						Outer.LogWarning($"Resyncing, accumulator is {_accumulator}, and loop count is {_frameLoops}. See comments above this line in Program.cs for more info.");
						_accumulator = 0.0f;
						break;
					}

					Update();
					Render();

					_accumulator -= _sixty_fps;
					_frameLoops++;

				}
			}
		} catch (Exception value) {

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

	public static event Action  OnFocusLost;
	public static event Action  OnFocusGained;
	static bool _HadFocus = true;
	static bool _HasFocus = false;
	public static void Update () {

		// Update our audio as soon as possible
		//AudioManager.Instance.Update();

		// This makes input and other events from the window work
		Window.DispatchEvents();

		// Update crucial game instances
		SceneManager.Instance.Update();

		ViewManager.Instance.Update();
		ViewManager.Instance.UseView();

		// OpenGL shit, we have to clear our frame buffer before we can draw to it
		RenderTexture.Clear(Color.Black);
		//Finally, draw our scene.
		SceneManager.Instance.Draw();
		// Draw over our scene.
		if (_debugMode) {
			DebugPipeline.Draw();
		}

		if (_debugMode) {
			ImGuiSfml.Update(Window, _deltaTimeFloat);
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
			RenderImGUI?.Invoke();
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
