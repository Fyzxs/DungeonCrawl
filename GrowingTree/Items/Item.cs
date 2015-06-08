using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingTree.Display;
using GrowingTree.Features;

namespace GrowingTree.Items
{
    class Item : Feature
    {
        public Item(Point point): base(new Boundary(point, 1, 1))
        {
        }

        public override bool IsPassable()
        {
            return true;
        }

        public override bool IsVisionBlocking()
        {
            return false;
        }

        protected override void DrawImpl(DrawGrid drawGrid)
        {
            var grid = Level.Instance.FeatureGrid;
            var player = Level.Instance.GetPlayer();
            if (!player.CanSee(grid[Left, Top]) && !Program.SystemState.DebugFlags.DrawAll)
            {
                //drawGrid.CurrentForeground = ConsoleColor.Green;
                //drawGrid.CurrentBackground = ConsoleColor.DarkRed;
                //drawGrid.Place('M');
                return;
            }

            drawGrid.CurrentForeground = ConsoleColor.Magenta;
            drawGrid.CurrentBackground = ConsoleColor.DarkMagenta;
            drawGrid.Place('I');
        }
    }
}
