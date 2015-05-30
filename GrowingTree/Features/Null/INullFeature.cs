using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowingTree.Features.Null
{
    interface INullFeature<T> where T: Feature
    {
        bool IsNullFeature(Feature feature);
    }
}
