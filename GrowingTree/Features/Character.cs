using System;

namespace GrowingTree.Features
{
    abstract class Character : Feature
    {
        protected Character(Point point) : base(new Boundary(point, 1, 1))
        {
        }

        public abstract void Move(ConsoleKey key = ConsoleKey.NoName);
    }
}
