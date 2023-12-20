using DewDrop.Utilities;

/// <summary>
/// Represents a Tile in a tile-based game.
/// </summary>
public struct Tile {
	/// <summary>
	/// The unique identifier for the Tile.
	/// </summary>
	public readonly uint Id;

	/// <summary>
	/// The position of the Tile in the game world.
	/// </summary>
	public readonly Vector2 Position;

	/// <summary>
	/// Indicates whether the Tile is flipped horizontally.
	/// </summary>
	public readonly bool FlipHorizontal;

	/// <summary>
	/// Indicates whether the Tile is flipped vertically.
	/// </summary>
	public readonly bool FlipVertical;

	/// <summary>
	/// Indicates whether the Tile is flipped diagonally.
	/// </summary>
	public readonly bool FlipDiagonal;

	/// <summary>
	/// The identifier for the Tile's animation.
	/// </summary>
	public readonly ushort AnimationId;

	/// <summary>
	/// Initializes a new instance of the Tile struct with specified properties.
	/// </summary>
	/// <param name="tileId">The unique identifier for the Tile.</param>
	/// <param name="position">The position of the Tile in the game world.</param>
	/// <param name="flipHoriz">Indicates whether the Tile is flipped horizontally.</param>
	/// <param name="flipVert">Indicates whether the Tile is flipped vertically.</param>
	/// <param name="flipDiag">Indicates whether the Tile is flipped diagonally.</param>
	/// <param name="animId">The identifier for the Tile's animation.</param>
	public Tile (uint tileId, Vector2 position, bool flipHoriz, bool flipVert, bool flipDiag, ushort animId) {
		Id = tileId;
		Position = position;
		FlipHorizontal = flipHoriz;
		FlipVertical = flipVert;
		FlipDiagonal = flipDiag;
		AnimationId = animId;
	}
}
