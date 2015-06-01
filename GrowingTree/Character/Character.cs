using System;
using GrowingTree.Features;

namespace GrowingTree.Character
{
    abstract class Character : Feature
    {
        protected Character(Point point) : base(new Boundary(point, 1, 1))
        {
        }

        public abstract void Move(ConsoleKey key = ConsoleKey.NoName);

        protected override void FillFeatureGrid(Feature[,] grid, int leftAdj, int topAdj)
        {
            //No Op
        }
    }
}
