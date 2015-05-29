using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowingTree
{
    enum Direction
    {
        Unknown,
        Up,
        Right,
        Down,
        Left
    }

    internal class Point
    {
        public int X;
        public int Y;

        public Point()
        {
            
        }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class Boundary
    {
        private readonly Point coords;

        public int Left
        {
            get { return coords.X; }
        }
        public int Top
        {
            get { return coords.Y; }
        }

        public int Width;
        public int Height;

        public Boundary(Point point)
        {
            coords = point;
        }
        public Boundary(Point point, int width, int height) : this(point)
        {
            Width = width;
            Height = height;
        }
    }
}
