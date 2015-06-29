using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    enum Direction
    {
        Unknown,
        Up,
        Right,
        Down,
        Left
    }

    public class Point
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

    public class Boundary
    {
        internal readonly Point Coords;

        public int Left
        {
            get { return Coords.X; }
        }
        public int Top
        {
            get { return Coords.Y; }
        }

        public int Width;
        public int Height;

        public Boundary(Point point)
        {
            Coords = point;
        }
        public Boundary(Point point, int width, int height) : this(point)
        {
            Width = width;
            Height = height;
        }
    }
}
