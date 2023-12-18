namespace DewDrop.Inspector; 

public class ButtonMethodAttribute : Attribute{
	public string MethodName { get; set; }
	public ButtonMethodAttribute (string methodName)
	{
		MethodName = methodName;
	}
}
