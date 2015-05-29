using System;
using System.Collections.Generic;
using System.Linq;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    abstract class Feature
    {
        /*
         * All Things Random from here
         */
        protected static readonly Random Rand = ThreadSafeRandom.ThisThreadsRandom;

        protected List<Feature> FeatureList = new List<Feature>();
        protected readonly Feature[,] FeatureGrid;
        protected Boundary ThisBoundary;
        public Dictionary<Character, bool> HaveSeen = new Dictionary<Character, bool>();

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
            FeatureGrid = new Feature[boundary.Width, boundary.Height];
            for (var top = 0; top < Height; top++)
            {
                for (var left = 0; left < Width; left++)
                {
                    FeatureGrid[left, top] = new NullFeature();
                }
            }
        }

        public void InsertFeature(Feature feature)
        {
            if (feature.FeatureList.Any())
            {
                foreach (var f in feature.FeatureList)
                {
                    FeatureGrid[feature.Left + f.Left, feature.Top + f.Top] = feature;
                }
            }
            else
            {
                /*
                 * Current theory is that the only things without sub features
                 * is a single tile sized object.
                 * If this becomes false later... Re-enabled the double loop below
                 */
                FeatureGrid[feature.Left, feature.Top] = feature;
                 
                //for (var top = feature.ThisBoundary.Top;
                //    top < feature.ThisBoundary.Top + feature.ThisBoundary.Height;
                //    top++)
                //{
                //    for (var left = feature.ThisBoundary.Left;
                //        left < feature.ThisBoundary.Left + feature.ThisBoundary.Width;
                //        left++)
                //    {
                //        FeatureGrid[left, top] = feature;
                //    }
                //}
            }
            FeatureList.Add(feature);
        }
    }
}
