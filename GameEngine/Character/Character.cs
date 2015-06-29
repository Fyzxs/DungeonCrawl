using System;
using GameEngine.Features;

namespace GameEngine.Character
{
    public abstract class Character : Feature
    {
        protected readonly Vision CharacterVision;

        protected Character(Point point) : base(new Boundary(point, 1, 1))
        {
            CharacterVision = new Vision(this);
        }

        public abstract int VisionDistance { get; }

        public abstract void Move(ConsoleKey key = ConsoleKey.NoName);

        protected virtual bool CanMove(int xMod, int yMod, Feature[,] map)
        {
            return ThisBoundary.Coords.X + xMod >= 0 &&
                ThisBoundary.Coords.Y + yMod >= 0 &&
                ThisBoundary.Coords.X + xMod < Level.Instance.Width &&
                ThisBoundary.Coords.Y + yMod < Level.Instance.Height &&
                (map[ThisBoundary.Coords.X + xMod, ThisBoundary.Coords.Y + yMod]).IsPassable();
        }

        protected override void FillFeatureGrid(Feature[,] grid, int leftAdj, int topAdj)
        {
            //No Op
        }
    }
}
