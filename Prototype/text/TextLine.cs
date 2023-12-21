using System;

namespace Mother4.Scripts.Text; 

public class TextLine
{
	readonly string _text;
	readonly bool _bullet;
	readonly ITextCommand[] _commands;

	public string Text
	{
		get => _text;
	}

	public bool HasBullet
	{
		get => _bullet;
	}

	public ITextCommand[] Commands
	{
		get => _commands;
	}

	public TextLine(bool bullet, ITextCommand[] commands, string text)
	{
		this._bullet = bullet;
		this._commands = commands;
		this._text = text;
	}
}