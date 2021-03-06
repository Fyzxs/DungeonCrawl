﻿using System;
using System.Linq;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class NullFeature : Feature
    {
        public static readonly NullFeature Instance = new NullFeature();

        public static bool IsNullFeature(Feature feature)
        {
            return feature is NullFeature;
        }
        private NullFeature() : base(new Boundary(new Point{X=0, Y=0}))
        {
        }
        protected override void DrawImpl(DrawGrid drawGrid)
        {
        }
    }
}
