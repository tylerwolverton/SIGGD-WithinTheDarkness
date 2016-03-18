using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Engine.Tiles;
using System.IO;

namespace Engine.Acting.Projectiles
{
    public class MagicPrimaryBehavior : ProjectileBehavior
    {
        private Actor frozen1, frozen2;
        private Vector2 oldVelocity1, oldVelocity2;
        private float oldMass1, oldMass2;
        private float oldFriction;
        private int frozenTime1, frozenTime2;
        private int duration;

        public MagicPrimaryBehavior(MagicPrimary actor) : base(actor,20)
        {
            this.duration = 40;
        }

        public override void run()
        {
            if (duration <= 0 && actor.imgIndex == 0)
            {
                (actor as ILife).life.life = 0;
            }
            duration--;

            //Check if one actor has been hit
            if (frozen1 != null)
            {
                //Check if two actors have been hit
                /*if (frozen2 != null)
                {
                    //Make sure actor is frozen every tick
                    frozen2.active = false;
                    //Unfreeze the second enemy and delete ice ball
                    if (frozenTime2 == 0)
                    {
                        //Restore original enemy variables
                        frozen2.active = true;
                        frozen2.velocity = oldVelocity2;
                        frozen2.mass = oldMass2;
                        (actor as ILife).life.life = 0;
                    }
                    
                    frozenTime2--;
                }
                 */
                //Make sure actor is frozen every tick
                //frozen1.active = false;
                //((actor.world as GameWorld).player.myBehavior as PlayerBehavior).lockcooldown = 200;
                //Unfreeze first enemy
                if (frozenTime1 == 0)
                {
                    //Restore original enemy variables
                    //frozen1.active = true;
                    ((actor.world as GameWorld).player.myBehavior as PlayerBehavior).lockcooldown = 0;
                    ((actor.world as GameWorld).player.myBehavior as PlayerBehavior).shotcooldown = 0;
                    ((Player)frozen1).velocity = oldVelocity1;
                    ((Player)frozen1).mass = oldMass1;
                    ((Player)frozen1).frictionCoefficient = oldFriction;

                    //If only one enemy has been hit, delete ice ball
                    if (frozen2 == null)
                    {
                        (actor as ILife).life.life = 0;
                    }
                } 
                frozenTime1--;
            }
            //Glow
            int beams = 100;
            float increment = (float)Math.PI * 2 / beams;
            for (float x = 0; x < Math.PI * 2; x += increment)
            {
                actor.world.castRay(actor.position, new Vector2((float)Math.Cos(x), (float)Math.Sin(x)), new Color(0, 0, 1f));
            }
        }

        //Determine which actors to freeze
        public void freezeActor(Actor a)
        {
            if (frozen1 == null)
            {
                //Save enemy variables
                oldVelocity1 = a.velocity;
                oldMass1 = a.mass;
                oldFriction = a.frictionCoefficient;
                frozen1 = a;
                frozenTime1 = 60;
            }
            else
            {
                //Save enemy variables
                oldVelocity2 = a.velocity;
                oldMass2 = a.mass;
                frozen2 = a;
                frozenTime2 = 180;
            }
        }
    }


    public class MagicPrimary : Actor, ILife
    {
        private int hasCollided = 0;
        public Vector2 oldvelocity, oldervelocity;
        public Actor first;
        public Life life { get; private set; }

        public MagicPrimary(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 12, Constants.WORLD2MODEL_MAGICPRIMARY, 0)
        {

            anim = null;
            myBehavior = new MagicPrimaryBehavior(this);
            active = true;
            frictionCoefficient = Constants.MAGICPRIMARY_FRICTION;
            mass = Constants.MAGICPRIMARY_MASS;

            // Life and death
            life = new Life(this, Constants.MAGICPRIMARY_HEALTH);
            life.deathEvent += delegate() { removeMe = true; };

            this.setGlow(Constants.MAGICPRIMARY_GLOW);

            // MASKING
            this.actorcategory = ActorCategory.enemyprojectile;
            this.collisionmask = ActorCategory.friendly;
            this.collisionimmunitymask = ActorCategory.enemy;
            this.damageimmunitymask = ActorCategory.enemy | ActorCategory.enemyprojectile | ActorCategory.friendlyprojectile;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("005_Arrow");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("025_MagicPrimary");
        }

        public override void hitWall()
        {
            if (hasCollided == 0)
            {
                this.life.life = 0;
            }
            if (hasCollided == 1)
            {
                this.velocity = new Vector2(0, 0);
                this.imgIndex = 1;
                this.collisionmask = 0;
            }
        }

        public override void collision(Actor a)
        {   
            //Don't allow an enemy to be hit twice
            if (a == first || hasCollided == 1 || ((Player)a).mass == float.MaxValue) { return; }
            first = a;              
            hasCollided++;

            //Determine the enemy to freeze
            ((MagicPrimaryBehavior)myBehavior).freezeActor(a);

            //Set behavior false and lock in place
            //a.active = false;
            ((world as GameWorld).player.myBehavior as PlayerBehavior).lockcooldown = 200;
            ((world as GameWorld).player.myBehavior as PlayerBehavior).shotcooldown = 200;
            ((Player)a).mass = float.MaxValue;
            ((Player)a).velocity = new Vector2(0, 0);
            ((Player)a).frictionCoefficient = 1f;

            //Stop ice ball if it collides with a second enemy
            if (hasCollided == 1)
            {
                this.velocity = new Vector2(0,0);
                this.imgIndex = 1;
                this.collisionmask = 0;
            }

            ILife l = a as ILife;
            if (l != null)
            {
                l.life.life -= 0;
            }
        }
    }
}

