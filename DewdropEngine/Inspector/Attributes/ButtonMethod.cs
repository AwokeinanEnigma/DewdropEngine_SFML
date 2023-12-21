namespace DewDrop.Inspector.Attributes; 

[AttributeUsage (AttributeTargets.Method)]
public class ButtonMethodAttribute : Attribute {
	public string MethodName { get; set; }
	public ButtonMethodAttribute (string methodName)
	{
		MethodName = methodName;
	}
}
