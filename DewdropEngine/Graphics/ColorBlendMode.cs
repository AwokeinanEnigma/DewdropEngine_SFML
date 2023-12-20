/// <summary>
/// Enum representing different color blending modes in the DewDrop Graphics system.
/// </summary>
public enum ColorBlendMode {
	/// <summary>
	/// Replace mode, replaces the existing color.
	/// </summary>
	Replace,

	/// <summary>
	/// Multiply mode, multiplies the color values.
	/// </summary>
	Multiply,

	/// <summary>
	/// Screen mode, adds two colors together and keeps the brighter pixels.
	/// </summary>
	Screen,

	/// <summary>
	/// Add mode, adds the color values together.
	/// </summary>
	Add
}
