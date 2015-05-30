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
        public Dictionary<Character, bool> HaveSeen = new Dictionary<Character, bool>();
        internal Feature Parent = null;
        public char AssignedCharacter = '~';

        public Feature[,] FeatureGrid
        {
            get
            {
                var start = DateTime.Now.Millisecond;
                var grid = new Feature[Width, Height];
                FillFeatureGrid(grid, Left, Top);
                for (var i = 0; i < grid.GetLength(0); i++)
                {
                    for (var j = 0; j < grid.GetLength(1); j++)
                    {
                        if (grid[i, j] == null)
                        {
                            grid[i, j] = NullFeature.Instance;
                        }
                    }
                }

                var end = DateTime.Now.Millisecond;
                Debug.WriteLine("Took [{0}]ms", (end-start));
                return grid;
            }
        }

        protected virtual void FillFeatureGrid(Feature[,] grid, int leftAdj, int topAdj)
        {
            if (FeatureList.Count == 0)
            {
                grid[leftAdj + Left, topAdj + Top] = this;
                return;
            }
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
            //for (var top = 0; top < Height; top++)
            //{
            //    for (var left = 0; left < Width; left++)
            //    {
            //        FeatureGrid[left, top] = new NullFeature();
            //    }
            //}
        }

        public void InsertFeature(Feature feature)
        {
            if (feature == this) { throw new ArgumentException("Cannot insert into self");}
            feature.Parent = this;

            //if (!feature.FeatureList.Any())
            //{
            //    FeatureGrid[feature.Left, feature.Top] = feature;
            //}
            //else
            //{
            //    foreach (var f in feature.FeatureList)
            //    {
            //        FeatureGrid[feature.Left + f.Left, feature.Top + f.Top] = feature;
            //    }
            //}
            FeatureList.Add(feature);
        }
        public void RemoveFeature(Feature feature)
        {
            if (!FeatureList.Remove(feature)) return;
            
            //if (!feature.FeatureList.Any())
            //{
            //    FeatureGrid[feature.Left, feature.Top] = NullFeature.Instance;
            //    if (Parent != null)
            //    {
            //        Parent.FeatureGrid[feature.Left + Parent.Left, feature.Top + Parent.Top] = NullFeature.Instance;
            //    }
            //}
            //else
            //{
            //    foreach (var f in feature.FeatureList)
            //    {
            //        FeatureGrid[feature.Left + f.Left, feature.Top + f.Top] = NullFeature.Instance;
            //        if (Parent != null)
            //        {
            //            Parent.FeatureGrid[feature.Left + Parent.Left, feature.Top + Parent.Top] = NullFeature.Instance;
            //        }
            //    }
            //}
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
