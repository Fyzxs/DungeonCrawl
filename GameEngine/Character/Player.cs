using System;
using System.Collections.Generic;
using GameEngine.Display;
using GameEngine.Features;

namespace GameEngine.Character
{
    public class Player : Character
    {
        private const int DefaultVisionDistance = 3;

        public override int VisionDistance
        {
            get { return DefaultVisionDistance; }
        }

        public Player(Point startingLocation) : base(startingLocation)
        {
            Move(0, 0);//Triggers initial visible zone
        }

        public bool CanSee(Character character)
        {
            return CharacterVision.CanSee(character);
        }
        public bool CanSee(Feature feature)
        {
            return CharacterVision.CanSee(feature);
        }

        public bool HasSeen(Feature feature)
        {
            return CharacterVision.HasSeen(feature);
        }


        public override void Move(ConsoleKey key = ConsoleKey.NoName)
        {
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
                default:
                    Move(0, 0);
                    break;
            }
        }

        private void Move(int xMod, int yMod)
        {
            var map = Level.Instance.FeatureGrid;

            if (!CanMove(xMod, yMod, map)) return;

            ThisBoundary.Coords.X += xMod;
            ThisBoundary.Coords.Y += yMod;

            CharacterVision.FlagActive(map);
        }
    }
}
