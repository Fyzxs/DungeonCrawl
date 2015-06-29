using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Pathing
{
    public enum Intersectional
    {
        None = 0,
        Bottom = 10,
        BottomTop = 11,
        BottomRight = 12,
        BottomLeft = 13,
        BottomTopRight = 14,
        BottomTopLeft = 15,
        BottomTopLeftRight = 16,
        BottomLeftRight = 17,
        Top = 20,
        TopRight = 21,
        TopLeft = 22,
        TopLeftRight = 23,
        Left = 30,
        LeftRight = 31,
        Right = 40
    }
}