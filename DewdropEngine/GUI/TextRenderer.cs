#region

using DewDrop.Graphics;
using DewDrop.GUI.Fonts;
using DewDrop.Resources;
using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;

#endregion

namespace DewDrop.GUI;

/// <summary>
///     This is a renderable which simply displays text on the screen.
/// </summary>
public class TextRenderer : Renderable {
	public override Vector2 RenderPosition {
		get => _position;
		set {
			_position = value;
			_drawText.Position = new Vector2f(_position.x + FontData.XCompensation, _position.y + FontData.YCompensation);
		}
	}

	public string Text {
		get => _text;
		set {
			_text = value;
			UpdateText();
		}
	}

	public Color Color {
		get => _drawText.FillColor;
		set {
			_drawText.FillColor = value;
			_colorDirty = true;
		}
	}

	public int Length {
		get => _length;
		set {
			if (value != _length)
				_lengthDirty = true;

			_length = value;
			
		}
	}
	public int Index
	{
		get
		{
			return _index;
		}
		set
		{
			if (value != _index)
				_lengthDirty = true;
			_index = value;
		}
	}

	public FontData FontData { get; }

	bool _colorDirty;
	bool _lengthDirty;

	readonly RenderStates _renderStates;
	readonly Text _drawText;
	readonly Shader _shader;
	int _length;
	int _index;
	string _text;


	public TextRenderer (Vector2 position, int depth, FontData font, string text, int index, int length) {
		_position = position;
		this._text = text;

		_index = index;
		_length = length;

		_depth = depth;
		FontData = font;

		_drawText = new Text(string.Empty, FontData.Font, FontData.Size);
		_drawText.Position = new Vector2f(position.x + FontData.XCompensation, position.y + FontData.YCompensation);
		UpdateText();

		_shader = new Shader(EmbeddedResourcesHandler.GetResourceStream("text.vert"), null, EmbeddedResourcesHandler.GetResourceStream("text.frag"));
		_shader.SetUniform("color", new Vec4(_drawText.FillColor));
		_shader.SetUniform("threshold", font.AlphaThreshold);
		_renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, _shader);
	}
	public TextRenderer (Vector2 position, int depth, string text) : this(position, depth, new FontData(), text) { }
	public TextRenderer (Vector2 position, int depth, FontData font, string text) {
		_position = position;
		this._text = text;


		_depth = depth;
		FontData = font;

		_drawText = new Text(string.Empty, FontData.Font, FontData.Size);
		_drawText.Position = new Vector2f(position.x + FontData.XCompensation, position.y + FontData.YCompensation);
		UpdateText();

		_shader = new Shader(EmbeddedResourcesHandler.GetResourceStream("text.vert"), null, EmbeddedResourcesHandler.GetResourceStream("text.frag"));
		_shader.SetUniform("color", new Vec4(_drawText.FillColor));
		_shader.SetUniform("threshold", font.AlphaThreshold);
		_renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, null, _shader);
	}

	public Vector2 FindCharacterPosition (uint index) {
		uint num = Math.Max(0U, Math.Min((uint)_text.Length, index));
		return _drawText.FindCharacterPos(num);
	}

	void UpdateText () {
		_drawText.DisplayedString = _text;
		FloatRect localBounds = _drawText.GetLocalBounds();

		float width = Math.Max(1f, localBounds.Width);
		float height = Math.Max(1f, localBounds.Height);
		_size = new Vector2(width, height);
	}
	public void Reset(string text, int index, int length)
	{
		this._text = text;
		_index = index;
		_length = length;
		this.UpdateText(index, length);
	}

	private void UpdateText(int index, int length)
	{
		this._drawText.DisplayedString = this._text.Substring(index, length);
		FloatRect localBounds = this._drawText.GetLocalBounds();
		_size = new Vector2f(Math.Max(1f, localBounds.Width), Math.Max(16f, localBounds.Height));
	}
	
	public override void Draw (RenderTarget target) {
		if (_lengthDirty) {
			UpdateText(Index, Length);
			_lengthDirty = false;
		}
		if (_colorDirty) {
			_shader.SetUniform("color", new Vec4(_drawText.FillColor));
			_colorDirty = false;
		}

		target.Draw(_drawText, _renderStates);
	}

	protected override void Dispose (bool disposing) {
		if (!_disposed && disposing) {
			_drawText.Dispose();
		}

		_disposed = true;
	}
}
	