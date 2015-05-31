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
            var orgColor = drawGrid.CurrentBackground;
            var player = Level.Instance.GetPlayer();
            drawGrid.CurrentBackground = player.HasSeen(this) ? ConsoleColor.DarkGray : orgColor;
            drawGrid.CurrentBackground = player.CanSee(this) ? ConsoleColor.White : drawGrid.CurrentBackground; 
            if (Program.SystemState.DebugFlags.DrawAll)
            {
                drawGrid.CurrentBackground = ConsoleColor.DarkYellow;
                drawGrid.CurrentBackground = ConsoleColor.DarkGray;
            }
            drawGrid.Place('D');
            drawGrid.CurrentBackground = orgColor;
        }

    }
}
