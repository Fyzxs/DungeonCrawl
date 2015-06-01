using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrowingTree.Character;
using GrowingTree.Display;

namespace GrowingTree.Features.Null
{
    class NullPlayer : Player
    {
        public static readonly NullPlayer Instance = new NullPlayer();

        public static bool IsNullCharacter(Player feature)
        {
            return feature is NullPlayer;
        }
        private NullPlayer()
            : base(new Point { X = 0, Y = 0 })
        {
        }

        public override void Move(ConsoleKey key = ConsoleKey.NoName)
        {
        }
        protected override void DrawImpl(DrawGrid drawGrid)
        {
        }
    }
}
