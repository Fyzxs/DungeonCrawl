using System.Collections.Generic;
using System.Linq;
using GrowingTree.Features;

namespace GrowingTree.Character
{
    internal class Vision
    {
        private readonly Character character;
        private readonly HashSet<Feature> visibleFeatures = new HashSet<Feature>();
        private readonly HashSet<Feature> hasSeenFeatures = new HashSet<Feature>();

        public Vision(Character character)
        {
            this.character = character;
        }

        public bool CanSee(Feature feature)
        {
            return visibleFeatures.Contains(feature);
        }
        public bool HasSeen(Feature feature)
        {
            return hasSeenFeatures.Contains(feature);
        }

        private bool ProcessTile(Feature[,] map, int x, int y, int width, int height)
        {
            Feature tile;
            if (x < 0 || x >= width ||
                y < 0 || y >= height ||
                (tile = map[x, y]).IsVisionBlocking())
            {
                return false;
            }

            visibleFeatures.Add(tile);
            hasSeenFeatures.Add(tile);
            return true;
        }

        public void FlagActive(Feature[,] map)
        {
            var visionDistance = character.VisionDistance;
            var width = map.GetLength(0);
            var height = map.GetLength(1);
            const int rightLineBlockedIndex = 0;
            const int leftLineBlockedIndex = 1;
            const int downLineBlockedIndex = 2;
            const int upLineBlockedIndex = 3;
            const int upLeftLineBlockedIndex = 4;
            const int downLeftLineBlockedIndex = 5;
            const int downRightLineBlockedIndex = 6;
            const int upRightLineBlockedIndex = 7;
            // The ordering of this doesn't really matter - Just needs to be 
            // vision distance separated - Except the first one; that's just up 1
            // ReSharper disable once ConvertToConstant.Local
            var rightUpBlockedIndexStart = 8;
            var downRightBlockedIndexStart = rightUpBlockedIndexStart + visionDistance;
            var upRightBlockedIndexStart = downRightBlockedIndexStart + visionDistance;
            var rightDownBlockedIndexStart = upRightBlockedIndexStart + visionDistance;
            var upLeftBlockedIndexStart = rightDownBlockedIndexStart + visionDistance;
            var leftUpBlockedIndexStart = upLeftBlockedIndexStart + visionDistance;
            var downLeftBlockedIndexStart = leftUpBlockedIndexStart + visionDistance;
            var leftDownBlockedIndexStart = downLeftBlockedIndexStart + visionDistance;

            /*
             * It's big enough
             */
            var blocked = new bool[8 * (1 + visionDistance)];

            //We need to recalc the visible things
            visibleFeatures.Clear();
            for (var pos = 1; pos < visionDistance; pos++)
            {
                //The posSec for loop only runs when pos is greater than 2
                // so if it's < 2; continue main loop
                if (pos >= 2)
                {
                    for (var posSec = pos; posSec < visionDistance; posSec++)
                    {
                        if (!blocked[rightUpBlockedIndexStart + pos - 1] &&
                            !blocked[rightLineBlockedIndex] &&
                            !blocked[upRightLineBlockedIndex])
                        {
                            var rightUpX = character.Left + posSec;
                            var rightUpY = character.Top - pos + 1;
                            blocked[rightUpBlockedIndexStart + pos - 1] =
                                !ProcessTile(map, rightUpX, rightUpY, width, height);
                        }

                        if (!blocked[upRightBlockedIndexStart + pos - 1] &&
                            !blocked[upLineBlockedIndex] &&
                            !blocked[upRightLineBlockedIndex])
                        {
                            var upRightX = character.Left + pos - 1;
                            var upRightY = character.Top - posSec;
                            blocked[upRightBlockedIndexStart + pos - 1] =
                                !ProcessTile(map, upRightX, upRightY, width, height);
                        }

                        if (!blocked[downRightBlockedIndexStart + pos - 1] &&
                            !blocked[downRightLineBlockedIndex] &&
                            !blocked[downLineBlockedIndex])
                        {
                            var downRightX = character.Left + pos - 1;
                            var downRightY = character.Top + posSec;
                            blocked[downRightBlockedIndexStart + pos - 1] =
                                !ProcessTile(map, downRightX, downRightY, width, height);
                        }

                        if (!blocked[rightDownBlockedIndexStart + pos - 1] &&
                            !blocked[downRightLineBlockedIndex] &&
                            !blocked[rightLineBlockedIndex])
                        {
                            var rightDownX = character.Left + posSec;
                            var rightDownY = character.Top + pos - 1;
                            blocked[rightDownBlockedIndexStart + pos - 1] =
                                !ProcessTile(map, rightDownX, rightDownY, width, height);
                        }

                        if (!blocked[upLeftBlockedIndexStart + pos - 1] &&
                            !blocked[upLeftLineBlockedIndex] &&
                            !blocked[upLineBlockedIndex])
                        {
                            var upLeftX = character.Left - pos + 1;
                            var upLeftY = character.Top - posSec;
                            blocked[upLeftBlockedIndexStart + pos - 1] =
                                !ProcessTile(map, upLeftX, upLeftY, width, height);
                        }

                        if (!blocked[leftUpBlockedIndexStart + pos - 1] &&
                            !blocked[upLeftLineBlockedIndex] &&
                            !blocked[leftLineBlockedIndex])
                        {
                            var leftUpX = character.Left - posSec;
                            var leftUpY = character.Top - pos + 1;
                            blocked[leftUpBlockedIndexStart + pos - 1] =
                                !ProcessTile(map, leftUpX, leftUpY, width, height);
                        }

                        if (!blocked[downLeftBlockedIndexStart + pos - 1] &&
                            !blocked[downLeftLineBlockedIndex] &&
                            !blocked[downLineBlockedIndex])
                        {
                            var downLeftX = character.Left - pos + 1;
                            var downLeftY = character.Top + posSec;
                            blocked[downLeftBlockedIndexStart + pos - 1] =
                                !ProcessTile(map, downLeftX, downLeftY, width, height);
                        }


                        if (!blocked[leftDownBlockedIndexStart + pos - 1] &&
                            !blocked[downLeftLineBlockedIndex] &&
                            !blocked[leftLineBlockedIndex])
                        {
                            var leftDownX = character.Left - posSec;
                            var leftDownY = character.Top + pos - 1;
                            blocked[leftDownBlockedIndexStart + pos - 1] =
                                !ProcessTile(map, leftDownX, leftDownY, width, height);
                        }
                    }
                }

                #region Straight Lines

                if (!blocked[rightLineBlockedIndex])
                {
                    var rX = character.Left + pos;
                    var rY = character.Top;
                    blocked[rightLineBlockedIndex] = !ProcessTile(map, rX, rY, width, height);
                }


                if (!blocked[leftLineBlockedIndex])
                {
                    var lX = character.Left - pos;
                    var lY = character.Top;
                    blocked[leftLineBlockedIndex] = !ProcessTile(map, lX, lY, width, height);
                }

                if (!blocked[downLineBlockedIndex])
                {
                    var dX = character.Left;
                    var dY = character.Top + pos;
                    blocked[downLineBlockedIndex] = !ProcessTile(map, dX, dY, width, height);
                }

                if (!blocked[upLineBlockedIndex])
                {
                    var uX = character.Left;
                    var uY = character.Top - pos;
                    blocked[upLineBlockedIndex] = !ProcessTile(map, uX, uY, width, height);
                }
                #endregion

                #region Diagonal

                if (!blocked[upRightLineBlockedIndex])
                {
                    var urDiagX = character.Left + pos;
                    var urDiagY = character.Top - pos;
                    blocked[upRightLineBlockedIndex] = !ProcessTile(map, urDiagX, urDiagY, width, height);
                }

                if (!blocked[downLeftLineBlockedIndex])
                {
                    var dlDiagX = character.Left - pos;
                    var dlDiagY = character.Top + pos;
                    blocked[downLeftLineBlockedIndex] = !ProcessTile(map, dlDiagX, dlDiagY, width, height);
                }

                if (!blocked[upLeftLineBlockedIndex])
                {
                    var ulDiagX = character.Left - pos;
                    var ulDiagY = character.Top - pos;
                    blocked[upLeftLineBlockedIndex] = !ProcessTile(map, ulDiagX, ulDiagY, width, height);
                }

                if (!blocked[downRightLineBlockedIndex])
                {
                    var drDiagX = character.Left + pos;
                    var drDiagY = character.Top + pos;
                    blocked[downRightLineBlockedIndex] = !ProcessTile(map, drDiagX, drDiagY, width, height);
                }

                #endregion
            }
        }
    }
}