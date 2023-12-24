/*using SFML.System;
using DewDrop.Entities;
using DewDrop.GUI;
using DewDrop.Utilities;
using SFML.Graphics;

namespace Prototype; 

public class TextWriter : RenderableEntity
{
	public override string Name { get; }
	public TextRenderer TextRenderer { get; set; }
	private string _text;
	private int CurrentCharacterIndex { get; set; }
	private float TextSpeed { get; set; }
	private Clock Clock { get; set; }

	private bool _writing;
	List<string> _lines = new();
	int _currentLine = 0;
	const int LineLength = 45;
	public TextWriter(Vector2 position, float textSpeed = 0.05f)
	{
		Position = position;
		Origin = new Vector2(0, 0);
		Depth = 50000;
		Visible = true;
		_size = new Vector2(0, 500);
		TextSpeed = textSpeed; // adjust this value to control the speed of the text
		Clock = new Clock();
	}

	public void Write(string text)
	{
		_text = text;
		// split the text into lines containing 45 characters each
		_lines = new List<string>();
		for (int i = 0; i < _text.Length; i += LineLength)
		{
			_lines.Add(_text.Substring(i, Math.Min(LineLength, _text.Length - i)));
		}
		_writing = true;
		CurrentCharacterIndex = 0;
		Clock.Restart();
	}

	public override void Update()
	{
		if (Clock.ElapsedTime.AsSeconds() >= TextSpeed && _writing)
		{
			if (CurrentCharacterIndex < _text.Length)
			{
				CurrentCharacterIndex++;
				if (CurrentCharacterIndex > _lines[_currentLine].Length)
				{
					_currentLine++;
					TextRenderer.Text += Environment.NewLine;
					_text = _lines[_currentLine];
					CurrentCharacterIndex = 0;
				}
				TextRenderer.Text = _text.Substring(0, CurrentCharacterIndex); // replace with the actual position
				Clock.Restart();
			}
			else
			{
				_writing = false;
			}
		}
	}
	public override void Draw (RenderTarget target) {
			
	}
}*/