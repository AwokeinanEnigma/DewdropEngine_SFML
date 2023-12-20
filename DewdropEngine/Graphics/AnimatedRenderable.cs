// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
namespace DewDrop.Graphics;

/// <summary>
///     An abstract class for animated graphics
/// </summary>
public abstract class AnimatedRenderable : Renderable {
    /// <summary>
    ///     How many frames this animated graphic has
    /// </summary>
    public int Frames {
		get => _frames;
		protected set => _frames = value;
	}
    /// <summary>
    ///     The frame this animated graphic is on
    /// </summary>
    public float Frame {
		get => _frame;
		set => _frame = Math.Max(0f, Math.Min(_frames, value));
	}
    /// <summary>
    ///     The duration of various frames
    /// </summary>
    public float[] Speeds {
		get => _speeds;
		protected set => _speeds = value;
	}

    /// <summary>
    ///     Speed modifier ( this is multiplied by the current speed )
    /// </summary>
    public float SpeedModifier {
		get => _speedModifier;
		set => _speedModifier = value;
	}
    /// <summary>
    ///     Invoked when the renderable has finished an animation.
    /// </summary>
    public event AnimationCompleteHandler OnAnimationComplete;

	protected void AnimationComplete () {
		OnAnimationComplete?.Invoke(this);
	}

	protected int _frames;
	protected float _frame;
	protected float _speedIndex;
	protected float[] _speeds;
	protected float _speedModifier;

	public delegate void AnimationCompleteHandler (AnimatedRenderable renderable);
}
