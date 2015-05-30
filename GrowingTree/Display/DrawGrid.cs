using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GrowingTree.Features;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace GrowingTree.Display
{
    class DrawGrid
    {
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct CharUnion
        {
            [FieldOffset(0)]
            public char UnicodeChar;
            [FieldOffset(0)]
            public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)]
            public CharUnion Char;
            [FieldOffset(2)]
            public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        public struct Data
        {
            public ConsoleColor Foreground;
            public ConsoleColor Background;
            public char Character;
        }

        public readonly int Height;
        public readonly int Width;
        private readonly Data[,] grid;
        private int gridX;
        private int gridY;
        public ConsoleColor CurrentForeground = ConsoleColor.Black;
        public ConsoleColor CurrentBackground = ConsoleColor.Black;
        private DrawGrid(int width, int height)
        {
            Width = width;
            Height = height;
            grid = new Data[Width, Height];
        }

        public static void DebugDraw(Feature feature)
        {
            Draw(feature);
        }
        public static void Draw(Feature feature)
        {
            var d = new DrawGrid(feature.Width, feature.Height);
            feature.Draw(d);
            d.SpeedDraw();
        }

        public void Place(char c)
        {
            Place(new Data { Character = c, Background = CurrentBackground, Foreground = CurrentForeground });
        }
        public void Place(Data data)
        {
            grid[gridX, gridY] = data;
        }
        public void AdjustLeft(int amount)
        {
            gridX += amount;
        }

        public void AdjustDown(int amount)
        {
            gridY += amount;
        }

        public void AdjustRight(int amount)
        {
            gridX -= amount;
        }

        public void AdjustUp(int amount)
        {
            gridY -= amount;
        }

        private void SpeedDraw()
        {
            SafeFileHandle h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if (h.IsInvalid)
            {
                Console.WriteLine("UNABLE TO SPEED DRAW!");
                return;
            };
            CharInfo[] buf = new CharInfo[Width * Height];
            SmallRect rect = new SmallRect() { Left = 0, Top = 0, Right = (short) Width, Bottom = (short) Height };

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var data = grid[x, y];
                    var foreground = ConsoleColorToColorAttribute(data.Foreground, false);
                    var background = ConsoleColorToColorAttribute(data.Background, true);
                    var attribute = (short) (foreground | background);
                    buf[y * Width + x].Char.AsciiChar = (byte) (data.Character == '\0' ? ' ' : data.Character);
                    buf[y * Width + x].Attributes = attribute;
                }
            }
            bool b = WriteConsoleOutput(h, buf,
                new Coord() { X = (short) Width, Y = (short) Height },
                new Coord() { X = 0, Y = 0 },
                ref rect);
            if (!b)
            {
                Console.WriteLine("No Write... Iz Sad");
            }
            //for (byte character = 65; character < 65 + 26; ++character)
            //{
            //    for (short attribute = 0; attribute < 15; ++attribute)
            //    {
            //        for (int i = 0; i < buf.Length; ++i)
            //        {
            //            buf[i].Attributes = attribute;
            //            buf[i].Char.AsciiChar = character;
            //        }

            //        bool z = WriteConsoleOutput(h, buf,
            //            new Coord() { X = 80, Y = 25 },
            //            new Coord() { X = 0, Y = 0 },
            //            ref rect);
            //    }
            //}
        }
        private ushort ConsoleColorToColorAttribute(ConsoleColor color, bool isBackground)
        {
            ushort color1 = (ushort)color;
            if (isBackground)
                color1 = (ushort)((int)color1 << 4);
            return color1;
        }
    }

    /*
    
      SafeFileHandle h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

      if (!h.IsInvalid)
      {
        CharInfo[] buf = new CharInfo[80 * 25];
        SmallRect rect = new SmallRect() { Left = 0, Top = 0, Right = 80, Bottom = 25 };

        for (byte character = 65; character < 65 + 26; ++character)
        {
          for (short attribute = 0; attribute < 15; ++attribute)
          {
            for (int i = 0; i < buf.Length; ++i)
            {
              buf[i].Attributes = attribute;
              buf[i].Char.AsciiChar = character;
            }

            bool b = WriteConsoleOutput(h, buf,
              new Coord() { X = 80, Y = 25 },
              new Coord() { X = 0, Y = 0 },
              ref rect);
          }
        }
      }
      Console.ReadKey();*/
}
