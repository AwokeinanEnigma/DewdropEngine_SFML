namespace DewDrop.Scenes.Transitions;

public class InstantTransition : ITransition
{
    public bool IsComplete => true;

    public float Progress => 1f;

    public bool ShowNewScene => true;

    public bool Blocking { get; set; }

    public void Update()
    {
    }

    public void Draw()
    {
    }

    public void Reset()
    {
    }

    public void Destroy()
    {
    }
}