using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using GameEngine.Features;

namespace GameEngine.Display
{
    public abstract class DrawScreen
    {
        public interface IData
        {
        }

        public interface IDrawImpl
        {
            //Needs both so a drawimpl can call draw
            void DrawImpl(Feature feature, DrawScreen drawScreen);
        }

        public interface IDraw
        {
            void Draw(Feature feature, DrawScreen drawScreen);
        }


        protected static Dictionary<Type, IDrawImpl> DrawImplDictionary = new Dictionary<Type, IDrawImpl>();
        private static Type drawScreenType;
        public readonly int Height;
        public readonly int Width;
        private readonly IData[,] grid;
        private int gridX;
        private int gridY;


        public static void SetDrawScreen(Type drawScreenClass)
        {
            if (!drawScreenClass.IsSubclassOf(typeof (DrawScreen)))
            {
                throw new InvalidOperationException("Draw Type must be a sub class of DrawScreen");
            }
            drawScreenType = drawScreenClass;
        }
        public static void DebugDraw(Feature feature)
        {
            if (SystemState.DebugFlags.Draw)
            {
                Draw(feature);
            }
        }
        public static void Draw(Feature feature)
        {
            var types = new [] {typeof(int), typeof(int)};
            var paramd = new object[] {feature.Width, feature.Height};
            var ctor = drawScreenType.GetConstructor(types);
            if (ctor == null)
            {
                throw new InvalidOperationException("Must have double int constructor");
            }
            var obj = (DrawScreen)ctor.Invoke(paramd);
            if (obj == null)
            {
                throw new InvalidOperationException("Unable to create DrawScreen derived object");
            }
        }

        protected DrawScreen(int width, int height)
        {
            if (DrawImplDictionary.Count == 0)
            {
                throw new InvalidOperationException("DrawImplDictionary MUST be set up in static initialization");
            }

            Width = width;
            Height = height;
            grid = new IData[Width, Height];
        }


        public void Place(IData data)
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

    }
}
