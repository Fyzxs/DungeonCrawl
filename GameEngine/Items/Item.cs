using System;
using GameEngine;
using GameEngine.Display;
using GameEngine.Features;

namespace GameEngine.Items
{
    public class Item : Feature
    {
        public Item(Point point): base(new Boundary(point, 1, 1))
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
