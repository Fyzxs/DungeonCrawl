using System;
using System.Collections.Generic;
using System.Linq;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    internal class Room : Feature
    {
        private const int MaxRetries = 100;
        private const int MaxSize = 10;
        private const int MinSize = 4;

        public static Room GenerateRoom(Level level)
        {
            for (var attempts = 0; attempts < MaxRetries; attempts++)
            {
                //Get a size
                var sizeW = Rand.Next(MinSize, MaxSize);
                var sizeH = Rand.Next(MinSize, Math.Min(sizeW*2, MaxSize));

                //Get a location
                var x = Rand.Next(0, level.Width - sizeW);
                var y = Rand.Next(0, (level.Height - sizeH));
                var room = new Room(new Point() {X = x, Y = y}, sizeW, sizeH);
                if (!IsValid(room, level.GetRooms()))
                {
                    continue;
                }

                return room;
            }
            return null;
        }

        private static bool IsValid(Room room, IEnumerable<Room> rooms)
        {
            return rooms.All(room.NoOverlap);
        }

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

        protected override void DrawImpl(DrawGrid drawGrid)
        {
            drawGrid.CurrentBackground = ConsoleColor.Gray;
            drawGrid.CurrentForeground = ConsoleColor.DarkGray;
            base.DrawImpl(drawGrid);
        }
    }

}