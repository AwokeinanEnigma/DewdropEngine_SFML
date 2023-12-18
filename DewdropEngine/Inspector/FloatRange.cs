namespace DewDrop.Inspector; 

public class FloatRangeAttribute : Attribute {
	public float Min { get; set; }
	public float Max { get; set; }
	public FloatRangeAttribute (float min, float max)
	{
		Min = min;
		Max = max;
	}
}
