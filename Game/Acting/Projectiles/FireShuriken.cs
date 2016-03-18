using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Tiles;
using System.Diagnostics;
using System.IO;
using Engine.Textures;

namespace Engine.Acting.Projectiles
{
    public class FireShurikenBehavior : ProjectileBehavior
    {

        private float explosionCooldown;
        private int duration = 250;
        public FireShurikenBehavior(FireShuriken actor)
            : base(actor, 0)
        {
            this.explosionCooldown = 30f;
            this.actor = actor;
        }

        public override void run()
        {
            (actor as FireShuriken).oldervelocity = (actor as FireShuriken).oldvelocity;
            (actor as FireShuriken).oldvelocity = (actor as FireShuriken).velocity;
            
            duration--;
            if (duration <= 0)
            {
                (actor as ILife).life.life = 0;
            }
            base.run();
        }
    }
    public class FireShuriken : Actor, ILife
    {
        public Vector2 oldvelocity;
        public Vector2 oldervelocity;
        public Animation defaultAnimation;
        public float damage;
        public Life life { get; private set; }

        public FireShuriken(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, Constants.FIRESHURIKEN_SIZE, Constants.WORLD2MODEL_FIRESHURIKEN, 0)
        {
            defaultAnimation = new Animation(0, 2, 3f, true, 0, 0);

            anim = defaultAnimation;
            myBehavior = new FireShurikenBehavior(this);
            active = true;
            frictionCoefficient = Constants.FIRESHURIKEN_FRICTION;
            elasticity = Constants.FIRESHURIKEN_ELASTICITY;
            mass = Constants.FIRESHURIKEN_MASS;

            // TODO: Add to Constants
            life = new Life(this, Constants.FIRESHURIKEN_HEALTH);
            // Setup death
            life.deathEvent += delegate() { removeMe = true; };
            
            damage = Constants.FIRESHURIKEN_DAMAGE;

            this.setGlow(2.0f);

            // MASKING
            this.actorcategory = ActorCategory.friendlyprojectile;
            this.collisionmask = ActorCategory.enemy | ActorCategory.enemyprojectile;
            this.collisionimmunitymask = ActorCategory.friendly;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("003_FireShuriken");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("003_FireShuriken");
        }
        public override void hitWall()
        {
        }
        public override void collision(Actor a)
        {
            /*life.life -= 15;
            Vector2 dir = new Vector2(oldervelocity.X, oldervelocity.Y);
            if (dir != null)
            {
                dir.Normalize();
                dir *= 25;
                a.addImpulse(dir);

            }*/

            ILife live = (a as ILife);
            if (live != null)
            {
                live.life.life -= damage;
            }
        }
    }
}