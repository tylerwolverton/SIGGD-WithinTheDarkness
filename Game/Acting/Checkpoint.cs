using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.Acting
{
    public class Checkpoint
    {
        public readonly int order;
        public readonly Vector2 position;

        public Checkpoint(int order, Vector2 position) {
            this.order = order;
            this.position = position;
        }
    }
}
