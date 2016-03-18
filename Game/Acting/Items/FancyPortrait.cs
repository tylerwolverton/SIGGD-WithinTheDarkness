using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Engine.Textures;

namespace Engine.Acting.Items
{
    public class FancyPortrait : Furniture
    {

        public FancyPortrait(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction)
        {
            textureSet = world.tileEngine.resourceComponent.getTextureSet("037_Details");
            anim = new Animation(3, 3, 1f, true, 0, 0);
            world2model = Constants.WORLD2MODEL_FANCYPORT;
        }
    }
}
