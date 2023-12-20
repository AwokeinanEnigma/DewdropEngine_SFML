using System;

namespace Mother4.Scripts.Text; 

internal class TextPause : ITextCommand
{
	public int Position
	{
		get => _position;
		set => _position = value;
	}

	public int Duration
	{
		get => _duration;
		
	}

	public TextPause(int position, int duration)
	{
		this._position = position;
		this._duration = duration;
	}

	int _position;
	readonly int _duration;
}