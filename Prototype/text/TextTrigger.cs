using System;

namespace Mother4.Scripts.Text; 

public class TextTrigger : ITextCommand
{
	public int Position
	{
		get => position;
		set => position = value;
	}

	public int Type
	{
		get => type;
	}

	public string[] Data
	{
		get => this.data;
		
	}

	public TextTrigger(int position, int type, string[] data)
	{
		this.position = position;
		this.type = type;
		this.data = data;
	}

	private int position;

	private int type;

	private string[] data;
}