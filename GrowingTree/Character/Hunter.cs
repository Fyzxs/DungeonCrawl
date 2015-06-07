using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingTree.Features;
using GrowingTree.Pathing;

namespace GrowingTree.Character
{
    class Hunter : Monster
    {
        private const int RecalcSteps = 10;
        private int moveCtr = 0;
        List<Feature> path = new List<Feature>(); 

        public Hunter(Point startingLocation) : base(startingLocation)
        {
        }


        public override void Move(ConsoleKey key = ConsoleKey.NoName)
        {
            if (path.Count == 0 || moveCtr >= RecalcSteps)
            {
                var player = Level.Instance.GetPlayer();
                var startTile = Level.Instance.FeatureGrid[player.Left, player.Top];
                var goalTile = Level.Instance.FeatureGrid[this.Left, this.Top];
                path = PathFinding.CustomPathFinding(Level.Instance.FeatureGrid,
                    startTile,
                    goalTile);
                if (path.Count == 0)
                {
                    return;
                }
                path.RemoveAt(0); //Removes Monster Tile
                moveCtr = 0;
            }
            
            var tile = path[0];
            path.RemoveAt(0);
            var xPos = tile.Parent == null || tile.Parent is Level ? tile.Left : tile.Parent.Left + tile.Left;
            var yPos = tile.Parent == null || tile.Parent is Level ? tile.Top : tile.Parent.Top + tile.Top;
            ThisBoundary.Coords.X = xPos;
            ThisBoundary.Coords.Y = yPos;

            moveCtr++;

        }
    }
}
