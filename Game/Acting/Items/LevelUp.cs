using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Tiles;
using System.IO;
using Engine.Textures;

namespace Engine.Acting.Items
{
    public class LevelUpBehavior : PathfindBehavior
    {
        public int delay;
        public LevelUp levelUp;
        public LevelUpBehavior(LevelUp levelUp, Actor target)
            : base(levelUp as Actor, target, 0f)
        {
            delay = 0;
            this.levelUp = levelUp;
        }

        public override void run()
        {
            if (target == null)
            {
                actor.force = Vector2.Zero;
                return;
            }
            delay++;

            if (delay >= 40)
            {
                levelUp.removeMe = true;
            }
            actor.world2model.Y--;
            base.run();
        }
    }

    public class LevelUp : Actor
    {
        public int dieSound;
        public LevelUp(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 6, Constants.WORLD2MODEL_LEVELUP, 0)
        {
            anim = new Animation(0, 4, 24f, false, 0, 0); ;
            myBehavior = new LevelUpBehavior(this, world.player);
            active = true;
            frictionCoefficient = 0.1f;
            elasticity = .5f;
            mass = 10;
            // MASKING
            this.actorcategory = ActorCategory.nocategory;
            this.collisionmask = ActorCategory.nocategory;

            textureSet = world.tileEngine.resourceComponent.getTextureSet("010_LevelUp");
        }
    }
}
