using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    abstract class Feature : IComparable<Feature>
    {
        /*
         * All Features Random from here
         */
        protected static readonly Random Rand = ThreadSafeRandom.ThisThreadsRandom;

        internal List<Feature> FeatureList = new List<Feature>();
        //protected readonly Feature[,] FeatureGrid;
        protected Boundary ThisBoundary;
        internal Feature Parent = null;
        public char AssignedCharacter = '~';

        public virtual bool IsPassable()
        {
            return false;
        }

        public virtual bool IsVisionBlocking()
        {
            return true;
        }

        protected virtual void FillFeatureGrid(Feature[,] grid, int leftAdj, int topAdj)
        {
            grid[leftAdj + Left, topAdj + Top] = this;
            foreach (var feature in FeatureList)
            {
                feature.FillFeatureGrid(grid, leftAdj + Left, topAdj + Top);
            }
        }
        
        public int Left
        {
            get { return ThisBoundary.Left; }
        }
        
        public int Top
        {
            get { return ThisBoundary.Top; }
        }

        public int Width
        {
            get { return ThisBoundary.Width; }
        }

        public int Height
        {
            get { return ThisBoundary.Height; }
        }

        protected virtual void DrawImpl(DrawGrid drawGrid)
        {
            foreach (var feature in FeatureList)
            {
                feature.Draw(drawGrid);
            }
        }
        public void Draw(DrawGrid drawGrid)
        {
            drawGrid.AdjustLeft(ThisBoundary.Left);
            drawGrid.AdjustDown(ThisBoundary.Top);
            var f = drawGrid.CurrentForeground;
            var b = drawGrid.CurrentBackground;
            DrawImpl(drawGrid);
            drawGrid.CurrentForeground = f;
            drawGrid.CurrentBackground = b;
            drawGrid.AdjustRight(ThisBoundary.Left);
            drawGrid.AdjustUp(ThisBoundary.Top);
        }

        protected Feature(Boundary boundary)
        {
            ThisBoundary = boundary;
        }

        public void InsertFeature(Feature feature)
        {
            if (feature == this) { throw new ArgumentException("Cannot insert into self");}
            feature.Parent = this;

            FeatureList.Add(feature);
        }
        public void RemoveFeature(Feature feature)
        {
            FeatureList.Remove(feature);
        }

        protected List<T> GetFeaturesOfType<T>() where T : Feature
        {
            /*
             * Pretty sure this will also need to search base classes
             * of items in the FeatureList
             * AKA: Looking for Tile needs to find FloorTile
             */
            return FeatureList.OfType<T>().ToList();
        }

        public int CompareTo(Feature other)
        {
            if (other == null)
            {
                return 1;
            }

            if (Top < other.Top)
            {
                return 1;
            }

            if (Top > other.Top)
            {
                return -1;
            }
            //Top Must be ==
            if (Left < other.Left)
            {
                return 1;
            }

            if (Left > other.Left)
            {
                return -1;
            }

            //Left must be ==
            return 0;
        }
    }
}
