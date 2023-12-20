using System;
using System.Collections.Generic;

namespace Mother4.Scripts.Text; 

public class TextBlock
{
	public List<TextLine> Lines
	{
		get => _lines;
	}
	readonly List<TextLine> _lines;
	public TextBlock(List<TextLine> lines)
	{
		this._lines = new List<TextLine>(lines);
	}
}