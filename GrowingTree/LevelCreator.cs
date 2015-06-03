using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrowingTree.Display;
using GrowingTree.Features;

namespace GrowingTree
{
    class LevelCreator
    {

        protected static readonly Random Rand = ThreadSafeRandom.ThisThreadsRandom;

        public static Level CreateLevel(int width, int height)
        {
            var level = new Level(width, height);

            GenerateRooms(level);
            GenerateHallways(level);
            GenerateDoors(level);
            
            level.RefreshFeatureGrid();
            MinimizeHallways(level);
            level.RefreshFeatureGrid();
            MinimizeDoors(level);

            return level;
        }
        #region Generate
        #region Rooms
        private static void GenerateRooms(Level level)
        {
            Room room;
            while ((room = GenerateRoom(level)) != null)
            {
                level.InsertFeature(room);
            }
        }
        private static Room GenerateRoom(Level level)
        {
            const int maxRetries = 100;
            const int maxSize = 8;
            const int minSize = 3;

            for (var attempts = 0; attempts < maxRetries; attempts++)
            {
                //Get a size
                var sizeW = Rand.Next(minSize, maxSize);
                var sizeH = Rand.Next(minSize, Math.Min(sizeW * 2, maxSize));

                //Get a location
                var x = Rand.Next(0, level.Width - sizeW);
                var y = Rand.Next(0, (level.Height - sizeH));
                var room = new Room(new Point() { X = x, Y = y }, sizeW, sizeH);
                if (!IsValid(room, level.GetRooms()))
                {
                    continue;
                }

                return room;
            }
            return null;
        }
        private static bool IsValid(Room room, IEnumerable<Room> rooms)
        {
            return rooms.All(room.NoOverlap);
        }
        #endregion
        #region Hallways
        // ReSharper disable once SuggestBaseTypeForParameter
        private static void GenerateHallways(Level level)
        {
            Hallway hall;
            DrawGrid.DebugDraw(level);
            var featureGrid = level.FeatureGrid;
            while ((hall = GenerateHallway(featureGrid)) != null)
            {
                level.InsertFeature(hall);
                DrawGrid.DebugDraw(level);
            }
        }
        private static Hallway GenerateHallway(Feature[,] featureGrid)
        {
            var width = featureGrid.GetLength(0);
            var height = featureGrid.GetLength(1);
            var tryPoint = new Point(0, 0);
            while (FindOpenPoint(tryPoint, out tryPoint, featureGrid) < 8 && tryPoint != null)
            {
                if (tryPoint.Y < height)
                {
                    tryPoint.Y++;
                }
                else if (tryPoint.X < width)
                {
                    tryPoint.Y = 0;
                    tryPoint.X++;
                }
                else
                {
                    tryPoint = null;
                    break;
                }
            }

            if (tryPoint == null)
            {
                return null;
            }

            var floor = new FloorTile(tryPoint);
            var buildingCells = new Dictionary<Point, Direction> { { tryPoint, Direction.Down } };
            var selectedCells = new List<FloorTile> { floor };
            featureGrid[tryPoint.X, tryPoint.Y] = floor;

            #region Select Hallways Cells
            while (true)
            {
                var carvable = GetNextCarvable(buildingCells.Last(), featureGrid);
                if (carvable.Key == null)
                {
                    buildingCells.Remove(buildingCells.Last().Key);
                    if (buildingCells.Count == 0)
                    {
                        break;
                    }
                    continue;
                }
                if (buildingCells.Any(kvp => kvp.Key.X == carvable.Key.X && kvp.Key.Y == carvable.Key.Y))
                {
                    if (buildingCells.Count == 0)
                    {
                        break;
                    }
                    continue;
                }
                buildingCells.Add(carvable.Key, carvable.Value);
                var newFloor = new FloorTile(carvable.Key);
                selectedCells.Add(newFloor);
                featureGrid[newFloor.Left, newFloor.Top] = newFloor;
            }
            #endregion

            #region Find Boundary
            var minX = featureGrid.GetLength(0);
            var minY = featureGrid.GetLength(1);
            var maxX = 0;
            var maxY = 0;

            foreach (var cell in selectedCells)
            {
                var x = cell.Left;
                var y = cell.Top;
                if (x < minX)
                {
                    minX = x;
                }
                if (x > maxX)
                {
                    maxX = x;
                }
                if (y < minY)
                {
                    minY = y;
                }
                if (y > maxY)
                {
                    maxY = y;
                }
            }
            #endregion

            var hallway = new Hallway(new Boundary(new Point() { X = minX, Y = minY }, maxX - minX + 1, maxY - minY + 1));

            foreach (var adjCell in selectedCells.Select(cell => new FloorTile(new Point(cell.Left - hallway.Left, cell.Top - hallway.Top))))
            {
                hallway.InsertFeature(adjCell);
            }
            return hallway;
        }
        private static KeyValuePair<Point, Direction> GetNextCarvable(KeyValuePair<Point, Direction> location, Feature[,] featureGrid)
        {
            var floor = location.Key;
            var availableCells = new Dictionary<Point, Direction>();
            const int requiredCount = 6, usePreviousDirection = 2;

            #region Available Selection
            var upPoint = new Point() { X = floor.X, Y = floor.Y - 1 };
            var downPoint = new Point() { X = floor.X, Y = floor.Y + 1 };
            var leftPoint = new Point() { X = floor.X - 1, Y = floor.Y };
            var rightPoint = new Point() { X = floor.X + 1, Y = floor.Y };

            var validTypes = new List<Type> { typeof(NullFeature) };
            var upCount = Utilities.OpenCount(upPoint, featureGrid, validTypes, Direction.Up);
            var downCount = Utilities.OpenCount(downPoint, featureGrid, validTypes, Direction.Down);
            var leftCount = Utilities.OpenCount(leftPoint, featureGrid, validTypes, Direction.Left);
            var rightCount = Utilities.OpenCount(rightPoint, featureGrid, validTypes, Direction.Right);

            if (upCount >= requiredCount)
            {
                availableCells.Add(upPoint, Direction.Up);
            }
            if (downCount >= requiredCount)
            {
                availableCells.Add(downPoint, Direction.Down);
            }
            if (leftCount >= requiredCount)
            {
                availableCells.Add(leftPoint, Direction.Left);
            }
            if (rightCount >= requiredCount)
            {
                availableCells.Add(rightPoint, Direction.Right);
            }
            #endregion

            if (availableCells.Count == 0)
            {
                return new KeyValuePair<Point, Direction>(null, Direction.Unknown);
            }
            var index = Rand.Next(availableCells.Count);
            var chance = Rand.Next(1, 11);
            var usePrevious = chance <= usePreviousDirection;
            var lazy = new KeyValuePair<Point, Direction>(null, Direction.Unknown);
            for (var i = 0; i < availableCells.Count; i++)
            {
                var kvp = availableCells.ElementAt(i);
                var d = kvp.Value;
                if (usePrevious && d == location.Value)
                {
                    return kvp;
                }
                if (index == i)
                {
                    lazy = kvp;
                }
            }
            return lazy;
        }

        public static int FindOpenPoint(Point startPoint, out Point outPoint, Feature[,] featureGrid)
        {
            var openValidTypes = new List<Type> { typeof(NullFeature) };
            var leftStart = startPoint.X;
            var topStart = startPoint.Y;
            var width = featureGrid.GetLength(0);
            var height = featureGrid.GetLength(1);
            if (topStart >= height)
            {
                leftStart += 1;
                topStart = 0;
            }
            for (var left = leftStart; left < width; left++)
            {
                for (var top = topStart; top < height; top++)
                {
                    if (!NullFeature.IsNullFeature(featureGrid[left, top]))
                    {
                        continue;
                    }

                    outPoint = new Point() { X = left, Y = top };


                    var openCount = Utilities.OpenCount(outPoint, featureGrid, openValidTypes);

                    return openCount;
                }
            }
            outPoint = null;
            return 0;
        }
        #endregion
        #region Doors
        private static void GenerateDoors(Level level)
        {
            var doors = SelectDoors(level);
            foreach (var door in doors)
            {
                level.InsertFeature(door);
            }
        }
        private static IEnumerable<Door> SelectDoors(Level level)
        {
            var rooms = level.GetRooms();
            var availableDoors = new List<Door>();
            var map = level.FeatureGrid;


            /*
             * This currently favors certain door positions.
             * To update
             * ALL matching room/feature connections needs to be bucketed - from there; randomly selected which one to use
             */
            foreach (var room in rooms)
            {
                for (var x = room.Left; x < room.Left + room.Width; x++)
                {
                    var top = room.Top;
                    var bottom = room.Top + room.Height - 1;
                    var topPoint = new Point(x, top - 1);
                    var bottomPoint = new Point(x, bottom + 1);
                    if (top >= 2 && !NullFeature.IsNullFeature(map[x, top - 2]))
                    {
                        var tile = map[x, top - 2];
                        while (tile is FloorTile)
                        {
                            tile = tile.Parent;
                        }
                        var door = new Door(topPoint) { Connection = new Tuple<Room, Feature>(room, tile) };
                        if (!availableDoors.Any(d => door.Connection.Item1 == d.Connection.Item1 && door.Connection.Item2 == d.Connection.Item2))
                        {
                            availableDoors.Add(door);
                        }
                    }

                    if (bottom + 2 < level.Height && !NullFeature.IsNullFeature(map[x, bottom + 2]))
                    {
                        var tile = map[x, bottom + 2];
                        while (tile is FloorTile)
                        {
                            tile = tile.Parent;
                        }
                        var door = new Door(bottomPoint) { Connection = new Tuple<Room, Feature>(room, tile) };
                        if (!availableDoors.Any(d => door.Connection.Item1 == d.Connection.Item1 && door.Connection.Item2 == d.Connection.Item2))
                        {
                            availableDoors.Add(door);
                        }
                    }
                }


                for (var y = room.Top; y < room.Top + room.Height; y++)
                {
                    var left = room.Left;
                    var right = room.Left + room.Width - 1;
                    var leftPoint = new Point(left - 1, y);
                    var rightPoint = new Point(right + 1, y);

                    /*
                     * This currently favors the left side of the room; that'll need to be updated later.
                     * Doors are generated right now - fine tuning is for later
                     */
                    if (left >= 2 && !NullFeature.IsNullFeature(map[left - 2, y]))
                    {
                        var tile = map[left - 2, y];
                        while (tile is FloorTile)
                        {
                            tile = tile.Parent;
                        }
                        var door = new Door(leftPoint) { Connection = new Tuple<Room, Feature>(room, tile) };
                        if (!availableDoors.Any(d => door.Connection.Item1 == d.Connection.Item1 && door.Connection.Item2 == d.Connection.Item2))
                        {
                            availableDoors.Add(door);
                        }
                    }

                    if (right + 2 < level.Width && !NullFeature.IsNullFeature(map[right + 2, y]))
                    {
                        var tile = map[right + 2, y];
                        while (tile is FloorTile)
                        {
                            tile = tile.Parent;
                        }
                        var door = new Door(rightPoint) { Connection = new Tuple<Room, Feature>(room, tile) };
                        if (!availableDoors.Any(d => door.Connection.Item1 == d.Connection.Item1 && door.Connection.Item2 == d.Connection.Item2))
                        {
                            availableDoors.Add(door);
                        }
                    }
                }

            }
            return availableDoors;
        }
        #endregion
        #endregion

        #region Minimize

        private static void MinimizeHallways(Level level)
        {
            const char leftT = (char)185;       // ╣
            const char vertical = (char)186;    // ║
            const char upperRight = (char)187;  // ╗
            const char lowerRight = (char)188;  // ╝
            const char lowerLeft = (char)200;   // ╚
            const char upperLeft = (char)201;   // ╔
            const char bottomT = (char)202;     // ╩
            const char upperT = (char)203;      // ╦
            const char rightT = (char)204;      // ╠
            const char horizontal = (char)205;  // ═
            const char allways = (char)206;     // ╬

            var map = level.FeatureGrid;
            foreach (var hallway in level.GetHallways())
            {
                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
                while (ProcessHallyway(level, map, hallway))
                {
                    //Twiddle... Twiddle
                }
                foreach (var feature in hallway.FeatureList)
                {
                    var x = hallway.Left + feature.Left;
                    var y = hallway.Top + feature.Top;
                    var leftIsNull = x <= 0 || NullFeature.IsNullFeature(map[x - 1, y]);
                    var rightIsNull = x >= level.Width - 1 ||NullFeature.IsNullFeature(map[x + 1, y]);
                    var upIsNull = y <= 0 || NullFeature.IsNullFeature(map[x, y - 1]);
                    var downIsNull = y >= level.Height - 1 || NullFeature.IsNullFeature(map[x, y + 1]);

                    if (!upIsNull && !downIsNull && !leftIsNull && rightIsNull)
                    {
                        feature.AssignedCharacter = leftT;
                    }
                    else if (!upIsNull && !downIsNull && leftIsNull && !rightIsNull)
                    {
                        feature.AssignedCharacter = rightT;
                    }
                    else if (upIsNull && downIsNull && !leftIsNull && !rightIsNull)
                    {
                        feature.AssignedCharacter = horizontal;
                    }
                    else if (upIsNull && downIsNull && leftIsNull && rightIsNull)
                    {
                        feature.AssignedCharacter = allways;
                    }
                    else if (!upIsNull && !downIsNull && leftIsNull && rightIsNull)
                    {
                        feature.AssignedCharacter = vertical;
                    }
                    else if (upIsNull && !downIsNull && !leftIsNull && rightIsNull)
                    {
                        feature.AssignedCharacter = upperRight;
                    }
                    else if (!upIsNull && downIsNull && !leftIsNull && rightIsNull)
                    {
                        feature.AssignedCharacter = lowerRight;
                    }
                    else if (!upIsNull && downIsNull && leftIsNull && !rightIsNull)
                    {
                        feature.AssignedCharacter = lowerLeft;
                    }
                    else if (upIsNull && !downIsNull && leftIsNull && !rightIsNull)
                    {
                        feature.AssignedCharacter = upperLeft;
                    }
                    else if (!upIsNull && downIsNull && !leftIsNull && !rightIsNull)
                    {
                        feature.AssignedCharacter = bottomT;
                    }
                    else if (upIsNull && !downIsNull && !leftIsNull && !rightIsNull)
                    {
                        feature.AssignedCharacter = upperT;
                    }

                }
            }
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static bool ProcessHallyway(Level level, Feature[,] map, Hallway hallway)
        {
            var pruned = false;
            for (var i = hallway.FeatureList.Count - 1; i >= 0; i--)
            {
                var feature = hallway.FeatureList[i];
                var posX = hallway.Left + feature.Left;
                var posY = hallway.Top + feature.Top;
                var rightPosX = posX + 1;
                var rightPosY = posY;
                var openCount = 0;
                openCount += rightPosX >= level.Width || NullFeature.IsNullFeature(map[rightPosX, rightPosY]) ? 1 : 0;

                var leftPosX = posX - 1;
                var leftPosY = posY;
                openCount += leftPosX < 0 || NullFeature.IsNullFeature(map[leftPosX, leftPosY]) ? 1 : 0;

                var topPosX = posX;
                var topPosY = posY - 1;
                openCount += topPosY < 0 || NullFeature.IsNullFeature(map[topPosX, topPosY]) ? 1 : 0;

                var bottomPosX = posX;
                var bottomPosY = posY + 1;
                openCount += bottomPosY >= level.Height || NullFeature.IsNullFeature(map[bottomPosX, bottomPosY]) ? 1 : 0;

                if (openCount < 3)
                {
                    continue;
                }
                pruned = true;
                hallway.RemoveFeature(feature);
                map[posX, posY] = NullFeature.Instance;
                i++;
                if (i >= hallway.FeatureList.Count)
                {
                    i = hallway.FeatureList.Count - 1; //Reset to start at the top
                }
            }
            return pruned;
        }


        private static void MinimizeDoors(Level level)
        {
            var map = level.FeatureGrid;
            var doors = level.GetDoors();
            var doorIter = doors.GetEnumerator();
            while (doorIter.MoveNext())
            {
                var door = doorIter.Current;

                var posX = door.Left;
                var posY = door.Top;
                var rightPosX = posX + 1;
                var rightPosY = posY;
                var openCount = 0;
                openCount += rightPosX >= level.Width || NullFeature.IsNullFeature(map[rightPosX, rightPosY]) ? 1 : 0;

                var leftPosX = posX - 1;
                var leftPosY = posY;
                openCount += leftPosX < 0 || NullFeature.IsNullFeature(map[leftPosX, leftPosY]) ? 1 : 0;

                var topPosX = posX;
                var topPosY = posY - 1;
                openCount += topPosY < 0 || NullFeature.IsNullFeature(map[topPosX, topPosY]) ? 1 : 0;

                var bottomPosX = posX;
                var bottomPosY = posY + 1;
                openCount += bottomPosY >= level.Height || NullFeature.IsNullFeature(map[bottomPosX, bottomPosY]) ? 1 : 0;

                if (openCount < 3)
                {
                    continue;
                }
                level.RemoveFeature(door);
                map[posX, posY] = NullFeature.Instance;
                
            }
        }
        #endregion
    }
}
