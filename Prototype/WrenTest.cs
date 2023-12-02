using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;

public class WrenTest
{
	public void Log (string message) {
		Outer.LogESL(message);
	}
	public void LogWarning (string message) {
		Outer.LogWarning(message);
	}
	public void LogError (string? message) {
		Outer.LogError(message, null);
	}
}
	