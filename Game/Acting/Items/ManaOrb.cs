using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace Engine.Acting.Items
{
    public class ManaOrbBehavior : PathfindBehavior
    {

        public int delay;
        public ManaOrb manaorb;

        public ManaOrbBehavior(ManaOrb manaorb, Actor target)
            : base(manaorb as Actor, target, 0f)
        {
            delay = 30;
            this.manaorb = manaorb;
        }

        public override void run()
        {

            if (target == null)
            {
                actor.force = Vector2.Zero;
                return;
            }

            delay--;

            if (delay <= 0)
            {
                manaorb.collisionmask = Actor.ActorCategory.friendly | Actor.ActorCategory.friendlyprojectile;
            }

            force = 0.4f - (Vector2.Distance(actor.position, target.position) / 300f);

            if (force > 0)
                base.run();
            if (Vector2.Distance(actor.position, target.position) > 600f)
                actor.removeMe = true;
        }
    }

    public class ManaOrb : Actor
    {

        public ManaOrb(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 6, Constants.WORLD2MODEL_MANAORB, 0)
        {

            anim = null;
            myBehavior = new ManaOrbBehavior(this, world.player);
            active = true;
            frictionCoefficient = 0.1f;
            elasticity = .5f;
            mass = 10;

            // MASKING
            this.actorcategory = ActorCategory.powerup;
            this.collisionmask = ActorCategory.nocategory;
            this.collisionimmunitymask = ActorCategory.enemy | ActorCategory.friendlyprojectile;
            textureSet = world.tileEngine.resourceComponent.getTextureSet("007_ManaOrb");
        }

        public override void collision(Actor a)
        {

            Player p = a as Player;
            if (p == null) return;
            //p.mana += 5;
            removeMe = true;
        }
    }
}