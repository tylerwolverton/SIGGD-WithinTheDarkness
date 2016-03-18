using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Engine.Acting.Attacks;
using Engine;
using Engine.Tiles;
using Engine.Acting;
using System.Diagnostics;
using Engine.Textures;

namespace Engine
{

    public class DeathTileBehavior : Behavior
    {
        
        Actor actor;

        const int LIFESPAN = 200;
        int lifeSpanTimer = LIFESPAN;

        public DeathTileBehavior(Actor actor)
            : base(actor)
        {
            this.actor = actor;
            
        }

        public override void run()
        {
            actor.removeMe = (--lifeSpanTimer == 0); //The much sought after one-liner! (Written by Scott Wilkewitz)
        }
    }

    public class DeathTile : Actor
    {

        const bool ACTIVE = true;

        const int MASS = int.MaxValue;
        const float FRICTION = 1f;

        const int EXP = 0;
        const int DAMAGE = 2;

        const String TEX_DIR = "017_DemonQuestionMark";
        const int IMG_INDEX = 80;
        const int SIZE = 48;

        Animation deathAnimation;

        public DeathTile(GameWorld world, Vector2 position)
            : base(world, position, new Vector2(0, 0), SIZE, new Vector2(-16, -24), IMG_INDEX)
        {

            myBehavior = new DeathTileBehavior(this);
            deathAnimation = new Animation(IMG_INDEX, IMG_INDEX, 8, false, 0, 0);

            active = ACTIVE;
            mass = MASS;
            frictionCoefficient = FRICTION;

            textureSet = world.tileEngine.resourceComponent.getTextureSet(TEX_DIR);

            // MASKING
            this.actorcategory = ActorCategory.enemy;
            this.collisionmask = ActorCategory.enemy | ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy | ActorCategory.enemyprojectile;
        }

        public override void collision(Actor a)
        {

            if (anim != deathAnimation)
            {

                ILife liveA = a as ILife;
                if (liveA != null && !liveA.life.isGod)
                {

                    Vector2 dir = new Vector2(a.position.X - this.position.X, a.position.Y - this.position.Y);
                    dir.Normalize();
                    dir *= 250f;
                    a.addImpulse(dir);

                    liveA.life.life -= DAMAGE;
                }

               base.collision(a);
            }
        }
    }
}