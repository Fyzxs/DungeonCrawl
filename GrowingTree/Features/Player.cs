using System;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class Player : Character
    {
        public string Message = "";

        public Player(Point startingLocation) : base(startingLocation)
        {

        }

        protected override void DrawImpl(DrawGrid drawGrid)
        {
            drawGrid.CurrentForeground = ConsoleColor.Green;
            drawGrid.CurrentBackground = ConsoleColor.DarkGreen;
            drawGrid.Place('P');
        }

        public override void Move(ConsoleKey key = ConsoleKey.NoName)
        {
            Message = "";
            switch (key)
            {
                case ConsoleKey.DownArrow:
                    Move(0, 1);
                    break;
                case ConsoleKey.UpArrow:
                    Move(0, -1);
                    break;
                case ConsoleKey.LeftArrow:
                    Move(-1, 0);
                    break;
                case ConsoleKey.RightArrow:
                    Move(+1, 0);
                    break;
            }
        }

        private void Move(int xMod, int yMod)
        {
            var map = Level.Instance.FeatureGrid;

            if (ThisBoundary.Coords.X + xMod < 0 ||
                ThisBoundary.Coords.Y + yMod < 0 ||
                ThisBoundary.Coords.X + xMod >= Level.Instance.Width ||
                ThisBoundary.Coords.Y + yMod >= Level.Instance.Height ||
                NullFeature.IsNullFeature(map[ThisBoundary.Coords.X + xMod, ThisBoundary.Coords.Y + yMod]))
            {
                return;
            }

            ThisBoundary.Coords.X += xMod;
            ThisBoundary.Coords.Y += yMod;
        }
    }
}
