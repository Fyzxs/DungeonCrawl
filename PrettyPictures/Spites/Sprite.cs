using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettyPictures.Spites
{
    internal abstract class Sprite
    {
        protected int Left;
        protected int Top;
        protected int Width;
        protected int Height;

        protected bool InvalidateBitMap;

        protected Bitmap SpriteMap;

        protected Sprite(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public Bitmap GetBitmap()
        {
            if (SpriteMap != null && !InvalidateBitMap) return SpriteMap;

            SpriteMap = GenerateBitmap();
            SpriteMap.MakeTransparent(Color.FromArgb(255, 255, 0));
            return SpriteMap;
        }

        protected abstract Bitmap GenerateBitmap();
    }
}
