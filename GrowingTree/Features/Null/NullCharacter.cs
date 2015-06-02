using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingTree.Display;

namespace GrowingTree.Features.Null
{
    class NullCharacter : Character.Character
    {
        public static readonly NullCharacter Instance = new NullCharacter();

        public static bool IsNullCharacter(Character.Character feature)
        {
            return feature is NullCharacter;
        }
        private NullCharacter()
            : base(new Point { X = 0, Y = 0 })
        {
        }

        public override int VisionDistance
        {
            get { return Instance.VisionDistance; }
        }

        public override void Move(ConsoleKey key = ConsoleKey.NoName)
        {
        }
        protected override void DrawImpl(DrawGrid drawGrid)
        {
        }
    }
}
