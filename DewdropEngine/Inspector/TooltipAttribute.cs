﻿namespace DewDrop.Inspector; 

public class TooltipAttribute : Attribute{
	public string Tooltip { get; set; }
	public TooltipAttribute (string tooltip)
	{
		Tooltip = tooltip;
	}
}