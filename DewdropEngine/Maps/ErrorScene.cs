using DewDrop.Graphics;
using DewDrop.GUI;
using DewDrop.GUI.Fonts;
using DewDrop.Scenes;
using DewDrop.Utilities;
namespace DewDrop.Scenes; 

public class ErrorScene : SceneBase {
	TextRenderer _extra;
	readonly RenderPipeline _pipeline;
	public ErrorScene (Exception e) {
		var main = new TextRenderer(new Vector2(4, 0), 0, new FontData(), $"Program '{Engine.ApplicationData.Name}' has encountered an " + Environment.NewLine + "unrecoverable error.");
		var error = new TextRenderer(new Vector2(4, 45), 0, new FontData(), "The error was: " + Environment.NewLine + e.Message + Environment.NewLine);
		var contact = new TextRenderer(new Vector2(4, 90), 0, new FontData(), $"Please report this error to {Engine.ApplicationData.Developer}.");
		_pipeline = new RenderPipeline(Engine.RenderTexture);
		_pipeline.Add(main);
		_pipeline.Add(error);
		_pipeline.Add(contact);
	}

	public override void Focus () {
		base.Focus();
		ViewManager.Instance.Center = new Vector2(160, 90);
		;
	}
	public override void Draw () {
		_pipeline.Draw();
		base.Draw();
	}
}
