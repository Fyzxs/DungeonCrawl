 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.Display;
using GameEngine.Features;
using GameEngine.Pathing;

namespace GameEngineConsole
{
    public class DrawScreenImpl : DrawScreen
    {
        class FloorTileDraw : IDrawImpl
        {
            public void DrawImpl(Feature feature, DrawScreen drawScreen)
            {
                var drawGrid = (DrawScreenImpl) drawScreen;
                var orgColor = drawGrid.CurrentBackground;
                var player = Level.Instance.GetPlayer();
                drawGrid.CurrentBackground = player.HasSeen(feature) ? ConsoleColor.DarkGray : orgColor;
                drawGrid.CurrentBackground = player.CanSee(feature) ? ConsoleColor.White : drawGrid.CurrentBackground;
                if (Program.SystemState.DebugFlags.DrawAll)
                {
                    drawGrid.CurrentBackground = player.CanSee(feature) ? ConsoleColor.White : ConsoleColor.DarkYellow;
                }
                drawGrid.Place(new DataImpl(feature.AssignedIntersectional));
                drawGrid.CurrentBackground = orgColor;
            }
        }

        class HallwayDraw : IDrawImpl
        {
            public void DrawImpl(Feature feature, DrawScreen drawScreen)
            {
                throw new NotImplementedException();
            }
        }

        class DoorDraw : IDrawImpl
        {
            public void DrawImpl(Feature feature, DrawScreen drawScreen)
            {
                throw new NotImplementedException();
            }
        }

        class FeatureDraw : IDrawImpl, IDraw
        {
            public void DrawImpl(Feature feature, DrawScreen drawScreen)
            {
                throw new NotImplementedException();
            }

            public void Draw(Feature feature, DrawScreen drawScreen)
            {
                throw new NotImplementedException();
            }
        }

        class RoomDraw : IDrawImpl
        {
            public void DrawImpl(Feature feature, DrawScreen drawScreen)
            {
                throw new NotImplementedException();
            }
        }

        static DrawScreenImpl()
        {
            DrawImplDictionary.Add(typeof(FloorTile), new FloorTileDraw());
        }

        protected class DataImpl : IData
        {
            public readonly char Character;

            public DataImpl(Intersectional assignedIntersectional)
            {

                const char leftT = (char)185;       // ╣
                const char vertical = (char)186;    // ║
                const char upperLeft = (char)187;   // ╗
                const char lowerLeft = (char)188;   // ╝
                const char lowerRight = (char)200;  // ╚
                const char upperRight = (char)201;  // ╔
                const char bottomT = (char)202;     // ╩
                const char upperT = (char)203;      // ╦
                const char rightT = (char)204;      // ╠
                const char horizontal = (char)205;  // ═
                const char allways = (char)206;     // ╬

                switch (assignedIntersectional)
                {
                    case Intersectional.BottomTop:
                        Character = vertical;
                        break;
                    case Intersectional.BottomRight:
                        Character = upperRight;
                        break;
                    case Intersectional.BottomLeft:
                        Character = upperLeft;
                        break;
                    case Intersectional.BottomTopRight:
                        Character = rightT;
                        break;
                    case Intersectional.BottomTopLeft:
                        Character = leftT;
                        break;
                    case Intersectional.BottomTopLeftRight:
                        Character = allways;
                        break;
                    case Intersectional.BottomLeftRight:
                        Character = upperT;
                        break;
                    case Intersectional.TopRight:
                        Character = lowerRight;
                        break;
                    case Intersectional.TopLeft:
                        Character = lowerLeft;
                        break;
                    case Intersectional.TopLeftRight:
                        Character = bottomT;
                        break;
                    case Intersectional.LeftRight:
                        Character = horizontal;
                        break;
                    default:
                        Character = '#';
                        break;
                }
            }
        }

        public ConsoleColor CurrentForeground = ConsoleColor.Black;
        public ConsoleColor CurrentBackground = ConsoleColor.Black;

        public DrawScreenImpl(int width, int height) : base(width, height)
        {
        }
    }
}
