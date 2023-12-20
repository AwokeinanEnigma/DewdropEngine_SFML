namespace DewDrop.Inspector.Attributes;

[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
public class TooltipAttribute : Attribute{
	public string Tooltip { get; set; }
	public TooltipAttribute (string tooltip)
	{
		Tooltip = tooltip;
	}
}
