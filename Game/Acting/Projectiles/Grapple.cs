using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Engine.Tiles;
using System.IO;
using Engine.Textures;

namespace Engine.Acting.Projectiles
{
    public class GrappleBehavior : ProjectileBehavior
    {
        private int duration = Constants.GRAPPLE_DURATION;
        public Actor grabbedActor;
        int reverseTime;
        public GrappleBehavior(Grapple actor)
            : base(actor, 20)
        {
        }

        public override void run()
        {
            actor.imgIndex = Actor.indexFromDirection(actor.velocity, 16, .25f);
            (actor as Grapple).oldervelocity = (actor as Grapple).oldvelocity;
            (actor as Grapple).oldvelocity = (actor as Grapple).velocity;

            duration--;
            if (duration == 0)
            {
                actor.velocity = -1 * actor.velocity;
                //actor.addImpulse(reversed);
            
            }
            if (grabbedActor!=null)
            {
                grabbedActor.active = false;
                grabbedActor.velocity=actor.velocity;
                if (duration > 0)
                   {
                    reverseTime = duration;
                    duration = 1;
                    
                }

            }
            if (duration == -Constants.GRAPPLE_DURATION+reverseTime)
            {

                actor.velocity = Vector2.Zero;
                if (grabbedActor != null)
                {
                    grabbedActor.velocity = Vector2.Zero;
                    //grabbedActor.active = false;

                }
                else
                {
                    (actor as ILife).life.life = 0;
                }

            }

            if (duration <= -Constants.GRAPPLE_DURATION + reverseTime)
            {
                actor.velocity = Vector2.Zero;
            }

            if (duration < -Constants.GRAPPLE_DURATION + reverseTime - Constants.GRAPPLE_STUN_DURATION)
            {
                if (grabbedActor != null)
                grabbedActor.active=true;
                (actor as ILife).life.life = 0;

            }

            //base.run();
        }
        public void grabActor(Actor a)
        {
            grabbedActor = a;
        }

        
    }

    public class Grapple : Actor, ILife
    {
        public Vector2 oldvelocity, oldervelocity;
        public Actor grabbedActor;
        float playerMass;
        public Life life { get; private set; }

        public Grapple(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 4, Constants.WORLD2MODEL_GRAPPLE, 16)
        {
            Animation grappleAnim = new Animation(16, 20, 3f, true, 32, 32);
            anim = grappleAnim;
            myBehavior = new GrappleBehavior(this);
            active = true;
            
            frictionCoefficient = Constants.GRAPPLE_FRICTION;
            elasticity = Constants.GRAPPLE_ELASTICITY;
            mass = Constants.GRAPPLE_MASS;
            

            // Life and death
            life = new Life(this, Constants.GRAPPLE_HEALTH);
            life.deathEvent += delegate() { removeMe = true; };

            this.setGlow(Constants.GRAPPLE_GLOW);

            // MASKING
            this.actorcategory = ActorCategory.friendlyprojectile;
            this.collisionmask = ActorCategory.enemy;
            this.collisionimmunitymask = ActorCategory.friendly;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("005_Arrow");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("021_Grapple");
            grabbedActor = null;
            playerMass = world.player.mass;

        }

        public override void collision(Actor a)
        {
            if (grabbedActor == null )
            {


                if (playerMass > a.mass*0)
                {
                    grabbedActor = a;
                    ((GrappleBehavior)myBehavior).grabActor(a);
                }
            }
           
        }
    }
}
