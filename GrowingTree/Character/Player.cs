using System;
using System.Collections.Generic;
using GrowingTree.Display;
using GrowingTree.Features;

namespace GrowingTree.Character
{
    internal class Player : Character
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

        protected override void DrawImpl(DrawGrid drawGrid)
        {
            drawGrid.CurrentForeground = ConsoleColor.Green;
            drawGrid.CurrentBackground = ConsoleColor.DarkGreen;
            drawGrid.Place('P');
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

        // /*
        // * Returns if the tile was valid
        // */
        //private bool ProcessTile(Feature[,] map, int x, int y, int width, int height)
        //{
        //    Feature tile;
        //    if (x < 0 || x >= width ||
        //        y < 0 || y >= height ||
        //        NullFeature.IsNullFeature(tile = map[x, y]))
        //    {
        //        return false;
        //    }

        //    visibleFeatures.Add(tile);
        //    hasSeenFeatures.Add(tile);
        //    return true;
        //}

        //private void FlagActive(Feature[,] map)
        //{
        //    var width = map.GetLength(0);
        //    var height = map.GetLength(1);
        //    const int rightLineBlockedIndex = 0;
        //    const int leftLineBlockedIndex = 1;
        //    const int downLineBlockedIndex = 2;
        //    const int upLineBlockedIndex = 3;
        //    const int upLeftLineBlockedIndex = 4;
        //    const int downLeftLineBlockedIndex = 5;
        //    const int downRightLineBlockedIndex = 6;
        //    const int upRightLineBlockedIndex = 7;
        //    // The ordering of this doesn't really matter - Just needs to be 
        //    // vision distance separated - Except the first one; that's just up 1
        //    const int rightUpBlockedIndexStart = 8;
        //    const int downRightBlockedIndexStart = rightUpBlockedIndexStart + VisionDistance;
        //    const int upRightBlockedIndexStart = downRightBlockedIndexStart + VisionDistance;
        //    const int rightDownBlockedIndexStart = upRightBlockedIndexStart + VisionDistance;
        //    const int upLeftBlockedIndexStart = rightDownBlockedIndexStart + VisionDistance;
        //    const int leftUpBlockedIndexStart = upLeftBlockedIndexStart + VisionDistance;
        //    const int downLeftBlockedIndexStart = leftUpBlockedIndexStart + VisionDistance;
        //    const int leftDownBlockedIndexStart = downLeftBlockedIndexStart + VisionDistance;

        //    /*
        //     * EXPLANATION TIME
        //     * It's reduced from 8 * (VisionDistance - 1) * 8
        //     * The first 8 is the 8 cardinal directions
        //     * Each quadrant has an up and down direction; and there will be 
        //     * N-1 of them.
        //     * Since there's 2 in each quadrant; 2*4 = 8... the end 8.
        //     */
        //    var blocked = new bool[8 * (1 + VisionDistance)];

        //    //We need to recalc the visible things
        //    visibleFeatures.Clear();
        //    for (var pos = 1; pos < VisionDistance; pos++)
        //    {
        //        //The posSec for loop only runs when pos is greater than 2
        //        // so if it's < 2; continue main loop
        //        if (pos >= 2)
        //        {
        //            for (var posSec = pos; posSec < VisionDistance; posSec++)
        //            {
        //                if (!blocked[rightUpBlockedIndexStart + pos - 1] &&
        //                    !blocked[rightLineBlockedIndex] &&
        //                    !blocked[upRightLineBlockedIndex])
        //                {
        //                    var rightUpX = Left + posSec;
        //                    var rightUpY = Top - pos + 1;
        //                    blocked[rightUpBlockedIndexStart + pos - 1] =
        //                        !ProcessTile(map, rightUpX, rightUpY, width, height);
        //                }

        //                if (!blocked[upRightBlockedIndexStart + pos - 1] &&
        //                    !blocked[upLineBlockedIndex] &&
        //                    !blocked[upRightLineBlockedIndex])
        //                {
        //                    var upRightX = Left + pos - 1;
        //                    var upRightY = Top - posSec;
        //                    blocked[upRightBlockedIndexStart + pos - 1] =
        //                        !ProcessTile(map, upRightX, upRightY, width, height);
        //                }

        //                if (!blocked[downRightBlockedIndexStart + pos - 1] &&
        //                    !blocked[downRightLineBlockedIndex] &&
        //                    !blocked[downLineBlockedIndex])
        //                {
        //                    var downRightX = Left + pos - 1;
        //                    var downRightY = Top + posSec;
        //                    blocked[downRightBlockedIndexStart + pos - 1] =
        //                        !ProcessTile(map, downRightX, downRightY, width, height);
        //                }

        //                if (!blocked[rightDownBlockedIndexStart + pos - 1] &&
        //                    !blocked[downRightLineBlockedIndex] &&
        //                    !blocked[rightLineBlockedIndex])
        //                {
        //                    var rightDownX = Left + posSec;
        //                    var rightDownY = Top + pos - 1;
        //                    blocked[rightDownBlockedIndexStart + pos - 1] =
        //                        !ProcessTile(map, rightDownX, rightDownY, width, height);
        //                }

        //                if (!blocked[upLeftBlockedIndexStart + pos - 1] &&
        //                    !blocked[upLeftLineBlockedIndex] &&
        //                    !blocked[upLineBlockedIndex])
        //                {
        //                    var upLeftX = Left - pos + 1;
        //                    var upLeftY = Top - posSec;
        //                    blocked[upLeftBlockedIndexStart + pos - 1] =
        //                        !ProcessTile(map, upLeftX, upLeftY, width, height);
        //                }

        //                if (!blocked[leftUpBlockedIndexStart + pos - 1] &&
        //                    !blocked[upLeftLineBlockedIndex] &&
        //                    !blocked[leftLineBlockedIndex])
        //                {
        //                    var leftUpX = Left - posSec;
        //                    var leftUpY = Top - pos + 1;
        //                    blocked[leftUpBlockedIndexStart + pos - 1] =
        //                        !ProcessTile(map, leftUpX, leftUpY, width, height);
        //                }

        //                if (!blocked[downLeftBlockedIndexStart + pos - 1] &&
        //                    !blocked[downLeftLineBlockedIndex] &&
        //                    !blocked[downLineBlockedIndex])
        //                {
        //                    var downLeftX = Left - pos + 1;
        //                    var downLeftY = Top + posSec;
        //                    blocked[downLeftBlockedIndexStart + pos - 1] =
        //                        !ProcessTile(map, downLeftX, downLeftY, width, height);
        //                }


        //                if (!blocked[leftDownBlockedIndexStart + pos - 1] &&
        //                    !blocked[downLeftLineBlockedIndex] &&
        //                    !blocked[leftLineBlockedIndex])
        //                {
        //                    var leftDownX = Left - posSec;
        //                    var leftDownY = Top + pos - 1;
        //                    blocked[leftDownBlockedIndexStart + pos - 1] =
        //                        !ProcessTile(map, leftDownX, leftDownY, width, height);
        //                }
        //            }
        //        }

        //        #region Straight Lines

        //        if (!blocked[rightLineBlockedIndex])
        //        {
        //            var rX = Left + pos;
        //            var rY = Top;
        //            blocked[rightLineBlockedIndex] = !ProcessTile(map, rX, rY, width, height);
        //        }


        //        if (!blocked[leftLineBlockedIndex])
        //        {
        //            var lX = Left - pos;
        //            var lY = Top;
        //            blocked[leftLineBlockedIndex] = !ProcessTile(map, lX, lY, width, height);
        //        }

        //        if (!blocked[downLineBlockedIndex])
        //        {
        //            var dX = Left;
        //            var dY = Top + pos;
        //            blocked[downLineBlockedIndex] = !ProcessTile(map, dX, dY, width, height);
        //        }

        //        if (!blocked[upLineBlockedIndex])
        //        {
        //            var uX = Left;
        //            var uY = Top - pos;
        //            blocked[upLineBlockedIndex] = !ProcessTile(map, uX, uY, width, height);
        //        }
        //        #endregion

        //        #region Diagonal

        //        if (!blocked[upRightLineBlockedIndex])
        //        {
        //            var urDiagX = Left + pos;
        //            var urDiagY = Top - pos;
        //            blocked[upRightLineBlockedIndex] = !ProcessTile(map, urDiagX, urDiagY, width, height);
        //        }

        //        if (!blocked[downLeftLineBlockedIndex])
        //        {
        //            var dlDiagX = Left - pos;
        //            var dlDiagY = Top + pos;
        //            blocked[downLeftLineBlockedIndex] = !ProcessTile(map, dlDiagX, dlDiagY, width, height);
        //        }

        //        if (!blocked[upLeftLineBlockedIndex])
        //        {
        //            var ulDiagX = Left - pos;
        //            var ulDiagY = Top - pos;
        //            blocked[upLeftLineBlockedIndex] = !ProcessTile(map, ulDiagX, ulDiagY, width, height);
        //        }

        //        if (!blocked[downRightLineBlockedIndex])
        //        {
        //            var drDiagX = Left + pos;
        //            var drDiagY = Top + pos;
        //            blocked[downRightLineBlockedIndex] = !ProcessTile(map, drDiagX, drDiagY, width, height);
        //        }

        //        #endregion
        //    }
        //}

    }
}
