﻿#region

using DewDrop.Utilities;

#endregion

namespace DewDrop.Graphics;

/// <summary>
///     A class containing data associated with a sprite.
/// </summary>
public struct SpriteDefinition {
	/// <summary>
	///     Has options for different modes of animation.
	/// </summary>
	public enum AnimationMode {
		Invalid = -1,

		/// <summary>
		///     Makes the animation go in order. Ex. 0 -> 1 -> 2
		/// </summary>
		Continous = 0,

		/// <summary>
		///     Makes the animation go 0-2-1-3.
		/// </summary>
		ZeroTwoOneThree = 1,

		/// <summary>
		///     The max amount of options in this enum.
		/// </summary>
		Maximum
	}

	public bool IsValid => Name != null;

	/// <summary>
	///     The name of the sprite definition.
	/// </summary>
	public string Name { get; }

	/// <summary>
	///     The top left pixel of the sprite definition on the spritesheet
	/// </summary>
	public Vector2 Coords { get; private set; }

	/// <summary>
	///     The bounds of the sprite definition on the spritesheet
	/// </summary>
	public Vector2 Bounds { get; private set; }

	/// <summary>
	///     The offset of the sprite
	/// </summary>
	public Vector2 Offset { get; private set; }

	/// <summary>
	///     How many frames this sprite definition has
	/// </summary>
	public int Frames { get; private set; }

	/// <summary>
	///     The speeds of the animations within the sprite definitions
	/// </summary>
	public float[] Speeds { get; private set; }

	/// <summary>
	///     Should we flip this sprite horizontally?
	/// </summary>
	public bool FlipX { get; private set; }

	/// <summary>
	///     Should we flip this sprite vertically
	/// </summary>
	public bool FlipY { get; private set; }

	/// <summary>
	///     What mode this sprite definition is in
	/// </summary>
	public AnimationMode Mode { get; private set; }

	/// <summary>
	///     Additional data
	/// </summary>
	public int[] Data { get; private set; }


	/// <summary>
	///     Creates a new sprite definition
	/// </summary>
	/// <param name="name">The name of the Sprite Definition</param>
	/// <param name="coords">The coordinates of the sprite definition relat</param>
	/// <param name="bounds"></param>
	/// <param name="origin"></param>
	/// <param name="frames"></param>
	/// <param name="speeds"></param>
	/// <param name="flipX"></param>
	/// <param name="flipY"></param>
	/// <param name="mode"></param>
	/// <param name="data">Extra data</param>
	public SpriteDefinition (string name, Vector2 coords, Vector2 bounds, Vector2 origin, int frames, float[] speeds, bool flipX, bool flipY, int mode, int[] data) {
		Name = name;
		Coords = coords;
		Bounds = bounds;
		Offset = origin;
		Frames = frames;
		Speeds = speeds;
		FlipX = flipX;
		FlipY = flipY;
		Mode = (AnimationMode)mode;
		Data = data;
	}
}
