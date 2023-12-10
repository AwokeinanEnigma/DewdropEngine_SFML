namespace DewDrop.Scenes;

/// <summary>
///     Generic exception used if the scene stack is empty.
/// </summary>
class EmptySceneStackException : Exception {
	public EmptySceneStackException () : base("The scene stack is empty!") {
	}
}
