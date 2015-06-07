using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrowingTree.Character;
using GrowingTree.Display;
using GrowingTree.Features;
using GrowingTree.Pathing;
using Microsoft.Win32.SafeHandles;

namespace GrowingTree
{
    class Program
    {
        internal static class SystemState
        {
            public static bool ShouldQuit = false;

            public static class DebugFlags
            {
                //Shows the entire map; not just visible sections
                public static bool DrawAll = false;
                //Allows regeneration of maps without quitting
                public static bool Regen = true;
                //If the debug drawing should happen
                public static bool Draw = false;
                //Toggle the Heuristics
                public static bool SearchHeuristics = false;
            }
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

            do
            {
                SystemState.DebugFlags.Regen = false;
                SystemState.ShouldQuit = false;
                MainLoop();
            } while (SystemState.DebugFlags.Regen);

            Console.SetCursorPosition(0, 80-10);
            Console.WriteLine();
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static void MainLoop()
        {
            var level = LevelCreator.CreateLevel(60, 50);
            DrawGrid.Draw(level);
            var startRoom = level.GetRooms().First();
            level.InsertFeature(new Player(new Point(startRoom.Left + 1, startRoom.Top + 1)));

            var endRoom = level.GetRooms().Last();
            level.InsertFeature(new Hunter(new Point(endRoom.Left + 1, endRoom.Top + 1)));
            while (!SystemState.ShouldQuit)
            {
                Thread.Sleep(100);
                Level.Instance.RefreshFeatureGrid();
                if (ProcessInput())
                {
                    DrawGrid.Draw(level);
                }
            }
        }

        static bool ProcessInput()
        {
            if (!Console.KeyAvailable) return true;

            var key = Console.ReadKey(true);
            //Clear the buffer
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            switch (key.Key)
            {
                case ConsoleKey.R:
                    SystemState.DebugFlags.Regen = true;
                    goto case ConsoleKey.Q;//goto is being accepted as a forced 'fall through'
                case ConsoleKey.Q:
                    SystemState.ShouldQuit = true;
                    return true;
                case ConsoleKey.D:
                    SystemState.DebugFlags.DrawAll = !SystemState.DebugFlags.DrawAll;
                    SystemState.DebugFlags.Draw = !SystemState.DebugFlags.Draw;
                    return true;
                case ConsoleKey.P:
                {
                    var monster = Level.Instance.GetMonsters().FirstOrDefault();
                    if (monster == null)
                    {
                        return true;
                    }
                    var player = Level.Instance.GetPlayer();
                    var startTile = Level.Instance.FeatureGrid[player.Left, player.Top];
                    var goalTile = Level.Instance.FeatureGrid[monster.Left, monster.Top];
                    PathFinding.CustomPathFinding(Level.Instance.FeatureGrid,
                        startTile,
                        goalTile);
                    return false;
                }
                case ConsoleKey.H:
                    SystemState.DebugFlags.SearchHeuristics = !SystemState.DebugFlags.SearchHeuristics;
                    return true;
                default:
                    Level.Instance.GetPlayer().Move(key.Key);
                    foreach (var monster in Level.Instance.GetMonsters())
                    {
                        monster.Move();
                    }
                    return true;
            }
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
    }
}
