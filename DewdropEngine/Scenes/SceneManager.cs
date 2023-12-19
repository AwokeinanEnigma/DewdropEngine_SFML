#region

using DewDrop.Scenes.Transitions;
using DewDrop.Utilities;
using ImGuiNET;

#endregion

namespace DewDrop.Scenes;

/// <summary>
///     Manager for scenes, handling transitions to new scenes and such.
/// </summary>
public static class SceneManager {
    /// <summary>
    ///     Manages the stack of scenes.
    /// </summary>
    class SceneStack { 
	    List<SceneBase> list;

		public SceneBase this [int i] => list[i];

        /// <summary>
        ///     The amount of scenes in the stack
        /// </summary>
        public  int Count => list.Count;

        /// <summary>
        ///     Creates a new scene stack.
        /// </summary>
        public  SceneStack () {
			list = new List<SceneBase>();
		}

        /// <summary>
        ///     Clears the entire scene list
        /// </summary>
        public void Clear () {
			list.Clear();
		}

        /// <summary>
        ///     Adds a scene to the top of the list
        /// </summary>
        /// <param name="scene">The scene to add to the top of the scene list</param>
        public void Push (SceneBase scene) {
			list.Add(scene);
		}

        /// <summary>
        ///     Gets the scene from the bottom of the scene list
        /// </summary>
        /// <returns>The scene at the bottom of the scene list</returns>
        public  SceneBase Peek () {
			return Peek(0);
		}

		public  SceneBase? Peek (int i) {
			//if we're outside of the list of scenes
			if (i < 0 || i >= list.Count) {
				return null;
			}

			// return the scene from the list
			return list[list.Count - i - 1];
		}

        /// <summary>
        ///     Gets a scene from the top of the scene list
        /// </summary>
        /// <returns>The scene at the top of the scene list</returns>
        public SceneBase Pop () {
			// create result
			SceneBase? result = null;

			if (list.Count > 0) {
				// go to top of list
				result = list[list.Count - 1];
				// remove entry at top of list
				list.RemoveAt(list.Count - 1);
			}

			// return scene
			return result;
		}
	}

	enum SceneManagerState {
		Scene,
		Transition
	}

	#region Properties

	/// <summary>
    ///     The transition that is currently being used.
    /// </summary>
    public static ITransition Transition { get; set; }

    /// <summary>
    ///     If true, the scene manager is currently transitioning between scenes
    /// </summary>
    public static bool IsTransitioning => state == SceneManagerState.Transition;

    /// <summary>
    ///     Are we not displaying a scene?
    /// </summary>
    public static bool IsEmpty { get; private set; }

    /// <summary>
    ///     Are we drawing two scenes at once?
    /// </summary>
    public static bool CompositeMode { get; set; }

	#endregion

	#region Scene related fields
	static SceneManagerState state;

	static SceneStack scenes;

	static SceneBase previousScene;

	#endregion

	#region Boolean fields.

	// if popped then we'll completely clean up the previous scene
	static bool popped;

	// if true then the new scene is currently being shown
	static bool newSceneShown;

	// if false and we're transitioning we need to clean up our shit
	static bool cleanupFlag;

	// if true we're drawing multiple scenes

	#endregion

	#region Methods
	
	internal static void Initialize ( SceneBase startScene ) {
	    // make new scenestack
	    scenes = new SceneStack();
	    // we have no scenes so we set this to true
	    IsEmpty = true;
	    // push the start scene
	    // no transition so just use the empty one
	    Transition = new InstantTransition();
	    Push(startScene);
	    // even though we have no scenes, still set the scene state to Scene
	    state = SceneManagerState.Transition;
	    /*Engine.OnRenderImGui += () => {
		    ImGui.Begin("Scene Manager");
		    ImGui.Text($"Scene state: {state}");
		    ImGui.End();
	    };*/
    }

    /// <summary>
    ///     Pushes a new scene to the stack
    /// </summary>
    /// <param name="newScene">The scene to push to the stack</param>
    public static void Push (SceneBase newScene) {
		Push(newScene, false);
	}

	public static void Push (SceneBase newScene, bool swap) {
		if (state != SceneManagerState.Transition) {
			// if we have other scenes
			if (scenes.Count > 0) {
				previousScene = swap ? scenes.Pop() : scenes.Peek();
				popped = swap;
			}

			// push this scene to the top
			scenes.Push(newScene);
			// transition
			SetupTransition();
			// we're not empty
			IsEmpty = false;
		}
	}

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static SceneBase Pop () {
		if (scenes.Count > 0) {
			// get scene from the top
			SceneBase result = scenes.Pop();
			// we've popped!
			popped = true;

			// ???
			if (scenes.Count > 0) {
				scenes.Peek();
				SetupTransition();
			}
			// if we don't have any other scenes, we're empty
			else {
				IsEmpty = true;
			}

			previousScene = result;
			return result;
		}

		// if our scene list is empty, throw an exception
		throw new EmptySceneStackException();
	}

	static void SetupTransition () {
		// reset transition
		Transition.Reset();
		// set our state
		state = SceneManagerState.Transition;

		// disable input
		// InputManager.Instance.Enabled = false;
	}

	public static SceneBase Peek () {
		if (scenes.Count > 0) {
			return scenes.Peek();
		}

		throw new EmptySceneStackException();
	}

    /// <summary>
    ///     Clears the scene list
    /// </summary>
    public static void Clear () {
		SceneBase scene = scenes.Peek();
		while (scenes.Count > 0) {
			SceneBase scene2 = scenes.Pop();
			if (scene2 == scene) {
				scene2.Unfocus();
			}
			scene2.Dispose();
		}
	}

	public static void Update () {
		UpdateScene();
		if (state == SceneManagerState.Transition) {
			UpdateTransition();
		}
	}

	static void UpdateScene () {
		if (scenes.Count > 0) {
			SceneBase scene = scenes.Peek();
			scene.Update();
			return;
		}

		throw new EmptySceneStackException();
	}

	static void UpdateTransition () {
		// if we haven't shown a new scene but you can
		if (!newSceneShown && Transition.ShowNewScene) {
			// and if there is a previous scene
			if (previousScene != null) {
				// if popped is true,
				// then we completely dispose of the previous scene
				if (popped) {
					previousScene.Dispose();
					popped = false;
				}
				// if it isn't
				else {
					// just unfocus it lmao
					previousScene.Unfocus();
				}
			}

			// get the new scene which is at the bottom of the list
			SceneBase scene = scenes.Peek();

			// focus!
			scene.Focus();

			// set previous scene to null
			previousScene = null;

			// our new scene is shown and we can finally tell the world.
			// lmao.
			newSceneShown = true;
		}

		// a transition can allow the scene manager to show a new scene without being complete
		// like for example, if you spawn a bunch of shit on screen for a transition
		// then half way through you want to show the new scene behind all that shit
		// and then remove all that shit to reveal the scene
		if (!Transition.IsComplete) {
			// update transition
			Transition.Update();

			// update previous scene if this transition isn't blocking
			if (!Transition.Blocking && previousScene != null) {
				previousScene.Update();
			}

			// progress ranges between 0 & 1
			// 0.5 is halfway
			// so what this is saying is "Okay if the transition is halfway done let's clean up our shit and make sure we don't clean up our shit again"		
			if (Transition.Progress > 0.5f && !cleanupFlag) {
				// TextureManager.Instance.Purge();
				GC.Collect();
				// transition.Destroy();
				cleanupFlag = true;
			}
		}
		// but if the transition is complete, then we've already cleaned up our shit
		// and we can set the state of the Scene Manager to scene
		else {
			state = SceneManagerState.Scene;
			newSceneShown = false;

			Peek().TransitionIn();
			//InputManager.Instance.Enabled = true;

			// make sure upon a new transition we can clean up our shit again
			cleanupFlag = false;
		}
	}

	public static void AbortTransition () {
		if (state == SceneManagerState.Transition) {
			if (previousScene != null) {
				previousScene.Dispose();
				previousScene = null;
			}

			if (!newSceneShown) {
				SceneBase scene = scenes.Peek();
				scene.Focus();
			}

			state = SceneManagerState.Scene;
			newSceneShown = false;

			//InputManager.Instance.Enabled = true;

			cleanupFlag = false;
		}
	}

	static void CompositeDraw () {
		int count = scenes.Count;
		for (int i = 0; i < count - 1; i++) {
			if (scenes[i + 1].DrawBehind) {
				scenes[i].Draw();
			}
		}
	}

	public static void Draw () {
		if (scenes.Count > 0) {
			if (Transition.ShowNewScene) {
				if (CompositeMode) {
					CompositeDraw();
				}

				SceneBase scene = scenes.Peek();
				scene.Draw();
			} else if (previousScene != null) {
				previousScene.Draw();
			}

			if (!Transition.IsComplete) {
				Transition.Draw();
			}
		}
	}

	#endregion
}
