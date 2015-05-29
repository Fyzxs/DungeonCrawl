using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowingTree
{
 /*
 * Growing tree algorithm: This is a general algorithm, capable of creating
 * Mazes of different textures. It requires storage up to the size of the Maze.
 * Each time you carve a cell, add that cell to a list. Proceed by picking a 
 * cell from the list, and carving into an unmade cell next to it. If there are
 * no unmade cells next to the current cell, remove the current cell from the 
 * list. The Maze is done when the list becomes empty. The interesting part
 * that allows many possible textures is how you pick a cell from the list. 
 * For example, if you always pick the most recent cell added to it, this 
 * algorithm turns into the recursive backtracker. If you always pick cells at 
 * random, this will behave similarly but not exactly to Prim's algorithm. If 
 * you always pick the oldest cells added to the list, this will create Mazes 
 * with about as low a "river" factor as possible, even lower than Prim's 
 * algorithm. If you usually pick the most recent cell, but occasionally pick a
 * random cell, the Maze will have a high "river" factor but a short direct 
 * solution. If you randomly pick among the most recent cells, the Maze will 
 * have a low "river" factor but a long windy solution.
 */

    internal class GrowingTree
    {
        private static readonly Random Rand = new Random();


        private readonly char[,] map;
        private readonly List<Point> cells;
        private readonly int width;
        private int height;

        public void Draw()
        {
            var sb = new StringBuilder();
            
            for (var row = 0; row < width; row++)
            {
                for (var col = 0; col < height; col++)
                {
                    sb.Append(map[row, col] == (char)0 ? " " : Program.FloorAscii.ToString());
                }
                sb.AppendLine();
            }
            Console.SetCursorPosition(0,0);
            Console.Write(sb.ToString());
        }

        private bool Exists(Point p)
        {
            return cells.Any(e => p.x == e.x && p.y == e.y);
        }

        public GrowingTree(int width, int height)
            : this(width, height, null, null)
        {
        }
        public GrowingTree(int width, int height, char[,] map, Point start)
        {
            Debug.Assert(width > 0);
            Debug.Assert(height > 0);
            this.map = map ?? new char[width, height];
            this.width = width;
            this.height = height;
            cells = new List<Point>();
            if (start == null)
            {
                cells.Add(
                    new Point()
                    {
                        x = Rand.Next(width - 2),
                        y = Rand.Next(height - 2)
                    });
            }
            else
            {
                cells.Add(start);
            }
            this.map[cells[0].x, cells[0].y] = Program.FloorAscii;

        }

        /// <summary>
        /// Perform a step of carving out the maze
        /// </summary>
        /// <returns>[FALSE] if there maze is complete</returns>
        public bool Step()
        {
            var nextCell = StepPickCell();

            var carvable = GetACarvable(cells[nextCell]);
            if (carvable == null)
            {
                cells.RemoveAt(nextCell);
                return cells.Count > 0;
            }
            map[carvable.x, carvable.y] = Program.FloorAscii;
            cells.Add(carvable);
            return true;
        }

        private Point GetACarvable(Point point)
        {
            //Find available directions to carve
            var availableCells = new List<Point>();

            #region Direction Selection
            if (point.x + 1 < width && map[point.x + 1, point.y] == (char)0)
            {
                /*
                 * [ ][ ][+1 -1][+2 -1][ ]
                 * [ ][ ][  X  ][+2  0][ ]
                 * [ ][ ][+1 +1][+2 +1][ ]
                 */
                if ((point.x + 2 >= width || point.y - 1 < 0       || map[point.x + 2, point.y - 1] == (char)0) && 
                    (point.x + 2 >= width ||                          map[point.x + 2, point.y]     == (char)0) &&
                    (point.x + 2 >= width || point.y + 1 >= height || map[point.x + 2, point.y + 1] == (char)0) &&
                    (point.x + 1 >= width || point.y + 1 >= height || map[point.x + 1, point.y + 1] == (char)0) && 
                    (point.x + 1 >= width || point.y - 1 < 0       || map[point.x + 1, point.y - 1] == (char)0))
                {
                    var p = new Point { x = point.x + 1, y = point.y, from = Direction.East };
                    if (!Exists(p))
                    {
                        availableCells.Add(p);
                    }
                }
            }
            if (point.x - 1 > 0 && map[point.x - 1, point.y] == (char)0)
            {
                /*
                 * [ ][-2 -1][-1 -1][ ][ ]
                 * [ ][-2  0][  X  ][ ][ ]
                 * [ ][-2 +1][-1 +1][ ][ ]
                 */
                if ((point.x - 2 < 0 || point.y - 1 < 0       || map[point.x - 2, point.y - 1] == (char)0) &&
                    (point.x - 2 < 0 ||                          map[point.x - 2, point.y]     == (char)0) &&
                    (point.x - 2 < 0 || point.y + 1 >= height || map[point.x - 2, point.y + 1] == (char)0) &&
                    (point.x - 1 < 0 || point.y + 1 >= height || map[point.x - 1, point.y + 1] == (char)0) &&
                    (point.x - 1 < 0 || point.y - 1 < 0       || map[point.x - 1, point.y - 1] == (char)0))
                {
                    var p = new Point { x = point.x - 1, y = point.y, from = Direction.West };
                    if (!Exists(p))
                    {
                        availableCells.Add(p);
                    }
                }
            }
            if (point.y + 1 < height && map[point.x, point.y + 1] == (char)0)
            {
                /*
                 * [ ][-1 +1][  X  ][ 1 +1][ ]
                 * [ ][-1 +2][ 0 +2][ 1 +2][ ]
                 * [ ][     ][     ][     ][ ]
                 */
                if ((point.x - 1 < 0       || point.y + 2 >= height || map[point.x - 1, point.y + 2] == (char)0) &&
                    (                         point.y + 2 >= height || map[point.x,     point.y + 2] == (char)0) &&
                    (point.x + 1 >= width  || point.y + 2 >= height || map[point.x + 1, point.y + 2] == (char)0) &&
                    (point.x - 1 < 0       || point.y + 1 >= height || map[point.x - 1, point.y + 1] == (char)0) &&
                    (point.x + 1 >= width  || point.y + 1 >= height || map[point.x + 1, point.y + 1] == (char)0))
                {
                    var p = new Point { x = point.x, y = point.y + 1, from = Direction.North };
                    if (!Exists(p))
                    {
                        availableCells.Add(p);
                    }
                }
            }
            if (point.y - 1 > 0 && map[point.x, point.y - 1] == (char)0)
            {
                /*
                 * [ ][     ][     ][     ][ ]
                 * [ ][-1 -2][ 0 -2][ 1 -2][ ]
                 * [ ][-1 -1][  X  ][ 1 -1][ ]
                 */
                if ((point.x - 1 < 0      || point.y - 2 < 0 || map[point.x - 1, point.y - 2] == (char)0) && 
                    (                        point.y - 2 < 0 || map[point.x,     point.y - 2] == (char)0) &&
                    (point.x + 1 >= width || point.y - 2 < 0 || map[point.x + 1, point.y - 2] == (char)0) &&
                    (point.x - 1 < 0      || point.y - 1 < 0 || map[point.x - 1, point.y - 1] == (char)0) &&
                    (point.x + 1 >= width || point.y - 1 < 0 || map[point.x + 1, point.y - 1] == (char)0))
                {
                    var p = new Point { x = point.x, y = point.y - 1, from = Direction.South };
                    if (!Exists(p))
                    {
                        availableCells.Add(p);
                    }
                }
            }
            #endregion

            if (availableCells.Count == 0)
            {
                return null;
            }
            var index = Rand.Next(availableCells.Count);
            var usePrevious = Rand.Next(1, 11) <= 5;
            Point lazy = null;
            for (var i = 0; i < availableCells.Count; i++)
            {
                var p = availableCells[i];
                if (usePrevious && p.from == point.from)
                {
                    return p;
                }
                if (index == i)
                {
                    lazy = p;
                }
            }
            if (lazy == null)
            {
                return null;
            }
            if (lazy.from != point.from)
            {
                return lazy;
            }
            availableCells.RemoveAt(index);
            return availableCells.Count == 0 ? lazy : availableCells[Rand.Next(availableCells.Count)];
        }

        private int StepPickCell()
        {
            //Pick cell
            //Favor early for N
            //Favor late for N
            //Favor middle for N
            //Dislike random

            return cells.Count - 1;//Rand.Next(cells.Count);//cells.Count-1;//Rand.Next(cells.Count/2, cells.Count);
        }


    }
}
