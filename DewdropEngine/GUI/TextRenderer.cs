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
/// Handles the rendering of text.
/// </summary>
public class TextRenderer : Renderable {
	/// <summary>
	/// Gets or sets the position of the TextRenderer.
	/// </summary>
	public override Vector2 RenderPosition {
		get => _position;
		set {
			_position = value;
			_drawText.Position = new Vector2f(_position.x + FontData.XCompensation, _position.y + FontData.YCompensation);
		}
	}
	/// <summary>
	/// Gets or sets the text to be rendered.
	/// </summary>
	public string Text {
		get => _text;
		set {
			_text = value;
			UpdateText();
		}
	}
	/// <summary>
	/// Gets or sets the color of the text.
	/// </summary>
	public Color Color {
		get => _drawText.FillColor;
		set {
			_drawText.FillColor = value;
			_colorDirty = true;
		}
	}
	/// <summary>
	/// Gets or sets the length of the text to be rendered.
	/// </summary>
	public int Length {
		get => _length;
		set {
			if (value != _length)
				_lengthDirty = true;

			_length = value;
			
		}
	}
	/// <summary>
	/// Gets or sets the index of the text to be rendered.
	/// </summary>
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
	/// <summary>
	/// Gets the font data of the text to be rendered.
	/// </summary>
	public FontData FontData { get; }

	bool _colorDirty;
	bool _lengthDirty;

	readonly RenderStates _renderStates;
	readonly Text _drawText;
	readonly Shader _shader;
	int _length;
	int _index;
	string _text;

	/// <summary>
	/// Initializes a new instance of the TextRenderer class with specified position, depth, font, text, index, and length.
	/// </summary>
	/// <param name="position">The position of the TextRenderer.</param>
	/// <param name="depth">The depth of the TextRenderer.</param>
	/// <param name="font">The font of the text.</param>
	/// <param name="text">The text to be rendered.</param>
	/// <param name="index">The index of the text to be rendered.</param>
	/// <param name="length">The length of the text to be rendered.</param>
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
	
	/// <summary>
	/// Initializes a new instance of the TextRenderer class with specified position, depth, and text.
	/// </summary>
	/// <param name="position">The position of the TextRenderer.</param>
	/// <param name="depth">The depth of the TextRenderer.</param>
	/// <param name="text">The text to be rendered.</param>
	public TextRenderer (Vector2 position, int depth, string text) : this(position, depth, new FontData(), text) { }

	/// <summary>
	/// Initializes a new instance of the TextRenderer class with specified position, depth, font, and text.
	/// </summary>
	/// <param name="position">The position of the TextRenderer.</param>
	/// <param name="depth">The depth of the TextRenderer.</param>
	/// <param name="font">The font of the text.</param>
	/// <param name="text">The text to be rendered.</param>
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

	/// <summary>
	/// Finds the position of a character at a specified index.
	/// </summary>
	/// <param name="index">The index of the character.</param>
	/// <returns>The position of the character.</returns>
	public Vector2 FindCharacterPosition (uint index) {
		uint safeIndex = Math.Max(0U, Math.Min((uint)_text.Length, index));
		return _drawText.FindCharacterPos(safeIndex);
	}

	/// <summary>
	/// Updates the text renderer's size.
	/// </summary>
	void UpdateText () {
		_drawText.DisplayedString = _text;
		FloatRect localBounds = _drawText.GetLocalBounds();

		float width = Math.Max(1f, localBounds.Width);
		float height = Math.Max(1f, localBounds.Height);
		_size = new Vector2(width, height);
	}
	
	/// <summary>
	/// Resets the text, index, and length to be rendered.
	/// </summary>
	/// <param name="text">The new text to be rendered.</param>
	/// <param name="index">The new index of the text to be rendered.</param>
	/// <param name="length">The new length of the text to be rendered.</param>
	public void Reset(string text, int index, int length)
	{
		this._text = text;
		_index = index;
		_length = length;
		this.UpdateText(index, length);
	}

	/// <summary>
	/// Updates the text to be rendered based on the specified index and length.
	/// </summary>
	/// <param name="index">The index of the text to be rendered.</param>
	/// <param name="length">The length of the text to be rendered.</param>
	private void UpdateText(int index, int length)
	{
		this._drawText.DisplayedString = this._text.Substring(index, length);
		FloatRect localBounds = this._drawText.GetLocalBounds();
		_size = new Vector2f(Math.Max(1f, localBounds.Width), Math.Max(16f, localBounds.Height));
	}
	
	/// <summary>
	/// Draws the text on the specified render target.
	/// </summary>
	/// <param name="target">The render target on which to draw.</param>
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

	/// <summary>
	/// Disposes of the TextRenderer and its resources.
	/// </summary>
	/// <param name="disposing">Indicates whether the TextRenderer is currently being disposed.</param>
	protected override void Dispose (bool disposing) {
		if (!_disposed && disposing) {
			_drawText.Dispose();
		}

		_disposed = true;
	}
}
	