using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GrowingTree
{
    class RoomPlacement
    {
        private static readonly Random Rand = new Random();

        private const int MaxSize = 10;
        private const int MinSize = 5;

        private const int MaxRetries = 1000;

        public readonly char[,] map;
        public readonly int width;
        public readonly int height;

        public RoomPlacement(int width, int height)
        {
            this.width = width;
            this.height = height;
            map = new char[width, height];
        }


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
            Console.SetCursorPosition(0, 0);
            Console.Write(sb.ToString());
        }

        public bool Step()
        {
            for (var attempts = 0; attempts < MaxRetries; attempts++)
            {
                //Get a size
                var sizeW = Rand.Next(MinSize, MaxSize);
                var sizeH = Rand.Next(MinSize, MaxSize);

                //Get a location
                var x = Rand.Next(1, width-sizeW-1);
                var y = Rand.Next(1, height-sizeH-1);//The extra 1(s) is because I'm being lazy in the "IsValid" method

                if (!IsValid(sizeW, sizeH, x, y))
                {
                    continue;
                }

                //Draw in said location
                for (var w = 0; w < sizeW; w++)
                {
                    for (var h = 0; h < sizeH; h++)
                    {
                        map[w + x, h + y] = Program.FloorAscii;
                    }
                }
                return true;
            }
            return false;
        }

        private bool IsValid(int sizeW, int sizeH, int x, int y)
        {
            for (var w = -1; w < sizeW+1; w++)
            {
                for (var h = -1; h < sizeH+1; h++)
                {
                    if (map[w + x, h + y] != (char) 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Point FindOpenNine()
        {
            for (var w = 0; w < width; w++)
            {
                for (var h = 0; h < height; h++)
                {
                    var point = new Point() {x = w, y = h};

                    #region Direction Selection
                    if (point.x + 1 < width && map[point.x + 1, point.y] == (char)0)
                    {
                        /*
                         * [ ][ ][+1 -1][+2 -1][ ]
                         * [ ][ ][  X  ][+2  0][ ]
                         * [ ][ ][+1 +1][+2 +1][ ]
                         */
                        if (!((point.x + 2 >= width || point.y - 1 < 0 || map[point.x + 2, point.y - 1] == (char)0) &&
                            (point.x + 2 >= width || map[point.x + 2, point.y] == (char)0) &&
                            (point.x + 2 >= width || point.y + 1 >= height || map[point.x + 2, point.y + 1] == (char)0) &&
                            (point.x + 1 >= width || point.y + 1 >= height || map[point.x + 1, point.y + 1] == (char)0) &&
                            (point.x + 1 >= width || point.y - 1 < 0 || map[point.x + 1, point.y - 1] == (char)0)))
                        {
                            continue;
                        }
                    }
                    if (point.x - 1 > 0 && map[point.x - 1, point.y] == (char)0)
                    {
                        /*
                         * [ ][-2 -1][-1 -1][ ][ ]
                         * [ ][-2  0][  X  ][ ][ ]
                         * [ ][-2 +1][-1 +1][ ][ ]
                         */
                        if (!((point.x - 2 < 0 || point.y - 1 < 0 || map[point.x - 2, point.y - 1] == (char)0) &&
                            (point.x - 2 < 0 || map[point.x - 2, point.y] == (char)0) &&
                            (point.x - 2 < 0 || point.y + 1 >= height || map[point.x - 2, point.y + 1] == (char)0) &&
                            (point.x - 1 < 0 || point.y + 1 >= height || map[point.x - 1, point.y + 1] == (char)0) &&
                            (point.x - 1 < 0 || point.y - 1 < 0 || map[point.x - 1, point.y - 1] == (char)0)))
                        {
                            continue;
                        }
                    }
                    if (point.y + 1 < height && map[point.x, point.y + 1] == (char)0)
                    {
                        /*
                         * [ ][-1 +1][  X  ][ 1 +1][ ]
                         * [ ][-1 +2][ 0 +2][ 1 +2][ ]
                         * [ ][     ][     ][     ][ ]
                         */
                        if (!((point.x - 1 < 0 || point.y + 2 >= height || map[point.x - 1, point.y + 2] == (char)0) &&
                            (point.y + 2 >= height || map[point.x, point.y + 2] == (char)0) &&
                            (point.x + 1 >= width || point.y + 2 >= height || map[point.x + 1, point.y + 2] == (char)0) &&
                            (point.x - 1 < 0 || point.y + 1 >= height || map[point.x - 1, point.y + 1] == (char)0) &&
                            (point.x + 1 >= width || point.y + 1 >= height || map[point.x + 1, point.y + 1] == (char)0)))
                        {
                            continue;
                        }
                    }
                    if (point.y - 1 > 0 && map[point.x, point.y - 1] == (char)0)
                    {
                        /*
                         * [ ][     ][     ][     ][ ]
                         * [ ][-1 -2][ 0 -2][ 1 -2][ ]
                         * [ ][-1 -1][  X  ][ 1 -1][ ]
                         */
                        if (!((point.x - 1 < 0 || point.y - 2 < 0 || map[point.x - 1, point.y - 2] == (char)0) &&
                            (point.y - 2 < 0 || map[point.x, point.y - 2] == (char)0) &&
                            (point.x + 1 >= width || point.y - 2 < 0 || map[point.x + 1, point.y - 2] == (char)0) &&
                            (point.x - 1 < 0 || point.y - 1 < 0 || map[point.x - 1, point.y - 1] == (char)0) &&
                            (point.x + 1 >= width || point.y - 1 < 0 || map[point.x + 1, point.y - 1] == (char)0)))
                        {
                            continue;
                        }
                    }
                    #endregion

                    return point;//If we're here; then it's valid
                }
            }
            return null;
        }
    }
}
