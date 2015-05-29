using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GrowingTree.Display;
using GrowingTree.Features;
using Microsoft.Win32.SafeHandles;

namespace GrowingTree
{
    class Program
    {
        private static readonly Level level = new Level(60, 50);

        internal static class SystemState
        {
            public static bool ShouldQuit = false;
        }
        /// <summary>
        /// Event handler for ^C key press
        /// </summary>
        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("{0} hit, quitting...", e.SpecialKey);
            SystemState.ShouldQuit = true;
            e.Cancel = true; // Set this to true to keep the process from quitting immediately
        }
        static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.CursorVisible = false;

            Console.WriteLine("Hit Enter To Draw");
            Console.ReadLine();
            Console.Clear();

            MainLoop();
            //DrawRooms();
            //DrawMaze();

            //player = new Player(Level.Instance.Rooms[0].Source);
            //monster = new Monster(Level.Instance.Rooms[Level.Instance.Rooms.Count - 1].Source);

            //BeAPlayer();
            
            Console.WriteLine();
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static void MainLoop()
        {
            DrawFeatureRooms();//Initialize
            const int threadSleepInterval = 100;
            DrawGrid.Draw(level);
            
        }

        static void MyPaint()
        { // Set the window size and title
            Console.Title = "Code Page 437: MS-DOS ASCII Characters";

            for (byte b = 0; b < byte.MaxValue; b++)
            {
                char c = Encoding.GetEncoding(437).GetChars(new byte[] { b })[0];
                switch (b)
                {
                    case 8: // Backspace
                    case 9: // Tab
                    case 10: // Line feed
                    case 13: // Carriage return
                        c = '.';
                        break;
                }

                Console.Write("{0:000} {1}   ", b, c);

                // 7 is a beep -- Console.Beep() also works
                if (b == 7) Console.Write(" ");

                if ((b + 1) % 8 == 0)
                    Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void DrawFeatureRooms()
        {
            Level.Instance.GenerateRooms();
            //Level.Instance.Draw();
            Hallway hall;
            while ((hall = Hallway.GenerateHallway(Level.Instance)) != null)
            {
                Level.Instance.InsertFeature(hall);
            }
            //Level.Instance.Draw();

            Console.SetCursorPosition(0, Level.Instance.Height+10);
        }

        //private static void BeAPlayer()
        //{
        //    for (;;)
        //    {
        //        Level.Instance.DrawMap();
        //        player.Draw();
        //        monster.Draw();
        //        Console.SetCursorPosition(0, Level.Instance);
        //        player.Move();
        //        monster.Move();
        //        Console.WriteLine("Time to Move... Player");
        //        Console.WriteLine(player.Message);
        //    }
        //}

        //static void DrawMap()
        //{
        //    var map = new Level(160, 50);
        //    map.GenerateRooms();
        //    map.GenerateHallways();
        //    map.GenerateDoors();
        //    map.PruneHallways();
        //    map.PruneDoors();

        //    map.DrawMap();
        //}

        //static void DrawRooms()
        //{
        //    var rooms = new RoomPlacement(55, 160);

        //    rooms.Draw();
        //    while (rooms.Step())
        //    {
        //        rooms.Draw();
        //    }
        //    rooms.Draw();

        //    var open = rooms.FindOpenPoint();
        //    do
        //    {

        //        var maze = new GrowingTree(55, 160, rooms.map, open);
        //        maze.Draw();
        //        while (maze.Step())
        //        {
        //            maze.Draw();
        //        }
        //        maze.Draw();
   
        //    } while ((open = rooms.FindOpenPoint()) != null);
        //    rooms.Draw();
        //}
        //static void DrawMaze()
        //{
        //    var maze = new GrowingTree(55, 170);
        //    maze.Draw();
        //    while (maze.Step())
        //    {
        //        maze.Draw();
        //    }
        //    maze.Draw();
        //}
    }
}
