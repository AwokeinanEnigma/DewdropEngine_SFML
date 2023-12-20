namespace DewDrop.Inspector.Attributes; 

public class ButtonMethodAttribute : Attribute {
	public string MethodName { get; set; }
	public ButtonMethodAttribute (string methodName)
	{
		MethodName = methodName;
	}
}
