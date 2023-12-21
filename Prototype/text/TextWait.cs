using System;

namespace Mother4.Scripts.Text; 

internal class TextWait : ITextCommand
{
	public int Position
	{
		get => _position;
		set => _position = value;
	}

	public TextWait(int position)
	{
		this._position = position;
	}

	int _position;
}