using System;
using System.Collections.Generic;
using System.Linq;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    internal class Player : Character
    {
        private const int VisionDistance = 3;

        public string Message = "";
        private readonly List<Feature> visibleFeatures = new List<Feature>(VisionDistance*VisionDistance*2);
        private readonly List<Feature> hasSeenFeatures = new List<Feature>();

        public Player(Point startingLocation) : base(startingLocation)
        {

        }

        public bool CanSee(Feature feature)
        {
            return visibleFeatures.Contains(feature);
        }

        public bool HasSeen(Feature feature)
        {
            return hasSeenFeatures.Contains(feature);
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

            FlagActive(map);
        }

        private void FlagActive(Feature[,] map)
        {
            var blocked = new bool[VisionDistance*2+1];
            visibleFeatures.Clear();
            for (var pos = 0; pos < VisionDistance; pos++)
            {
                for (var posSec = pos; posSec < VisionDistance; posSec++)
                {
                    var noAdjustment = map[Left+pos, Top+posSec];
                    if (!NullFeature.IsNullFeature(noAdjustment))
                    {
                        visibleFeatures.Add(noAdjustment);
                        hasSeenFeatures.Add(noAdjustment);
                    }
                }
            }
        }

    }
}
