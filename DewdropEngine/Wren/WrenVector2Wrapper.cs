using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using System.Runtime.CompilerServices;
namespace DewDrop.Wren;

[WrenClass("Vector2")]
public class WrenVector2Wrapper {
	public Vector2 Vector;

	//IronWren needs this to be here or else it'll throw an exception
	// You can put wren code here if you want
	private const string constructorCode = "";

	// this is a wren method
	[WrenCode]
	private const string print = "print() {\nSystem.print(\"Vector (%(x), %(y))\")\n}";

	double X {
		get {
			return Vector.x;
		}
	}
	double Y {
		get {
			return Vector.y;
		}
	}

	public WrenVector2Wrapper (Vector2 vector) {
		Outer.Log("invoked native c# constructor");
		Vector = vector;
	}
	public WrenVector2Wrapper (double x, double y) {
		Outer.Log("invoked native c# constructor");
		Vector = new Vector2((float)x, (float)y);
	}

	[WrenConstructor("x", "y", Code = "field:constructorCode")]
	private WrenVector2Wrapper (WrenVM vm) {
		Vector.x = (float)vm.GetSlotDouble(1);
		Vector.y = (float)vm.GetSlotDouble(2);
	}

	[WrenMethod("getLength", "vector")]
	private static void getLength (WrenVM vm) {
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);

		vm.SetSlotDouble(0, Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y)));
	}

	[WrenProperty(PropertyType.Get, "x")]
	private void GetX (WrenVM vm) {
		vm.SetSlotDouble(0, X);
	}

	[WrenProperty(PropertyType.Get, "y")]
	private void GetY (WrenVM vm) {
		vm.SetSlotDouble(0, Y);
	}

	[WrenProperty(PropertyType.Set, "x")]
	private void SetX (WrenVM vm) {
		Vector.x = (float)vm.GetSlotDouble(1);
	}

	[WrenProperty(PropertyType.Set, "y")]
	private void SetY (WrenVM vm) {
		Vector.y = (float)vm.GetSlotDouble(1);
	}
	
	//[WrenFinalizer]
	private void wrenFinalize()
	{
		Outer.Log("Vector Finalized!");
	}
}
