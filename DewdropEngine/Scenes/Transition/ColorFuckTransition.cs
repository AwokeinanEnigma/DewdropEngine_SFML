#region

using DewDrop.GUI;
using SFML.Graphics;
using SFML.System;

#endregion
namespace DewDrop.Scenes.Transitions;

public class ColorFuckTransition : ITransition {
	public bool IsComplete { get; private set; }

	public float Progress { get; private set; }

	public bool ShowNewScene {
		get => Progress > 0.5f;
	}

	public bool Blocking { get; set; }

	float duration;
	Color[] givenColor;

	const int STEPS = 10;

	float speed;

	RenderTarget target;

	Vertex[] verts;

	RenderStates renderStates;

	public ColorFuckTransition (float duration, Color[] colors) {
		this.duration = duration;
		givenColor = colors;
		Initialize();
	}
	void Initialize () {
		float num = 60f*duration;
		speed = 1f/num;
		IsComplete = false;
		Progress = 0f;
		target = Engine.RenderTexture;
		float num2 = 160f;
		float num3 = 90f;
		verts = new Vertex[4];
		verts[0] = new Vertex(new Vector2f(-num2, -num3), givenColor[0]);
		verts[1] = new Vertex(new Vector2f(num2, -num3), givenColor[1]);
		verts[2] = new Vertex(new Vector2f(num2, num3), givenColor[2]);
		verts[3] = new Vertex(new Vector2f(-num2, num3), givenColor[3]);
		Transform transform = new Transform(1f, 0f, ViewManager.Instance.FinalCenter.X, 0f, 1f, ViewManager.Instance.FinalCenter.Y, 0f, 0f, 1f);
		renderStates = new RenderStates(transform);
	}

	public void Update () {
		Progress += speed;
		IsComplete = Progress > 1f;
		byte b = (byte)(255.0*(Math.Cos(Progress*2f*Math.PI + Math.PI)/2.0 + 0.5));
		b /= 25;
		b *= 25;
		verts[0].Color.A = b;
		verts[1].Color.A = b;
		verts[2].Color.A = b;
		verts[3].Color.A = b;
	}

	public void Draw () {
		renderStates.Transform = new Transform(1f, 0f, ViewManager.Instance.FinalCenter.X, 0f, 1f, ViewManager.Instance.FinalCenter.Y, 0f, 0f, 1f);
		target.Draw(verts, PrimitiveType.Quads, renderStates);
	}

	public void Reset () {

		IsComplete = false;
		Progress = 0f;
		verts[0].Color.A = 0;
		verts[1].Color.A = 0;
		verts[2].Color.A = 0;
		verts[3].Color.A = 0;
	}

	public void Destroy () {
		Array.Clear(verts, 0, verts.Length);
		verts = null;
	}
}
