using System;
using System.Net;
using GameEngine.Display;

namespace GameEngine.Features
{
    public class FloorTile : Tile
    {
        public FloorTile(Point point) : base(point)
        {
        }

        public override bool IsPassable()
        {
            return true;
        }

        public override bool IsVisionBlocking()
        {
            return false;
        }

    }
}
