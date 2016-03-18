using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;
using System.IO;
using Engine.Tiles;
using Engine.Acting.Projectiles;
using Engine.Acting.Items;
using Engine.Acting.Enemies;
using Engine.Textures;
using Engine.Audio;

namespace Engine.Acting.Bosses
{
    public class MrHammerBehavior : Behavior
    {
        private int currentDirection;
        private Actor actor;
        private Actor target;
        private PathfindBehavior pathFindBehavior;
        private int shotCooldown;
        private int lockCooldown;
        private int chargeCount;
        private int chargeTimeOut;
        private bool seen = false;
        private bool isCharging;

        private Vector2 chargeForce;  // Force to use while charging


        public MrHammerBehavior(Actor actor, Actor target)
            : base(actor)
        {

            this.actor = actor;
            this.target = target;
            this.pathFindBehavior = new PathfindBehavior(actor, target, Constants.MRHAMMER_PATHFIND);
            this.shotCooldown = 0;
            this.lockCooldown = 0;
            this.chargeCount = 480;
            this.chargeTimeOut = 0;
            this.isCharging = false;

            // Define animation actions
            // Hammer attack
            Animation.Action beginSmash = (frame) =>
            {
                actor.world.tileEngine.audioComponent.playSound(actor.audioSet[0], false);

            };

            Animation.Action smash = (frame) =>
            {
                //Spawn explosions
                // Outer Outer Ring
                for (int i = 0; i < 30; i++)
                {
                    Vector2 rotated = Vector2.Transform(new Vector2(0f, 4f), Matrix.CreateRotationZ(MathHelper.ToRadians(12 * i)));
                    actor.world.addActor(new Explosion(actor.world as GameWorld, actor.position + rotated * (actor.size / 3 + 5),
                    rotated * 4f));
                }

                // Outer Ring
                for (int i = 0; i < 18; i++)
                {
                    Vector2 rotated = Vector2.Transform(new Vector2(0f, 3f), Matrix.CreateRotationZ(MathHelper.ToRadians(20 * i)));
                    actor.world.addActor(new Explosion(actor.world as GameWorld, actor.position + rotated * (actor.size / 3 + 5),
                    rotated * 4f));
                }

                // Inner Ring
                for (int i = 0; i < 9; i++)
                {
                    Vector2 rotated = Vector2.Transform(new Vector2(0f, 2f), Matrix.CreateRotationZ(MathHelper.ToRadians(40 * i)));
                    actor.world.addActor(new Explosion(actor.world as GameWorld, actor.position + rotated * (actor.size / 3 + 5),
                    rotated * 4f));
                }


                //Determine which actors are hit
                foreach (Actor a in actor.getConeAround(100, new Vector2(0, 0), 360, null))
                {
                    ILife liveAct = a as ILife;
                    Vector2 impulse = new Vector2();
                    impulse = a.position - this.actor.position;
                    impulse.Normalize();
                    impulse *= Constants.MRHAMMER_KNOCKBACK;
                    
                    if (a.collisionimmunitymask != Actor.ActorCategory.enemy)
                    {
                        if (liveAct != null)
                            liveAct.life.life -= Constants.MRHAMMER_HAMMER_DAMAGE;
                    }
                    a.addImpulse(impulse);
                    
                }

                // Play explosion sound
                AudioSet temp = actor.world.tileEngine.resourceComponent.getAudioSet("020_FirePillar");
                actor.world.tileEngine.audioComponent.playSound(temp[0], false);
            };


            // Charge attack
            Animation.Action castStunWarning = frame =>
            {
                actor.world.addActor(new StunWarning(actor.world as GameWorld, target.position, new Vector2(0, 0)));
            };

            Animation.Action castCircle = frame =>
            {
                // Ring of Icy Stun
                int randomHole = (actor.world as GameWorld).tileEngine.randGen.Next(0, 9);
                
                for (int i = 0; i < 9; i++)
                {
                    // Spawn ice ball unless it's the one being left out
                    Vector2 rotated = Vector2.Transform(new Vector2(0f, 2f), Matrix.CreateRotationZ(MathHelper.ToRadians(40 * i)));
                    if (i != randomHole)
                    {
                        actor.world.addActor(new MagicPrimary(actor.world as GameWorld, target.position + rotated * (actor.size / 3 + 8),
                        rotated * 4f));
                    }
                }

                // Initiate charge
                pathFindBehavior.run();
                isCharging = true;
                chargeForce = actor.force/2;
            };

            // Add actions to the hammer animation
            int animNum = 0;
            foreach (Animation a in ((MrHammer)actor).smashAnimation)
            {
                switch (animNum)
                {
                    case 0:
                        a.addFrameAct(163, smash);
                        break;
                    case 1:
                        a.addFrameAct(85, smash);
                        break;
                    case 2:
                        a.addFrameAct(124, smash);
                        break;
                    case 3:
                        a.addFrameAct(46, smash);
                        break;
                }
                a.addBeginAct(beginSmash);
                animNum++;
            }

            // Add actions to the casting animation
            foreach (Animation a in ((MrHammer)actor).castCircleAnimation)
            {
                a.addBeginAct(castStunWarning);
                a.addEndAct(castCircle);
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
            actor.anim = ((MrHammer)actor).walkingAnimation[currentDirection];

            //ANIMATION SPEED
            if (actor.anim != null)//For now, we apply the same speed to every animation. Eventually we will need to change this for different animations.
            {
                actor.anim.ticksPerFrame = 19 / (float)Math.Sqrt(actor.velocity.Length() * 2 + 1);//These values are calculated experimentally based on what looks good.
            }
        }

        public override void run()
        {
            shotCooldown--;
            chargeCount--;
            chargeTimeOut--;
            if (seen && (actor.world as GameWorld).hammerTriggered)
            {
                // Within follow distance
                if ((target.position - actor.position).Length() < Constants.MRHAMMER_FOLLOWDIST && !isCharging)
                {
                    // Within charge distance
                    if ((target.position - actor.position).Length() < Constants.MRHAMMER_CHARGEDIST && shotCooldown <= 0)
                    {
                        // Within melee distance
                        if ((target.position - actor.position).Length() < Constants.MRHAMMER_ATTACKDIST && shotCooldown <= 0)
                        {
                            // Hammer attack
                            actor.anim = ((MrHammer)actor).smashAnimation[currentDirection];
                            shotCooldown = 40;
                            lockCooldown = 40;
                            actor.world.tileEngine.audioComponent.playSound(actor.audioSet[0], false);
                            ((MrHammer)actor).mass = float.MaxValue;
                        }
                        else
                        {
                            // Spawn stun circle and charge
                            if (chargeCount <= 0 && (target.position - actor.position).Length() > Constants.MRHAMMER_MIN_CHARGE)
                            {
                                actor.anim = ((MrHammer)actor).castCircleAnimation[currentDirection];
                                shotCooldown = 25;
                                lockCooldown = 25;
                                chargeCount = Constants.MRHAMMER_CHARGE_COUNT;
                                chargeTimeOut = Constants.MRHAMMER_CHARGE_TIMEOUT;
                            }
                            else
                            {
                                pathFindBehavior.run();
                            }

                        }
                    }
                    else
                    {
                        pathFindBehavior.run();
                    }
                }

                else if (isCharging)
                {
                    // Check speed of Mr Hammer and stop him from charging
                    if (((MrHammer)actor).velocity.Length() < .4f || chargeTimeOut <= 0)
                    {
                        // Restore normal friction
                        isCharging = false;
                        ((MrHammer)actor).frictionCoefficient = Constants.MRHAMMER_FRICTION;
                    }
                    else
                    {
                        // Lower friction and apply force for charge
                        ((MrHammer)actor).frictionCoefficient = .0001f;
                        actor.force = chargeForce;
                    }
                }
                
                else
                {
                    seen = false;
                }
            }
            else
            {
                if ((target.position - actor.position).Length() < Constants.MRHAMMER_FOLLOWDIST && actor.world.hasLineOfSight(actor.position, target.position, true))
                {
                    seen = true;
                }
            }

            // Reset Mr Hammer's mass to normal and determine walking animation
            if (lockCooldown <= 0)
            {
                if (((MrHammer)actor).mass == float.MaxValue)
                {
                    ((MrHammer)actor).mass = Constants.MRHAMMER_MASS;
                }
                determineDirectionAnimation();
            }
            else
            {
                lockCooldown--;
            }

            //Glow
            int beams = 300;
            float increment = (float)Math.PI * 2 / beams;
            for (float x = 0; x < Math.PI * 2; x += increment)
            {
                actor.world.castRay(((MrHammer)actor).position, new Vector2((float)Math.Cos(x), (float)Math.Sin(x)), new Color(.3f,0,0));
            }
        }
    }

    public class MrHammer : Actor, ILife
    {
        public Animation[] walkingAnimation;
        public Animation[] smashAnimation;
        public Animation[] castCircleAnimation;
        public Life life { get; protected set; }
        public int damage;

        public MrHammer(GameWorld world, Vector2 position, Vector2 velocity)
            : base(world, position, velocity, 45, Constants.WORLD2MODEL_MRHAMMER, 30)
        {

            // Initialize general actor variables
            this.actorName = "Hammer";
            this.damage = Constants.MRHAMMER_DAMAGE;  
            this.mass = Constants.MRHAMMER_MASS;
            this.frictionCoefficient = Constants.MRHAMMER_FRICTION;

            // Initialize life related variables
            life = new Life(this, Constants.MRHAMMER_HEALTH);
            life.lifeChangingEvent += getHurt;
            life.deathEvent += delegate() { MrHammer_deathEvent(this); };

            

            // Initialize animations
            walkingAnimation = new Animation[]{
                new Animation(14, 19, 5f, true, -96, -96),  // Right
                new Animation(0, 3, 5f, true, -64, -96),       // Down
                new Animation(8, 13, 5f, true, -96, -96),  // Left
                new Animation(4, 7, 5f, true, -36, -96),       // Up
            };

            smashAnimation = new Animation[]{
                new Animation(137, 175, 1f, true, -32, -96),  // Right
                new Animation(59, 97, 1f, true, -64, -96),       // Down
                new Animation(98, 136, 1f, true, -96, -96),  // Left
                new Animation(20, 58, 1f, true, -36, -96),       // Up
            };

            castCircleAnimation = new Animation[]{
                new Animation(230, 247, 1f, false, -32, -96),  // Right
                new Animation(194, 211, 1f, false, -64, -96),       // Down
                new Animation(212, 229, 1f, false, -96, -96),  // Left
                new Animation(176, 193, 1f, false, -36, -96),       // Up
            };
            // Initialize Behavior
            myBehavior = new MrHammerBehavior(this, world.player);
            active = true;

            // MASKING
            this.actorcategory = ActorCategory.enemy;
            this.collisionmask = ActorCategory.enemy | ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            // Initialize audio and texture sets
            audioSet = world.tileEngine.resourceComponent.getAudioSet("024_Hammer");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("030_Hammer");
        }

        protected void MrHammer_deathEvent(Actor deadActor)
        {
            // Add explosions for death event
            deadActor.world.addActor(new Explosion(deadActor.world as GameWorld, deadActor.position,new Vector2(0,0)));

            // Outer Ring
            for (int i = 0; i < 5; i++)
            {
                Vector2 rotated = Vector2.Transform(new Vector2(0f, 1f), Matrix.CreateRotationZ(MathHelper.ToRadians(72 * i)));
                deadActor.world.addActor(new Explosion(deadActor.world as GameWorld, deadActor.position + rotated * (deadActor.size / 2 + 5),
                rotated * 4f));
            }
            (world as GameWorld).player.killedEnemy();
            removeMe = true;
            (world as GameWorld).hammerDead = true;

            // Spawn white blob on death
            deadActor.world.addActor(new Blob(deadActor.world as GameWorld, deadActor.position, new Vector2(0,0),1));

            // Manipulate walls for closing in boss room
            for (int i = 0; i < (world as GameWorld).width; i++)
            {
                for (int j = 0; j < (world as GameWorld).height; j++)
                {

                    if ((world as GameWorld).tileArray[i, j].tag % 100 == 11 && (world as GameWorld).tileArray[i, j].action == 0)
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

        public override void collision(Actor a)
        {
            Life.collisionDamage(this, a, this.damage);
        }

        public void getHurt(float oldHealth)
        {
            Numbers.spawn(this, (int)(oldHealth - life.life));
        }
    }
}
