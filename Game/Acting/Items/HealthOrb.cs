using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace Engine.Acting.Items
{
    public class HealthOrbBehavior : PathfindBehavior
    {

        public int delay;
        public HealthOrb healthorb;

        public HealthOrbBehavior(HealthOrb healthorb, Actor target)
            : base(healthorb as Actor, target, 0f)
        {
            delay = 30;
            this.healthorb = healthorb;
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
                healthorb.collisionmask = Actor.ActorCategory.friendly | Actor.ActorCategory.friendlyprojectile;
            }

            force = 1f - (Vector2.Distance(actor.position, target.position) / 400f);

            if (force > 0)
                base.run();
            if (Vector2.Distance(actor.position, target.position) > 600f)
                actor.removeMe = true;
        }
    }

    public class HealthOrb : Actor
    {

        public HealthOrb(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 6, Constants.WORLD2MODEL_HEALTHORB, 0)
        {
            anim = null;
            myBehavior = new HealthOrbBehavior(this, world.player);
            active = true;
            frictionCoefficient = 0.1f;
            elasticity = .5f;
            mass = 10;

            // MASKING
            this.actorcategory = ActorCategory.powerup;
            this.collisionmask = ActorCategory.nocategory;
            this.collisionimmunitymask = ActorCategory.enemy | ActorCategory.friendlyprojectile;
            textureSet = world.tileEngine.resourceComponent.getTextureSet("006_HealthOrb");
        }

        public override void collision(Actor a)
        {

            ILife liveActor = a as ILife;
            if (liveActor == null) return;

            liveActor.life.life += 5;
            this.removeMe = true;
        }
    }
}