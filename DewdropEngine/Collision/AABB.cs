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
	
	public Vector2 GetNormal(Vector2 point) {
		Vector2 center = Position + Size / 2;
		Vector2 direction = point - center;
		if (Math.Abs(direction.X) > Math.Abs(direction.Y)) {
			// Point is closer to a vertical edge
			return direction.X > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
		} else {
			// Point is closer to a horizontal edge
			return direction.Y > 0 ? new Vector2(0, 1) : new Vector2(0, -1);
		}
	}
	
	public FloatRect GetFloatRect () {
		return floatRect;
	}
}
