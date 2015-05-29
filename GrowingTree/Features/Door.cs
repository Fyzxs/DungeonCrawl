using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class Door : Feature
    {
        //Yes; rooms and hallways need a common interface
        private Tuple<Room, Feature> connection;


        public Door(Point point) : base(new Boundary(point, 1, 1))
        {
        }

        protected override void DrawImpl(DrawGrid drawGrid)
        {
            drawGrid.CurrentBackground = ConsoleColor.Green;
            drawGrid.CurrentForeground = ConsoleColor.Gray;
            var c = 'D';// Encoding.GetEncoding(437).GetChars(new byte[] { 19 })[0];
            drawGrid.Place(c);
        }

        public static List<Door> GenerateDoors(Level level)
        {
            var rooms = level.GetRooms();
            var availableDoors = new List<Door>();
            foreach (var room in rooms)
            {
                for (var x = 0; x < room.Width; x++)
                {
                    var top = room.Top;
                    var bottom = room.Top + room.Height - 1;
                    var topPoint = new Point(x + room.Left, top - 1);
                    var bottomPoint = new Point(x + room.Left, bottom + 1);
                    var map = level.CopyFeatureGrid();


                    if (top >= 2 && !NullFeature.IsNullFeature(map[x, top - 2]))
                    {
                        var door = new Door(topPoint) {connection = new Tuple<Room, Feature>(room, map[x, top - 1])};
                        if (!availableDoors.Any(d => door.connection.Item1 == d.connection.Item1 &&
                                                     door.connection.Item2 == d.connection.Item2))
                        {
                            availableDoors.Add(door);
                        }
                    }
                          
                    if (top + room.Height - 1 + 2 < level.Height && !NullFeature.IsNullFeature(map[x, top + room.Height - 1 + 2]))
                    {
                        var door = new Door(bottomPoint)
                        {
                            connection = new Tuple<Room, Feature>(room, map[x, top + room.Height - 1 + 1])
                        };
                        if (!availableDoors.Any(d => door.connection.Item1 == d.connection.Item1 &&
                                                     door.connection.Item2 == d.connection.Item2))
                        {
                            availableDoors.Add(door);
                        }
                    }
                }
            }
            return SelectDoors(availableDoors);
        }
        
        static List<Door> SelectDoors(List<Door> potentialDoors)
        {

            //Shuffle required for our elimination process
            potentialDoors.Shuffle();
            var doors = new List<Door>();

            for (var x = potentialDoors.Count - 1; x >= 0; x--)
            {
                var door = potentialDoors[x];
                potentialDoors.RemoveAt(x);
                //Pruning all doors that touch
                 //d.ThisBoundary.Left == door.ThisBoundary.Left + 1 ||
                 //                           d.ThisBoundary.Left == door.ThisBoundary.Left - 1 ||
                 //                           d.ThisBoundary.Top == door.ThisBoundary.Top + 1 ||
                 //                           d.ThisBoundary.Top == door.ThisBoundary.Top - 1 ||
                for (var y = x-1; y >= 0; y--)
                {
                    var d = potentialDoors[y];
                    if(door.connection.Item1 == d.connection.Item1 && 
                         door.connection.Item2 == d.connection.Item2){
                             potentialDoors.Remove(d);
                             x--;
                    }
                }
                doors.Add(door);
            }

            //If we've removed all same connections; we're going to be low density graph
            return potentialDoors;
        }
    }
}
