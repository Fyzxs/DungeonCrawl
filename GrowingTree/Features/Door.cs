using System;
using System.Collections.Generic;
using System.Text;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class Door : Feature
    {
        //Yes; rooms and hallways need a common interface
        public static readonly List<Room> Rooms = new List<Room>();
        public static readonly List<Hallway> Hallways = new List<Hallway>();


        public Door(Point point) : base(new Boundary(point, 1, 1))
        {
        }

        protected override void DrawImpl(DrawGrid drawGrid)
        {
            drawGrid.CurrentBackground = ConsoleColor.DarkGray;
            drawGrid.CurrentForeground = ConsoleColor.Gray;
            var c = Encoding.GetEncoding(437).GetChars(new byte[] { 19 })[0];
            drawGrid.Place(c);
        }
    }
}
