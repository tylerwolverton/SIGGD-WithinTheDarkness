using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Tiles;
using Microsoft.Xna.Framework;
using Engine.Acting;
using Engine.Acting.Enemies;
using System.Diagnostics;
using System.IO;
using Engine.Acting.Attacks;
using Engine.Textures;

namespace Engine.Acting.Bosses
{
    public class GigaBlobBehavior : Behavior
    {

        public static bool fork = false;
        protected  Actor actor;
        protected PathfindBehavior pathFindBehavior;
        protected  ShootAttack shootBehavior;
        private bool seen = false;
        private Actor target;
        private float shotcooldown;
        private float shotrate;
        private bool added = false;

        public float oldDamage;
        public GigaBlobBehavior(Actor actor, Actor target)
            : base(actor)
        {
            this.shotcooldown = Constants.BLOB_BEHAVIOR_SHOTCOOLDOWN;
            this.shotrate = Constants.BLOB_BEHAVIOR_SHOTRATE;
            this.target = target;
            this.actor = actor;
            this.pathFindBehavior = new PathfindBehavior(actor, target, Constants.BLOB_BEHAVIOR_PATHFIND);
            this.shootBehavior = new ShootAttack(actor);
            shootBehavior.target = target;
            shootBehavior.projectileId = actor.world.actorFactory.getActorId(Constants.BLOB_BEHAVIOR_SHOT);
            oldDamage = (actor as GigaBlob).life.life;

        }

        public override void run()
        {
            if (!added)
            {
                (actor.world as GameWorld).blobCount++;
                added = true;
            }
            if (seen)
            {
                if ((target.position - actor.position).Length() < Constants.BLOB_BEHAVIOR_FOLLOWDIST)
                {
                    pathFindBehavior.run();
                    if (oldDamage != (actor as GigaBlob).life.life)
                    {
                        bool addedActor = false;
                        oldDamage = (actor as GigaBlob).life.life;
                        Actor a = null;
                        if((actor.world as GameWorld).blobCount < Constants.MAX_WORLD_BLOBS)
                           
                        while (a == null || addedActor == false)
                        {
                            Vector2 blobpos = 100*(new Vector2((float)actor.world.tileEngine.randGen.NextDouble()-.5f,(float)actor.world.tileEngine.randGen.NextDouble()-.5f));
                            a = actor.world.actorFactory.createActor(actor.world.actorFactory.getActorId("Blob"), actor.position + blobpos, blobpos, (actor as GigaBlob).color);
                            addedActor = actor.world.addActor(a);
                            

                            
                        }
                        if(a != null)
                        actor.world.tileEngine.audioComponent.playSound(a.audioSet[actor.world.tileEngine.randGen.Next(4, 6)], false);
                    }
                }
                else
                {
                    seen = false;
                }

            }
            else
            {
                if ((target.position - actor.position).Length() < Constants.BLOB_BEHAVIOR_FOLLOWDIST && actor.world.hasLineOfSight(actor.position, target.position, true))
                {
                    seen = true;
                    actor.anim = new Animation(0, 3, 8f + (float)actor.world.tileEngine.randGen.NextDouble(), true);
                }
            }
        }
    }
    public class GigaBlob : Engine.Acting.Enemies.Blob
    {

        public GigaBlob(GameWorld world, Vector2 position, Vector2 velocity)
            : base(world, position, velocity,-1)
        {
            life = new Life(this, Constants.GIGABLOB_HEALTH*2);
            life.life = Constants.GIGABLOB_HEALTH;
            this.size = 20 * 5;
            this.world2model = Constants.WORLD2MODEL_GIGABLOB;
            this.expvalue = 0;
            this.damage = Constants.GREEN_BLOB_DAMAGE;
            this.mass = Constants.GREEN_BLOB_MASS*5;
            this.frictionCoefficient = Constants.GREEN_BLOB_FRICTION;
            
            actorName = "GigaBlob";

            if (color < 0.25)
            {
                textureSet = world.tileEngine.resourceComponent.getTextureSet("031_GigaBlob");
                color = 0.25;
                this.damage = Constants.GREEN_BLOB_DAMAGE;
                this.mass = Constants.GREEN_BLOB_MASS;
                this.frictionCoefficient = Constants.GREEN_BLOB_FRICTION;
            }
            else if (color < 0.5)
            {
                color = 0.79;
                textureSet = world.tileEngine.resourceComponent.getTextureSet("032_BlueGigaBlob");
                this.damage = Constants.BLUE_BLOB_DAMAGE;
                this.mass = Constants.GREEN_BLOB_MASS * 5;
                this.frictionCoefficient = Constants.BLUE_BLOB_FRICTION;
            }
            else if (color < 0.75)
            {
                color = 0.9;
                textureSet = world.tileEngine.resourceComponent.getTextureSet("033_RedGigaBlob");
                this.damage = Constants.RED_BLOB_DAMAGE;
                this.mass = Constants.GREEN_BLOB_MASS * 5;
                this.frictionCoefficient = Constants.RED_BLOB_FRICTION;
            }
            else
            {
                color = .999;
                textureSet = world.tileEngine.resourceComponent.getTextureSet("034_WhiteGigaBlob");
                this.damage = Constants.GREEN_BLOB_DAMAGE;
                this.mass = Constants.GREEN_BLOB_MASS * 5;
                this.frictionCoefficient = Constants.WHITE_BLOB_FRICTION*25;
            }
            this.color = color;
            this.active = true;
            life.lifeChangingEvent += getHurt;
            life.deathEvent += delegate() { GigaBlob_deathEvent(this); };
            myBehavior = new GigaBlobBehavior(this, world.player);


        }

        protected void GigaBlob_deathEvent(Actor deadActor)
        {
            Actor conglomerate = new GigaBlobBreaker(world as GameWorld, position);
            world.addActor(conglomerate);
            (world as GameWorld).player.killedEnemy();
            (world as GameWorld).blobCount--;
            Animation deathAnim = new Animation(5, 10, 10f, false);
            deathAnim.addEndAct((frame) => { removeMe = true; });
            anim = deathAnim;
            (world as GameWorld).blobDead = true;
            if (world.tileEngine.randGen.NextDouble() < Constants.BLOB_DROPCHANCE)
            {
                Pickup.spawn(this, world.actorFactory.getActorId("HealthOrb"), 3);

            }

            for (int i = 0; i < (world as GameWorld).width; i++)
            {
                for (int j = 0; j < (world as GameWorld).height; j++)
                {

                    if ((world as GameWorld).tileArray[i, j].tag % 100  == 10 && (world as GameWorld).tileArray[i, j].action == 0)
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

        
    }
}
