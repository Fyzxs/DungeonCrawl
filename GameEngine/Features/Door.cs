using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using GameEngine.Display;

namespace GameEngine.Features
{
    public class Door : Feature
    {
        //Yes; rooms and hallways need a common interface
        public Tuple<Room, Feature> Connection;


        public Door(Point point) : base(new Boundary(point, 1, 1))
        {
        }

        public override bool IsPassable()
        {
            return true;
        }

        public override bool IsVisionBlocking()
        {
            return false;
        }

    }
}
