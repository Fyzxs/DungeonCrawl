using System;
using System.Collections.Generic;
using System.Linq;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class Level : Feature
    {
        public static Level Instance;
        
        public Level(int width, int height) : base(new Boundary(new Point {X=0, Y=0}, width, height))
        {
            Instance = this;
            for (var h = 0; h < Height; h++)
            {
                for (var w = 0; w < Width; w++)
                {
                    FeatureGrid[w, h] = NullFeature.Instance;
                }
            }
        }

        public void GenerateRooms()
        {
            Room room;

            while ((room = Room.GenerateRoom(this)) != null)
            {
                InsertFeature(room);
            }
        }

        internal Feature[,] CopyFeatureGrid()
        {
            var map = new Feature[Width, Height];
            for (var h = 0; h < Height; h++)
            {
                for (var w = 0; w < Width; w++)
                {
                    map[w, h] = FeatureGrid[w, h];
                }
            }
            return map;
        }

        internal IEnumerable<Room> GetRooms()
        {
            return FeatureList.OfType<Room>().ToList();
        }

        /*
        public void GenerateHallways()
        {
            Point open;

            while ((open = FindOpenPoint()) != null)
            {
                var hallway = Hallway.GenerateHallway(this, open);
                if (hallway != null)
                {
                    AddHallway(hallway);
                }
            }
        }

        public void PruneHallways()
        {
            foreach (var hallway in Hallways)
            {
                var removed = false;
                do
                {
                    removed = false;
                    for (var i = hallway.cooridorPoints.Count - 1; i >= 0; i--)
                    {
                        var point = hallway.cooridorPoints[i];
                        if (!IsDeadEnd(point, false)) continue;
                        hallway.RemoveCooridorPoint(point);
                        MapTiles[point.x, point.y] = TileType.Empty;
                        //DrawMap();
                        removed = true;
                    }
                } while (removed);
            }
        }
        public void PruneDoors()
        {
            for(var i = Doors.Count-1; i >= 0; i--)
            {
                var door = Doors[i];
                if (!IsDeadEnd(door.Location, true)) continue;
                Doors.RemoveAt(i);
                MapTiles[door.Location.x, door.Location.y] = TileType.Empty;
                //DrawMap();
            }
        }

        public void PruneRooms()
        {
            //prune doorless

            var touches = new Dictionary<Room, List<Room>>();

            //Build Level of all the rooms this one touches

            //from map select biggest list
            //loop rooms comparing to list - if no touchy; remove.
        }

        public bool IsDeadEnd(Point point, bool checkingDoor)
        {
            if (point == null)
            {
                return false;
            }

            var otherIsLeftOf = point.x - 1 > 0 && (MapTiles[point.x - 1, point.y] != TileType.Empty 
                || (checkingDoor && MapTiles[point.x - 1, point.y] == TileType.Door));
            var otherIsAbove = point.y - 1 > 0 && (MapTiles[point.x, point.y - 1] != TileType.Empty
                || (checkingDoor && MapTiles[point.x, point.y - 1] == TileType.Door));
            var otherIsRightOf = point.x + 1 < Width && (MapTiles[point.x + 1, point.y] != TileType.Empty
                || (checkingDoor && MapTiles[point.x + 1, point.y] == TileType.Door));
            var otherIsBelow = point.y + 1 < Height && (MapTiles[point.x, point.y + 1] != TileType.Empty
                || (checkingDoor && MapTiles[point.x, point.y + 1] == TileType.Door));

            var ctr = 0;
            if (otherIsLeftOf)
            {
                ctr++;
            }
            if (otherIsAbove)
            {
                ctr++;
            }
            if (otherIsRightOf)
            {
                ctr++;
            }
            if (otherIsBelow)
            {
                ctr++;
            }

            return ctr <= 1;
        }

        public void GenerateDoors()
        {
            foreach (var room in Rooms)
            {
                //DrawMap();
                var potentialDoors = new List<Door>();

                //Room Top
                for (var w = room.Source.x; w < room.Source.x + room.Width; w++)
                {
                    if (room.Source.y - 1 < 0 || room.Source.y - 2 < 0 || MapTiles[w, room.Source.y - 2] == TileType.Empty) continue;

                    potentialDoors.Add(new Door(new Point{ x = w, y = room.Source.y - 1 }));
                    //DrawMap(potentialDoors);
                }

                //Room Bottom
                for (var w = room.Source.x; w < room.Source.x + room.Width; w++)
                {
                    if (room.Height + room.Source.y + 1 >= Height ||
                        MapTiles[w, room.Height + room.Source.y + 1] == TileType.Empty) continue;
                    potentialDoors.Add(new Door(new Point{ x = w, y = room.Height + room.Source.y}));

                   // DrawMap(potentialDoors);
                }

                //Room Left
                for (var h = room.Source.y; h < room.Source.y + room.Height; h++)
                {
                    if (room.Source.x - 1 < 0 || room.Source.x - 2 < 0 || MapTiles[room.Source.x - 2, h] == TileType.Empty) continue;
                    potentialDoors.Add(new Door(new Point{ x = room.Source.x - 1, y = h }));


                   // DrawMap(potentialDoors);
                }

                //Room Right
                for (var h = room.Source.y; h < room.Source.y + room.Height; h++)
                {
                    if (room.Width + room.Source.x + 1 >= Width || MapTiles[room.Width + room.Source.x + 1, h] == TileType.Empty) continue;
                    potentialDoors.Add(new Door(new Point{ x = room.Width + room.Source.x, y = h }));
                    //DrawMap(potentialDoors);
                }

                if (potentialDoors.Count == 0)
                {
                    continue;
                }
                foreach (var door in SelectDoors(potentialDoors))
                {
                    //DrawMap();
                    AddDoor(door);
                    room.Doors.Add(door);
                }
            }
        }

        public List<Door> SelectDoors(List<Door> potentialDoors)
        {
            const int maxDoors = 8;
            const int maxPerSide = 1;

            var lastChance = potentialDoors[Rand.Next(potentialDoors.Count)];

            var numberOfDoors = Rand.Next(1, maxDoors+1);//Start at 1 and last is exclusive; so up by 1
            var doors = new List<Door>();

            //Shuffle required for our elimination process
            potentialDoors.Shuffle();

            for (var x = 0; x < numberOfDoors; x++)
            {
                if (potentialDoors.Count == 0)
                {
                    continue; 
                }
                var door = potentialDoors[Rand.Next(potentialDoors.Count)];

                var hCount = 0;
                var vCount = 0;
                var skipDoor = false;
                foreach (var d in doors)
                {
                    //No doors next to each other
                    if (door.Location.x == d.Location.x + 1 ||
                        door.Location.x == d.Location.x - 1 ||
                        door.Location.y == d.Location.y + 1 ||
                        door.Location.y == d.Location.y - 1)
                    {
                        skipDoor = true;
                        break;
                    }
                    if (door.Location.x == d.Location.x) hCount++;
                    if (door.Location.y == d.Location.y) vCount++;
                }
                if (skipDoor || hCount >= maxPerSide || vCount >= maxPerSide)
                {
                    potentialDoors.Remove(door);//Too many on a side
                    continue;
                }

                 /First few doors have low chance; goes up as we eliminate doors
                 
                if (7 < Rand.Next(10)) continue;

                doors.Add(door);
                potentialDoors.Remove(door);
            }

            if (doors.Count == 0)
            {
                doors.Add(lastChance);
            }

            return doors;
        } 
        */
        public int FindOpenPoint(Point startPoint, out Point outPoint)
        {
            var openValidTypes = new List<Type>{typeof (NullFeature)};
            var leftStart = startPoint.X;
            var topStart = startPoint.Y;
            if (topStart >= Height)
            {
                leftStart += 1;
                topStart = 0;
            }
            for (var left = leftStart; left < Width; left++)
            {
                for (var top = topStart; top < Height; top++)
                {
                    if (!NullFeature.IsNullFeature(FeatureGrid[left, top]))
                    {
                        continue;
                    }

                    outPoint = new Point() { X = left, Y = top };


                    var openCount = Utilities.OpenCount(outPoint, FeatureGrid, openValidTypes);
                    
                    return openCount;
                }
            }
            outPoint = null;
            return 0;
        }
    }
}
