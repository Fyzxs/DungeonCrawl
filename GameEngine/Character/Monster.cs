using System;
using GameEngine.Display;
using GameEngine.Features;

namespace GameEngine.Character
{
    public class Monster : Character
    {
        private ConsoleKey previousDirection = ConsoleKey.DownArrow;

        public Monster(Point startingLocation) : base(startingLocation)
        {
        }


        public override int VisionDistance
        {
            get { return 2; }
        }

        public override void Move(ConsoleKey key = ConsoleKey.NoName)
        {
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
            CharacterVision.FlagActive(Level.Instance.FeatureGrid);
        }

        private bool Move(int xMod, int yMod)
        {
            var map = Level.Instance.FeatureGrid;

            if (!CanMove(xMod, yMod, map)) return false;

            ThisBoundary.Coords.X += xMod;
            ThisBoundary.Coords.Y += yMod;
            return true;
        }
    }
}
