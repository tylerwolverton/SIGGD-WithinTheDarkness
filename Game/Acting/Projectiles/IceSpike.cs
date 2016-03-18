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
    public class IceSpikeBehavior : ProjectileBehavior
    {
        private int duration = Constants.ICE_DURATION;
        public IceSpikeBehavior(IceSpike actor)
            : base(actor, 0)
        {
            this.actor = actor;
        }

        public override void run()
        {
            duration--;
            if (duration <= 0)
            {
                (actor as ILife).life.life = 0;
            }
            int beams = 35;
            Color fifthwhite = new Color(0,0, 0.06f);
            float increment = (float)Math.PI * 2 / beams;
            //offset is a rand double from 0 to 1; helps hide individual rays
            float offset = (float)actor.world.tileEngine.randGen.NextDouble(); 
            for (float x = 0; x < Math.PI * 2; x += increment)
            {
                actor.world.castRay(actor.position, new Vector2((float)Math.Cos(x+offset), (float)Math.Sin(x+offset)), fifthwhite);
            }
            base.run();
        }
    }
    public class IceSpike : Actor, ILife
    {
        public Animation defaultAnimation;

        public Life life { get; private set; }

        public IceSpike(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, new Vector2(0,0), Constants.ICE_SIZE, Constants.WORLD2MODEL_ICESPIKE, 0)
        {
            defaultAnimation = new Animation(0, 4, 3f, false, 0, 0);
            anim = defaultAnimation;
            myBehavior = new IceSpikeBehavior(this);
            active = true;
            frictionCoefficient = Constants.ICE_FRICTION;

            mass = float.MaxValue;

            // Setup death
            life = new Life(this, Constants.ICE_HEALTH);
            Animation deathAnim = new Animation(4, 8, 5f, false, 0, 0);
            deathAnim.addEndAct((frame) => { removeMe = true; });
            life.deathEvent += delegate() { anim = deathAnim; };


            this.setGlow(2);
            // MASKING
            this.actorcategory = ActorCategory.friendlyprojectile;
            this.collisionmask = ActorCategory.enemy | ActorCategory.enemyprojectile | ActorCategory.friendlyprojectile | ActorCategory.friendly;
            this.collisionimmunitymask = ActorCategory.enemy;
            this.damageimmunitymask = ActorCategory.enemy | ActorCategory.enemyprojectile | ActorCategory.friendlyprojectile | ActorCategory.friendly;

            textureSet = world.tileEngine.resourceComponent.getTextureSet("008_IceSpike");
        }
        public override void hitWall()
        {
            removeMe = true;
        }
        public override void collision(Actor a)
        {
            a.addImpulse(a.position - this.position);
        }

        public void getHurt(float d)
        {
#if false
            Numbers num = new Numbers(world as GameWorld, position - new Vector2(0f, 0f),
                    this.velocity);
            num.world2model = new Vector2(-3f, -66f);
            num.frictionCoefficient = this.frictionCoefficient ;
            num.imgIndex = ((int)Math.Round(d)) % 10;
            world.addActor(num);

            num = new Numbers(world as GameWorld, position - new Vector2(0f, 0f),
            this.velocity);
            num.world2model = new Vector2(-13f, -66f);
            num.frictionCoefficient = this.frictionCoefficient ;
            num.imgIndex = ((int)Math.Round(d)) / 10;
            world.addActor(num);
            base.getHurt(d);
#endif
        }
    }
}