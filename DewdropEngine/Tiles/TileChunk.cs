﻿#region

using DewDrop.Graphics;
using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Tiles;

public class TileChunk : Renderable
{
    #region Properties

        public bool AnimationEnabled
        {
            get => _animationEnabled;
            set => _animationEnabled = value;
        }
    
        public override Vector2 RenderPosition
        {
            get => _position;
            set
            {
                _position = value;
                ResetTransform();
            }
        }
    
        public override Vector2 Origin
        {
            get => _origin;
            set
            {
                _origin = value;
                ResetTransform();
            }
        }
        public SpritesheetTexture TilesetSpritesheet => _tileset;
        
    #endregion
    
    #region Fields
    
    private static readonly Shader TileGroupShader = new(EmbeddedResourcesHandler.GetResourceStream("pal.vert"), null, EmbeddedResourcesHandler.GetResourceStream("pal.frag"));

    private Vertex[] _vertices;
    private AnimatedTile[] _tileAnimations;

    private readonly SpritesheetTexture _tileset;
    private RenderStates _renderState;
    
    private bool _animationEnabled;
    private readonly SFML.Graphics.Glsl.Vec4 _blendColor;
    #endregion
    
    public TileChunk(List<Tile> tiles, string resource, int depth, Vector2 position, uint palette, bool enableAnimations = true, Color blendColor = default)
    {
        _tileset = TextureManager.Instance.Use(resource);
        _tileset.CurrentPalette = palette;
       
        _position = position;
        _depth = depth;
        
        _renderState = new RenderStates(BlendMode.Alpha, Transform.Identity, _tileset.Image, TileGroupShader);
        _animationEnabled = enableAnimations;

        if (blendColor != default)
        {
            _blendColor = new SFML.Graphics.Glsl.Vec4(blendColor);
        }
        else
        {
            _blendColor = new SFML.Graphics.Glsl.Vec4(Color.White);
        }

        CreateAnimations(_tileset.GetSpriteDefinitions());
        CreateVertexArray(tiles);
        ResetTransform();
        //tiles = null;
        //tiles.Clear();
    }

    private void ResetTransform()
    {
        Transform identity = Transform.Identity;
        identity.Translate(_position - _origin);
        _renderState.Transform = identity;
    }

    public int GetTileId(Vector2 location)
    {
        Vector2 Vector2 = location - _position + _origin;
        uint num = (uint)(Vector2.X / 8f + Vector2.Y / 8f * (_size.X / 8f));
        Vertex vertex = _vertices[(int)(UIntPtr)(num * 4U)];
        Vector2 texCoords = vertex.TexCoords;
        return (int)(texCoords.X / 8f + texCoords.Y / 8f * (_tileset.Image.Size.X / 8U));
    }

    // you have an IDE, use it 
    private void TileIDToTextureCoords(uint id, out uint tx, out uint ty)
    {
        tx = id * 8U % _tileset.Image.Size.X;
        ty = id * 8U / _tileset.Image.Size.X * 8U;
    }

    private void CreateAnimations(ICollection<SpriteDefinition> definitions)
    {
        _tileAnimations = new AnimatedTile[definitions.Count];
        foreach (SpriteDefinition spriteDefinition in definitions)
        {
            
            int.TryParse(spriteDefinition.Name, out int tileId);
            
            if (tileId >= 0)
            {
                if (spriteDefinition.Data != null && spriteDefinition.Data.Length > 0)
                {
                    int[] data = spriteDefinition.Data;
                    float speed = spriteDefinition.Speeds[0];
                    _tileAnimations[tileId].Tiles = data;
                    _tileAnimations[tileId].VertexIndexes = new List<int>();
                    _tileAnimations[tileId].AnimationSpeed = speed;
                }
                else
                {
                    DDDebug.LogError($"Tried creating tile animation data for animation {tileId}, but there was no tile data.", null);
                    //Console.WriteLine("Tried to load tile animation data for animation {0}, but there was no tile data.", tileId);
                }
            }
        }
    }

    private void AddVertexIndex(Tile tile, int index)
    {
        if (tile.AnimationId > 0)
        {
            int num = tile.AnimationId - 1;
            _tileAnimations[num].VertexIndexes.Add(index);
        }
    }

    private unsafe void CreateVertexArray(List<Tile> tiles)
    {
        _vertices = new Vertex[tiles.Count * 4];
        
        // these are declared OUTSIDE of the loop to avoid allocating extra memory 
        uint textureX = 0U;
        uint textureY = 0U;
        
        Vector2 v = default;
        Vector2 v2 = default;
        
        fixed (Vertex* ptr = _vertices)
        {
            for (int i = 0; i < tiles.Count; i++)
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
                
                TileIDToTextureCoords(tile.ID, out textureX, out textureY);
                
                // normal tile
                if (!tile.FlipHorizontal && !tile.FlipVertical)
                {
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
                else if (tile.FlipHorizontal && !tile.FlipVertical)
                {
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
                else if (!tile.FlipHorizontal && tile.FlipVertical)
                {
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
                else
                {
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
                
                AddVertexIndex(tile, i * 4);
            }
        }

        _size = v2 - v;
    }

    private unsafe void UpdateAnimations()
    {
        if (!_animationEnabled)
        {
            return;
        }

        for (int i = 0; i < _tileAnimations.Length; i++)
        {
            AnimatedTile tileAnimation = _tileAnimations[i];
            float num = Engine.Frame * tileAnimation.AnimationSpeed;
            uint num2 = (uint)tileAnimation.Tiles[(int)num % tileAnimation.Tiles.Length];
            TileIDToTextureCoords(num2 - 1U, out uint tileX, out uint tileY);
            fixed (Vertex* ptr = _vertices)
            {
                for (int j = 0; j < tileAnimation.VertexIndexes.Count; j++)
                {
                    int num5 = tileAnimation.VertexIndexes[j];
                    Vertex* ptr2 = ptr + num5;
                    
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

    public override void Draw(RenderTarget target)
    {
        TileGroupShader.SetUniform("image", _tileset.Image);
        TileGroupShader.SetUniform("palette", _tileset.Palette);
        TileGroupShader.SetUniform("palIndex", _tileset.CurrentPaletteFloat);
        TileGroupShader.SetUniform("palSize", _tileset.PaletteSize);
        TileGroupShader.SetUniform("blend", _blendColor);
        TileGroupShader.SetUniform("blendMode", 1f);

        UpdateAnimations();
        target.Draw(_vertices, PrimitiveType.Quads, _renderState);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            TextureManager.Instance.Unuse(_tileset);
        }

        _disposed = true;
    }
}