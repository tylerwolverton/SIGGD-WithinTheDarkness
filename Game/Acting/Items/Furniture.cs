using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace Engine.Acting.Items
{
    public class FurnitureBehavior
    {
    }
    public class Furniture : Actor
    {
        public Furniture(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 6, new Vector2(-8f, -16f), 0)
        {
            ignoreAvE = true;
        }
    }
}
