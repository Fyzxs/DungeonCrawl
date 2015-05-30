using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrowingTree.Display;
using GrowingTree.Features;
using Microsoft.Win32.SafeHandles;

namespace GrowingTree
{
    class Program
    {
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
            level.InsertFeature(new Monster(new Point(endRoom.Left + 1, endRoom.Top + 1)));
            while (!SystemState.ShouldQuit)
            {
                Thread.Sleep(100);
                ProcessInput();
                DrawGrid.Draw(level);
            }
        }

        static void ProcessInput()
        {
            if (!Console.KeyAvailable) return;

            var key = Console.ReadKey();
            //Clear the buffer
            while (Console.KeyAvailable)
            {
                Console.ReadKey(false);
            }
            switch (key.Key)
            {
                case ConsoleKey.Q:
                    SystemState.ShouldQuit = true;
                    return;
                default:
                    Level.Instance.GetPlayer().Move(key.Key);
                    foreach (var monster in Level.Instance.GetMonsters())
                    {
                        monster.Move();
                    }
                    return;
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
