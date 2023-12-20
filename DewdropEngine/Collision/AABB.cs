#region

using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Collision;

/// <summary>
/// Represents an Axis-Aligned Bounding Box (AABB) used for collision detection.
/// </summary>
// ReSharper disable once InconsistentNaming
public readonly struct AABB {
	readonly FloatRect _floatRect;

	/// <summary>
	///     The position of the AABB.
	/// </summary>
	public readonly Vector2 Position;

	/// <summary>
	///     The size of the AABB.
	/// </summary>
	public readonly Vector2 Size;

	/// <summary>
	///     Indicates whether the AABB is a player.
	/// </summary>
	public readonly bool IsPlayer;

	/// <summary>
	///     Indicates whether the AABB is only a player.
	/// </summary>
	public readonly bool OnlyPlayer;

	/// <summary>
	///     Initializes a new instance of the AABB struct with specified position and size.
	/// </summary>
	/// <param name="position">The position of the AABB.</param>
	/// <param name="size">The size of the AABB.</param>
	public AABB (Vector2 position, Vector2 size) {
		Position = position;
		Size = size;
		IsPlayer = false;
		OnlyPlayer = false;
		_floatRect = new FloatRect(Position.X, Position.Y, Size.X, Size.Y);
	}

	/// <summary>
	///     Initializes a new instance of the AABB struct with specified position, size, and player flags.
	/// </summary>
	/// <param name="position">The position of the AABB.</param>
	/// <param name="size">The size of the AABB.</param>
	/// <param name="isPlayer">Indicates whether the AABB is a player.</param>
	/// <param name="onlyPlayer">Indicates whether the AABB is only a player.</param>
	public AABB (Vector2 position, Vector2 size, bool isPlayer, bool onlyPlayer) {
		Position = position;
		Size = size;
		IsPlayer = isPlayer;
		OnlyPlayer = onlyPlayer;
		_floatRect = new FloatRect(Position.X, Position.Y, Size.X, Size.Y);
	}

	/// <summary>
	///     Gets the normal of the AABB at a specified point.
	/// </summary>
	/// <param name="point">The point at which to get the normal.</param>
	/// <returns>The normal of the AABB at the specified point.</returns>
	public Vector2 GetNormal (Vector2 point) {
		Vector2 center = Position + Size/2;
		Vector2 direction = point - center;
		if (Math.Abs(direction.X) > Math.Abs(direction.Y)) {
			// Point is closer to a vertical edge
			return direction.X > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
		}
		// Point is closer to a horizontal edge
		return direction.Y > 0 ? new Vector2(0, 1) : new Vector2(0, -1);
	}

	/// <summary>
	///     Gets the FloatRect representation of the AABB.
	/// </summary>
	/// <returns>The FloatRect representation of the AABB.</returns>
	public FloatRect GetFloatRect () {
		return _floatRect;
	}
}