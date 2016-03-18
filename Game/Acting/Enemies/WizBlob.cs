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
    public class WizBlobBehavior : Behavior
    {

        protected readonly WizBlob actor;
        protected readonly PathfindBehavior pathFindBehavior;
        protected readonly ShootAttack shootBehavior;

        private int currentDirection;
        public int lockCooldown;
        public float shotcooldown;
        private float shotrate;
        private bool seen = false;
        private Actor target;

        public WizBlobBehavior(WizBlob actor, Actor target)
            : base(actor)
        {

            this.actor = actor;
            this.target = target;
            this.pathFindBehavior = new PathfindBehavior(actor, target, .1f);

            this.shootBehavior = new ShootAttack(actor);
            shootBehavior.target = target;
            shootBehavior.speed = 1;
            shootBehavior.projectileId = actor.world.actorFactory.getActorId("FireBlob");

            this.shotcooldown = 160f;
            this.lockCooldown = 0;
            this.shotrate = 150f;

            Animation.Action shootFireblob = frame =>
            {
                this.shootBehavior.run();
                actor.anim.reset();
            };

            Animation.Action reset = frame =>
            {
                determineDirectionAnimation();
            };

            int animNum = 0;
            foreach (Animation a in ((WizBlob)actor).shootFireblobAnimation)
            {
                
                switch (animNum)
                {
                    case 0:
                        a.addFrameAct(84, shootFireblob);
                        break;
                    case 1:
                        a.addFrameAct(36, shootFireblob);
                        break;
                    case 2:
                        a.addFrameAct(60, shootFireblob);
                        break;
                    case 3:
                        a.addFrameAct(12, shootFireblob);
                        break;
                }
                //a.addEndAct(reset);
                animNum++;
            }
        }

        private void determineDirectionAnimation()
        {
            Vector2 dir = actor.velocity;       // Direction to face

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
            actor.anim = ((WizBlob)actor).floatingAnimation[currentDirection]; 
        }

        public override void run()
        {
            if (seen)
            {
                if ((target.position - actor.position).Length() < 700)
                {
                    if ((target.position - actor.position).Length() < 150)
                    {
                        actor.frictionCoefficient = .95f;
                    }
                    else
                    {
                        if (actor.frictionCoefficient == .95f)
                        {
                            actor.frictionCoefficient = Constants.WIZBLOB_FRICTION;
                        }
                    }
                    pathFindBehavior.run();
                    shotcooldown--;
                    if (shotcooldown <= 0)
                    {
                        // makes shots occur at .5 to 1.5 shotrate
                        shotcooldown = shotrate;
                        lockCooldown = 24;

                        // Shoot
                        actor.anim = ((WizBlob)actor).shootFireblobAnimation[currentDirection];
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
            // Change direction of floating animation
            if (lockCooldown <= 0)
            {
                determineDirectionAnimation();
            }
            else
            {
                lockCooldown--;
            }
        }

        public override void saveState(System.IO.BinaryWriter writer)
        {
            base.saveState(writer);
            writer.Write(shotcooldown);
            shootBehavior.saveState(writer);
        }

        public override void loadState(System.IO.BinaryReader reader)
        {
            base.loadState(reader);
            shotcooldown = reader.ReadSingle();
            shootBehavior.loadState(reader);
        }
    }

    public class WizBlob : Actor, ILife
    {

        //Move to game constants class
        const bool ACTIVE = true;
        const int EXP = 80;
        const int HEALTH = 120;
        const int MASS = 55;

        public Vector2 startPos;
        public Life life { get; private set; }

        public Animation[] shootFireblobAnimation;
        public Animation[] floatingAnimation;
        public Animation deathAnimation;

        public WizBlob(GameWorld world, Vector2 position) :
            base(world, position, new Vector2(0, 0), 28, Constants.WORLD2MODEL_WIZBLOB, 0)
        {
            actorName = "WizBlob";
            startPos = position;
            life = new Life(this, HEALTH);
            life.deathEvent += delegate() { WizBlob_deathEvent(this); };
            life.lifeChangingEvent += getHurt;

            floatingAnimation = new Animation[]{
                new Animation(174, 197, 2f, true, -80, -64),  // Right
                new Animation(222, 245, 2f, true, -48, -64),       // Down
                new Animation(150, 173, 2f, true, -16, -64),  // Left
                new Animation(198, 221, 2f, true, -16, -64),       // Up
            };

            shootFireblobAnimation = new Animation[]{
                new Animation(72, 95, 1f, false, -46, -64),  // Right
                new Animation(24, 47, 1f, false, -50, -66),       // Down
                new Animation(48, 71, 1f, false, -80, -64),  // Left
                new Animation(0, 23, 1f, false, -48, -76),       // Up
            };

            deathAnimation = new Animation(96, 149, 1.5f, false, -48, -64);
            anim = null;

            myBehavior = new WizBlobBehavior(this, world.player);

            active = ACTIVE;
            mass = MASS;
            this.frictionCoefficient = Constants.WIZBLOB_FRICTION;

            // MASKING
            this.actorcategory = ActorCategory.enemy;
            this.collisionmask = ActorCategory.enemy | ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy | ActorCategory.enemyprojectile;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("012_Octo");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("026_WizBlob");
        }

        void WizBlob_deathEvent(Actor deadActor)
        {

            (world as GameWorld).player.totalEXP += EXP;
            (world as GameWorld).player.killedEnemy();

            if (world.tileEngine.randGen.NextDouble() < 1.2f)
            {
                Pickup.spawn(this, world.actorFactory.getActorId("ManaOrb"), 2);
            }

            if (world.tileEngine.randGen.NextDouble() < .1f)
            {
                Pickup.spawn(this, world.actorFactory.getActorId("HealthOrb"), 1);
            }

            deathAnimation.addEndAct((frame) => { removeMe = true; });
            anim = deathAnimation;
            world.tileEngine.audioComponent.playSound(audioSet[0], false);
        }

        public override void collision(Actor a)
        {

            if (anim != deathAnimation)
            {
                base.collision(a);
            }
        }

        public void getHurt(float oldLife)
        {

            if (anim != deathAnimation)
            {
                Numbers.spawn(this, (int)(oldLife - life.life));
                (myBehavior as WizBlobBehavior).lockCooldown = 20;
            }
        }
    }
}
