using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;
using Engine.Audio;

namespace Engine.Acting.Projectiles
{
    public class LaserArrowBehavior : Behavior
    {
        public Actor actor;
        public LaserArrowBehavior(LaserArrow actor)
            : base(actor)
            
        {
            this.actor = actor;
        }

        public override void run()
        {
            actor.imgIndex = Actor.indexFromDirection(actor.velocity, 24, .25f)%12;
            
            
            actor.velocity = Vector2.Normalize(actor.velocity)*200;
        }
    }
    public class LaserArrow : Actor
    {
        public Actor hasCollided;
        private int wallbounces = 3;
        private int damage;

        public LaserArrow(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 8, Constants.WORLD2MODEL_LASERARROW, Actor.indexFromDirection(direction, 24, .25f)%12)
        {
            anim = null;
            myBehavior = new LaserArrowBehavior(this);
            active = true;
            frictionCoefficient = 0.0001f;
            elasticity = 1.0f;
            mass = 1;
            damage = 15;

            this.setGlow(2.0f);
            // MASKING
            this.actorcategory = ActorCategory.friendlyprojectile;
            this.collisionmask = ActorCategory.enemy;

            textureSet = world.tileEngine.resourceComponent.getTextureSet("027_LaserArrow");
            
            AudioSet lucyLaser = world.tileEngine.resourceComponent.getAudioSet("027_LucyLaser");
            world.tileEngine.audioComponent.playSound(lucyLaser[0], false);
        }
        public override void hitWall()
        {
            wallbounces--;
            if (wallbounces < 0)
            {
                removeMe = true;
            }
            
            AudioSet lucyLaser = world.tileEngine.resourceComponent.getAudioSet("027_LucyLaser");
            world.tileEngine.audioComponent.playSound(lucyLaser[1], false);
        }
        public override void collision(Actor a)
        {
            if (a == hasCollided) { return; };
            hasCollided = a;
            ILife live = a as ILife;
            if (live != null)
            {
                live.life.life -= damage;
            }
            removeMe = false;
        }
    }
}