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
using Engine.Acting.Projectiles;
using Engine.Textures;

namespace Engine.Acting.Enemies
{
    public class SentinelBehavior : Behavior
    {
        protected readonly Actor actor;
        protected readonly Behavior pathFindBehavior;
        protected readonly ShootAttack shootBehavior;
        private float shotcooldown;
        private bool seen = false;
        private Actor target;
        public SentinelBehavior(Actor actor, Actor target)
            : base(actor)
        {
            this.actor = actor;
            this.target = target;
            this.pathFindBehavior = new PathfindBehavior(actor, target, 10f);

            this.shootBehavior = new ShootAttack(actor);
            shootBehavior.target = target;
            shootBehavior.projectileId = actor.world.actorFactory.getActorId("Laser");
            shootBehavior.offset = new Vector2(0, -10);
            shootBehavior.radius = 0;

            this.shotcooldown = 160f;
        }

        public override void run()
        {
            if (seen)
            {
                float dist = (target.position-actor.position).Length();
                if (dist < 700)
                {
                    if (dist > 150) {
                        pathFindBehavior.run();
                    }
                    shotcooldown--;
                    if (shotcooldown <= 0)
                    {
                        if (target != null)
                        {
                            // makes shots occur at .5 to 1.5 shotrate
                            shootBehavior.speed = (target.position - actor.position).Length()*.05f;
                            shootBehavior.run();
                            shotcooldown = 20f;
                        }
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

    public class Sentinel : Actor, ILife
    {

        private int expvalue;
        public Life life { get; private set; }

        public Sentinel(GameWorld world, Vector2 position)
            : base(world, position, new Vector2(0, 0), 40, Constants.WORLD2MODEL_SENTINEL, 0)
        {
            actorName = "Sentinel";
            this.expvalue = 5;
           
            anim = new Animation(0, 3, 20f + (float)world.tileEngine.randGen.NextDouble(), true, 0, -10);

            myBehavior = new SentinelBehavior(this, world.player);
            active = true;
            mass = 50;

            life = new Life(this, Constants.SENTINEL_HEALTH);
            life.deathEvent += Sentinel_deathEvent;
            life.lifeChangingEvent += getHurt;

            this.frictionCoefficient = Constants.SENTINEL_FRICTION;

            // MASKING
            this.actorcategory = ActorCategory.enemy;
            this.collisionmask = ActorCategory.enemy | ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("015_Sentinel");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("015_Sentinel");
        }

        void Sentinel_deathEvent()
        {

            (world as GameWorld).player.totalEXP += this.expvalue;
            (world as GameWorld).player.killedEnemy();

            anim = new Animation(4, 9, 10f, false, 0, -20);
            anim.addEndAct((frame) => { removeMe = true; });

            Pickup.spawn(this, world.actorFactory.getActorId("HealthOrb"), 3);            
        }
        public void getHurt(float oldHealth)
        {
            Numbers.spawn(this, (int)(oldHealth - life.life));
                        
            world.tileEngine.audioComponent.playSound(audioSet[1], false);
        }
    }
}