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
using Engine.Acting.Projectiles;
using Engine.Textures;

namespace Engine.Acting.Enemies
{

    public class BasiliskBehavior : Behavior
    {

        protected readonly Actor actor;
        public Actor target;
        private bool seen;

        PathfindBehavior pathFindBehavior;
        public int lockCooldown;

        //Basilisk Charge
        private int chargeDelay = Constants.BASILISK_CHARGE_DELAY;
        private int acidPitDelay = Constants.BASILISK_ACID_PIT_DELAY;
        private int acidPitCooldown = Constants.BASILISK_ACID_PIT_COOLDOWN;
        private int chargeCooldown = Constants.BASILISK_CHARGE_COOLDOWN;

        //Acid Pit
        private int acidPitDist = Constants.BASILISK_ACID_PIT_DIST;

        public BasiliskBehavior(Actor actor, Actor target)
            : base(actor)
        {

            this.target = target;
            this.actor = actor;
            this.seen = false;
            this.lockCooldown = 0;
            pathFindBehavior = new PathfindBehavior(actor, target, 0.12f);
        }

        public void determineWalkDirection()
        {
            Vector2 dir = actor.velocity;       // Direction to face
            int currentDirection;

            if (Math.Abs(dir.X) > Math.Abs(dir.Y))
            {
                if (dir.X > 0)
                {
                    currentDirection = 0;  // Right
                }
                else
                {
                    currentDirection = 2;  // Left
                }
            }
            else
            {
                if (dir.Y > 0)
                {
                    currentDirection = 1;  // Down
                }
                else
                {
                    currentDirection = 3; // Up
                }

            }
            actor.anim = ((Basilisk)actor).walkingAnimation[currentDirection];
        }

        public override void run()
        {
            lockCooldown--;
            //Follow
            if (seen)
            {
                if (target != null && (target.position - actor.position).Length() < 300.0f)
                {

                    pathFindBehavior.run();
                    if (lockCooldown <= 0)
                    {
                        determineWalkDirection();
                    }
                }

                acidPitCooldown--;
                chargeCooldown--;
                if (target != null && acidPitCooldown <= 0)
                {

                    acidPitCooldown = acidPitDelay;

                    // Fire acid pits
                    Vector2 v = (target.position - (actor.position));
                    v.Normalize();

                    double angle = Math.Atan2((double)v.Y, (double)v.X);

                    for (double i = -Math.PI / 2; i <= Math.PI / 2; i += Math.PI / 12)
                    {

                        double newAngle = angle + i;
                        Vector2 offset = acidPitDist * new Vector2((float)Math.Cos(newAngle), (float)Math.Sin(newAngle));

                        actor.world.addActor(new DeathTileBall((actor.world as GameWorld), actor.position, new Vector2(0f, 0f), target.position + offset));
                    }
                }
                if (chargeCooldown <= 0)
                {
                    // Charge attack
                    Vector2 impulse = (target.position - actor.position);
                    impulse.Normalize();
                    actor.addImpulse(impulse * 500);
                    chargeCooldown = chargeDelay;
                }
            }
            else if (!seen && actor.world.hasLineOfSight(actor.position, target.position, false))
            {
                seen = true;
            }
            else
            {
                seen = false;
            }
        }
    }

    public class Basilisk : Actor, ILife
    {

        public Life life { get; private set; }
        public Animation[] walkingAnimation;
        public Animation deathAnimation;

        public Basilisk(GameWorld world, Vector2 position)
            : base(world, position, new Vector2(0, 0), Constants.BASILISK_SIZE, Constants.WORLD2MODEL_BASILISK, Constants.BASILISK_IMG_INDEX)
        {

            // Initialize general actor variables
            mass = Constants.BASILISK_MASS;
            frictionCoefficient = Constants.BASILISK_FRICTION;

            // Initialize life variables
            life = new Life(this, Constants.BASILISK_HEALTH);
            life.deathEvent += delegate() { Demon_deathEvent(this); };
            life.lifeChangingEvent += getHurt;

            // Initilize animations
            walkingAnimation = new Animation[]{
                new Animation(0, 3, 7f, true, 0, 0),  // Right
                new Animation(4, 7, 7f, true, 0, 0),       // Down
                new Animation(8, 11, 7f, true, 0, 0),  // Left
                new Animation(12, 15, 7f, true, 0, 0),       // Up
            };

            this.deathAnimation = new Animation(0, 0, 1f, false, 0, 0);

            //Initialize behavior
            myBehavior = new BasiliskBehavior(this, world.player);
            active = true;

            // MASKING
            this.actorcategory = ActorCategory.enemy;
            this.collisionmask = ActorCategory.enemy | ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy | ActorCategory.enemyprojectile;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            // Initialize audio and texture sets
            audioSet = world.tileEngine.resourceComponent.getAudioSet("020_FirePillar");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("039_Basilisk");
        }

        void Demon_deathEvent(Actor deadActor)
        {

            // Add explosion for death animation
            deadActor.world.addActor(new Explosion(deadActor.world as GameWorld, deadActor.position, new Vector2(0, 0)));
            (world as GameWorld).player.killedEnemy(0.3f);

            deathAnimation.addEndAct((frame) => { removeMe = true; });
            anim = deathAnimation;

            (world as GameWorld).player.totalEXP += Constants.BASILISK_EXP;

            // Spawn health upon death
            Pickup.spawn(this, world.actorFactory.getActorId("HealthOrb"), 3);
            deadActor.world.tileEngine.audioComponent.playSound(audioSet[0], false);
        }

        public override void collision(Actor a)
        {
            if (anim != deathAnimation)
            {
                Life.collisionDamage(this, a, Constants.BASILISK_DAMAGE);
            }
        }

        public void getHurt(float oldLife)
        {

            
            if (anim != deathAnimation)
            {
                Numbers.spawn(this, (int) (oldLife - life.life));
                (myBehavior as BasiliskBehavior).lockCooldown = 30;
            }

        }
    }
}