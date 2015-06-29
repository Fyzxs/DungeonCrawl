using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Display;

namespace GameEngine.Features
{
    public class Room : Feature
    {
        public Room(Point source, int width, int height) : base(new Boundary(source, width, height))
        {
            for (var top = 0; top < height; top++)
            {
                for (var left = 0; left < width; left++)
                {
                    InsertFeature(new FloorTile(new Point(left, top)));
                }
            }
        }

        public bool NoOverlap(Room other)
        {
            if (other == null)
            {
                return false;
            }

            var otherIsLeftOf = other.ThisBoundary.Left + other.Width <ThisBoundary.Left;
            var otherIsAbove  = other.ThisBoundary.Top + other.Height < ThisBoundary.Top;
            var otherIsRightOf = other.ThisBoundary.Left > ThisBoundary.Left + Width;
            var otherIsBelow = other.ThisBoundary.Top > ThisBoundary.Top + Height;

            //[  ][  ][  ][ ][ ]
            //[ o][ o][-1][Left][ ]
            //[ o][ o][-1][Left][ ]
            //[-1][-1][  ][ ][ ]
            //[ Top][ Top][  ][ ][ ]

            //[  ][  ][  ][ Top][ Top]
            //[  ][  ][  ][ Top][ Top]
            //[  ][  ][  ][+1][+1]
            //[ Left][ Left][+1][ o][ o]
            //[ Left][ Left][+1][ o][ o]       

            return otherIsLeftOf || otherIsAbove || otherIsRightOf || otherIsBelow;
        }
    }

}