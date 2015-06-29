using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public static class SystemState
    {
        public static bool ShouldQuit = false;

        public static class DebugFlags
        {
            //Shows the entire map; not just visible sections
            public static bool DrawAll = false;
            //Allows regeneration of maps without quitting
            public static bool Regen = true;
            //If the debug drawing should happen
            public static bool Draw = false;
            //Toggle the Heuristics
            public static bool SearchHeuristics = false;
        }
    }
}
