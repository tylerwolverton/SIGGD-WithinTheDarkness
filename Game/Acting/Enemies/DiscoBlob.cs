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
    public class DiscoBlobBehavior : Behavior
    {
        protected readonly Actor actor;
        protected readonly PathfindBehavior pathFindBehavior;
        protected readonly ShootAttack shootBehavior;
        private bool seen = false;
        private Actor target; 
        public int colorSwap, count;
        public Color[] blobColor;

        public DiscoBlobBehavior(Actor actor, Actor target)
            : base(actor)
        {
            this.target = target;
            this.actor = actor;
            this.pathFindBehavior = new PathfindBehavior(actor, target, Constants.BLOB_BEHAVIOR_PATHFIND);
            colorSwap = 0;
            count = 0;
 
            blobColor = new Color[]{
                /*
                new Color(.7f,0,0),
                new Color(.7f,.7f,0),
                new Color(0,.7f,0),
                new Color(0,.7f,.7f),
                new Color(0,0,.7f),
                new Color(.7f,0,.7f),
                */
                new Color(1f,0,0),
                new Color(1f,1f,0),
                new Color(0,1f,0),
                new Color(0,1f,1f),
                new Color(0,0,1f),
                new Color(1f,0,1f),
             };
        }

        public override void run()
        {
            
            // GLOWING
            int beams = 300;
            float increment = (float)Math.PI * 2 / beams;
            if (Vector2.Distance(actor.position, (actor.world as GameWorld).player.position) < 700)
            for (float x = 0; x < Math.PI * 2; x += increment)
            {
                actor.world.castRay(actor.position, new Vector2((float)Math.Cos(x), (float)Math.Sin(x)), blobColor[colorSwap % 6]);
            }
            // Control how fast the color changes
            if (count % 20 == 0)
            {
                colorSwap++;
            }
            count++;

            // Do the same pathfinding as blob
            if (seen){
                if ((target.position - actor.position).Length() < Constants.BLOB_BEHAVIOR_FOLLOWDIST)
                {
                    pathFindBehavior.run();
                }
                else
                {
                    seen = false;
                }
                
            } else {
                if ((target.position - actor.position).Length() < Constants.BLOB_BEHAVIOR_FOLLOWDIST && actor.world.hasLineOfSight(actor.position, target.position, true))
                {
                    seen = true;
                    actor.anim = new Animation(0, 3, 8f + (float)actor.world.tileEngine.randGen.NextDouble(), true);
                }
            }
        }
    }

    public class DiscoBlob : Blob
    {
        int order;  // Order for checkpoints

        public DiscoBlob(GameWorld world, Vector2 position, Vector2 velocity, double color, int order)
            : base(world, position, new Vector2(0, 0), 0)
        {
            life = new Life(this, 5);

            this.expvalue = 0;
            this.damage = 0;
            this.mass = Constants.RED_BLOB_MASS;
            this.frictionCoefficient = 1.0f;
            this.actualColor = 3;
            this.order = order;
            actorName = "DiscoBlob";
            textureSet = world.tileEngine.resourceComponent.getTextureSet("029_DiscoBlob");
            this.color = color;
            this.active = true;
            life.lifeChangingEvent += getHurt;
            life.deathEvent += delegate() { Blob_deathEvent(this); };
            myBehavior = new DiscoBlobBehavior(this, world.player);
        }

        protected  void Blob_deathEvent(Actor deadActor)
        {
            (world as GameWorld).player.totalEXP += this.expvalue;
            (world as GameWorld).player.killedEnemy();

            Animation deathAnim = new Animation(5, 10, 10f, false);
            deathAnim.addEndAct((frame) => { removeMe = true; });
            anim = deathAnim;
            
            Pickup.spawn(this, world.actorFactory.getActorId("HealthOrb"), 20);

            // Update the player's current checkpoint
            Player p = (world as GameWorld).player;
            if (p.checkpoint == null || p.checkpoint.order < order) {
                p.checkpoint = new Checkpoint(order, position);
            }
            world.tileEngine.audioComponent.playSound(audioSet[0], false);
        }
    }
}