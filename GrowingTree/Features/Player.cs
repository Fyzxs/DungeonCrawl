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
            visibleFeatures.Clear();
            for (var x = 0; x <= VisionDistance; x++)
            {
                var cellFeature = Level.Instance.FeatureGrid[Left + x, Top];
                if (NullFeature.IsNullFeature(cellFeature))
                {
                    break;
                }


                FlagActiveSubFeatures(cellFeature, map);
            }
        }

        private void FlagActiveSubFeatures(Feature cellFeature, Feature[,] map)
        {
            if (cellFeature.FeatureList.Count > 0)
            {
                for (var xStart = 0; xStart <= VisionDistance; xStart++)
                {
                    if (NullFeature.IsNullFeature(map[Left + xStart, Top]))
                    {
                        break;
                    }
                    for (int x = xStart, y = 0; x <= VisionDistance; x++, y--)
                    {
                        if (NullFeature.IsNullFeature(map[Left + x, Top]))
                        {
                            break;
                        }
                        var breakFurther = false;
                        foreach (var feature in cellFeature.FeatureList)
                        {
                            if (Top+y < 0 || NullFeature.IsNullFeature(map[Left + x, Top + y]))
                            {
                                breakFurther = true;
                                break;
                            }
                            if (feature.Left == Left - cellFeature.Left + x && feature.Top == Top - cellFeature.Top + y)
                            {
                                if (NullFeature.IsNullFeature(feature))
                                {
                                    breakFurther = true;
                                    break;
                                }
                                visibleFeatures.Add(feature);
                                hasSeenFeatures.Add(feature);
                            }
                        }
                        if (breakFurther)
                        {
                            break;
                        }
                    }
                }
            }

            // [ ][ ][ ][ ][ ][ ][ ][ ]
            // [ ][ ][ ][ ][ ][ ][ ][ ]
            // [ ][ ][ ][ ][ ][ ][ ][ ]
            // [ ][ ][ ][x][+1, 0][ ][ ][ ]
            // [ ][ ][ ][ ][ ][ ][ ][ ]
            // [ ][ ][ ][ ][ ][ ][ ][ ]
            // [ ][ ][ ][ ][ ][ ][ ][ ]
        }
    }
}