using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingTree.Features;

namespace GrowingTree
{
    class Utilities
    {
        /*
         * 90% this will need a invalidTypes... which will make it change to
         *      int OpenCount(Point p, Feature[,] map, List<Type> types, bool validTypeCheck)
         * and the use will be
         * if (OutOfBounds(p, -1, -1, width, height) || 
                validTypeCheck == types.Any(x => x == map[p.X - 1, p.Y - 1].GetType()))
         */
        public static int OpenCount(Point p, Feature[,] map, List<Type> validTypes, Direction direction = Direction.Unknown)
        {
            int openCount = 0, width = map.GetLength(0), height = map.GetLength(1);

             //* [ ][        ][      ][        ][ ]
             //* [ ][Left-1, Top-1][Left, Top-1][Left+1, Top-1][ ]
             //* [ ][Left-1, Top  ][Left, Top  ][Left+1, Top  ][ ]
             //* [ ][Left-1, Top+1][Left, Top+1][Left+1, Top+1][ ]
             //* [ ][        ][       ][       ][ ]
             
            #region Direction Selection

            if (p.X < 0 || p.Y < 0 || p.X >= width || p.Y >= height || //Origin is bad
                !NullFeature.IsNullFeature(map[p.X, p.Y])) // already selected
            {
                return -1;
            }

            var xMod = -1;
            var yMod = -1;
            if (IsValid(p, xMod, yMod, map, validTypes))
            {
                openCount++;
            }
            else if (IsFailure(p, xMod, yMod, map, validTypes, direction))
            {
                return -1;
            }

            xMod = 0;
            yMod = -1;
            if (IsValid(p, xMod, yMod, map, validTypes))
            {
                openCount++;
            }
            else if (IsFailure(p, xMod, yMod, map, validTypes, direction))
            {
                return -1;
            }

            xMod = +1;
            yMod = -1;
            if (IsValid(p, xMod, yMod, map, validTypes))
            {
                openCount++;
            }
            else if (IsFailure(p, xMod, yMod, map, validTypes, direction))
            {
                return -1;
            }

            xMod = -1;
            yMod = 0;
            openCount += IsValid(p, xMod, yMod, map, validTypes) ? 1 : 0;

            xMod = +1;
            yMod = 0;
            if (IsValid(p, xMod, yMod, map, validTypes))
            {
                openCount++;
            }
            else if (IsFailure(p, xMod, yMod, map, validTypes, direction))
            {
                return -1;
            }

            xMod = -1;
            yMod = +1;
            if (IsValid(p, xMod, yMod, map, validTypes))
            {
                openCount++;
            }
            else if (IsFailure(p, xMod, yMod, map, validTypes, direction))
            {
                return -1;
            }

            xMod = 0;
            yMod = +1;
            if (IsValid(p, xMod, yMod, map, validTypes))
            {
                openCount++;
            }
            else if (IsFailure(p, xMod, yMod, map, validTypes, direction))
            {
                return -1;
            }

            xMod = +1;
            yMod = +1;
            if (IsValid(p, xMod, yMod, map, validTypes))
            {
                openCount++;
            }
            else if (IsFailure(p, xMod, yMod, map, validTypes, direction))
            {
                return -1;
            }

            #endregion

            return openCount;
        }

        private static bool IsValid(Point p, int xMod, int yMod, Feature[,] map, IEnumerable<Type> validTypes)
        {
            int width = map.GetLength(0), height = map.GetLength(1);
            return OutOfBounds(p, xMod, yMod, width, height) ||
                   validTypes.Any(x => x == map[p.X + xMod, p.Y + yMod].GetType());
        }

        private static bool IsFailure(Point p, int xMod, int yMod, Feature[,] map, 
            IEnumerable<Type> validTypes, Direction direction)
        {
            var fd = new List<Direction>();

            if (xMod == +1){
                fd.Add(Direction.Right);
            }
            if (yMod == -1)
            {
                fd.Add(Direction.Up);
            }
            if (yMod == +1)
            {
                fd.Add(Direction.Down);
            }
            if (xMod == -1)
            {
                fd.Add(Direction.Left);
            }

            return validTypes.All(x => x != map[p.X + xMod, p.Y + yMod].GetType()) &&
                fd.Any(d => d == direction);
        }

        private static bool OutOfBounds(Point point, int xMod, int yMod, int width, int height)
        {
            return
                point.X + xMod < 0 || point.X + xMod >= width ||
                point.Y + yMod < 0 || point.Y + yMod >= height;
        }
    }
}
