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
    public class ExplosionBehavior : ProjectileBehavior
    {
        public ExplosionBehavior(Explosion actor)
            : base(actor, 0)
        {
            this.actor = actor;
        }

        public override void run()
        {
            // GLOWING
            int beams = 20;
            Color fifthwhite = new Color(1.0f, 1.0f, 1.0f);
            float increment = (float)Math.PI * 2 / beams;
            for (float x = 0; x < Math.PI * 2; x += increment)
            {
                actor.world.castRay(actor.position, new Vector2((float)Math.Cos(x), (float)Math.Sin(x)), fifthwhite);
            }
            base.run();
        }
    }

    public class Explosion : Actor, ILife
    {
        public Animation defaultAnimation;
        public Vector2 playerPosition;

        public Life life { get; private set; }

        public Explosion(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, new Vector2(0, 0), Constants.EXPLOSION_SIZE, Constants.WORLD2MODEL_EXPLOSION, 0)
        {
            defaultAnimation = new Animation(0, 4, 6f, false, 0, 0);
            anim = defaultAnimation;
            myBehavior = new ExplosionBehavior(this);
            active = true;
            frictionCoefficient = Constants.EXPLOSION_FRICTION;

            mass = float.MaxValue;
            playerPosition = world.player.position;
            // Setup death
            life = new Life(this, Constants.EXPLOSION_HEALTH);
            defaultAnimation.addEndAct((frame) => { removeMe = true; });
            life.deathEvent += delegate() { anim = defaultAnimation; };


            this.setGlow(2);
            // MASKING
            this.actorcategory = ActorCategory.friendlyprojectile;
            this.collisionmask = ActorCategory.enemyprojectile | ActorCategory.enemy;
            this.collisionimmunitymask = ActorCategory.friendly;
            this.damageimmunitymask =  ActorCategory.friendlyprojectile;

            textureSet = world.tileEngine.resourceComponent.getTextureSet("023_Explosion");

        }
        public override void hitWall()
        {
            removeMe = true;
        }
        public override void collision(Actor a)
        {
            // Hack so Mr Hammer's explosions don't collide with him
            if (a.actorName != "Hammer")
            {
                a.force += (a.mass * (Vector2.Normalize(a.position - playerPosition)));
            }
        }
    }
}