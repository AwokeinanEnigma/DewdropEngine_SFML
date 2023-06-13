#region

using DewDrop.Graphics;
using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;
using SFML.Graphics.Glsl;

#endregion

namespace DewDrop.Tiles;

public class TileChunk : Renderable
{
    public bool AnimateTiles
    {
        get => _animateTiles;
        set => _animateTiles = value;
    }

    #region Fields

    private bool _animateTiles;
    private SpritesheetTexture _tilesetTexture;
    private Vertex[] _tileVertices;
    private TileAnimation[] _tileAnimations;
    private RenderStates _renderStates;
    private static readonly Shader PaletteShader = new(EmbeddedResourcesHandler.GetResourceStream("pal.vert"), null, EmbeddedResourcesHandler.GetResourceStream("pal.frag"));

    #endregion

    public TileChunk(Tile[] tiles, string tilesetImage, uint palette, Vector2 position, uint depth)
    {
        _tilesetTexture = TextureManager.Instance.Use(tilesetImage);
        _tilesetTexture.CurrentPalette = palette;

        _position = position;
        _depth = (int)depth;
        _animateTiles = true;

        _renderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, _tilesetTexture.Image, PaletteShader);

        MakeAnimations(_tilesetTexture.GetSpriteDefinitions());
        CreateVertexArray(tiles);
        SetTransform();
    }

    private void SetTransform()
    {
        Transform identity = Transform.Identity;
        identity.Translate(_position - _origin);

        _renderStates.Transform = identity;
    }

    #region Animations

    private void MakeAnimations(ICollection<SpriteDefinition> definitions)
    {
        _tileAnimations = new TileAnimation[definitions.Count];
        foreach (SpriteDefinition spriteDefinition in definitions)
        {
            if (int.TryParse(spriteDefinition.Name, out int animationIndex) && animationIndex >= 0)
            {
                if (spriteDefinition.Data != null && spriteDefinition.Data.Length > 0)
                {
                    int[] data = spriteDefinition.Data;
                    float speed = spriteDefinition.Speeds[0];

                    _tileAnimations[animationIndex].Tiles = data;
                    _tileAnimations[animationIndex].VertexIndexes = new List<int>();
                    _tileAnimations[animationIndex].AnimationSpeed = speed;
                }
                else
                {
                    Debug.LogWarning($"Failed to load sprite definition for animation index '{animationIndex}'");
                }
            }
        }
    }

    private unsafe void UpdateAnimations()
    {
        for (int i = 0; i < _tileAnimations.Length; i++)
        {
            TileAnimation tileAnimation = _tileAnimations[i];

            float speed = Engine.Frame * tileAnimation.AnimationSpeed;
            uint tileID = (uint)tileAnimation.Tiles[(int)speed % tileAnimation.Tiles.Length];
            TileIDToTextureCoordinates(tileID - 1U, out uint tileX, out uint tileY);

            fixed (Vertex* ptr = _tileVertices)
            {
                for (int j = 0; j < tileAnimation.VertexIndexes.Count; j++)
                {
                    int vertexIndex = tileAnimation.VertexIndexes[j];
                    Vertex* ptr2 = ptr + vertexIndex;

                    ptr2->TexCoords.X = tileX;
                    ptr2->TexCoords.Y = tileY;

                    ptr2[1].TexCoords.X = tileX + 8U;
                    ptr2[1].TexCoords.Y = tileY;

                    ptr2[2].TexCoords.X = tileX + 8U;
                    ptr2[2].TexCoords.Y = tileY + 8U;

                    ptr2[3].TexCoords.X = tileX;
                    ptr2[3].TexCoords.Y = tileY + 8U;
                }
            }
        }
    }

    #endregion

    #region Vertices

    private unsafe void CreateVertexArray(Tile[] tiles)
    {
        _tileVertices = new Vertex[tiles.Length * 4];

        uint tileX = 0U;
        uint tileY = 0U;

        Vector2 v = default;
        Vector2 v2 = default;

        fixed (Vertex* ptr = _tileVertices)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                Vertex* ptr2 = ptr + i * 4;
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

                TileIDToTextureCoordinates(tile.ID, out tileX, out tileY);

                if (!tile.FlipHorizontal && !tile.FlipVertical)
                {

                    ptr2->TexCoords.X = tileX;
                    ptr2->TexCoords.Y = tileY;

                    ptr2[1].TexCoords.X = tileX + 8U;
                    ptr2[1].TexCoords.Y = tileY;

                    ptr2[2].TexCoords.X = tileX + 8U;
                    ptr2[2].TexCoords.Y = tileY + 8U;

                    ptr2[3].TexCoords.X = tileX;
                    ptr2[3].TexCoords.Y = tileY + 8U;

                }
                else if (tile.FlipHorizontal && !tile.FlipVertical)
                {
                    ptr2->TexCoords.X = tileX + 8U;
                    ptr2->TexCoords.Y = tileY;

                    ptr2[1].TexCoords.X = tileX;
                    ptr2[1].TexCoords.Y = tileY;

                    ptr2[2].TexCoords.X = tileX;
                    ptr2[2].TexCoords.Y = tileY + 8U;

                    ptr2[3].TexCoords.X = tileX + 8U;
                    ptr2[3].TexCoords.Y = tileY + 8U;

                }
                else if (!tile.FlipHorizontal && tile.FlipVertical)
                {
                    ptr2->TexCoords.X = tileX;
                    ptr2->TexCoords.Y = tileY + 8U;

                    ptr2[1].TexCoords.X = tileX + 8U;
                    ptr2[1].TexCoords.Y = tileY + 8U;

                    ptr2[2].TexCoords.X = tileX + 8U;
                    ptr2[2].TexCoords.Y = tileY;

                    ptr2[3].TexCoords.X = tileX;
                    ptr2[3].TexCoords.Y = tileY;
                }
                else
                {
                    ptr2->TexCoords.X = tileX + 8U;
                    ptr2->TexCoords.Y = tileY + 8U;

                    ptr2[1].TexCoords.X = tileX;
                    ptr2[1].TexCoords.Y = tileY + 8U;

                    ptr2[2].TexCoords.X = tileX;
                    ptr2[2].TexCoords.Y = tileY;

                    ptr2[3].TexCoords.X = tileX + 8U;
                    ptr2[3].TexCoords.Y = tileY;
                }

                v.X = Math.Min(v.X, ptr2->Position.X);
                v.Y = Math.Min(v.Y, ptr2->Position.Y);

                v2.X = Math.Max(v2.X, ptr2[2].Position.X - v.X);
                v2.Y = Math.Max(v2.Y, ptr2[2].Position.Y - v.Y);

                AddVertIndex(tile, i * 4);
            }
        }

        _size = v2 - v;
    }

    private void AddVertIndex(Tile tile, int index)
    {
        if (tile.AnimationId > 0)
        {
            int animationId = tile.AnimationId - 1;
            _tileAnimations[animationId].VertexIndexes.Add(index);
        }
    }

    #endregion

    private void TileIDToTextureCoordinates(uint id, out uint tx, out uint ty)
    {
        tx = id * 8U % _tilesetTexture.Image.Size.X;
        ty = id * 8U / _tilesetTexture.Image.Size.X * 8U;
    }

    public override void Draw(RenderTarget target)
    {
        PaletteShader.SetUniform("image", _tilesetTexture.Image);
        PaletteShader.SetUniform("palette", _tilesetTexture.Palette);
        PaletteShader.SetUniform("palIndex", _tilesetTexture.CurrentPaletteFloat);
        PaletteShader.SetUniform("palSize", _tilesetTexture.PaletteSize);
        PaletteShader.SetUniform("blend", new Vec4(Color.White));
        PaletteShader.SetUniform("blendMode", 1f);

        if (AnimateTiles)
            UpdateAnimations();

        target.Draw(_tileVertices, PrimitiveType.Quads, _renderStates);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {

            // Unuse tileset
            TextureManager.Instance.Unuse(_tilesetTexture);

            // Additionally, dispose of the tileset and set it to null.
            // Just for safety :^)
            _tilesetTexture.Dispose();
            _tilesetTexture = null;

            // These would stay in memory and just shit up MEMORY AND SHIT UP PERFORMANCE
            Array.Clear(_tileVertices, 0, _tileVertices.Length);
            _tileVertices = null;

            // For safety, we're going to clear the animations array.
            Array.Clear(_tileAnimations, 0, _tileAnimations.Length);
            _tileAnimations = null;
        }

        _disposed = true;
    }
}