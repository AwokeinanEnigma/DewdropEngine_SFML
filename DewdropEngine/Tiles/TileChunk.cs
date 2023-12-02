#region

using DewDrop.Graphics;
using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.Graphics.Glsl;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace DewDrop.Tiles;

public class TileChunk : Renderable {
	#region Properties

	public bool AnimationEnabled { get; set; }

	public override Vector2 RenderPosition {
		get => _position;
		set {
			_position = value;
			ResetTransform();
		}
	}

	public override Vector2 Origin {
		get => _origin;
		set {
			_origin = value;
			ResetTransform();
		}
	}
	public SpritesheetTexture TilesetSpritesheet { get; }

	#endregion

	#region Fields

	static readonly Shader TileGroupShader = new Shader(EmbeddedResourcesHandler.GetResourceStream("pal.vert"), null, EmbeddedResourcesHandler.GetResourceStream("pal.frag"));

	Vertex[] _vertices;
	AnimatedTile[] _tileAnimations;

	RenderStates _renderState;

	readonly Vec4 _blendColor;

	#endregion

	/// <summary>
	/// Initializes a new instance of the TileChunk class.
	/// </summary>
	/// <param name="tiles">The list of tiles.</param>
	/// <param name="resource">The resource string.</param>
	/// <param name="depth">The depth of the chunk.</param>
	/// <param name="position">The position of the chunk.</param>
	/// <param name="palette">The palette color.</param>
	/// <param name="enableAnimations">Whether animations are enabled or not.</param>
	/// <param name="blendColor">The blend color.</param>
	public TileChunk (List<Tile> tiles, string resource, int depth, Vector2 position, uint palette, bool enableAnimations = true, Color blendColor = default) 
	{
		// Use the TextureManager to get the spritesheet for the given resource
		TilesetSpritesheet = TextureManager.Instance.UseSpritesheet(resource);
		// Set the current palette color for the spritesheet
		TilesetSpritesheet.CurrentPalette = palette;
		_position = position;
		_depth = depth;
		// Set the render state for the chunk
		_renderState = new RenderStates(BlendMode.Alpha, Transform.Identity, TilesetSpritesheet.Image, TileGroupShader);
		// Set the animation enabled flag
		AnimationEnabled = enableAnimations;
		if (blendColor != default) 
		{
			// If a blend color is provided, create a new Vec4 object with the blend color
			_blendColor = new Vec4(blendColor);
		} 
		else 
		{
			// If no blend color is provided, create a new Vec4 object with the default white color
			_blendColor = new Vec4(Color.White);
		}
		// Create animations based on the sprite definitions from the spritesheet
		CreateAnimations(TilesetSpritesheet.GetSpriteDefinitions());
		// Create the vertex array for the tiles
		CreateVertexArray(tiles);
		// Reset the transform of the chunk
		ResetTransform();
		// Clear the tiles list (optional)
		//tiles = null;
		//tiles.Clear();
	}

	void ResetTransform () {
		Transform identity = Transform.Identity;
		identity.Translate(_position - _origin);
		_renderState.Transform = identity;
	}

	/// <summary>
	/// Calculates the tile ID at the given location.
	/// </summary>
	/// <param name="location">The location to calculate the tile ID for.</param>
	/// <returns>The tile ID at the given location.</returns>
	public int GetTileId(Vector2 location)
	{
		// Calculate the relative position of the location within the tile chunk
		Vector2 relativePosition = location - _position + _origin;

		// Calculate the tile index based on the relative position
		uint tileIndex = (uint)(relativePosition.X / 8f + relativePosition.Y / 8f * (_size.X / 8f));

		// Get the vertex at the calculated tile index
		Vertex vertex = _vertices[(int)(UIntPtr)(tileIndex * 4U)];

		// Get the texture coordinates of the vertex
		Vector2 texCoords = vertex.TexCoords;

		// Calculate the tile ID based on the texture coordinates and the size of the tileset image
		return (int)(texCoords.X / 8f + texCoords.Y / 8f * (TilesetSpritesheet.Image.Size.X / 8U));
	}

	// you have an IDE, use it 
	/// <summary>
	/// Calculates the texture coordinates for a given tile ID.
	/// </summary>
	/// <param name="id">The tile ID.</param>
	/// <param name="tx">The x-coordinate of the texture.</param>
	/// <param name="ty">The y-coordinate of the texture.</param>
	void TileIDToTextureCoords(uint id, out uint tx, out uint ty)
	{
		// Calculate the x-coordinate of the texture
		tx = id * 8U % TilesetSpritesheet.Image.Size.X;
		// Calculate the y-coordinate of the texture
		ty = id * 8U / TilesetSpritesheet.Image.Size.X * 8U;
	}

	/// <summary>
	/// Creates animations based on the given sprite definitions.
	/// </summary>
	/// <param name="definitions">The collection of sprite definitions.</param>
	void CreateAnimations(ICollection<SpriteDefinition> definitions)
	{
		// Create an array to store the animated tiles
		_tileAnimations = new AnimatedTile[definitions.Count];
		// Iterate over each sprite definition
		foreach (SpriteDefinition spriteDefinition in definitions)
		{
			// Try to parse the tile ID from the sprite definition name
			int.TryParse(spriteDefinition.Name, out int tileId);
			// Check if the tile ID is valid
			if (tileId >= 0)
			{
				// Check if the sprite definition has data
				if (spriteDefinition.Data != null && spriteDefinition.Data.Length > 0)
				{
					// Get the data and speed from the sprite definition
					int[] data = spriteDefinition.Data;
					float speed = spriteDefinition.Speeds[0];
					// Set the data, vertex indexes, and animation speed for the tile animation
					_tileAnimations[tileId].Tiles = data;
					_tileAnimations[tileId].VertexIndexes = new List<int>();
					_tileAnimations[tileId].AnimationSpeed = speed;
				}
				else
				{
					// Log an error if there is no tile data for the animation
					Outer.LogError($"Tried creating tile animation data for animation {tileId}, but there was no tile data.", null);
					//Console.WriteLine("Tried to load tile animation data for animation {0}, but there was no tile data.", tileId);
				}
			}
		}
	}
	
	/// <summary>
	/// AddVertexIndex adds the index to the VertexIndexes of the TileAnimation.
	/// </summary>
	/// <param name="tile">The tile object.</param>
	/// <param name="index">The index to add.</param>
	void AddVertexIndex (Tile tile, int index) 
	{
		// Check if the AnimationId of the tile is more than 0
		if (tile.AnimationId > 0) 
		{
			// Calculate the animation number
			int animationNum = tile.AnimationId - 1;

			// Add the index to the VertexIndexes list of the TileAnimation
			_tileAnimations[animationNum].VertexIndexes.Add(index);
		}
	}

	/// <summary>
	/// Creates a vertex array based on a list of tiles.
	/// </summary>
	/// <param name="tiles">The list of tiles.</param>
	unsafe void CreateVertexArray (List<Tile> tiles) {
		_vertices = new Vertex[tiles.Count * 4];

		// these are declared OUTSIDE of the loop to avoid allocating extra memory 
		uint textureX = 0U;
		uint textureY = 0U;

		Vector2 v = default;
		Vector2 v2 = default;

		fixed (Vertex* ptr = _vertices) {
			for (int i = 0; i < tiles.Count; i++) {
				Vertex* ptr2 = ptr + i*4;
				Tile tile = tiles[i];
				float x = tile.Position.X;
				float y = tile.Position.Y;

				ptr2->Position.X = x;
				ptr2->Position.Y = y;

				ptr2[1].Position.X = x + 8f;
				ptr2[1].Position.Y = y;

				ptr2[2].Position.X = x + 8f;
				ptr2[2].Position.Y = y + 8f;

				ptr2[3].Position.X = x;
				ptr2[3].Position.Y = y + 8f;

				TileIDToTextureCoords(tile.ID, out textureX, out textureY);

				// normal tile
				if (!tile.FlipHorizontal && !tile.FlipVertical) {
					ptr2->TexCoords.X = textureX;
					ptr2->TexCoords.Y = textureY;

					ptr2[1].TexCoords.X = textureX + 8U;
					ptr2[1].TexCoords.Y = textureY;

					ptr2[2].TexCoords.X = textureX + 8U;
					ptr2[2].TexCoords.Y = textureY + 8U;

					ptr2[3].TexCoords.X = textureX;
					ptr2[3].TexCoords.Y = textureY + 8U;
				}
				// horizontally flipped tile
				else if (tile.FlipHorizontal && !tile.FlipVertical) {
					ptr2->TexCoords.X = textureX + 8U;
					ptr2->TexCoords.Y = textureY;

					ptr2[1].TexCoords.X = textureX;
					ptr2[1].TexCoords.Y = textureY;

					ptr2[2].TexCoords.X = textureX;
					ptr2[2].TexCoords.Y = textureY + 8U;

					ptr2[3].TexCoords.X = textureX + 8U;
					ptr2[3].TexCoords.Y = textureY + 8U;
				}
				// vertically flipped tile
				else if (!tile.FlipHorizontal && tile.FlipVertical) {
					ptr2->TexCoords.X = textureX;
					ptr2->TexCoords.Y = textureY + 8U;

					ptr2[1].TexCoords.X = textureX + 8U;
					ptr2[1].TexCoords.Y = textureY + 8U;

					ptr2[2].TexCoords.X = textureX + 8U;
					ptr2[2].TexCoords.Y = textureY;

					ptr2[3].TexCoords.X = textureX;
					ptr2[3].TexCoords.Y = textureY;
				}
				// horizontally and vertically flipped tile!
				else {
					ptr2->TexCoords.X = textureX + 8U;
					ptr2->TexCoords.Y = textureY + 8U;

					ptr2[1].TexCoords.X = textureX;
					ptr2[1].TexCoords.Y = textureY + 8U;

					ptr2[2].TexCoords.X = textureX;
					ptr2[2].TexCoords.Y = textureY;

					ptr2[3].TexCoords.X = textureX + 8U;
					ptr2[3].TexCoords.Y = textureY;
				}

				v.X = Math.Min(v.X, ptr2->Position.X);
				v.Y = Math.Min(v.Y, ptr2->Position.Y);

				v2.X = Math.Max(v2.X, ptr2[2].Position.X - v.X);
				v2.Y = Math.Max(v2.Y, ptr2[2].Position.Y - v.Y);

				AddVertexIndex(tile, i*4);
			}
		}

		_size = v2 - v;
	}

	/// <summary>
	/// Updates the animations for the tile chunk.
	/// </summary>
	unsafe void UpdateAnimations()
	{
		// Check if animation is enabled
		if (!AnimationEnabled)
		{
			return;
		}
		// Iterate over each tile animation
		for (int i = 0; i < _tileAnimations.Length; i++)
		{
			AnimatedTile tileAnimation = _tileAnimations[i];
			// Calculate the current frame based on animation speed
			float frame = Engine.Frame * tileAnimation.AnimationSpeed;
			// Get the tile ID for the current frame
			uint tileID = (uint)tileAnimation.Tiles[(int)frame % tileAnimation.Tiles.Length];
			// Get the texture coordinates for the tile ID
			TileIDToTextureCoords(tileID - 1U, out uint tileX, out uint tileY);
			// Update the texture coordinates for each vertex index
			fixed (Vertex* ptr = _vertices)
			{
				for (int j = 0; j < tileAnimation.VertexIndexes.Count; j++)
				{
					int vertexIndex = tileAnimation.VertexIndexes[j];
					Vertex* vertexPtr = ptr + vertexIndex;
					// Update the texture coordinates for the vertex
					vertexPtr->TexCoords.X = tileX;
					vertexPtr->TexCoords.Y = tileY;
					vertexPtr[1].TexCoords.X = tileX + 8U;
					vertexPtr[1].TexCoords.Y = tileY;
					vertexPtr[2].TexCoords.X = tileX + 8U;
					vertexPtr[2].TexCoords.Y = tileY + 8U;
					vertexPtr[3].TexCoords.X = tileX;
					vertexPtr[3].TexCoords.Y = tileY + 8U;
				}
			}
		}
	}
	public override void Draw (RenderTarget target) {
		TileGroupShader.SetUniform("image", TilesetSpritesheet.Image);
		TileGroupShader.SetUniform("palette", TilesetSpritesheet.Palette);
		TileGroupShader.SetUniform("palIndex", TilesetSpritesheet.CurrentPaletteFloat);
		TileGroupShader.SetUniform("palSize", TilesetSpritesheet.PaletteSize);
		TileGroupShader.SetUniform("blend", _blendColor);
		TileGroupShader.SetUniform("blendMode", 1f);

		UpdateAnimations();
		target.Draw(_vertices, PrimitiveType.Quads, _renderState);
	}

	protected override void Dispose (bool disposing) {
		if (!_disposed) {
			TextureManager.Instance.Unuse(TilesetSpritesheet);
			Array.Clear(_tileAnimations, 0, _tileAnimations.Length);
			Array.Clear(_vertices, 0, _vertices.Length);
		}

		_disposed = true;
	}
}
