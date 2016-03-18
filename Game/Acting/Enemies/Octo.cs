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
using Engine.Textures;

namespace Engine.Acting.Enemies
{
    public class OctoBehavior : Behavior
    {

        protected readonly Octo actor;
        protected readonly PathfindBehavior pathFindBehavior;
        protected readonly ShootAttack shootBehavior;

        private float shotcooldown;
        private float shotrate;
        private bool seen = false;
        private Actor target;

        public OctoBehavior(Octo actor, Actor target)
            : base(actor)
        {

            this.actor = actor;
            this.target = target;
            this.pathFindBehavior = new PathfindBehavior(actor, target, .06f);

            this.shootBehavior = new ShootAttack(actor);
            shootBehavior.target = target;
            shootBehavior.speed = 6;
            shootBehavior.projectileId = actor.world.actorFactory.getActorId("SlimeBall");

            actor.octoball.addEndAct(
                (frame) => {
                    shootBehavior.run(); actor.world.tileEngine.audioComponent.playSound(actor.audioSet[1], false); 
                }
            );

            actor.octoball.addEndAct(
                (frame) => {
                    actor.anim.reset(); actor.anim = actor.floatingAnimation; shotcooldown = 100f;
                }
            );

            this.shotcooldown = 160f;
            this.shotrate =  140f;
        }

        public override void run()
        {
            if (seen)
            {
                if ((target.position - actor.position).Length() < 700)
                {

                    pathFindBehavior.run();
                    shotcooldown--;
                    if (shotcooldown <= 0 && pathFindBehavior.canSee && actor.anim != actor.octoball)
                    {
                        // makes shots occur at .5 to 1.5 shotrate
                        

                        // Shoot
                        actor.anim = actor.octoball;
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

    public class Octo : Actor, ILife
    {

        //Move to game constants class
        const bool ACTIVE = true;
        
        public int damage = Constants.OCTO_DAMAGE;
        public Vector2 startPos;
        public Life life { get; private set; }

        public Animation octoball;
        public Animation floatingAnimation;
        public Animation deathAnimation;

        public Octo(GameWorld world, Vector2 position) : 
            base(world, position, new Vector2(0,0), Constants.OCTO_RADIUS, Constants.WORLD2MODEL_OCTO, 0)
        {
            actorName = "Octo";
            startPos = position;
            life = new Life(this, Constants.OCTO_HEALTH);
            life.deathEvent += delegate() { Octo_deathEvent(this); };
            life.lifeChangingEvent += getHurt;

            floatingAnimation = new Animation(0,3, 8f, true,0,-10);
            octoball = new Animation(11, 19, 8f, false, 0, -10);
            deathAnimation = new Animation(4, 10, 10f, false, 0, -20);
            anim = floatingAnimation;
            
            myBehavior = new OctoBehavior(this, world.player);

            active = ACTIVE;
            mass = Constants.OCTO_MASS;

            // MASKING
            this.actorcategory = ActorCategory.enemy;
            this.collisionmask = ActorCategory.enemy | ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("012_Octo");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("012_Octo");
        }

        void Octo_deathEvent(Actor deadActor)
        {

            
            (world as GameWorld).player.killedEnemy();

            if (world.tileEngine.randGen.NextDouble() < 1.2f)
            {
                Pickup.spawn(this, world.actorFactory.getActorId("ManaOrb"), 3);
            }

            if (world.tileEngine.randGen.NextDouble() < .1f)
            {
                Pickup.spawn(this, world.actorFactory.getActorId("HealthOrb"), 3);
            }

            deathAnimation.addEndAct((frame) => {removeMe = true;});
            anim = deathAnimation;
            world.tileEngine.audioComponent.playSound(audioSet[0], false);
        }

        public override void collision(Actor a)
        {
            Life.collisionDamage(this, a, this.damage);
            if (anim != deathAnimation)
            {
                base.collision(a);
            }
        }

        public void getHurt(float oldHealth)
        {

            
                Numbers.spawn(this, (int)(oldHealth - life.life));
            
        }
    }
}
