using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Tiles;
using System.Diagnostics;
using System.IO;
using Engine.Textures;

namespace Engine.Acting.Projectiles
{
    public class SwordSlashBehavior : ProjectileBehavior
    {
        private int currentDirection;
        private int duration;

        public SwordSlashBehavior(SwordSlash actor)
            : base(actor, 0)
        {
            this.actor = actor;
            this.currentDirection = 0;
            this.duration = 15;
        }

        public override void run()
        {
            Vector2 dir = ((SwordSlash)actor).direction;       // Direction to face

            if (Math.Abs(dir.X) > Math.Abs(dir.Y)) //(actor.position.X) - (((SwordSlash)actor).playerPosition.X) > 0)
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
            actor.anim = ((SwordSlash)actor).animation[currentDirection];
            duration--;
            if (duration <= 0)
            {
                actor.removeMe = true;
            }
            //base.run();
        }
    }

    public class SwordSlash : Actor, ILife
    {
        public Animation[] animation;
        public Vector2 playerPosition;
        public Vector2 direction;

        public Life life { get; private set; }

        public SwordSlash(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, new Vector2(0, 0), Constants.EXPLOSION_SIZE, Constants.WORLD2MODEL_EXPLOSION, 0)
        {
            animation = new Animation[]
            {
                    new Animation(4, 5, 7.5f, false, -16, 16),  // Right
                    new Animation(0, 1, 7.5f, false, 0, 0),  // Down
                    new Animation(2, 3, 5.7f, false, 16, 16),  // Left
                    new Animation(6, 7, 7.5f, false, 0, 16),  // Up
            };

            this.direction = direction;
            myBehavior = new SwordSlashBehavior(this);
            active = true;
            frictionCoefficient = Constants.EXPLOSION_FRICTION;

            mass = float.MaxValue;
            playerPosition = world.player.position;

            // Setup death
            life = new Life(this, Constants.EXPLOSION_HEALTH);

            //this.setGlow(2);
            // MASKING
            this.actorcategory = ActorCategory.nocategory;
            //this.collisionmask = ActorCategory.enemyprojectile | ActorCategory.enemy;
            //this.collisionimmunitymask = ActorCategory.friendly;
            //this.damageimmunitymask = ActorCategory.friendlyprojectile;

            textureSet = world.tileEngine.resourceComponent.getTextureSet("041_SwordSlash");

        }
    }
}
