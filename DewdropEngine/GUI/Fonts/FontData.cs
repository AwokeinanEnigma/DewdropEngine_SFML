using DewDrop.Resources;
using DewDrop.Utilities;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DewDrop.GUI.Fonts
{
    public class FontData : IDisposable
    {
        private bool disposed;
        private Font font;
        private int xComp;
        private int yComp;
        private int lineHeight;
        private int wHeight;
        private uint fontSize;
        private float alphaThreshold;

        public Font Font
        {
            get
            {
                return this.font;
            }
        }

        public int XCompensation
        {
            get
            {
                return this.xComp;
            }
        }

        public int YCompensation
        {
            get
            {
                return this.yComp;
            }
        }

        public int LineHeight
        {
            get
            {
                return this.lineHeight;
            }
        }

        public int WHeight
        {
            get
            {
                return this.wHeight;
            }
        }

        public uint Size
        {
            get
            {
                return this.fontSize;
            }
        }

        public float AlphaThreshold
        {
            get
            {
                return this.alphaThreshold;
            }
        }

        public FontData()
        {
            this.font = new Font(EmbeddedResourcesHandler.GetResourceStream("openSansPX.ttf"));

            this.fontSize = 16U;
            this.wHeight = (int)font.GetGlyph(41U, fontSize, false, 1).Bounds.Height;
            this.lineHeight = (int)(wHeight * 1.20000004768372);
            this.alphaThreshold = 0.0f;
        }

        public FontData(Font font, uint fontSize, int lineHeight, int xComp, int yComp)
        {
            this.font = font;
            this.fontSize = fontSize;
            this.lineHeight = lineHeight;
            this.xComp = xComp;
            this.yComp = yComp;
            this.wHeight = (int)this.font.GetGlyph(41U, this.fontSize, false, 1).Bounds.Height;
            // Console.WriteLine($"wHeight = {wHeight}");
            this.alphaThreshold = 0.8f;
        }

        ~FontData()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                this.font.Dispose();
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

