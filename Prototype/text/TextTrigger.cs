using System;

namespace Mother4.Scripts.Text; 

public class TextTrigger : ITextCommand
{
	public int Position
	{
		get => _position;
		set => _position = value;
	}

	public int Type
	{
		get => _type;
	}

	public string[] Data
	{
		get => this._data;
		
	}

	public TextTrigger(int position, int type, string[] data)
	{
		this._position = position;
		this._type = type;
		this._data = data;
	}

	int _position;
	readonly int _type;
	readonly string[] _data;
}