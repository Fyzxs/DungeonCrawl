using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettyPictures.Spites.Map
{
    abstract class MapSprite : Sprite
    {
        private const string MapSpriteLocation = "scbw_tiles.png";

        protected MapSprite(int left, int top, int width, int height) : base(left, top, width, height)
        {
        }

        protected override Bitmap GenerateBitmap()
        {
            var bitmap = new Bitmap(MapSpriteLocation);
            // Clone a portion of the Bitmap object.
            var cloneRect = new Rectangle(Left, Top, Width, Height);
            var format = bitmap.PixelFormat;
            return bitmap.Clone(cloneRect, format);
        }
    }
}
