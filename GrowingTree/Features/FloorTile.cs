using System;
using System.Net;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class FloorTile : Tile
    {
        public FloorTile(Point point) : base(point)
        {
        }
        
        protected override void DrawImpl(DrawGrid drawGrid)
        {
            var orgColor = drawGrid.CurrentBackground;
            var player = Level.Instance.GetPlayer();
            drawGrid.CurrentBackground = player.HasSeen(this) ? ConsoleColor.DarkGray : orgColor;
            drawGrid.CurrentBackground = player.CanSee(this) ? ConsoleColor.White : drawGrid.CurrentBackground;
            if (Program.SystemState.DebugFlags.DrawAll)
            {
                drawGrid.CurrentBackground = player.CanSee(this) ? ConsoleColor.White : ConsoleColor.DarkYellow;
            }
            drawGrid.Place(AssignedCharacter == '~' ? '#' : AssignedCharacter);
            drawGrid.CurrentBackground = orgColor;
        }
    }
}
