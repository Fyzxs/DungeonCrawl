using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Features.Null
{
    public interface INullFeature<T> where T: Feature
    {
        bool IsNullFeature(Feature feature);
    }
}
