using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Engine.Acting.Items;

namespace Engine.Acting
{
    public class NumbersBehavior : PathfindBehavior
    {
        public int delay;
        public Numbers number;
        public NumbersBehavior(Numbers number, Actor target)
            : base(number as Actor, target, 0f)
        {
            delay = 0;
            this.number = number;
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
                number.removeMe = true;
            }
            actor.world2model.Y--;
            base.run();
        }
    }

    public class Numbers : Actor
    {
        // Spawn some numbers
        public static void spawn(Actor a, int val) {
            Numbers num;
            int xOffset = 0;
            
            while (val > 0) {
                num = new Numbers(a.world as GameWorld, a.position,  a.velocity);
                num.world2model = new Vector2(-3f + xOffset, -66f);
                num.frictionCoefficient = a.frictionCoefficient/2;
                num.imgIndex = val % 10;
                a.world.addActor(num);
                xOffset -= 10;
                val /= 10;      
            }
        }

        public int dieSound;
        public Numbers(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 6, Constants.WORLD2MODEL_NUMBERS, 0)
        {
            anim = null;
            myBehavior = new NumbersBehavior(this, world.player);
            active = true;
            frictionCoefficient = 0.1f;
            elasticity = .5f;
            mass = 10;
            // MASKING
            this.actorcategory = ActorCategory.nocategory;
            this.collisionmask = ActorCategory.nocategory;

            textureSet = world.tileEngine.resourceComponent.getTextureSet("011_Numbers");
        }
    }
}