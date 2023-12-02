#region

using DewDrop.Utilities;

#endregion

namespace DewDrop;

/// <summary>
///     The engine is able to have certain aspects of it manipulated to fit your needs.
///     This struct contains values used by the engine to determine certain aspects of itself.
///     Such as the size of the screen, and if VSync should be enabled.
/// </summary>
struct EngineConfigurationData {
	public Vector2 screen_size;

	public float screen_width => screen_size.x;

	public float screen_height => screen_size.y;
}
