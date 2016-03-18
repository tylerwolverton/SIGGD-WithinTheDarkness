using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Engine.Tiles;
using Engine.Acting.Projectiles;
using Engine.Acting.Items;
using Engine.Acting.Enemies;
using Engine.Textures;

namespace Engine.Acting.Bosses
{
    public class FirstBossBehavior : PathfindBehavior
    {

        private Vector2 direction;

        public enum BossForm
        {
            first,
            second,
            third,
        } 
        public BossForm bossForm;


        public FirstBossBehavior(Actor actor, Player player) : base(actor, player, 0.1f)
        {
            actor.velocity = new Vector2(0, 0.1f);
            this.target = player;
            Vector2 lastTurn = actor.position;
            bossForm = BossForm.first;
        }

        int delay = 200;
        int delaytime = 80;

        //DEMONZ!
        int demonDelay = 1000;
        int demonCooldown = 2000;
        bool sendDemon = true;

        public override void run()
        {
            if (target != null && target.world.hasLineOfSight(target.position, actor.position, false) && (actor.world as GameWorld).bossTriggered)
            {
                Vector2 dest = target.position - actor.position;
                if ((actor as ILife).life.life > 1500)
                {

                    bossForm = BossForm.first;

                    if (delay > 0)
                    {
                        delay--;
                    }
                    else
                    {
                        delay = 35 + (int)Math.Round(actor.world.tileEngine.randGen.NextDouble() * delaytime);
                        if (Math.Abs(dest.X) < Math.Abs(dest.Y))
                        {
                            this.direction = new Vector2(0, dest.Y);
                        }
                        else
                        {
                            this.direction = new Vector2(dest.X, 0);
                        }
                        this.direction.Normalize();
                    }
                    actor.force += this.direction * .5f;

                    int frame = Actor.indexFromDirection(this.direction, 4, .25f);
                    actor.anim = (actor as FirstBoss).moveAnimation[(int)bossForm][frame];
                }
                else if ((actor as ILife).life.life > 1000)
                {

                    bossForm = BossForm.second;

                    delay--;
                    if (delay > 75)
                    {

                    }
                    else
                    {
                        actor.force += dest * .003f;
                    }
                    if (delay < 0)
                    {
                        delay = 150;
                    }
                    if (Math.Abs(dest.X) < Math.Abs(dest.Y))
                    {
                        this.direction = new Vector2(0, dest.Y);
                    }
                    else
                    {
                        this.direction = new Vector2(dest.X, 0);
                    }
                    this.direction.Normalize();
                    int frame = Actor.indexFromDirection(this.direction, 4, .25f);
                    actor.anim = (actor as FirstBoss).moveAnimation[(int)bossForm][frame];
                }
                else
                {

                    bossForm = BossForm.third;

                    (actor as FirstBoss).damage = 0;
                    dest =  actor.position - (actor as FirstBoss).startPos;
                    actor.force = dest * -.003f;

                    
                    demonCooldown--;
                    if (demonCooldown <= 0)
                    {
                        sendDemon = true;
                        demonCooldown = demonDelay;
                    }
                    
                    if (dest.Length() < 150)
                    {
                        delay--;
                        if (delay < 0)
                        {
                            delay = (int)Math.Round(60 + (actor as ILife).life.life*.1f);
                            dest = target.position - actor.position;

                            if (Math.Abs(dest.X) < Math.Abs(dest.Y))
                            {
                                if (dest.Y > 0)
                                {
                                    direction = new Vector2(0, -1);
                                    actor.anim = (actor as FirstBoss).moveAnimation[(int)bossForm][2];
                                }
                                else
                                {

                                    direction = new Vector2(0, 1);
                                    actor.anim = (actor as FirstBoss).moveAnimation[(int)bossForm][0];
                                }
                            }
                            else
                            {
                                if (dest.X > 0)
                                {
                                    direction = new Vector2(1, 0);
                                    actor.anim = (actor as FirstBoss).moveAnimation[(int)bossForm][3];
                                }
                                else
                                {
                                    direction = new Vector2(-1, 0);
                                    actor.anim = (actor as FirstBoss).moveAnimation[(int)bossForm][1];
                                }
                            }
                        }
                        else
                        {
                            if (delay % 6 == 0)
                            {
                                Vector2 loc;
                                float width;
                                float height;
                                if (direction.Y == 0)
                                {

                                    width = 30 * Tile.size;
                                    height = 28 * Tile.size;
                                    if (direction.X < 0)
                                    {
                                        loc = new Vector2(((FirstBoss)actor).startPos.X - 30 * Tile.size + width * (float)actor.world.tileEngine.randGen.NextDouble(), ((FirstBoss)actor).startPos.Y - 14 * Tile.size + height * (float)actor.world.tileEngine.randGen.NextDouble());
                                    
                                    }
                                    else
                                    {
                                        loc = new Vector2(((FirstBoss)actor).startPos.X + width * (float)actor.world.tileEngine.randGen.NextDouble(), ((FirstBoss)actor).startPos.Y - 14 * Tile.size + height * (float)actor.world.tileEngine.randGen.NextDouble());                                   
                                    
                                        }
                                }
                                else
                                {
                                    width = 44 * Tile.size;
                                    height = 14 * Tile.size;
                                    if (direction.Y < 0)
                                    {
                                        loc = new Vector2(((FirstBoss)actor).startPos.X - 30 * Tile.size + width * (float)actor.world.tileEngine.randGen.NextDouble(), ((FirstBoss)actor).startPos.Y  + height * (float)actor.world.tileEngine.randGen.NextDouble());
                                    
                                    }
                                    else
                                    {
                                        loc = new Vector2(((FirstBoss)actor).startPos.X - 30 * Tile.size + width * (float)actor.world.tileEngine.randGen.NextDouble(), ((FirstBoss)actor).startPos.Y - 14 * Tile.size + height * (float)actor.world.tileEngine.randGen.NextDouble());
                                    }
                                }

                                target.world.addActor(new FirePillar((target.world as GameWorld), loc, Vector2.Zero));

                            }
                        }
                    }
                }
            }

            if (actor.velocity.Length() > 0.1f)
            {

                if (actor.velocity.Length() > 6.0f)//For if the boss' speed is ever dynamic? 
                {
                    actor.world.tileEngine.audioComponent.setSoundPitch(actor.audioSet[2], 1.0f);
                }
                else
                {
                    actor.world.tileEngine.audioComponent.setSoundPitch(actor.audioSet[2], 0.0f);
                }

                actor.world.tileEngine.audioComponent.playSound(actor.audioSet[2], false);
            }
            else
            {
                actor.world.tileEngine.audioComponent.stopSound(actor.audioSet[2]);
            }
        }
    }

    public class FirstBoss : Actor, ILife
    {

        public Life life { get; private set; }
        public Animation deathAnimation;
        public int damage = 8;
        public Vector2 startPos;
        public Animation[][] moveAnimation; //[first, second, third][down, left, up, right]//rearrange to fit Actor.indexFromDirection()

        public FirstBoss(GameWorld world, Vector2 position)
            : base(world, position, Vector2.Zero, 128, Constants.WORLD2MODEL_FIRSTBOSS, 0)
        {
            life = new Life(this, 2000);
            life.lifeChangingEvent += getHurt;
            startPos = position;
            life.deathEvent += FirstBoss_deathEvent;
            deathAnimation = new Animation(67, 82, 6f, false, 0, 0);
            deathAnimation.addEndAct((frame) => { removeMe = true; });

            moveAnimation = new Animation[][]{ 
                new Animation[]{
                    new Animation(4, 7, 10f, true, 0, 0),
                    new Animation(8, 11, 10f, true, 0, 0),
                    new Animation(0, 3, 10f, true, 0, 0),
                    new Animation(12, 15, 10f, true, 0, 0),
                },

                new Animation[]{
                    new Animation(20, 23, 6f, true, 0, 0),
                    new Animation(24, 27, 6f, true, 0, 0),
                    new Animation(16, 19, 6f, true, 0, 0),
                    new Animation(28, 31, 6f, true, 0, 0),
                },

                new Animation[]{
                    new Animation(32, 35, 6f, true, 0, 0),
                    new Animation(40, 43, 6f, true, 0, 0),
                    new Animation(36, 39, 6f, true, 0, 0),
                    new Animation(44, 47, 6f, true, 0, 0),
                }
            };

            
            this.elasticity = 0;
            this.active = true;
            this.myBehavior = new FirstBossBehavior(this, world.player);
            this.anim = moveAnimation[1][0];
            
            this.mass = 500;
            this.frictionCoefficient = .1f;

            this.actorcategory = ActorCategory.enemy;
            this.collisionmask = ActorCategory.enemy | ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy | ActorCategory.enemyprojectile;
            this.damageimmunitymask = ActorCategory.enemyprojectile;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("004_FirstBoss");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("004_FirstBoss");
            world.tileEngine.audioComponent.setSoundVolume(audioSet[2], 0.8f);
        }


        public void FirstBoss_deathEvent()
        {

            (world as GameWorld).player.killedEnemy(1.0f);
            foreach(Actor a in world.actors)
            {
                if(!(a is Player) && !(a is FirstBoss) && (a.actorcategory & ActorCategory.enemy)!=0  )
                {
                    a.removeMe = true;
                }
            }
            world.tileEngine.audioComponent.playSound(audioSet[0], false);
            (world.tileEngine as Graven).bossAlive = false;
            anim = deathAnimation;

            // Fade out, and end game
            anim.addEndAct((frame) => {
                world.tileEngine.graphicsComponent.fadeTo(Color.Black, 3000, () => {
                    // Note: This code sorta copied from player 
                    world.tileEngine.graphicsComponent.tint = GraphicsComponent.clear;
                    (world as GameWorld).gameIsDone = true;
                    (world.tileEngine as Graven).playerWon();
                });
            });
        }

        public override void collision(Actor a)
        {
            if (anim != deathAnimation)
            {

                Player p = a as Player;
                if (p != null && !p.life.isGod)
                {
                    if (life.life > 1000)
                    {
                        Vector2 dir = new Vector2(a.position.X - this.position.X, a.position.Y - this.position.Y);
                        dir.Normalize();
                        dir *= 1250f;
                        a.addImpulse(dir);
                    }
                }
                Life.collisionDamage(this, a, this.damage);
                base.collision(a);
            }
        }

        public void getHurt(float oldLife)
        {

           

                Numbers.spawn(this, (int)(oldLife - life.life));
            
        }
    }

}