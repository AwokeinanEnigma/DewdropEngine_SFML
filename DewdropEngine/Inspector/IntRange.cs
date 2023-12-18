namespace DewDrop.Inspector;
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
