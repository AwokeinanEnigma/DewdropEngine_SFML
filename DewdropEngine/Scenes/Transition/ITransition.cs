namespace DewDrop.Scenes.Transitions;

/// <summary>
///     Generic interface providing fields and methods for transitions.
/// </summary>
public interface ITransition {
    /// <summary>
    ///     If
    /// </summary>
    bool IsComplete { get; }

    /// <summary>
    ///     The progress of the transition
    /// </summary>
    float Progress { get; }

    /// <summary>
    ///     Allows the Scene Manager to show the new scene if true.
    /// </summary>
    bool ShowNewScene { get; }

    /// <summary>
    ///     If true, this transition is preventing the Scene Manager from progressing to the next scene.
    /// </summary>
    bool Blocking { get; set; }

    /// <summary>
    ///     This method is called every frame.
    /// </summary>
    void Update ();

    /// <summary>
    ///     Use this method to to draw whatever you need to draw.
    /// </summary>
    void Draw ();

    /// <summary>
    ///     Transitions are NOT initiated every time they're needed. Instead, they're restarted.
    ///     Do restart stuff here.
    /// </summary>
    void Reset ();
}
