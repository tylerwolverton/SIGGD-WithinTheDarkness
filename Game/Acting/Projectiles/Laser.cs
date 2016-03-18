using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;

namespace Engine.Acting.Projectiles
{
    public class LaserBehavior : ProjectileBehavior
    {
        public LaserBehavior(Laser actor)
            : base(actor, 20)
        {
        }

        public override void run()
        {
            actor.imgIndex = Actor.indexFromDirection(actor.velocity, 16, .25f);
            base.run();
        }
    }
    public class Laser : Actor
    {
        private int wallbounces = 1;
        private int damage;

        public Laser(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 4, Constants.WORLD2MODEL_LASER, Actor.indexFromDirection(direction, 16, .25f))
        {
            anim = null;
            myBehavior = new LaserBehavior(this);
            active = true;

            frictionCoefficient = Constants.SENTINAL_LASER_FRICTION;
            elasticity = Constants.SENTINAL_LASER__ELASTICITY;
            mass = Constants.SENTINAL_LASER_MASS;
            damage = Constants.SENTINAL_LASER_DAMAGE;

            // this.setGlow(2.0f);
            // MASKING
            this.actorcategory = ActorCategory.enemyprojectile;
            this.collisionmask = ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy | ActorCategory.enemyprojectile;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            textureSet = world.tileEngine.resourceComponent.getTextureSet("009_Laser");
        }
        public override void hitWall()
        {
            wallbounces--;
            if (wallbounces < 0)
            {
                removeMe = true;
            }
        }
        public override void collision(Actor a)
        {
            ILife live = a as ILife;
            if (live != null)
            {
                live.life.life -= damage;
            }
            removeMe = true;
        }
    }
}