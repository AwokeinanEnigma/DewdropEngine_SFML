#region

using DewDrop.Scenes.Transitions;

#endregion

namespace DewDrop.Scenes;

/// <summary>
///     Manager for scenes, handling transitions to new scenes and such.
/// </summary>
public class SceneManager
{
    /// <summary>
    ///     Manages the stack of scenes.
    /// </summary>
    private class SceneStack
    {
        private List<SceneBase> list;

        public SceneBase this[int i] => list[i];

        /// <summary>
        ///     The amount of scenes in the stack
        /// </summary>
        public int Count => list.Count;

        /// <summary>
        ///     Creates a new scene stack.
        /// </summary>
        public SceneStack()
        {
            list = new List<SceneBase>();
        }

        /// <summary>
        ///     Clears the entire scene list
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }

        /// <summary>
        ///     Adds a scene to the top of the list
        /// </summary>
        /// <param name="scene">The scene to add to the top of the scene list</param>
        public void Push(SceneBase scene)
        {
            list.Add(scene);
        }

        /// <summary>
        ///     Gets the scene from the bottom of the scene list
        /// </summary>
        /// <returns>The scene at the bottom of the scene list</returns>
        public SceneBase Peek()
        {
            return Peek(0);
        }

        public SceneBase Peek(int i)
        {
            //if we're outside of the list of scenes
            if (i < 0 || i >= list.Count)
            {
                return null;
            }

            // return the scene from the list
            return list[list.Count - i - 1];
        }

        /// <summary>
        ///     Gets a scene from the top of the scene list
        /// </summary>
        /// <returns>The scene at the top of the scene list</returns>
        public SceneBase Pop()
        {
            // create result
            SceneBase result = null;

            if (list.Count > 0)
            {
                // go to top of list
                result = list[list.Count - 1];
                // remove entry at top of list
                list.RemoveAt(list.Count - 1);
            }

            // return scene
            return result;
        }
    }

    private enum SceneManagerState
    {
        Scene,
        Transition
    }

    #region Properties

    /// <summary>
    ///     The active instance of the SceneManager
    /// </summary>
    public static SceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SceneManager();
            }

            return instance;
        }
    }

    /// <summary>
    ///     The transition that is currently being used.
    /// </summary>
    public ITransition Transition
    {
        get => transition;
        set => transition = value;
    }

    /// <summary>
    ///     If true, the scene manager is currently transitioning between scenes
    /// </summary>
    public bool IsTransitioning => state == SceneManagerState.Transition;

    /// <summary>
    ///     Are we not displaying a scene?
    /// </summary>
    public bool IsEmpty => isEmpty;

    /// <summary>
    ///     Are we drawing two scenes at once?
    /// </summary>
    public bool CompositeMode
    {
        get => isCompositeMode;
        set => isCompositeMode = value;
    }

    #endregion

    #region Scene related fields

    private static SceneManager instance;

    private SceneManagerState state;

    private SceneStack scenes;

    private SceneBase previousScene;

    private ITransition transition;

    #endregion

    #region Boolean fields.

    private bool isEmpty;

    // if popped then we'll completely clean up the previous scene
    private bool popped;

    // if true then the new scene is currently being shown
    private bool newSceneShown;

    // if false and we're transitioning we need to clean up our shit
    private bool cleanupFlag;

    // if true we're drawing multiple scenes
    private bool isCompositeMode;

    #endregion

    #region Methods

    /// <summary>
    ///     Creates a new scene manager.
    /// </summary>
    private SceneManager()
    {
        // make new scenestack
        scenes = new SceneStack();
        // we have no scenes so we set this to true
        isEmpty = true;
        // no transition so just use the empty one
        transition = new InstantTransition();
        // even though we have no scenes, still set the scene state to Scene
        state = SceneManagerState.Scene;
    }

    /// <summary>
    ///     Pushes a new scene to the stack
    /// </summary>
    /// <param name="newScene">The scene to push to the stack</param>
    public void Push(SceneBase newScene)
    {
        Push(newScene, false);
    }

    public void Push(SceneBase newScene, bool swap)
    {
        if (state != SceneManagerState.Transition)
        {
            // if we have other scenes
            if (scenes.Count > 0)
            {
                previousScene = swap ? scenes.Pop() : scenes.Peek();
                popped = swap;
            }

            // push this scene to the top
            scenes.Push(newScene);
            // transition
            SetupTransition();
            // we're not empty
            isEmpty = false;
        }
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public SceneBase Pop()
    {
        if (scenes.Count > 0)
        {
            // get scene from the top
            SceneBase result = scenes.Pop();
            // we've popped!
            popped = true;

            // ???
            if (scenes.Count > 0)
            {
                scenes.Peek();
                SetupTransition();
            }
            // if we don't have any other scenes, we're empty
            else
            {
                isEmpty = true;
            }

            previousScene = result;
            return result;
        }

        // if our scene list is empty, throw an exception
        throw new EmptySceneStackException();
    }

    private void SetupTransition()
    {
        // reset transition
        transition.Reset();
        // set our state
        state = SceneManagerState.Transition;

        // disable input
        // InputManager.Instance.Enabled = false;
    }

    public SceneBase Peek()
    {
        if (scenes.Count > 0)
        {
            return scenes.Peek();
        }

        throw new EmptySceneStackException();
    }

    /// <summary>
    ///     Clears the scene list
    /// </summary>
    public void Clear()
    {
        SceneBase scene = scenes.Peek();
        while (scenes.Count > 0)
        {
            SceneBase scene2 = scenes.Pop();
            if (scene2 == scene)
            {
                scene2.Unfocus();
            }

            scene2.Unload();
        }
    }

    public void Update()
    {
        UpdateScene();
        if (state == SceneManagerState.Transition)
        {
            UpdateTransition();
        }
    }

    private void UpdateScene()
    {
        if (scenes.Count > 0)
        {
            SceneBase scene = scenes.Peek();
            scene.Update();
            return;
        }

        throw new EmptySceneStackException();
    }

    private void UpdateTransition()
    {
        // if we haven't shown a new scene but you can
        if (!newSceneShown && transition.ShowNewScene)
        {
            // and if there is a previous scene
            if (previousScene != null)
            {
                // if popped is true,
                // then we completely dispose of the previous scene
                if (popped)
                {
                    previousScene.CompletelyUnload();
                    popped = false;
                }
                // if it isn't
                else
                {
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
        // like for example, if you spawn a bunch of hit on screen for a transition
        // then half way through you want to show the new scene behind all that shit
        // and then remove all that shit to reveal the scene
        if (!transition.IsComplete)
        {
            // update transition
            transition.Update();

            // update previous scene if this transition isn't blocking
            if (!transition.Blocking && previousScene != null)
            {
                previousScene.Update();
            }

            // progress ranges between 0 & 1
            // 0.5 is halfway
            // so what this is saying is "Okay if the transition is halfway done let's clean up our shit and make sure we don't clean up our shit again"
            if (transition.Progress > 0.5f && !cleanupFlag)
            {
                // TextureManager.Instance.Purge();
                GC.Collect();
                // transition.Destroy();
                cleanupFlag = true;
            }
        }
        // but if the transition is complete, then we've already cleaned up our shit
        // and we can set the state of the Scene Manager to scene
        else
        {
            state = SceneManagerState.Scene;
            newSceneShown = false;

            Peek().TransitionIn();
            //InputManager.Instance.Enabled = true;

            // make sure upon a new transition we can clean up our shit again
            cleanupFlag = false;
        }
    }

    public void AbortTransition()
    {
        if (state == SceneManagerState.Transition)
        {
            if (previousScene != null)
            {
                previousScene.CompletelyUnload();
                previousScene = null;
            }

            if (!newSceneShown)
            {
                SceneBase scene = scenes.Peek();
                scene.Focus();
            }

            state = SceneManagerState.Scene;
            newSceneShown = false;
            newSceneShown = false;

            //InputManager.Instance.Enabled = true;

            cleanupFlag = false;
        }
    }

    private void CompositeDraw()
    {
        int count = scenes.Count;
        for (int i = 0; i < count - 1; i++)
        {
            if (scenes[i + 1].DrawBehind)
            {
                scenes[i].Draw();
            }
        }
    }

    public void Draw()
    {
        if (scenes.Count > 0)
        {
            if (transition.ShowNewScene)
            {
                if (isCompositeMode)
                {
                    CompositeDraw();
                }

                SceneBase scene = scenes.Peek();
                scene.Draw();
            }
            else if (previousScene != null)
            {
                previousScene.Draw();
            }

            if (!transition.IsComplete)
            {
                transition.Draw();
            }
        }
    }

    #endregion
}