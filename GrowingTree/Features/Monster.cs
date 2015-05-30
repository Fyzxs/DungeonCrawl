using System;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class Monster : Character
    {
        public string Message = "";
        private ConsoleKey previousDirection = ConsoleKey.DownArrow;

        public Monster(Point startingLocation) : base(startingLocation)
        {
        }

        protected override void DrawImpl(DrawGrid drawGrid)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write("M");
        }

        public override void Move(ConsoleKey key = ConsoleKey.NoName)
        {
            Message = "";
            var x = new []{ConsoleKey.DownArrow, ConsoleKey.UpArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow};
            var moved = false;
            
            var dir = Rand.Next(10) < 7 ? previousDirection : x[Rand.Next(4)];
            while (!moved)
            {

                switch (dir)
                {
                    case ConsoleKey.DownArrow:
                        moved = Move(0, +1);
                        break;
                    case ConsoleKey.UpArrow:
                        moved = Move(0, -1);
                        break;
                    case ConsoleKey.LeftArrow:
                        moved = Move(-1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        moved = Move(+1, 0);
                        break;
                }
                if (moved)
                {
                    previousDirection = dir;
                }
                else
                {
                    dir = x[Rand.Next(4)];
                }
            }
        }



        private bool Move(int xMod, int yMod)
        {
            var map = Level.Instance.FeatureGrid;

            if (ThisBoundary.Coords.X + xMod < 0 ||
                ThisBoundary.Coords.Y + yMod < 0 ||
                ThisBoundary.Coords.X + xMod >= Level.Instance.Width ||
                ThisBoundary.Coords.Y + yMod >= Level.Instance.Height ||
                NullFeature.IsNullFeature(map[ThisBoundary.Coords.X + xMod, ThisBoundary.Coords.Y + yMod]))
            {
                return false;
            }

            ThisBoundary.Coords.X += xMod;
            ThisBoundary.Coords.Y += yMod;
            return true;
        }
    }
}
