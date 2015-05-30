using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class Door : Feature
    {
        //Yes; rooms and hallways need a common interface
        public Tuple<Room, Feature> Connection;


        public Door(Point point) : base(new Boundary(point, 1, 1))
        {
        }

        protected override void DrawImpl(DrawGrid drawGrid)
        {
            drawGrid.CurrentBackground = ConsoleColor.Green;
            drawGrid.CurrentForeground = ConsoleColor.Gray;
            drawGrid.Place('D');
        }

    }
}
