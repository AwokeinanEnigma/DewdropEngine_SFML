﻿using DewDrop.Utilities;
namespace DewDrop.Scenes;

/// <summary>
///     A scene is basically a part of the game containing logic
/// </summary>
public abstract class SceneBase : IDisposable {
	#region Properties

	public bool DrawBehind { get; set; }

	public bool HasLoaded { get; set; }

	#endregion

	#region Booleans

	protected bool disposed;

	#endregion

	~SceneBase () {
		Dispose(false);
	}

	#region Methods

    /// <summary>
    ///     Called when the scene is first loaded.
    /// </summary>
    public virtual void Focus () {
	}

	public virtual void TransitionIn () {
	}

    /// <summary>
    ///     Called when the scene manager is composite mode and this is the scene that's being drawed over. Don't use this as
    ///     the place to dispose of resources!
    /// </summary>
    public virtual void Unfocus () {
	}

    /// <summary>
    ///     Called every frame.
    /// </summary>
    public virtual void Update () {
	}

    /// <summary>
    ///     This is where you can draw graphics.
    /// </summary>
    public virtual void Draw () {
	}
    
	public void Dispose () {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose (bool disposing) {
		if (!disposed) {
		}
		GC.Collect();
		disposed = true;
	}

	#endregion
}
