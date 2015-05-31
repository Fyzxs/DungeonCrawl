using System;
using System.Collections.Generic;
using System.Linq;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class Hallway : Feature
    {
        public Hallway(Boundary boundary)
            : base(boundary)
        {
        }

        /*
         * Hallway needs an override to prevent an empty square from having a Hallway reference
         * in the grid.
         */
        protected override void FillFeatureGrid(Feature[,] grid, int leftAdj, int topAdj)
        {
            base.FillFeatureGrid(grid, leftAdj, topAdj);
            if (grid[leftAdj + Left, topAdj + Top] == this)
            {
                grid[leftAdj + Left, topAdj + Top] = NullFeature.Instance;
            }
        }
    }
}
