#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Collision;

/// <summary>
///     Struct for bounding boxes.
///     You can read more about axis-aligned minimum bounding boxes here
///     https://en.wikipedia.org/wiki/Minimum_bounding_box#Axis-aligned_minimum_bounding_box
/// </summary>
public struct AABB {
	FloatRect floatRect;

	public readonly Vector2 Position;

	public readonly Vector2 Size;

	public readonly bool IsPlayer;

	public readonly bool OnlyPlayer;

	public AABB (Vector2 position, Vector2 size) {
		Position = position;
		Size = size;
		IsPlayer = false;
		OnlyPlayer = false;
		floatRect = new FloatRect(Position.X, Position.Y, Size.X, Size.Y);
	}

	public AABB (Vector2 position, Vector2 size, bool isPlayer, bool onlyPlayer) {
		Position = position;
		Size = size;
		IsPlayer = isPlayer;
		OnlyPlayer = onlyPlayer;
		floatRect = new FloatRect(Position.X, Position.Y, Size.X, Size.Y);
	}

	public FloatRect GetFloatRect () {
		return floatRect;
	}
}
