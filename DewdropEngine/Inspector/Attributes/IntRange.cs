namespace DewDrop.Inspector.Attributes; 

[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
public class IntegerRangeAttribute : Attribute
{
	public IntegerRangeAttribute (int min, int max)
	{
		Min = min;
		Max = max;
	}
	
	public int Min { get; set; }
	public int Max { get; set; }

}
