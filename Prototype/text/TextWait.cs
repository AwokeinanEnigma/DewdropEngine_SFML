using System;

namespace Mother4.Scripts.Text; 

internal class TextWait : ITextCommand
{
	public int Position
	{
		get => position;
		set => position = value;
	}

	public TextWait(int position)
	{
		this.position = position;
	}

	private int position;
}