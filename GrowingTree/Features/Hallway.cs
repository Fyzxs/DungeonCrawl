using System;
using System.Collections.Generic;
using System.Linq;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class Hallway : Feature
    {
        //public void RemoveCooridorPoint(Point point)
        //{
        //    for(var i = cooridorPoints.Count - 1; i >= 0; i--)
        //    {
        //        if (cooridorPoints[i].x != point.x || cooridorPoints[i].y != point.y) continue;

        //        cooridorPoints.RemoveAt(i);
        //        map[point.x, point.y] = Level.TileType.Empty;
        //    }
        //}

        public static Hallway GenerateHallway(Level level)
        {
            var tryPoint = new Point(0, 0);
            while(level.FindOpenPoint(tryPoint, out tryPoint) < 8 && tryPoint != null)
            {
                if (tryPoint.Y < level.Height)
                {
                    tryPoint.Y++;
                }
                else if (tryPoint.X < level.Width)
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
            var buildingCells = new Dictionary<Point, Direction> { {tryPoint , Direction.Down } };
            var selectedCells = new List<FloorTile> { floor };

            var map = level.CopyFeatureGrid();
            map[tryPoint.X, tryPoint.Y] = floor;

            level.InsertFeature(floor);

            #region Select Hallways Cells
            while (true)
            {
                var carvable = GetNextCarvable(buildingCells.Last(), map);
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
                map[newFloor.Left, newFloor.Top] = newFloor;
                level.InsertFeature(newFloor);
            }
            #endregion

            #region Find Boundary
            var minX = map.GetLength(0);
            var minY = map.GetLength(1);
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
  
            var hallway = new Hallway(new Boundary(new Point() {X = minX, Y = minY}, maxX - minX + 1, maxY - minY + 1));

            foreach (var adjCell in selectedCells.Select(cell => new FloorTile(new Point(cell.Left - hallway.Left, cell.Top - hallway.Top))))
            {
                hallway.InsertFeature(adjCell);
            }
            return hallway;
        }

        private static KeyValuePair<Point, Direction> GetNextCarvable(KeyValuePair<Point, Direction> location, Feature[,] map)
        {
            var floor = location.Key;
            var availableCells = new Dictionary<Point, Direction>();
            const int requiredCount = 6, usePreviousDirection = 2;

            #region Available Selection
            var upPoint = new Point() {X = floor.X, Y = floor.Y - 1};
            var downPoint = new Point() { X = floor.X, Y = floor.Y + 1 };
            var leftPoint = new Point() { X = floor.X - 1, Y = floor.Y };
            var rightPoint = new Point() { X = floor.X + 1, Y = floor.Y };

            var validTypes = new List<Type> {typeof (NullFeature)};
            var upCount = Utilities.OpenCount(upPoint, map, validTypes, Direction.Up);
            var downCount = Utilities.OpenCount(downPoint, map, validTypes, Direction.Down);
            var leftCount = Utilities.OpenCount(leftPoint, map, validTypes, Direction.Left);
            var rightCount = Utilities.OpenCount(rightPoint, map, validTypes, Direction.Right);

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

        //public bool Step()
        //{
        //    var nextCell = StepPickCell();

        //    var carvable = GetACarvable(cells[nextCell]);
        //    if (carvable == null)
        //    {
        //        cells.RemoveAt(nextCell);
        //        return cells.Count > 0;
        //    }
        //    if (cells.Any(p => p.x == carvable.x && p.y == carvable.y))
        //    {
        //        return cells.Count > 0;
        //    }

        //    map[carvable.x, carvable.y] = Level.TileType.Floor;
        //    cooridorPoints.Add(carvable);
        //    cells.Add(carvable);
        //    return true;
        //}

        public Hallway(Boundary boundary)
            : base(boundary)
        {
        }

        //private void PrintMap()
        //{
        //    return;
        //    Console.SetCursorPosition(0, 0);
        //    var sb = new StringBuilder();

        //    for (var y = 0; y < height; y++)
        //    {
        //        for (var x = 0; x < width; x++)
        //        {
        //            sb.Append(map[x, y] == Level.TileType.Empty ? " " : "#");
        //        }
        //        sb.AppendLine();
        //    }
        //    Console.Write(sb.ToString());
        //}

        //public Hallway(int width, int height)
        //{
        //    cooridorPoints = new List<Point>();
        //    cells = new List<Point>();
        //    map = new Level.TileType[width, height];
        //    this.width = width;
        //    this.height = height;

        //}

        //private Point GetACarvable(Point point)
        //{
        //    //Find available directions to carve
        //    var availableCells = new List<Point>();

        //    #region Direction Selection

        //    if (point.x + 1 < width && map[point.x + 1, point.y] == Level.TileType.Empty)
        //    {
        //        /*
        //         * [ ][ ][+1 -1][+2 -1][ ]
        //         * [ ][>][  X  ][+2  0][ ]
        //         * [ ][ ][+1 +1][+2 +1][ ]
        //         */
        //        if ((point.x + 2 >= width || point.y - 1 < 0       || map[point.x + 2, point.y - 1] == Level.TileType.Empty) &&
        //            (point.x + 2 >= width ||                          map[point.x + 2, point.y    ] == Level.TileType.Empty) &&
        //            (point.x + 2 >= width || point.y + 1 >= height || map[point.x + 2, point.y + 1] == Level.TileType.Empty) &&
        //            (point.x + 1 >= width || point.y + 1 >= height || map[point.x + 1, point.y + 1] == Level.TileType.Empty) &&
        //            (point.x + 1 >= width || point.y - 1 < 0       || map[point.x + 1, point.y - 1] == Level.TileType.Empty))
        //        {
        //            var p = new Point { x = point.x + 1, y = point.y, from = Direction.Right };
        //            if (!Exists(p))
        //            {
        //                availableCells.Add(p);
        //            }
        //        }
        //    }
        //    if (point.x - 1 > 0 && map[point.x - 1, point.y] == Level.TileType.Empty)
        //    {
        //        /*
        //         * [ ][-2 -1][-1 -1][ ][ ]
        //         * [ ][-2  0][  X  ][<][ ]
        //         * [ ][-2 +1][-1 +1][ ][ ]
        //         */
        //        if ((point.x - 2 < 0 || point.y - 1 < 0       || map[point.x - 2, point.y - 1] == Level.TileType.Empty) &&
        //            (point.x - 2 < 0 ||                          map[point.x - 2, point.y    ] == Level.TileType.Empty) &&
        //            (point.x - 2 < 0 || point.y + 1 >= height || map[point.x - 2, point.y + 1] == Level.TileType.Empty) &&
        //            (point.x - 1 < 0 || point.y + 1 >= height || map[point.x - 1, point.y + 1] == Level.TileType.Empty) &&
        //            (point.x - 1 < 0 || point.y - 1 < 0       || map[point.x - 1, point.y - 1] == Level.TileType.Empty))
        //        {
        //            var p = new Point { x = point.x - 1, y = point.y, from = Direction.Left };
        //            if (!Exists(p))
        //            {
        //                availableCells.Add(p);
        //            }
        //        }
        //    }
        //    if (point.y + 1 <height && map[point.x, point.y + 1] == Level.TileType.Empty)
        //    {
        //        /*
        //         * [ ][-1 +1][  X  ][ 1 +1][ ]
        //         * [ ][-1 +2][ 0 +2][ 1 +2][ ]
        //         * [ ][     ][     ][     ][ ]
        //         */
        //        if ((point.x - 1 < 0 || point.y + 2 >= height || map[point.x - 1, point.y + 2] == Level.TileType.Empty) &&
        //            (point.y + 2 >= height || map[point.x, point.y + 2] == Level.TileType.Empty) &&
        //            (point.x + 1 >= width || point.y + 2 >= height || map[point.x + 1, point.y + 2] == Level.TileType.Empty) &&
        //            (point.x - 1 < 0 || point.y + 1 >= height || map[point.x - 1, point.y + 1] == Level.TileType.Empty) &&
        //            (point.x + 1 >= width || point.y + 1 >= height || map[point.x + 1, point.y + 1] == Level.TileType.Empty))
        //        {
        //            var p = new Point { x = point.x, y = point.y + 1, from = Direction.Up };
        //            if (!Exists(p))
        //            {
        //                availableCells.Add(p);
        //            }
        //        }
        //    }
        //    if (point.y - 1 > 0 && map[point.x, point.y - 1] == Level.TileType.Empty)
        //    {
        //        /*
        //         * [ ][     ][     ][     ][ ]
        //         * [ ][-1 -2][ 0 -2][ 1 -2][ ]
        //         * [ ][-1 -1][  X  ][ 1 -1][ ]
        //         */
        //        if ((point.x - 1 < 0 || point.y - 2 < 0 || map[point.x - 1, point.y - 2] == Level.TileType.Empty) &&
        //            (point.y - 2 < 0 || map[point.x, point.y - 2] == Level.TileType.Empty) &&
        //            (point.x + 1 >= width || point.y - 2 < 0 || map[point.x + 1, point.y - 2] == Level.TileType.Empty) &&
        //            (point.x - 1 < 0 || point.y - 1 < 0 || map[point.x - 1, point.y - 1] == Level.TileType.Empty) &&
        //            (point.x + 1 >= width || point.y - 1 < 0 || map[point.x + 1, point.y - 1] == Level.TileType.Empty))
        //        {
        //            var p = new Point { x = point.x, y = point.y - 1, from = Direction.Down };
        //            if (!Exists(p))
        //            {
        //                availableCells.Add(p);
        //            }
        //        }
        //    }
        //    #endregion

        //    if (availableCells.Count == 0)
        //    {
        //        return null;
        //    }
        //    var index = Rand.Next(availableCells.Count);
        //    var usePrevious = Rand.Next(1, 11) <= 5;
        //    Point lazy = null;
        //    for (var i = 0; i < availableCells.Count; i++)
        //    {
        //        var p = availableCells[i];
        //        if (usePrevious && p.from == point.from)
        //        {
        //            return p;
        //        }
        //        if (index == i)
        //        {
        //            lazy = p;
        //        }
        //    }
        //    if (lazy == null)
        //    {
        //        return null;
        //    }
        //    if (lazy.from != point.from)
        //    {
        //        return lazy;
        //    }
        //    var cell = availableCells.Count == 0 ? lazy : availableCells[index];
        //    return cell;
        //}

        //private bool Exists(Point p)
        //{
        //    return cells.Any(e => p.x == e.x && p.y == e.y);
        //}


        //private int StepPickCell()
        //{
        //    //Pick cell
        //    //Favor early for N
        //    //Favor late for N
        //    //Favor middle for N
        //    //Dislike random

        //    return cells.Count - 1;//Rand.Next(4*cells.Count/5, cells.Count);
        //        //0;//Rand.Next(cells.Count);//cells.Count-1;//Rand.Next(cells.Count/2, cells.Count);
        //}


    }
}
