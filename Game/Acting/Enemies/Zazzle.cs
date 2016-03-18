using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Tiles;
using Microsoft.Xna.Framework;
using Engine.Acting;
using System.Diagnostics;
using System.IO;
using Engine.Acting.Attacks;
using Engine.Acting;
using Engine.Textures;

namespace Engine.Acting.Enemies
{
    public class ZazzleBehavior : Behavior
    {
        protected readonly Actor actor; 
        protected readonly ShootAttack shootBehavior;
        private bool seen = false;
        private Actor target;
        private float shotcooldown;
        private float shotrate;

        public ZazzleBehavior(Actor actor, Actor target)
            : base(actor)
        {
            this.target = target;
            this.actor = actor;
            

            this.shootBehavior = new ShootAttack(actor);
            shootBehavior.target = target;
            shootBehavior.projectileId = actor.world.actorFactory.getActorId("ZazzleShot");
            shootBehavior.speed = 3;
            
            this.shotcooldown = 160f;
            this.shotrate = 40f;


        }
        public override void run()
        {

            if (seen)
            {
                if ((target.position-actor.position).Length() < 2000)
                {
                    actor.force = -target.velocity;
                        actor.force.Normalize();
                        actor.force *= .1f;
                        
                    
                    
                    
                        shotcooldown--;
                        if (shotcooldown <= 0)
                        {
                            // makes shots occur at .5 to 1.5 shotrate
                            shotcooldown = shotrate;
                            // Shoot
                            shootBehavior.run();
                        }
                    
                }
                else
                {
                    seen = false;
                }
            }
            else
            {
                if ((target.position - actor.position).Length() < 700 && actor.world.hasLineOfSight(actor.position, target.position, true))
                {
                    seen = true;
                }
            }
        }
   }

    public class Zazzle : Actor, ILife
    {
        private int expvalue;
        public static int zazzlesKilled = 0;
        public Life life { get; private set; }

        public Zazzle(GameWorld world, Vector2 position)
            : base(world, position, new Vector2(0, 0), Constants.ZAZZLE_SIZE, Constants.WORLD2MODEL_ZAZZLE, 0)
        {
            actorName = "Zazzle";
            this.expvalue = 40;
            anim = new Animation(0, 10, 8f, true);
            myBehavior = new ZazzleBehavior(this, world.player);
            active = true;
            mass = 30;
            world2model = new Vector2(-32, -32);
            life = new Life(this, 100);
            life.deathEvent += Zazzle_deathEvent;
            life.lifeChangingEvent += getHurt;

            _frictionCoeff = .1f;
            
            frictionCoefficient = 0.1f;
            
            // MASKING
            this.actorcategory = ActorCategory.enemy;
            this.collisionmask = ActorCategory.enemy | ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy | ActorCategory.enemyprojectile;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            textureSet = world.tileEngine.resourceComponent.getTextureSet("016_Zazzle");
        }

        void Zazzle_deathEvent()
        {
            world.addActor(new Acting.Projectiles.Explosion(world as GameWorld, position, new Vector2(0, 0)));
            (world as GameWorld).player.killedEnemy();
            zazzlesKilled++;
            if (zazzlesKilled == 2)
            {
                
                for (int i = 0; i < (world as GameWorld).width; i++)
                {
                    for (int j = 0; j < (world as GameWorld).height; j++)
                    {

                        if ((world as GameWorld).tileArray[i, j].tag % 100 == 12 && (world as GameWorld).tileArray[i, j].action == 0)
                        {
                            if (world.tileArray[i, j].imgIndex == 65)
                            {

                                (world as GameWorld).tileArray[i, j].solid = true;
                                (world as GameWorld).tileArray[i, j].opaque = true;
                            }


                            else
                            {
                                (world as GameWorld).tileArray[i, j].imgIndex = (world as GameWorld).tileArray[i, j].tag / 100;
                                if (world.tileArray[i, j].imgIndex == 64)
                                {

                                    (world as GameWorld).tileArray[i, j].solid = true;
                                    (world as GameWorld).tileArray[i, j].opaque = false;
                                }
                                else
                                {

                                    (world as GameWorld).tileArray[i, j].solid = false;
                                    (world as GameWorld).tileArray[i, j].opaque = false;
                                }
                            }
                        }
                    }
                }


            }

            anim = new Animation(0, 1, 8f, false);
            anim.addEndAct((frame) => { removeMe = true; });

            (world as GameWorld).player.totalEXP += this.expvalue;
            if (world.tileEngine.randGen.NextDouble() < .35)
            {
                Pickup.spawn(this, world.actorFactory.getActorId("HealthOrb"), 3);
            }
        }


        public override void collision(Actor a)
        {
            
                if (!life.isGod)
                {
                    Vector2 dir = new Vector2(a.position.X - this.position.X, a.position.Y - this.position.Y);
                    dir.Normalize();
                    dir *= 250f;
                    a.addImpulse(dir);
                }
                base.collision(a);
            

        }

        public void getHurt(float oldLife)
        {

            if (!life.dead)
            {
                Numbers.spawn(this, (int)(oldLife - life.life));
            }
        }
    }
}