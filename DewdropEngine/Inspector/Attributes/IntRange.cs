namespace DewDrop.Inspector.Attributes; 

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
