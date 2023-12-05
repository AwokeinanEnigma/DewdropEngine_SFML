using DewDrop.Utilities;
using IronWren;
using IronWren.AutoMapper;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace DewDrop.Wren;

[WrenClass("Vector2")]
public class WrenVector2Wrapper : IDisposable {
	public Vector2 Vector;

	//IronWren needs this to be here or else it'll throw an exception
	// You can put wren code here if you want
	private const string constructorCode = $"System.print(\"hello\")";

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

	[WrenMethod("New", "x", "y")]
	private static void New (WrenVM vm) {
		vm.EnsureSlots(2);
		var x = vm.GetSlotDouble(1);
		var y = vm.GetSlotDouble(2);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(x, y));
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
	
	[WrenMethod("Add", "vector")]
	private void Add (WrenVM vm) {
		vm.EnsureSlots(1);
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		Vector += vector.Vector;
	}
	
	[WrenMethod("Subtract", "vector")]
	private void Subtract (WrenVM vm) {
		vm.EnsureSlots(1);
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		Vector -= vector.Vector;
	}
	
	[WrenMethod("Multiply", "vector")]
	private void Multiply (WrenVM vm) {
		vm.EnsureSlots(1);
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		Vector *= vector.Vector;
	}
	
	[WrenMethod("Divide", "vector")]
	private void Divide (WrenVM vm) {
		vm.EnsureSlots(1);
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		Vector /= vector.Vector;
	}
	
	[WrenMethod("Truncate", "vector")]
	private static void Truncate (WrenVM vm) {
		vm.EnsureSlots(1);
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		vector = new WrenVector2Wrapper(Vector2.Truncate(vector.Vector));
		vm.SetSlotNewForeign(0, vector);
	}

	[WrenMethod("Normalize", "vector")]
	private static void Normalize (WrenVM vm) {
		vm.EnsureSlots(2);
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		vector.Vector = Vector2.Normalize(vector.Vector);
		vm.SetSlotNewForeign(0, vector);
	}
	
	[WrenMethod("DirectionToVector", "direction")]
	private static void DirectionToVector (WrenVM vm) {
		vm.EnsureSlots(2);
		int direction = (int)vm.GetSlotDouble(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(Vector2.DirectionToVector(direction)));
	}
	
	[WrenMethod("LeftNormal", "vector")]
	private static void LeftNormal (WrenVM vm) {
		vm.EnsureSlots(2);
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(Vector2.LeftNormal(vector.Vector)));
	}
	
	[WrenMethod("RightNormal", "vector")]
	private static void RightNormal (WrenVM vm) {
		vm.EnsureSlots(2);
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		vm.SetSlotNewForeign(0, new WrenVector2Wrapper(Vector2.RightNormal(vector.Vector)));
	}
	
	[WrenMethod("Dot", "vector")]
	private static void Dot (WrenVM vm) {
		vm.EnsureSlots(3);
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		var vector2 = vm.GetSlotForeign<WrenVector2Wrapper>(2);
		vm.SetSlotDouble(0, Vector2.DotProduct(vector.Vector, vector.Vector));
	}
	
	[WrenMethod("Magnitude", "vector")]
	private void Magnitude (WrenVM vm) {
		vm.EnsureSlots(2);
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		vm.SetSlotDouble(0, Vector2.Magnitude(vector.Vector));
	}
	
	[WrenMethod("VectorToDirection", "vector")]
	private void VectorToDirection (WrenVM vm) {
		vm.EnsureSlots(1);
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		vm.SetSlotDouble(0, Vector2.VectorToDirection(vector.Vector));
	}


	[WrenMethod("ToString", "vector")]
	private static void ToString (WrenVM vm) {
		vm.EnsureSlots(1);
		var vector = vm.GetSlotForeign<WrenVector2Wrapper>(1);
		vm.SetSlotString(0, vector.Vector.ToString());
	}
	protected virtual void Dispose (bool disposing) {
		if (disposing) {
			Outer.Log("Disposing of WrenVector2Wrapper");
		}
	}
	public void Dispose () {
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
