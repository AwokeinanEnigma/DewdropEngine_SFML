#region

using DewDrop.Graphics;
using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;

#endregion

namespace DewDrop.Tiles;

public class TileChunk : Renderable
{        public bool AnimationEnabled
        {
            get
            {
                return this.animationEnabled;
            }
            set
            {
                this.animationEnabled = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                this.ResetTransform();
            }
        }

        public override Vector2 Origin
        {
            get
            {
                return _origin;
            }
            set
            {
                _origin = value;
                this.ResetTransform();
            }
        }

        public SpritesheetTexture Tileset
        {
            get
            {
                return this.tileset;
            }
        }

        public TileChunk(IList<Tile> tiles, string resource, int depth, Vector2 position, uint palette)
        {
            this.tileset = TextureManager.Instance.Use(resource);
            this.tileset.CurrentPalette = palette;
            _position = position;
            this._depth = depth;
            this.renderState = new RenderStates(BlendMode.Alpha, Transform.Identity, this.tileset.Image, TILE_GROUP_SHADER);
            this.animationEnabled = true;
            this.CreateAnimations(this.tileset.GetSpriteDefinitions());
            this.CreateVertexArray(tiles);
            this.ResetTransform();
        }

        private void ResetTransform()
        {
            Transform identity = Transform.Identity;
            identity.Translate(_position - _origin);
            this.renderState.Transform = identity;
        }

        public int GetTileId(Vector2 location)
        {
            Vector2 Vector2 = location - _position + _origin;
            uint num = (uint)(Vector2.X / 8f + Vector2.Y / 8f * (_size.X / 8f));
            Vertex vertex = this.vertices[(int)((UIntPtr)(num * 4U))];
            Vector2 texCoords = vertex.TexCoords;
            return (int)(texCoords.X / 8f + texCoords.Y / 8f * (this.tileset.Image.Size.X / 8U));
        }

        private void IDToTexCoords(uint id, out uint tx, out uint ty)
        {
            tx = id * 8U % this.tileset.Image.Size.X;
            ty = id * 8U / this.tileset.Image.Size.X * 8U;
        }

        private void CreateAnimations(ICollection<SpriteDefinition> definitions)
        {
            this.animations = new TileAnimation[definitions.Count];
            foreach (SpriteDefinition spriteDefinition in definitions)
            {
                int num = -1;
                int.TryParse(spriteDefinition.Name, out num);
                if (num >= 0)
                {
                    if (spriteDefinition.Data != null && spriteDefinition.Data.Length > 0)
                    {
                        int[] data = spriteDefinition.Data;
                        float speed = spriteDefinition.Speeds[0];
                        this.animations[num].Tiles = data;
                        this.animations[num].VertIndexes = new List<int>();
                        this.animations[num].Speed = speed;
                    }
                    else
                    {
                        Console.WriteLine("Tried to load tile animation data for animation {0}, but there was no tile data.", num);
                    }
                }
            }
        }

        private void AddVertIndex(Tile tile, int index)
        {
            if (tile.AnimationId > 0)
            {
                try
                {
                    int num = tile.AnimationId - 1;
                    this.animations[num].VertIndexes.Add(index);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ index } was outside range of the array! Error: {ex}");
                    int num = tile.AnimationId;
                    this.animations[num].VertIndexes.Add(index);
                }
            }
        }

        private unsafe void CreateVertexArray(IList<Tile> tiles)
        {
            this.vertices = new Vertex[tiles.Count * 4];
            uint num = 0U;
            uint num2 = 0U;
            Vector2 v = default(Vector2);
            Vector2 v2 = default(Vector2);
            fixed (Vertex* ptr = this.vertices)
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
                    this.IDToTexCoords(tile.ID, out num, out num2);
                    if (!tile.FlipHorizontal && !tile.FlipVertical)
                    {
                        ptr2->TexCoords.X = num;
                        ptr2->TexCoords.Y = num2;
                        ptr2[1].TexCoords.X = num + 8U;
                        ptr2[1].TexCoords.Y = num2;
                        ptr2[2].TexCoords.X = num + 8U;
                        ptr2[2].TexCoords.Y = num2 + 8U;
                        ptr2[3].TexCoords.X = num;
                        ptr2[3].TexCoords.Y = num2 + 8U;
                    }
                    else if (tile.FlipHorizontal && !tile.FlipVertical)
                    {
                        ptr2->TexCoords.X = num + 8U;
                        ptr2->TexCoords.Y = num2;
                        ptr2[1].TexCoords.X = num;
                        ptr2[1].TexCoords.Y = num2;
                        ptr2[2].TexCoords.X = num;
                        ptr2[2].TexCoords.Y = num2 + 8U;
                        ptr2[3].TexCoords.X = num + 8U;
                        ptr2[3].TexCoords.Y = num2 + 8U;
                    }
                    else if (!tile.FlipHorizontal && tile.FlipVertical)
                    {
                        ptr2->TexCoords.X = num;
                        ptr2->TexCoords.Y = num2 + 8U;
                        ptr2[1].TexCoords.X = num + 8U;
                        ptr2[1].TexCoords.Y = num2 + 8U;
                        ptr2[2].TexCoords.X = num + 8U;
                        ptr2[2].TexCoords.Y = num2;
                        ptr2[3].TexCoords.X = num;
                        ptr2[3].TexCoords.Y = num2;
                    }
                    else
                    {
                        ptr2->TexCoords.X = num + 8U;
                        ptr2->TexCoords.Y = num2 + 8U;
                        ptr2[1].TexCoords.X = num;
                        ptr2[1].TexCoords.Y = num2 + 8U;
                        ptr2[2].TexCoords.X = num;
                        ptr2[2].TexCoords.Y = num2;
                        ptr2[3].TexCoords.X = num + 8U;
                        ptr2[3].TexCoords.Y = num2;
                    }
                    v.X = Math.Min(v.X, ptr2->Position.X);
                    v.Y = Math.Min(v.Y, ptr2->Position.Y);
                    v2.X = Math.Max(v2.X, ptr2[2].Position.X - v.X);
                    v2.Y = Math.Max(v2.Y, ptr2[2].Position.Y - v.Y);
                    this.AddVertIndex(tile, i * 4);
                }
            }
            _size = v2 - v;
        }

        private unsafe void UpdateAnimations()
        {
            if (!this.animationEnabled)
            {
                return;
            }
            for (int i = 0; i < this.animations.Length; i++)
            {
                TileAnimation tileAnimation = this.animations[i];
                float num = Engine.Frame * tileAnimation.Speed;
                uint num2 = (uint)tileAnimation.Tiles[(int)num % tileAnimation.Tiles.Length];
                this.IDToTexCoords(num2 - 1U, out uint num3, out uint num4);
                fixed (Vertex* ptr = this.vertices)
                {
                    for (int j = 0; j < tileAnimation.VertIndexes.Count; j++)
                    {
                        int num5 = tileAnimation.VertIndexes[j];
                        Vertex* ptr2 = ptr + num5;
                        ptr2->TexCoords.X = num3;
                        ptr2->TexCoords.Y = num4;
                        ptr2[1].TexCoords.X = num3 + 8U;
                        ptr2[1].TexCoords.Y = num4;
                        ptr2[2].TexCoords.X = num3 + 8U;
                        ptr2[2].TexCoords.Y = num4 + 8U;
                        ptr2[3].TexCoords.X = num3;
                        ptr2[3].TexCoords.Y = num4 + 8U;
                    }
                }
            }
        }

        public override void Draw(RenderTarget target)
        {
            TILE_GROUP_SHADER.SetParameter("image", this.tileset.Image);
            TILE_GROUP_SHADER.SetParameter("palette", this.tileset.Palette);
            TILE_GROUP_SHADER.SetParameter("palIndex", this.tileset.CurrentPaletteFloat);
            TILE_GROUP_SHADER.SetParameter("palSize", this.tileset.PaletteSize);
            TILE_GROUP_SHADER.SetParameter("blend", Color.White);
            TILE_GROUP_SHADER.SetParameter("blendMode", 1f);
            this.UpdateAnimations();
            target.Draw(this.vertices, PrimitiveType.Quads, this.renderState);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                TextureManager.Instance.Unuse(this.tileset);
            }
            this._disposed = true;
        }

        private static readonly Shader TILE_GROUP_SHADER = new(EmbeddedResourcesHandler.GetResourceStream("pal.vert"), null, EmbeddedResourcesHandler.GetResourceStream("pal.frag"));

        private Vertex[] vertices;

        private SpritesheetTexture tileset;

        private RenderStates renderState;

        private TileAnimation[] animations;

        private bool animationEnabled;

        private struct TileAnimation
        {
            public int[] Tiles;

            public IList<int> VertIndexes;

            public float Speed;
        }
    }
