using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Engine.Tiles;
using Engine.Acting;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Engine.Acting.Attacks;
using Engine.Acting.Projectiles;
using Engine.Acting.Items;
using Engine.Acting.Enemies;
using Engine.Input;
using Engine.Textures;
using Engine.Audio;

namespace Engine
{
    public class PlayerBehavior : Behavior
    {
        public enum Stance
        {
            sword,
            bow,
            magic,
        }

        public readonly Player player;

        public int badassPulseCool = Constants.LUCY_PULSE_RATE;
        public SlashAttack swordSlashBehavior;
        public SlashAttack swordChargeBehavior;
        public Predicate<int> swordFrames;
        private int swordSlashingAnimDir;

        public Stance stance;

        //Speed constants.
        private const float SPEED = 2.0f;
        private const float MAX_SPEED = 2.0f; //const correct ALL the consts!

        public float shotcooldown;
        private float shotrate;
        public float lockcooldown;
        public float godcooldown;
        public bool permanentGod = false;
        private bool ischarging;
        private bool isLaser;
        private bool ice;
        private Vector2 lastdirection; //rename these
        private Vector2 savedDirection;
        private Vector2 mouseUnitDir;

        public Vector2 swordDirectionOnClick;
        public Vector2 arrowDirectionOnClick; //Save for arrows -Scott 2-15-12
        public Vector2 magicDirectionOnClick;
        public Vector2 magicPrimaryClick;

        public Vector2 ARROWOFFSET = new Vector2(0, -24);
        public Vector2 MAGICOFFSET = new Vector2(0, -10);

        // Input Variables
        private SinglePressBinding cycleleft, cycleright;
        private SinglePressBinding[] stanceSelect;
        private SinglePressBinding primaryattack, secondaryattack, /*godMode,*/ testSkill1 /*noClip*/;
        private AxisBinding xmove, ymove, xaim, yaim;

        // Attack Behaviors
        private ShootAttack arrowShot;
        private ShootAttack laserShot;
        private ShootAttack fireTrapShot;
        private ShootAttack magicPrimary;

        //Assassinate things
        private int assCharges;
        private Actor assTarget;
        private int assTicks;
        private int assFrame;
        private bool assGod;
        private Vector2 assSpeed;
        private int assCooldown;
        public bool assPermission;

        public PlayerBehavior(Player player)
            : base(player)
        {

            this.player = player;
            this.shotcooldown = 0f;
            this.shotrate = 20f;
            this.stance = Stance.sword;
            this.ischarging = false;

            // Initialize shoot behavior
            arrowShot = new ShootAttack(player);
            arrowShot.projectileId = player.world.actorFactory.getActorId("Arrow");
            arrowShot.speed = Constants.ARROW_SPEED;
            arrowShot.radius = 0;
            laserShot = new ShootAttack(player);
            laserShot.projectileId = player.world.actorFactory.getActorId("LaserArrow");
            laserShot.speed = Constants.ARROW_SPEED;
            laserShot.radius = 0;

            //Fire Trap
            fireTrapShot = new ShootAttack(player);
            fireTrapShot.radius = 0;
            fireTrapShot.projectileId = player.world.actorFactory.getActorId("FireShuriken");
            fireTrapShot.speed = Constants.FIRESHURIKEN_SPEED;
            //Primary Magic
            magicPrimary = new ShootAttack(player);
            magicPrimary.projectileId = player.world.actorFactory.getActorId("FireBlob");
            magicPrimary.speed = Constants.WIZBLOB_FIREBLOB_SPEED;
            magicPrimary.radius = 0;

            //ASS
            assPermission = false;
            ((player.world.tileEngine.inputComponent as GameInput)[GameInput.PlayBindings.ASSSTANCE] as SinglePressBinding).downEvent += assFlip;

            // Initialize Animation Actions

            //Step to Animation binding
            Predicate<int> stepFrames = (frame) =>
            {
                return (frame % 3) == 1;
            };
            Animation.Action stepsound = delegate(int frame)
            {
                player.world.tileEngine.audioComponent.playSound(player.audioSet[3], false);
            };

            //Sword Behavior
            swordSlashBehavior = new SlashAttack(this.player, player.power, 75, 60, 0, 2, false);

            swordFrames = (frame) =>
            {
           
                return (player.anim.curFrame == (player.anim.frameEnd + player.anim.frameBegin) / 2
                || (ischarging && (player.anim.curFrame == player.anim.frameEnd || player.anim.curFrame % 4 == 1)) && player.anim.isNewFrame);
            };

            // Bow attack actions
            Animation.Action bowBeginAct = (frame) =>
            {

                //                      //Player position on screen
                //                      //          \/        //<-    Arrow offset      -       -       -       -       ->
                Vector2 bowPosition = AxisBinding.origin + (savedDirection * (player.size / 2 + 9) + ARROWOFFSET);
                MouseState mouseState = player.world.tileEngine.inputComponent.currentMouseState;
                Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
                arrowDirectionOnClick = mousePosition - bowPosition;
                arrowDirectionOnClick.Normalize();
            };

            Animation.Action bowAct = (frame) =>
            {

                player.world.tileEngine.audioComponent.playSound(player.audioSet[5], false);

                Vector2 offset = savedDirection * (player.size / 2 + 9) + ARROWOFFSET;
                int maxOffset;
                for (maxOffset=0; maxOffset<21; maxOffset+=3)
                {
                    if (!player.world.isAreaClear(new RectangleF(player.position.X + offset.X, player.position.Y + offset.Y, player.size / 2, player.size / 2)))
                        offset.Y += 3;
                    else
                        break;
                }

                //Primary attack
                if (!isLaser)
                {
                    arrowShot.offset = offset;
                    arrowShot.direction = arrowDirectionOnClick;
                    arrowShot.run();

                //Secondary attack
                }else
                {
                    laserShot.offset = offset;
                    laserShot.direction = arrowDirectionOnClick;
                    laserShot.run();
                }
            };


            // Magic attack animation actions
            Animation.Action magicBeginAct = (frame) =>
            {
                // Determine Direction in which to Shoot
                Vector2 handPosition = AxisBinding.origin + (savedDirection * (player.size / 2 + 9) + MAGICOFFSET);
                MouseState mouseState = player.world.tileEngine.inputComponent.currentMouseState;
                Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
                magicDirectionOnClick = mousePosition - handPosition;
                magicDirectionOnClick.Normalize();

                float scale = player.world.tileEngine.graphicsComponent.camera.scale - player.world.tileEngine.graphicsComponent.camera.scaleChange;
                
                Vector2 mousePrimaryPosition = new Vector2(mouseState.X / scale, (mouseState.Y) / scale);
                magicPrimaryClick = mousePrimaryPosition + player.position - AxisBinding.origin/scale;
            };


            Animation.Action magicAttack = (frame) =>
            {
                if (ice)
                {
                    //Spawn ice spikes
                    player.world.addActor(new IceSpike(player.world as GameWorld, player.position + magicDirectionOnClick * (player.size / 2 + 30) + MAGICOFFSET,
                    magicDirectionOnClick * 4f));
                    Vector2 rotated = Vector2.Transform(magicDirectionOnClick, Matrix.CreateRotationZ(MathHelper.ToRadians(40)));
                    player.world.addActor(new IceSpike(player.world as GameWorld, player.position + rotated * (player.size / 2 + 40) + MAGICOFFSET,
                    rotated * 4f));
                    rotated = Vector2.Transform(magicDirectionOnClick, Matrix.CreateRotationZ(MathHelper.ToRadians(-40)));
                    player.world.addActor(new IceSpike(player.world as GameWorld, player.position + rotated * (player.size / 2 + 40) + MAGICOFFSET,
                    rotated * 4f));
                    rotated = Vector2.Transform(magicDirectionOnClick, Matrix.CreateRotationZ(MathHelper.ToRadians(20)));
                    player.world.addActor(new IceSpike(player.world as GameWorld, player.position + rotated * (player.size / 2 + 35) + MAGICOFFSET,
                    rotated * 4f));
                    rotated = Vector2.Transform(magicDirectionOnClick, Matrix.CreateRotationZ(MathHelper.ToRadians(-20)));
                    player.world.addActor(new IceSpike(player.world as GameWorld, player.position + rotated * (player.size / 2 + 35) + MAGICOFFSET,
                    rotated * 4f));
                }
                else
                {
                    player.world.addActor(new LucyFirePillar(player.world as GameWorld, magicPrimaryClick, magicDirectionOnClick));                 
                }
            };


            Animation.Action magicReset = (frame) =>
            {
                lockcooldown = 0;
                shotcooldown = 0;
            };

            Animation.Action gigaBurst = (frame) =>
            {
                // Spawn Explosions
                // Spawn an Explosion on Lucy
                player.world.addActor(new Explosion(player.world as GameWorld, player.position, new Vector2(0, 0)));

                // Outer Outer Ring
                for (int i = 0; i < 18; i++)
                {
                    Vector2 rotated = Vector2.Transform(new Vector2(0f, 3f), Matrix.CreateRotationZ(MathHelper.ToRadians(20 * i)));
                    player.world.addActor(new Explosion(player.world as GameWorld, player.position + rotated * (player.size / 2 + 5),
                    rotated * 4f));
                }

                // Outer Ring
                for (int i = 0; i < 9; i++)
                {
                    Vector2 rotated = Vector2.Transform(new Vector2(0f, 2f), Matrix.CreateRotationZ(MathHelper.ToRadians(40 * i)));
                    player.world.addActor(new Explosion(player.world as GameWorld, player.position + rotated * (player.size / 2 + 5),
                    rotated * 4f));
                }

                // Inner Ring
                for (int i = 0; i < 3; i++)
                {
                    Vector2 rotated = Vector2.Transform(new Vector2(0f, 1f), Matrix.CreateRotationZ(MathHelper.ToRadians(120 * i)));
                    player.world.addActor(new Explosion(player.world as GameWorld, player.position + rotated * (player.size / 2 + 5),
                    rotated * 4f));
                }
                
                //Determine which actors are hit
                foreach (Actor a in player.getConeAround(80, mouseUnitDir, 360, null))
                {
                    ILife liveAct = a as ILife;
                    Vector2 impulse = new Vector2();
                    impulse = a.position - this.player.position;
                    impulse.Normalize();
                    impulse *= Constants.GIGABURST_KNOCKBACK;

                    if (a.actorcategory != Actor.ActorCategory.friendly)
                    {
                        if (liveAct != null)
                            liveAct.life.life -= Constants.GIGABURST_DAMAGE;
                    }

                    a.addImpulse(impulse);
                }
                
                // Play explosion sound
                AudioSet temp = player.world.tileEngine.resourceComponent.getAudioSet("020_FirePillar");
                player.world.tileEngine.audioComponent.playSound(temp[0], false);
            };

            // Assign animation actions
            Animation[] anims;   // Animation temporary, for great justice!

            // Bow
            anims = player.attack[(int)Stance.bow];
            foreach (Animation a in anims)
            {
                a.addBeginAct(bowBeginAct);
                a.addEndAct(bowAct);
            }

            // Magic
            // First Half Animation
            anims = player.attack[(int)Stance.magic];
            int animNum = 0;
            foreach (Animation a in anims)
            {
                a.addBeginAct(magicBeginAct);
                switch(animNum){
                    case 0:
                        a.addFrameAct(140,magicAttack);
                        break;
                    case 1:
                        a.addFrameAct(128,magicAttack);
                        break;
                    case 2:
                        a.addFrameAct(134,magicAttack);
                        break;
                    case 3:
                        a.addFrameAct(146,magicAttack);
                        break;
                }
                a.addEndAct(magicReset);
                animNum++;
            }

            // Giga Burst
            anims = player.gigaBurstAnim;
            foreach (Animation a in anims)
            {
                a.addEndAct(gigaBurst);
            }
            
            // Step sound
            foreach (Animation[] a in player.running)
            {
                foreach (Animation aa in a)
                {
                    aa.addPredAct(stepFrames, stepsound);
                }
            }
        }

        private void determineStanceAnimation()
        {
            Vector2 dir = Vector2.Zero;       // Direction to face
            Animation[] anim = null;          // Type of animation to display
            if (player.velocity.Length() <= .4f)
            {
                dir = lastdirection;
                anim = player.standing[(int)stance];
            }
            else
            {
                dir = player.velocity;
                anim = player.running[(int)stance];
            }

            if (Math.Abs(dir.X) > Math.Abs(dir.Y))
            {
                if (dir.X > 0)
                {
                    player.anim = anim[0];  // Right
                }
                else
                {
                    player.anim = anim[2];  // Left
                }
            }
            else
            {
                if (dir.Y > 0)
                {
                    player.anim = anim[1];  // Down
                }
                else
                {
                    player.anim = anim[3];  // Up
                }

            }
            if (player.velocity.Length() > .4f)
                lastdirection = player.velocity;

            //ANIMATION SPEED
            if (player.anim != null)//For now, we apply the same speed to every animation. Eventually we will need to change this for different animations.
            {
                player.anim.ticksPerFrame = 19 / (float)Math.Sqrt(player.velocity.Length() * 2 + 1);//These values are calculated experimentally based on what looks good.
            }
        }

        //Function to determine animation when attacking
        private Animation animationAttackChange(int frame, Stance stance)
        {
            return player.attack[(int)stance][frame];
        }

        public void assFlip()
        {
            if (player.isBadass&&!assPermission)
            {
                assPermission = true;
                stance = Engine.PlayerBehavior.Stance.sword;
                //player._badassMeter = 1.0f;
                player._mana = 0;
            }
            else if (assPermission)
            {
                assPermission = false;
                player._badassMeter = 0.0f;
                player.isBadass = false;
            }
        }

        public void assassinate()
        {
            //Assassinate
            if (shotcooldown < 0)
            {
                shotcooldown = 50;
                assCharges = 1;
                assGod = true;
                godcooldown = 20;
            }

            if (assTicks <= 0 && assCharges > 0 && assCooldown <= 0)
            {

                MouseState mouseState = player.world.tileEngine.inputComponent.currentMouseState;
                float scale = player.world.tileEngine.graphicsComponent.camera.scale - player.world.tileEngine.graphicsComponent.camera.scaleChange;

                Vector2 mousePrimaryPosition = new Vector2(mouseState.X / scale, (mouseState.Y) / scale);
                
                Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y + player.size / 2) - AxisBinding.origin;
                Vector2 clickPosition = mousePrimaryPosition + player.position - AxisBinding.origin / scale;
                float minDistance = 0;
                assTarget = null;
                foreach (Actor a in player.world.getConeAroundPos(clickPosition,50, Vector2.Zero, 360, player))
                {
                    if (a.actorcategory == Actor.ActorCategory.enemy)
                        if (assTarget == null || (a.position - player.position).Length() < minDistance)
                        {
                            
                            assTarget = a;
                            minDistance = (a.position - player.position).Length();
                        }

                }

                if (assTarget == null)
                {
                    //assCharges = 0;
                    shotcooldown = 25;
                    assGod = false;
                }
                else
                {
                    if (player.world.hasLineOfSight(new Rectangle((int)(player.position.X - player.size / 2), (int)(player.position.Y - player.size / 2), (int)player.width, (int)player.height),
                    new Rectangle((int)(assTarget.position.X - assTarget.size / 2), (int)(assTarget.position.Y - assTarget.size / 2), (int)assTarget.width, (int)assTarget.height)) 
                    && (player.position - assTarget.position).Length() < Constants.ASSASSINATE_DISTANCE
                       )
                    {
                        assTicks = Constants.ASSASSINATE_FRAMES_TO_TARGET;
                        assCharges--;
                        assSpeed = (assTarget.position - player.position) / (assTicks - 12);
                        assFrame = Actor.indexFromDirection(mouseUnitDir, 4, 0.5f);
                        //player.anim = player.assassAnim[frame];
                        assCooldown = Constants.ASSASSINATE_TIME_BETWEEN_DASHES;
                    }
                    else
                    {
                        assCharges = 0;
                        shotcooldown = 25;
                        assGod = false;
                    }
                }
            }
        }

        //Events for mouse clicks
        public void primaryDownEvent()
        {
           
            if (player.isBadass && assPermission)
            {
                assassinate();
                return;
            }

            //Primary sword attack
            if (shotcooldown < 0 && player.mana >= Constants.LUCY_SWORD_COST && stance == Stance.sword)
            {

                //Set Animation and Behavior
                //swordSlashingAnimDir = Actor.indexFromDirection(mouseUnitDir, 4, 0.5f);
                //swordSlashingDir = lastdirection;


                Vector2 direction = mouseUnitDir;
                player.force = direction / 4;
                player.mana -= Constants.LUCY_SWORD_COST;

                player.world.tileEngine.audioComponent.playSound(player.audioSet[player.world.tileEngine.randGen.Next(6, 9)], false);

                // set cooldowns
                shotcooldown = 15;
                lockcooldown = 15;
                
                lastdirection = mouseUnitDir;

                //Set Animation and Behavior
                swordSlashingAnimDir = Actor.indexFromDirection(mouseUnitDir, 4, 0.5f);
                Animation temp = animationAttackChange(swordSlashingAnimDir, stance);
                swordSlashBehavior.setAnimation(lastdirection, temp, swordFrames);
                /*
                // Spawn Sword Slash
                Vector2 swordPosition = AxisBinding.origin + (savedDirection * (player.size / 2 + 9) + ARROWOFFSET);
                MouseState mouseState = player.world.tileEngine.inputComponent.currentMouseState;
                Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
                swordDirectionOnClick = mousePosition - swordPosition;
                swordDirectionOnClick.Normalize();

                player.world.addActor(new SwordSlash(player.world as GameWorld, player.position + swordDirectionOnClick * (player.size / 2 + 30) + MAGICOFFSET,
                    swordDirectionOnClick));
                */
            }

            //Primary bow attack
            if (shotcooldown < 0 && player.mana >= Constants.LUCY_ARROW_COST && stance == Stance.bow)
            {

                player.world.tileEngine.audioComponent.playSound(player.audioSet[5], false);
                player.mana -= Constants.LUCY_ARROW_COST;
                if (player.regenWait < 10)
                    player.regenWait = 10;

                savedDirection = mouseUnitDir;

                lockcooldown = shotrate;
                shotcooldown = shotrate;// *2;

                //Determine which animation to use
                int frame = Actor.indexFromDirection(mouseUnitDir, 4, 0.5f);
                player.anim = animationAttackChange(frame, stance);
                isLaser = false;
            }

            //Primary magic attack
            if (shotcooldown < 0 && player.mana >= Constants.LUCY_SHURIKEN_COST && stance == Stance.magic && player.world.hasLineOfSight((player.world as GameWorld).player.position, player.world.getMouseWorldVector(), false))
            {
                //Play magic sound
                player.world.tileEngine.audioComponent.playSound(player.audioSet[4], false);
                player.mana -= Constants.LUCY_SHURIKEN_COST;
                if (player.regenWait < 20)
                    player.regenWait = 20;

                lastdirection = mouseUnitDir;
                savedDirection = mouseUnitDir;
                shotcooldown = 28;
                lockcooldown = 28;
                int frame = Actor.indexFromDirection(mouseUnitDir, 4, 0.5f);
                player.anim = animationAttackChange(frame, stance);
                ice = false;
            }
        }

        public void primaryUpEvent()
        {

            swordSlashBehavior.active = false;
        }

        public void secondaryDownEvent()
        {

            if (player.isBadass && assPermission)
            {
                assassinate();
                return;
            }

            //Giga Burst
            if (shotcooldown < 0 && player.mana >= Constants.LUCY_GIGABURST_COST && stance == Stance.sword)
            {
                shotcooldown = 25;
                lockcooldown = 25;
                player.mana -= Constants.LUCY_GIGABURST_COST;
                if (player.regenWait < 55)
                    player.regenWait = 55;

                int frame = Actor.indexFromDirection(mouseUnitDir, 4, 0.5f);
                player.anim = player.gigaBurstAnim[frame];
            }

            //Secondary bow attack
            if (shotcooldown < 0 && player.mana >= Constants.LUCY_LASER_COST && stance == Stance.bow)
            {

                player.world.tileEngine.audioComponent.playSound(player.audioSet[5], false);

                Vector2 direction = mouseUnitDir;
                savedDirection = mouseUnitDir;
                player.mana -= Constants.LUCY_LASER_COST;
                if (player.regenWait < 55)
                    player.regenWait = 55;

                // makes shots occur at .5 to 1.5 shotrate
                shotcooldown = 20;
                lockcooldown = 20;
                godcooldown = 30;
                lastdirection = mouseUnitDir;

                //Determine which animation to use
                int frame = Actor.indexFromDirection(mouseUnitDir, 4, 0.5f);
                player.anim = animationAttackChange(frame, stance);
                isLaser = true;
            }

            //Secondary magic attack
            if (shotcooldown < 0 && Constants.LUCY_ICE_COST >= 40 && stance == Stance.magic)
            {
                //Play magic sound
                player.world.tileEngine.audioComponent.playSound(player.audioSet[4], false);

                lastdirection = mouseUnitDir;
                savedDirection = mouseUnitDir;
                player.mana -= Constants.LUCY_ICE_COST;
                if (player.regenWait < 55)
                    player.regenWait = 55;

                // makes shots occur at .5 to 1.5 shotrate
                shotcooldown = 20;
                lockcooldown = 20;

                //Determine animation
                Debug.WriteLine(mouseUnitDir.X);
                Debug.WriteLine(mouseUnitDir.Y);
                int frame = Actor.indexFromDirection(mouseUnitDir, 4, 0.5f);
                player.anim = animationAttackChange(frame, stance);
                ice = true;
            }
        }

        public void secondaryUpEvent()
        {
        }

        public void inputInit()
        {
            GameInput input = player.world.tileEngine.inputComponent as GameInput;
            player.rumble();
            xmove = input[GameInput.PlayBindings.XMOVE] as AxisBinding;
            ymove = input[GameInput.PlayBindings.YMOVE] as AxisBinding;
            xaim = input[GameInput.PlayBindings.XAIM] as AxisBinding;
            yaim = input[GameInput.PlayBindings.YAIM] as AxisBinding;
            primaryattack = input[GameInput.PlayBindings.PRIMARYATTACK] as SinglePressBinding;
            secondaryattack = input[GameInput.PlayBindings.SECONDARYATTACK] as SinglePressBinding;
            stanceSelect = new SinglePressBinding[] {
                input[GameInput.PlayBindings.SWORDSTANCE] as SinglePressBinding,
                input[GameInput.PlayBindings.BOWSTANCE] as SinglePressBinding,
                input[GameInput.PlayBindings.MAGICSTANCE] as SinglePressBinding,
            };
            cycleleft = input[GameInput.PlayBindings.LCYCLESTANCE] as SinglePressBinding;
            cycleright = input[GameInput.PlayBindings.RCYCLESTANCE] as SinglePressBinding;

            testSkill1 = input[GameInput.PlayBindings.TESTSKILL1] as SinglePressBinding;
            //godMode = input[GameInput.PlayBindings.GODMODE] as SinglePressBinding;
            //noClip = input[GameInput.PlayBindings.NOCLIP] as SinglePressBinding;

            primaryattack.downEvent += primaryDownEvent;
            primaryattack.upEvent += primaryUpEvent;
            secondaryattack.downEvent += secondaryDownEvent;
            secondaryattack.upEvent += secondaryUpEvent;
        }



        public override void run()
        {

            //Handling Input is in a separate function.
            inputInit();

            // Regenerate mana
            if (player.regenWait == 0)
            {
                player.mana += Constants.LUCY_MANAREGEN;
            }
            else
            {
                player.regenWait--;
            }
            //INPUT RESPONSE SECTION
            //player.frictionCoefficient = 0.12f; //decomment for super slidy mode - used to test animations based on velocity, not input
            if (lockcooldown >= 0)
            {
                //if (!isSlashing)
                lockcooldown--;
            }
            if (godcooldown >= 0)
            {
                godcooldown--;
            }

            // Establish godhood
            player.life.isGod = godcooldown > 0;

            // Determine mouse input
            Vector2 mouseDir = new Vector2(xaim.position, -yaim.position);
            if (mouseDir == Vector2.Zero)
                mouseDir = lastdirection;
            mouseUnitDir = Vector2.Normalize(mouseDir);



            //////////////////////////////////
            // Determine player movement force
            //////////////////////////////////
            //player.force = Vector2.Zero;
            if (lockcooldown <= 0)
            {
                player.force = new Vector2(xmove.position, -ymove.position);
                player.force *= SPEED;
            }
            if (player.force.LengthSquared() > MAX_SPEED)
            {
                player.force = Vector2.Normalize(player.force);
                player.force *= MAX_SPEED;
            }

            player.force *= .7f; // scale by speed. ? what is this random scaling?

            // Handle stance change selection
            for (int i = 0; i < stanceSelect.Length; i++)
            {
                if (stanceSelect[i].isPressed)
                     stance = (Stance)i;
            }

            //ANIMATION CHANGE SECTION
            if (lockcooldown < 0 && (assTicks <= 0 || assTicks>15))
            {
                determineStanceAnimation();
            }

            //Handle change stance wraparound
            shotcooldown--;
            if (shotcooldown < 0 && !(assPermission))
            {
                if (cycleleft.isPressed)
                {
                    shotcooldown = 10;
                    stance++;
                    if (stance > Stance.magic)
                    {
                        stance = Stance.sword;
                    }
                }
                if (cycleright.isPressed)
                {
                    shotcooldown = 10;
                    stance--;
                    if (stance < Stance.sword)
                    {
                        stance = Stance.magic;
                    }
                }
            }

            swordSlashBehavior.run();

            ///////////////// New Skills /////////////////

            /*if (shotcooldown < 0 && testSkill1.isPressed && player.mana >= Constants.GRAPPLE_COOLDOWN && stance == Stance.bow) //Set arrowDirectionOnClick on click and shoot grapple on animation end!
            {
                int frame = Actor.indexFromDirection(mouseUnitDir, 4, 0.5f);
                //player.anim = animationAttackChange(frame, stance);
                savedDirection = mouseUnitDir;
                player.mana -= Constants.GRAPPLE_COOLDOWN;
                shotcooldown = 20;
                lockcooldown = 40;
                Vector2 bowPosition = AxisBinding.origin + (savedDirection * (player.size / 2 + 9) + ARROWOFFSET);
                MouseState mouseState = player.world.tileEngine.inputComponent.currentMouseState;
                Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
                arrowDirectionOnClick = mousePosition - bowPosition;
                arrowDirectionOnClick.Normalize();
                Grapple grappleHook = new Grapple(player.world as GameWorld, player.position + savedDirection * (player.size / 2 + 20) - new Vector2(0, 24f), arrowDirectionOnClick * Constants.GRAPPLE_SPEED);//!!!
                player.world.addActor(grappleHook);
            }*/

            
            if (assCooldown > 0)
            {
                assCooldown--;
            }
            if (assTicks > 0)
            {
                godcooldown = 40;
                if (assTicks == 15)
                {
                    player.anim = player.assassAnim[assFrame];
                    player.trail(true, 3);
                }
                if (assTicks > Constants.ASSASSINATE_FRAMES_TO_TARGET - 8)
                    assTicks--;
                else if (player.world.isAreaClear(new RectangleF((player.position.X+assSpeed.X - player.size / 2), (player.position.Y+assSpeed.Y - player.size / 2), player.width, player.height)))
                {
                    player.position += assSpeed;
                    if (assTicks != 1) 
                    player.velocity = Vector2.Zero;
                    assTicks--;
                }
                else
                {
                    assTicks = 0;
                    player.trail(false, 0);
                }
                if (assTicks == 0)
                {
                    //determineStanceAnimation();
                    //player.velocity = Vector2.Zero;
                    if(!(assTarget is Acting.Bosses.MrHammer) && !(assTarget is Acting.Bosses.FirstBoss) && !(assTarget is Acting.Bosses.GigaBlob))
                    (assTarget as ILife).life.life = 0;
                    else
                        (assTarget as ILife).life.life -= .10f*(assTarget as ILife).life.maxlife;
                    player.badassMeter -= 0.5f;
                    player.trail(false, 0);
                        shotcooldown = 25;
                        lockcooldown = 20;
                        assGod = false;
                        //godcooldown = 30;         
                    

                }

            }

           // if (shotcooldown < 0 && godMode.isPressed)
            //{
           //    permanentGod = !permanentGod;
            //    shotcooldown = 15;

           // }

            //if (shotcooldown < 0 && noClip.isPressed)
           // {
            //    player.ignoreAvE = !player.ignoreAvE;
           //    shotcooldown = 15;

           // }

            if (assGod)
            {
                player.life.isGod = true;
            }


            // GLOWING
            int beams = 250;
            float increment = (float)Math.PI * 2 / beams;
            World.ModifyTile mod = delegate(Tile tile) {


                if (player.isBadass && badassPulseCool < 4)
                {
                    tile.changeGlow(0, 0, 0.012f);
                }
                else
                {
                    tile.changeGlow(0.009f, 0.009f, 0.009f);
                }
                tile.val+=0.01f;
                
                if (tile.val > 1.0f) tile.val = 1.0f;
            };

#if false
            player.world.castLight(player.position);
#else
            // using the same vectors every time caused hard-edges / pixelation on tiles.
            // chooses a random starting point so using different rays.
            float offset = (float)player.world.tileEngine.randGen.NextDouble()*2*(float)Math.PI;
            for (float x = 0; x < Math.PI * 2; x += increment)
            {
                Vector2 dir = new Vector2((float)Math.Cos(x+offset), (float)Math.Sin(x+offset));
                // Tile under player was getting all rays applied to it, while each adjacent
                // tile got 1/4 of the rays applied to them (approx), so by pushing center to
                // the adjacent tile 1/4 of the time the tile under the player shouldn't be
                // disproportionately bright.
                if(x>Math.PI*0.5f)
                    player.world.castRay(player.position + Tile.size * dir, dir, mod);
                else
                    player.world.castRay(player.position, dir, mod);
            }

            if (badassPulseCool == 0)
                badassPulseCool = Constants.LUCY_PULSE_RATE;
            if (player.isBadass)
                badassPulseCool--;
#endif
        }

        //State saving
        public override void saveState(System.IO.BinaryWriter writer)
        {
            base.saveState(writer);
            writer.Write(shotcooldown);
            writer.Write(shotrate);
            writer.Write(lockcooldown);
            writer.Write(godcooldown);
            writer.Write(ischarging);
            writer.Write(isLaser);
            writer.Write(ice);
        }

        public override void loadState(System.IO.BinaryReader reader)
        {
            base.loadState(reader);
            shotcooldown = reader.ReadSingle();
            shotrate = reader.ReadSingle();
            lockcooldown = reader.ReadSingle();
            godcooldown = reader.ReadSingle();
            ischarging = reader.ReadBoolean();
            isLaser = reader.ReadBoolean();
            ice = reader.ReadBoolean();
        }
    }



    public class Player : Actor, ILife
    {
        public Animation[][] running;
        public Animation[][] standing;
        public Animation[][] attack;
        public Animation[][] magicAttack;
        public Animation[] gigaBurstAnim;
        public Animation[] assassAnim;
        public Animation[] getHurtAnimation;

        public Animation deathAnimation;
        public int regenWait = 0;
        public int power { get; set; }
        public int level { get; set; }
        public int skillPoints { get; set; }
        public int talentPoints { get; set; }

        public int killCount = 0;

        public PlayerBehavior pb;
        private int _totalEXP;
        public int totalEXP
        {
            get
            {
                return _totalEXP;
            }
            set
            {
                _totalEXP = value;
                
            }
        }

        //Badass
        public bool isBadass {set; get; }

        public float _badassMeter;
        public float badassMeter
        {
            set
            {

                if (isBadass && value > _badassMeter) return;
               
                if (value < 0.0f)
                {
                    isBadass = false;
                    (myBehavior as PlayerBehavior).assPermission = false;
                    _badassMeter = 0.0f;
                }
                else if (value > 1.0f)
                {
                    isBadass = true;                    
                }
                else _badassMeter = value;

                if (_badassMeter > (life.life / life.maxlife))
                {
                    this.life.life = _badassMeter * life.maxlife;
                }
            }

            get { return _badassMeter; }
        }

        public float _mana; //NEVER ACCESS THIS ONLY CHANGED TO PUBLIC FOR ASSASSINATE
        //public float skillCoolDownMax;

        public float mana
        {
            get { return _mana; }

            set
            {
                if ((myBehavior as PlayerBehavior)!=null && !(myBehavior as PlayerBehavior).assPermission)
                {
                    _mana = (value > 0) ? value : 0;
                    _mana = (value <= Constants.LUCY_MAXMANA) ? value : Constants.LUCY_MAXMANA;
                }
            }
        }

        public bool isManaFull()
        {
            return mana == Constants.LUCY_MAXMANA;
        }

        public Life _life;
        public Life life
        {
            get { return _life; }
        }

        // Most recent checkpoint the player has reached
        public Checkpoint checkpoint;

        // Saved collision mask (for respawn)
        ActorCategory savedColMask;

        public Player(World world, Vector2 position)
            : base(world, position, new Vector2(0, 0), 28, Constants.WORLD2MODEL_PLAYER , 0) // position bounding box at center of players feet
        {

            checkpoint = new Checkpoint(-1, position);

            //isBadass = true;
            //_badassMeter = 1.0f;

            _life = new Life(this, 100);

            // Arrays are ordered major Sword, Bow, Magic, minor Right, Up, Left, Down
            running = new Animation[][] {
                // Sword
                new Animation[] {            
                    new Animation(36, 41, 5f, true, -16, 0),  // Right
                    new Animation(42, 47, 5f, true, 0),       // Down
                    new Animation(48, 53, 5f, true, -16, 0),  // Left
                    new Animation(54, 59, 5f, true, 0),       // Up
                },
                // Bow
                new Animation[] {            
                    new Animation(156, 161, 7f, true, -16, 0),    // Right
                    new Animation(162, 167, 7f, true, -16, 0),    // Down
                    new Animation(168, 173, 7f, true, -16, 0),    // Left
                    new Animation(150, 155, 7f, true, -16, -32),  // Up 
                },
                // Magic
                new Animation[] {
                    new Animation(13, 18, 5f, true, -16, 0),  // Right
                    new Animation(1, 6, 5f,true),             // Down
                    new Animation(7, 12, 5f, true, -16, 0),   // Left
                    new Animation(19, 24, 5f, true),          // Up
                },
            };

            Animation swordStandLeftandUp = new Animation(100, 101, 40f, true, -16, 0);
            Animation swordStandRightandDown = new Animation(102, 103, 40f, true, -16, 0);
            Animation bowStandLeftandUp = new Animation(124, 125, 35f, true, -16, 0);
            Animation bowStandRightandDown = new Animation(122, 123, 35f, true, -16, 0);

            standing = new Animation[][] {
                // Sword
                new Animation[] {
                    swordStandRightandDown,  // Right
                    swordStandRightandDown,  // Down
                    swordStandLeftandUp,     // Left
                    swordStandLeftandUp,     // Up
                },
                // Bow
                new Animation[] {
                    bowStandRightandDown,  // Right
                    bowStandRightandDown,  // Down
                    bowStandLeftandUp,     // Left
                    bowStandLeftandUp,     // Up
                },
                // Magic
                new Animation[] {
                    new Animation(33, 33, 40f, false),  // Right
                    new Animation(25, 32, 40f, true),   // Down
                    new Animation(35, 35, 40f, false),  // Left
                    new Animation(34, 34, 40f, false),  // Up
                },
            };

            attack = new Animation[][] {
                // Sword
                new Animation[] {
                    new Animation(60, 69, 1.5f, false, -16, -3),   // Right
                    new Animation(80, 89, 1.5f, false, -16, -3),   // Down
                    new Animation(70, 79, 1.5f, false, -80, -3),   // Left
                    new Animation(90, 99, 1.5f, false, -16, -32),  // Up
                },

                // Bow
                new Animation[] {
                    new Animation(110, 112, 7f, false, -54, 0),    // Right
                    new Animation(119, 121, 7f, false, 0, 0),      // Down
                    new Animation(113, 115, 7f, false, -48, 0),    // Left
                    new Animation(116, 118, 7f, false, -16, -24),  // Up
                },

                // Magic
                //First Half
                new Animation[]{
                    new Animation(138, 143, 4.5f, false, -16, 0),  // Right
                    new Animation(126, 131, 4.5f, false, -16, 0),  // Down
                    new Animation(132, 137, 4.5f, false, -16, 0),  // Left
                    new Animation(144, 149, 4.5f, false, -16, 0),  // Up
                },
            };

            //Skill Animations
            gigaBurstAnim = new Animation[]
            {
                    new Animation(181, 183, 3f, false, -16, 0),  // Right
                    new Animation(184, 186, 3f, false, -16, 0),  // Down
                    new Animation(187, 189, 3f, false, -16, 0),  // Left
                    new Animation(190, 192, 3f, false, -16, 0),  // Up
            };

            assassAnim = new Animation[]
            {
                    new Animation(193, 199, 2f, false, -60, -3),   // Right
                    new Animation(200, 206, 2f, false, -16, -32),   // Down
                    new Animation(207, 213, 2f, false, -60, -3),   // Left
                    new Animation(214, 220, 2f, false, -16, -32),  // Up
            };

            //Hurt Animations
            getHurtAnimation = new Animation[]
            {
                    new Animation(221, 222, 15f, false, -16, 0),   // Right
                    new Animation(223, 224, 15f, false, -16, 0),   // Down
                    new Animation(225, 226, 15f, false, -16, 0),   // Left
                    new Animation(227, 228, 15f, false, -16, 0),   // Up
            };

            deathAnimation = new Animation(104, 109, 10f, false, 0, -3);
            deathAnimation.addEndAct((frame) => {
                playerDied();
            });
            life.deathEvent += () => {
                anim = deathAnimation;
                world.tileEngine.audioComponent.playSound(audioSet[0], false);
            };
            life.lifeChangingEvent += lifeChanged;

            this.level = 0;
            this.skillPoints = 3;
            this.talentPoints = 0;
            this.mana = Constants.LUCY_MAXMANA;
            power = 20;
            anim = null;
            pb = new PlayerBehavior(this);
            myBehavior = pb;
            active = true;

            // important that this stays 0 because she'll flip around after hitting a wall if this is non-zero
            elasticity = 0;
            // MASKING
            this.actorcategory = ActorCategory.friendly;
            this.collisionmask = ActorCategory.enemy |  ActorCategory.enemyprojectile | ActorCategory.friendlyprojectile | ActorCategory.powerup;
            this.collisionimmunitymask = ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;


            textureSet = world.tileEngine.resourceComponent.getTextureSet("000_Lucy");
            audioSet = world.tileEngine.resourceComponent.getAudioSet("000_Lucy");

            (world as GameWorld).player = this;
        }

        public void playerDied()
        {
            (world as GameWorld).gameIsDone = true;
            world.tileEngine.audioComponent.pauseSoundEngine();
            savedColMask = collisionmask;
            this.collisionmask = ActorCategory.nocategory;
            (world.tileEngine as Graven).playerDied();
        }

        public void respawn()
        {
            (world as GameWorld).gameIsDone = false;
            world.tileEngine.audioComponent.resumeSoundEngine();

            // Respawn
            position = checkpoint.position;
            life.life = 100;
            mana = Constants.LUCY_MAXMANA;

            // Ensure the player remains active
            actorcategory = ActorCategory.friendly;
            collisionmask = savedColMask;
            active = true;
        }

        private void levelUp()
        {
            LevelUp levelUp = new LevelUp(world as GameWorld, position - new Vector2(0f, 0f),
            this.velocity);
            levelUp.frictionCoefficient = this.frictionCoefficient / 2;
            world.addActor(levelUp);

            this.level++;
            this.skillPoints++;
            this.talentPoints++;
        }

        private int levelUpThreshold(int level)
        {
            return (int)(100 * Math.Pow(level, 1.5));
        }

        public override void collision(Actor a)
        {
            /* TODO: Death system */
            if (anim != deathAnimation)
            {
                base.collision(a);
            }
        }

        /* TODO: Move to Input */
        private float rumbleTime = 0;
        public void rumble()
        {
            if (rumbleTime > 0)
            {
                GamePad.SetVibration(PlayerIndex.One, .4f, .4f);
                rumbleTime -= .1f;
            }
            else
            {
                GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
            }
        }
        
        public void lifeChanged(float oldLife)
        {

            if (life.life - oldLife < 0 && !life.isGod)
            {

                Numbers.spawn(this, (int)(oldLife - life.life));

                if (velocity.Y < 0)
                {
                    if (velocity.X >= 0)
                    {
                        anim = getHurtAnimation[3];
                    }
                    if (velocity.X < 0)
                    {
                        anim = getHurtAnimation[2];
                    }
                }
                else
                {
                    if (velocity.X <= 0)
                    {
                        anim = getHurtAnimation[1];
                    }
                    if (velocity.X > 0)
                    {
                        anim = getHurtAnimation[0];
                    }
                }
                ((world as GameWorld).player.myBehavior as PlayerBehavior).lockcooldown = 5;
                ((world as GameWorld).player.myBehavior as PlayerBehavior).shotcooldown = 5;

                // GLOWING
                int beams = 250;
                float increment = (float)Math.PI * 2 / beams;
                World.ModifyTile mod = delegate(Tile tile)
                {

                   
                        tile.changeGlow(0.2f, 0, 0);
                    
                    tile.val += 0.01f;

                    if (tile.val > 1.0f) tile.val = 1.0f;
                };
                // using the same vectors every time caused hard-edges / pixelation on tiles.
                // chooses a random starting point so using different rays.
                float offset = (float)world.tileEngine.randGen.NextDouble() * 2 * (float)Math.PI;
                for (float x = 0; x < Math.PI * 2; x += increment)
                {
                    Vector2 dir = new Vector2((float)Math.Cos(x + offset), (float)Math.Sin(x + offset));
                    // Tile under player was getting all rays applied to it, while each adjacent
                    // tile got 1/4 of the rays applied to them (approx), so by pushing center to
                    // the adjacent tile 1/4 of the time the tile under the player shouldn't be
                    // disproportionately bright.
                    if (x > Math.PI * 0.5f)
                        world.castRay(position + Tile.size * dir, dir, mod);
                    else
                        world.castRay(position, dir, mod);
                }
                if (myBehavior != null)
                {
                    ((PlayerBehavior)myBehavior).godcooldown = 30;
                }

                world.tileEngine.audioComponent.playSound(audioSet[world.tileEngine.randGen.Next(1, 2)], false);

                rumbleTime += 1;
                GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
            }

            if (!isBadass && (life.life / life.maxlife) < badassMeter)
            {
                badassMeter = 0.0f;
            }
        }

        public void killedEnemy(float exp = 0.06f)
        {
            badassMeter += exp;
            killCount++;
        }

        //State saving
        public override void saveState(BinaryWriter writer)
        {
            throw new Exception("State saving: Player.saveState() should never be called");
        }

        //Special save function for Player because it must be loaded first
        public void savePlayerState(BinaryWriter writer)
        {
            base.saveState(writer);
            writer.Write(power);
            writer.Write(level);
            writer.Write(skillPoints);
            writer.Write(talentPoints);
            writer.Write(_totalEXP);
            writer.Write(_mana);
        }

        public override void loadState(BinaryReader reader)
        {
            base.loadState(reader);
            power = reader.ReadInt32();
            level = reader.ReadInt32();
            skillPoints = reader.ReadInt32();
            talentPoints = reader.ReadInt32();
            _totalEXP = reader.ReadInt32();
            _mana = reader.ReadSingle();
        }
    }
}
