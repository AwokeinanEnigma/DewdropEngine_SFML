namespace DewDrop.Utilities; 

public struct Vector3 {
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }
	public Vector3 (float x, float y, float z) {
		X = x;
		Y = y;
		Z = z;
	}
	public Vector3 (float x, float y) {
		X = x;
		Y = y;
		Z = 0;
	}
	public Vector3 () {
		X = 0;
		Y = 0;
		Z = 0;
	}
	public static Vector3 operator + (Vector3 a, Vector3 b) {
		return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}
	public static Vector3 operator - (Vector3 a, Vector3 b) {
		return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}
	public static Vector3 operator * (Vector3 a, Vector3 b) {
		return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
	}
	public static Vector3 operator / (Vector3 a, Vector3 b) {
		return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
	}
	public static Vector3 operator + (Vector3 a, float b) {
		return new Vector3(a.X + b, a.Y + b, a.Z + b);
	}
	public static Vector3 operator - (Vector3 a, float b) {
		return new Vector3(a.X - b, a.Y - b, a.Z - b);
	}
	public static Vector3 operator * (Vector3 a, float b) {
		return new Vector3(a.X * b, a.Y * b, a.Z * b);
	}
	public static Vector3 operator / (Vector3 a, float b) {
		return new Vector3(a.X / b, a.Y / b, a.Z / b);
	}
	public static Vector3 operator + (float a, Vector3 b) {
		return new Vector3(a + b.X, a + b.Y, a + b.Z);
	}
	public static Vector3 operator - (float a, Vector3 b) {
		return new Vector3(a - b.X, a - b.Y, a - b.Z);
	}
	public static Vector3 operator * (float a, Vector3 b) {
		return new Vector3(a * b.X, a * b.Y, a * b.Z);
	}
	public static Vector3 operator / (float a, Vector3 b) {
		return new Vector3(a / b.X, a / b.Y, a / b.Z);
	}
	public static Vector3 operator - (Vector3 a) {
		return new Vector3(-a.X, -a.Y, -a.Z);
	}
	public override string ToString () {
		return $"({X}, {Y}, {Z})";
	}
	public override bool Equals (object obj) {
		if (obj is Vector3) {
			return this == (Vector3)obj;
		}
		return false;
	}
	public override int GetHashCode () {
		return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
	}
	public static bool operator == (Vector3 a, Vector3 b) {
		return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
	}
	public static bool operator != (Vector3 a, Vector3 b) {
		return !(a == b);
	}
	public static Vector3 Zero { get; } = new Vector3(0, 0, 0);
	public static Vector3 One { get; } = new Vector3(1, 1, 1);
	public static Vector3 Up { get; } = new Vector3(0, 1, 0);
	
	public static Vector3 Down { get; } = new Vector3(0, -1, 0);
	public static Vector3 Left { get; } = new Vector3(-1, 0, 0);
	public static Vector3 Right { get; } = new Vector3(1, 0, 0);
	public static Vector3 Forward { get; } = new Vector3(0, 0, 1);
	public static Vector3 Backward { get; } = new Vector3(0, 0, -1);
}
