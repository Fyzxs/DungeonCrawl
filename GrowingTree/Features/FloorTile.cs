using System;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class FloorTile : Tile
    {
        public FloorTile(Point point) : base(point)
        {
        }

        protected override void DrawImpl(DrawGrid drawGrid)
        {
            drawGrid.Place('#');
        }
    }
}
