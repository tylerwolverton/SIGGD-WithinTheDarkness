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
    public class BlobBehavior : Behavior
    {

        public static bool fork = false;
        protected readonly Actor actor;
        protected readonly PathfindBehavior pathFindBehavior;
        protected readonly ShootAttack shootBehavior;
        private bool seen = false;
        public Actor target;
        private float shotcooldown;
        private float shotrate;
        private bool added = false;
        public BlobBehavior(Actor actor, Actor target)
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
                // Within follow distance
                if ((target.position - actor.position).Length() < Constants.BLOB_BEHAVIOR_FOLLOWDIST)
                {
                    pathFindBehavior.run();
                    if (fork)
                    {
                        shotcooldown--;
                        if (shotcooldown <= 0 && pathFindBehavior.canSee && (actor.world as GameWorld).blobCount < Constants.MAX_WORLD_BLOBS)
                        {
                            // makes shots occur at .5 to 1.5 shotrate
                            shotcooldown = shotrate + (float)(actor.world.tileEngine.randGen.NextDouble() * shotrate);
                            // Shoot
                            shootBehavior.run();
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
                if ((target.position - actor.position).Length() < Constants.BLOB_BEHAVIOR_FOLLOWDIST && actor.world.hasLineOfSight(actor.position, target.position, true))
                {
                    seen = true;
                    actor.anim = new Animation(0, 3, 8f + (float)actor.world.tileEngine.randGen.NextDouble(), true);
                }
            }
        }
    }

    public class Blob : Actor, ILife
    {

        public Life life { get; protected set; }
        public int expvalue;
        public int damage;
        public int actualColor;
        public Blob(GameWorld world, Vector2 position, Vector2 velocity, double color)
            : base(world, position, new Vector2(0, 0), 28, Constants.WORLD2MODEL_BLOB, 0)
        {
            // Initialize actor name
            actorName = "Blob";

            // Initialize animation
            anim = new Animation(11, 11, 6f, true, 8, 8);

            // Initialize audio set
            audioSet = world.tileEngine.resourceComponent.getAudioSet("001_Blob");

            // MASKING
            this.actorcategory = ActorCategory.enemy;
            this.collisionmask = ActorCategory.enemy | ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            if (color < 0)
                color = world.tileEngine.randGen.NextDouble();
            // Green Blob
            if (color < 0.5)
            {
                life = new Life(this, Constants.GREEN_BLOB_HEALTH);
                textureSet = world.tileEngine.resourceComponent.getTextureSet("001_Blob");
                // Initialize general actor variables
                this.damage = Constants.GREEN_BLOB_DAMAGE;
                this.mass = Constants.GREEN_BLOB_MASS;
                this.frictionCoefficient = Constants.GREEN_BLOB_FRICTION;
                this.actualColor = 0;
            }
            // Blue Blob
            else if (color < 0.8)
            {
                life = new Life(this, Constants.BLUE_BLOB_HEALTH);
                textureSet = world.tileEngine.resourceComponent.getTextureSet("002_BlueBlob");
                // Initialize general actor variables
                this.damage = Constants.BLUE_BLOB_DAMAGE;
                this.mass = Constants.BLUE_BLOB_MASS;
                this.frictionCoefficient = Constants.BLUE_BLOB_FRICTION;
                this.actualColor = 1;
            }
            // Red Blob
            else if (color < 0.995)
            {
                life = new Life(this, Constants.RED_BLOB_HEALTH);
                textureSet = world.tileEngine.resourceComponent.getTextureSet("013_RedBlob");
                // Initialize general actor variables
                this.damage = Constants.RED_BLOB_DAMAGE;
                this.mass = Constants.RED_BLOB_MASS;
                this.frictionCoefficient = Constants.RED_BLOB_FRICTION;
                this.actualColor = 2;

            }
            // White Blob
            else
            {
                life = new Life(this, Constants.WHITE_BLOB_HEALTH);
                textureSet = world.tileEngine.resourceComponent.getTextureSet("036_WhiteBlob");
                // Initialize general actor variables
                this.damage = Constants.WHITE_BLOB_DAMAGE;
                this.mass = Constants.WHITE_BLOB_MASS;
                this.frictionCoefficient = Constants.WHITE_BLOB_FRICTION;
                this.actualColor = 3;
                //MASKING
                this.actorcategory = ActorCategory.enemy;
                this.collisionmask = ActorCategory.enemy | ActorCategory.friendlyprojectile;
                this.collisionimmunitymask = ActorCategory.enemy | ActorCategory.friendly;
                this.damageimmunitymask = ActorCategory.friendlyprojectile;
            }
            // Initialize color and life events
            this.color = color;
            life.lifeChangingEvent += getHurt;
            life.deathEvent += delegate() { Blob_deathEvent(this); };

            // Initialize behavior
            myBehavior = new BlobBehavior(this, world.player);
            this.active = true;
        }

        protected void Blob_deathEvent(Actor deadActor)
        {
            (world as GameWorld).player.killedEnemy();
            (world as GameWorld).blobCount--;

            // Set animation to death animation
            Animation deathAnim = new Animation(5, 10, 10f, false);
            deathAnim.addEndAct((frame) => { removeMe = true; });
            anim = deathAnim;

            // Spawn health orbs
            if (world.tileEngine.randGen.NextDouble() < Constants.BLOB_DROPCHANCE)
            {
                Pickup.spawn(this, world.actorFactory.getActorId("HealthOrb"), 3);

            }
            world.tileEngine.audioComponent.playSound(audioSet[0], false);
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