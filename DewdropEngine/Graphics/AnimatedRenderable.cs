namespace DewDrop.Graphics;

/// <summary>
///     An abstract class for animated graphics
/// </summary>
public abstract class AnimatedRenderable : Renderable
{
    /// <summary>
    ///     How many frames this animated graphic has
    /// </summary>
    public int Frames
    {
        get => frames;
        protected set => frames = value;
    }
    /// <summary>
    ///     The frame this animated graphic is on
    /// </summary>
    public float Frame
    {
        get => frame;
        set => frame = Math.Max(0f, Math.Min(frames, value));
    }
    /// <summary>
    ///     The duration of various frames
    /// </summary>
    public float[] Speeds
    {
        get => speeds;
        protected set => speeds = value;
    }

    /// <summary>
    ///     Speed modifier ( this is multiplied by the current speed )
    /// </summary>
    public float SpeedModifier
    {
        get => speedModifier;
        set => speedModifier = value;
    }
    /// <summary>
    ///     Invoked when the renderable has finished an animation.
    /// </summary>
    public event AnimationCompleteHandler OnAnimationComplete;

    protected void AnimationComplete()
    {
        OnAnimationComplete?.Invoke(this);
    }

    protected int frames;
    protected float frame;
    protected float speedIndex;
    protected float[] speeds;
    protected float speedModifier;

    public delegate void AnimationCompleteHandler(AnimatedRenderable renderable);
}