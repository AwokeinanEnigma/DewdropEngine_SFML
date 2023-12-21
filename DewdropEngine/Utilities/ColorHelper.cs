#region

using SFML.Graphics;
using System.Globalization;
using System.Numerics;

#endregion

namespace DewDrop.Utilities;

/// <summary>
/// Provides helper methods for working with colors.
/// </summary>
public static class ColorHelper {
	
	/// <summary>
	/// Converts a hexadecimal color string to a Color object.
	/// </summary>
	/// <param name="hexString">The hexadecimal color string.</param>
	/// <returns>A Color object that represents the color of the hexadecimal string.</returns>
	public static Color? FromHexString (string hexString) {
		Color? result; 
		result = Int16.TryParse(hexString, NumberStyles.HexNumber, null, out short shortColor) ? FromInt((uint)shortColor) : Color.Black;
		return result;
	}

	/// <summary>
	/// Converts an integer to a Color object.
	/// </summary>
	/// <param name="color">The integer to convert.</param>
	/// <returns>A Color object that represents the color of the integer.</returns>
	public static Color FromInt (int color) {
		return FromInt((uint)color);
	}
	
	public static Vector4 ToNumericVector4 (Color self) => new Vector4(self.R/255.0f, self.G/255.0f, self.B/255.0f, self.A/255.0f);
	public static Color ToSfmlColor (Vector4 self) => new Color((byte)(self.X*255f), (byte)(self.Y*255f), (byte)(self.Z*255f), (byte)(self.W*255f));


    /// <summary>
    /// Converts an unsigned integer to a Color object.
    /// </summary>
    /// <param name="color">The unsigned integer to convert.</param>
    /// <returns>A Color object that represents the color of the unsigned integer.</returns>
    public static Color FromInt (uint color) {
		// inherited from carbine
		// i don't know how this code works, and frankly, i don't want to know. 
		byte alpha = (byte)(color >> 24);
		return new Color((byte)(color >> 16), (byte)(color >> 8), (byte)color, alpha);
	}

    /// <summary>
    /// Blends two colors together.
    /// </summary>
    /// <param name="col1">The first color to blend.</param>
    /// <param name="col2">The second color to blend.</param>
    /// <param name="amount">The amount to blend by. The higher the value, the less it'll blend. Vice versa.</param>
    /// <returns>A Color object that represents the blended color.</returns>
    public static Color Blend (Color col1, Color col2, float amount) {
		float num = 1f - amount;
		return new Color((byte)(col1.R*(double)num + col2.R*(double)amount), (byte)(col1.G*(double)num + col2.G*(double)amount), (byte)(col1.B*(double)num + col2.B*(double)amount), byte.MaxValue);
	}

    /// <summary>
    /// Blends two colors together, including their alpha values.
    /// </summary>
    /// <param name="col1">The first color to blend.</param>
    /// <param name="col2">The second color to blend.</param>
    /// <param name="amount">The amount to blend by. The higher the value, the less it'll blend. Vice versa.</param>
    /// <returns>A Color object that represents the blended color.</returns>
    public static Color BlendAlpha (Color col1, Color col2, float amount) {
		float num = 1f - amount;
		return new Color((byte)(col1.R*(double)num + col2.R*(double)amount), (byte)(col1.G*(double)num + col2.G*(double)amount), (byte)(col1.B*(double)num + col2.B*(double)amount), (byte)(col1.A*(double)num + col2.A*(double)amount));
	}

	/// <summary>
	/// Inverts a color.
	/// </summary>
	/// <param name="color">The color to invert.</param>
	/// <returns>A Color object that represents the inverted color.</returns>
	public static Color Invert (this Color color) {
		return new Color((byte)(byte.MaxValue - (uint)color.R), (byte)(byte.MaxValue - (uint)color.G), (byte)(byte.MaxValue - (uint)color.B), color.A);
	}
}
