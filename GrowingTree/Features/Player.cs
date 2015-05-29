using System;
using GrowingTree.Display;

namespace GrowingTree.Features
{
    class Player : Character
    {
        public string Message = "";

        public Player(Point startingLocation) : base(startingLocation)
        {

        }

        protected override void DrawImpl(DrawGrid drawGrid)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write("P");
        }

        public override void Move()
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Q)
            {
                return;
            }
            Move(key.Key);
        }

        private void Move(ConsoleKey key)
        {
            //Message = "";
            //switch (key)
            //{
            //    case ConsoleKey.DownArrow:
            //        Move(0, +1, "Below");
            //        break;
            //    case ConsoleKey.UpArrow:
            //        Move(0, -1, "Above");
            //        break;
            //    case ConsoleKey.LeftArrow:
            //        Move(-1, 0, "Left");
            //        break;
            //    case ConsoleKey.RightArrow:
            //        Move(+1, 0, "Right");
            //        break;
            //}

            //LookAround();
        }

        //private void LookAround()
        //{
        //    if (Location.x - 1 >= 0)
        //    {
        //        Message += "\r\nLeft of you is " + map[Location.x - 1, Location.y] + "                            ";
        //    }
        //    if (Location.x + 1 < Level.Instance.Width)
        //    {
        //        Message += "\r\nRight of you is " + map[Location.x + 1, Location.y] + "                            ";
        //    }
        //    if (Location.y - 1 >= 0)
        //    {
        //        Message += "\r\nAbove you is " + map[Location.x, Location.y - 1] + "                            ";
        //    }
        //    if (Location.y + 1 < Level.Instance.Height)
        //    {
        //        Message += "\r\nBelow you is " + map[Location.x, Location.y + 1] + "                            ";
        //    }
        //}

        //private void Move(int xMod, int yMod, string direction)
        //{
        //    if (map[Location.x + xMod, Location.y + yMod] == Level.TileType.Empty)
        //    {
        //        Message += "\r\nCannot Move There                                          ";
        //    }
        //    else
        //    {
        //        Location.x += xMod;
        //        Location.y += yMod;
        //        Message += "\r\nYou've Moved " + direction;
        //    }
        //}
    }
}
