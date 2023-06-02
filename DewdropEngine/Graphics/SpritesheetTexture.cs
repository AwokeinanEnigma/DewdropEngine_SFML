using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Dewdrop.Graphics;
using DewDrop.Utility;

namespace DewDrop.Graphics
{
    /// <summary>
    /// Holds information about a spritesheet's sprite definitions, its grayscale image, and its palette.
    /// </summary>
    public class SpritesheetTexture : ITexture
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("csfml-graphics", CallingConvention = CallingConvention.Cdecl)]
        private unsafe static extern void sfTexture_updateFromPixels(IntPtr texture, byte* pixels, uint width, uint height, uint x, uint y);

        #region Properties
        public Texture Image
        {
            get => this._imageTex;
            set => this._imageTex = value;
        }

        public Texture Palette => this._paletteTex;

        public uint CurrentPalette
        {
            get => this._currentPal;
            set
            {
                this._currentPal = Math.Min(this._totalPals, value);
            }
        }

        public float CurrentPaletteFloat => (float)this._currentPal / (float)this._totalPals;

        public uint PaletteCount => this._totalPals;

        public uint PaletteSize => this._palSize;

        #endregion

        #region Sprite Definitions

        private readonly SpriteDefinition _defaultDefinition;
        private readonly Dictionary<int, SpriteDefinition> _definitions;
     
        public SpriteDefinition GetSpriteDefinition(string name)
        {

            int hashCode = name.GetHashCode();
            return this.GetSpriteDefinition(hashCode);
        }

        public SpriteDefinition GetSpriteDefinition(int hash)
        {
            SpriteDefinition result;
            if (!_definitions.TryGetValue(hash, out result))
            {
                result = default;
            }
            return result;
        }

        public ICollection<SpriteDefinition> GetSpriteDefinitions()
        {
            return this._definitions.Values;
        }

        public SpriteDefinition GetDefaultSpriteDefinition()
        {
            return this._defaultDefinition;
        }

        
        #endregion

        #region Textures

        private readonly Texture _paletteTex;
        private Texture _imageTex;

        #endregion

        #region Palette

        private uint _currentPal;
        private readonly uint _totalPals;
        private readonly uint _palSize;

        #endregion


        private bool _disposed;

        public unsafe SpritesheetTexture(uint imageWidth, int[][] palettes, byte[] image, Dictionary<int, SpriteDefinition> definitions, SpriteDefinition defaultDefinition)
        {
            // create palette
            this._totalPals = (uint)palettes.Length;
            this._palSize = (uint)palettes[0].Length;
            this._paletteTex = new Texture(this._palSize, this._totalPals);

            
            uint imageHeight = (uint)(image.Length / (int)imageWidth);
            this._imageTex = new Texture(imageWidth, imageHeight);
            
            Color[] totalColors = new Color[this._palSize * this._totalPals];
            for (uint allPalettes = 0; allPalettes < this._totalPals; allPalettes++)
            {
                uint colors = 0;
                while (colors < palettes[allPalettes].Length)
                {
                    totalColors[allPalettes * this._palSize + colors] = ColorHelper.FromInt(palettes[allPalettes][colors]);
                    colors++;
                }
            }
            Color[] uncoloredPixels = new Color[imageWidth * imageHeight];
            uint pixels = 0;
            while (pixels < image.Length)
            {
                uncoloredPixels[pixels].A = byte.MaxValue;
                uncoloredPixels[pixels].R = image[pixels];
                uncoloredPixels[pixels].G = image[pixels];
                uncoloredPixels[pixels].B = image[pixels];
                pixels++;
            }

            fixed (Color* ptr = totalColors)
            {
                byte* b_pixels = (byte*)ptr;
                SpritesheetTexture.sfTexture_updateFromPixels(this._paletteTex.CPointer, b_pixels, this._palSize, this._totalPals, 0, 0);
            }
            fixed (Color* ptr2 = uncoloredPixels)
            {
                byte* pixels2 = (byte*)ptr2;
                SpritesheetTexture.sfTexture_updateFromPixels(this._imageTex.CPointer, pixels2, imageWidth, imageHeight, 0, 0);
            }
            this._definitions = definitions;
            this._defaultDefinition = defaultDefinition;
        }

        ~SpritesheetTexture()
        {
            this.Dispose(false);
        }

        public void ToFullColorTexture()
        {
            uint x1 = this._imageTex.Size.X;
            uint y1 = this._imageTex.Size.Y;
            Image image1 = new SFML.Graphics.Image(x1, y1);
            Image image2 = this._imageTex.CopyToImage();
            Image image3 = this._paletteTex.CopyToImage();
            for (uint y2 = 0; y2 < y1; ++y2)
            {
                for (uint x2 = 0; x2 < x1; ++x2)
                {
                    uint x3 = (uint) ((double) image2.GetPixel(x2, y2).R / (double) byte.MaxValue * (double) this._palSize);
                    Color pixel = image3.GetPixel(x3, this._currentPal);
                    image1.SetPixel(x2, y2, pixel);
                }
            }
            image1.SaveToFile("img.png");
            image2.SaveToFile("indImg.png");
            image3.SaveToFile("palImg.png");
        }

        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                this._imageTex.Dispose();
                this._paletteTex.Dispose();
            }
            this._disposed = true;
        }
    }
}
