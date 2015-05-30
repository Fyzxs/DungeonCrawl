using System;
using System.Net;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class FloorTile : Tile
    {
        public FloorTile(Point point) : base(point)
        {
        }

        protected override void FillFeatureGrid(Feature[,] grid, int leftAdj, int topAdj)
        {
            if (FeatureList.Count != 0)
            {
                return;
            }

            grid[leftAdj + Left, topAdj + Top] = Parent;
        }

        protected override void DrawImpl(DrawGrid drawGrid)
        {
            drawGrid.Place(AssignedCharacter == '~' ? '#' : AssignedCharacter);
        }
    }
}
