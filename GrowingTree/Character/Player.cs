using System;
using System.Collections.Generic;
using GrowingTree.Display;
using GrowingTree.Features;

namespace GrowingTree.Character
{
    internal class Player : Character
    {
        private const int VisionDistance = 5;

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
                    ////No D-R diag
                    ////Down Line
                    //var downRightX = Left + pos;
                    //var downRightY = Top + posSec + 1;
                    //var downRight = map[downRightX, downRightY];
                    //if (!NullFeature.IsNullFeature(downRight))
                    //{
                    //    visibleFeatures.Add(downRight);
                    //    hasSeenFeatures.Add(downRight);
                    //}

                    ////Has D-R diag
                    ////Right Line
                    //var rightDownX = Left + posSec;
                    //var rightDownY = Top + pos;
                    //var rightDown = map[rightDownX, rightDownY];
                    //if (!NullFeature.IsNullFeature(rightDown))
                    //{
                    //    visibleFeatures.Add(rightDown);
                    //    hasSeenFeatures.Add(rightDown);
                    //}

                    ////No T-L diag
                    ////Has Left Line
                    //var upLeftX = Left - pos;
                    //var upLeftY = Top - posSec - 1;
                    //var upLeft = map[upLeftX, upLeftY];
                    //if (!NullFeature.IsNullFeature(upLeft))
                    //{
                    //    visibleFeatures.Add(upLeft);
                    //    hasSeenFeatures.Add(upLeft);
                    //}

                    ////Has T-L Diag
                    ////Has Left Line
                    //var leftUpX = Left - posSec;
                    //var leftUpY = Top - pos;
                    //var leftUp = map[leftUpX, leftUpY];
                    //if (!NullFeature.IsNullFeature(leftUp))
                    //{
                    //    visibleFeatures.Add(leftUp);
                    //    hasSeenFeatures.Add(leftUp);
                    //}

                    ////Gotta get the diags we don't get below
                    //var ulDiagPos = pos+1;
                    //var ulDiag = map[Left + ulDiagPos, Top - ulDiagPos];
                    //if (!NullFeature.IsNullFeature(ulDiag))
                    //{
                    //    visibleFeatures.Add(ulDiag);
                    //    hasSeenFeatures.Add(ulDiag);
                    //}

                    //var dlDiagPos = pos+1;
                    //var dlDiag = map[Left - dlDiagPos, Top + dlDiagPos];
                    //if (!NullFeature.IsNullFeature(dlDiag))
                    //{
                    //    visibleFeatures.Add(dlDiag);
                    //    hasSeenFeatures.Add(dlDiag);
                    //}

                    // The previous sections cover the 
                    // horizontal, vertica, and diagnal lines
                    // 
                    // These needs to fill in the lower left 
                    // and upper right
                    if (pos >= 2)
                    {
                        var downRightX = Left + pos - 1;
                        var downRightY = Top + posSec;
                        var downRight = map[downRightX, downRightY];
                        if (!NullFeature.IsNullFeature(downRight))
                        {
                            visibleFeatures.Add(downRight);
                            hasSeenFeatures.Add(downRight);
                        }

                        var rightDownX = Left + posSec;
                        var rightDownY = Top + pos - 1;
                        var rightDown = map[rightDownX, rightDownY];
                        if (!NullFeature.IsNullFeature(rightDown))
                        {
                            visibleFeatures.Add(rightDown);
                            hasSeenFeatures.Add(rightDown);
                        }

                        var upLeftX = Left - pos + 1;
                        var upLeftY = Top - posSec;
                        var upLeft = map[upLeftX, upLeftY];
                        if (!NullFeature.IsNullFeature(upLeft))
                        {
                            visibleFeatures.Add(upLeft);
                            hasSeenFeatures.Add(upLeft);
                        }

                        var leftUpX = Left - posSec;
                        var leftUpY = Top - pos + 1;
                        var leftUp = map[leftUpX, leftUpY];
                        if (!NullFeature.IsNullFeature(leftUp))
                        {
                            visibleFeatures.Add(leftUp);
                            hasSeenFeatures.Add(leftUp);
                        }


                        var upRightX = Left + pos - 1;
                        var upRightY = Top - posSec;
                        var upRight = map[upRightX, upRightY];
                        if (!NullFeature.IsNullFeature(upRight))
                        {
                            visibleFeatures.Add(upRight);
                            hasSeenFeatures.Add(upRight);
                        }

                        var rightUpX = Left + posSec;
                        var rightUpY = Top - pos + 1;
                        var rightUp = map[rightUpX, rightUpY];
                        if (!NullFeature.IsNullFeature(rightUp))
                        {
                            visibleFeatures.Add(rightUp);
                            hasSeenFeatures.Add(rightUp);
                        }

                        var downLeftX = Left - pos + 1;
                        var downLeftY = Top + posSec;
                        var downLeft = map[downLeftX, downLeftY];
                        if (!NullFeature.IsNullFeature(downLeft))
                        {
                            visibleFeatures.Add(downLeft);
                            hasSeenFeatures.Add(downLeft);
                        }

                        var leftDownX = Left - posSec;
                        var leftDownY = Top + pos - 1;
                        var leftDown = map[leftDownX, leftDownY];
                        if (!NullFeature.IsNullFeature(leftDown))
                        {
                            visibleFeatures.Add(leftDown);
                            hasSeenFeatures.Add(leftDown);
                        }

                    }
                }
            }
        }

        /*
         * value to check
         * min is inclusive
         * max is exclusive
         */ 
        private bool IsValid(int value, int min, int max)
        {
            return value >= 0 && value < max;
        }

    }
}
