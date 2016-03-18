#if true
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
using Engine.Textures;

namespace Engine.Acting.Enemies
{
    public class SpikonBehavior : Behavior
    {
        protected readonly Actor actor;
        protected readonly PathfindBehavior pathFindBehavior;
        private bool seen = false;
        private Actor target;
        public SpikonBehavior(Actor actor, Actor target)
            : base(actor)
        {
            this.actor = actor;
            this.target = target;
            this.pathFindBehavior = new PathfindBehavior(actor, target, Constants.SPIKON_BEHAVIOR_PATHFIND);
        }

        public override void run()
        {
            if (seen)
            {
                if ((target.position - actor.position).Length() < Constants.SPIKON_BEHAVIOR_FOLLOWDIST)
                {
                    pathFindBehavior.run();
                }
                else
                {
                    seen = false;
                }

                
            }
            else
            {
                if ((target.position - actor.position).Length() < Constants.SPIKON_BEHAVIOR_FOLLOWDIST && actor.world.hasLineOfSight(actor.position, target.position, true))
                {
                    seen = true;
                }
            }
            
            int beams = 30;
            float increment = (float)Math.PI * 2 / beams;
            World.ModifyTile mod = delegate(Tile tile)
            {
                tile.changeGlow(-0.009f, -0.009f, -0.009f);
                tile.val += 0.01f;
                if (tile.val > 1.0f) tile.val = 1.0f;
            };
            // using the same vectors every time caused hard-edges / pixelation on tiles.
            // chooses a random starting point so using different rays.
            float offset = (float)actor.world.tileEngine.randGen.NextDouble() * 2 * (float)Math.PI;
            for (float x = 0; x < Math.PI * 2; x += increment)
            {
                Vector2 dir = new Vector2((float)Math.Cos(x + offset), (float)Math.Sin(x + offset));
                // Tile under player was getting all rays applied to it, while each adjacent
                // tile got 1/4 of the rays applied to them (approx), so by pushing center to
                // the adjacent tile 1/4 of the time the tile under the player shouldn't be
                // disproportionately bright.
                if (x > Math.PI * 0.5f)
                    actor.world.castRay(actor.position + Tile.size * dir, dir, mod);
                else
                    actor.world.castRay(actor.position, dir, mod);
            }
        }
    }

    public class Spikon : Actor, ILife
    {

        public Vector2 startPos;
        public Life life { get; private set; }
        public int expvalue;
        public int damage;

        public Spikon(GameWorld world, Vector2 position)
            : base(world, position, new Vector2(0, 0), 28, Constants.WORLD2MODEL_SPIKON, 0)
        {
            actorName = "Spikon";
            //initialize position and animation
            startPos = position;
            anim = new Animation(0, 3, 8f, true, 0, 0);

            life = new Life(this, Constants.SPIKON_HEALTH);
            life.lifeChangingEvent += getHurt;
            life.deathEvent += delegate() { Spikon_deathEvent(this); };

            myBehavior = new SpikonBehavior(this, world.player);
              
            // MASKING
            this.actorcategory = ActorCategory.enemy;
            this.collisionmask = ActorCategory.enemy | ActorCategory.friendly | ActorCategory.friendlyprojectile; ;
            this.collisionimmunitymask = ActorCategory.enemy;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("014_Spikon");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("014_Spikon");

            this.expvalue = Constants.SPIKON_EXP;
            this.damage = Constants.SPIKON_DAMAGE;
            this.mass = Constants.SPIKON_MASS;

            this.active = true;
        }

        void Spikon_deathEvent(Actor deadActor)
        {
            (world as GameWorld).player.totalEXP += this.expvalue;
            (world as GameWorld).player.killedEnemy();

            Animation deathAnim = new Animation(4, 7, 10f, false, 0, 0);
            deathAnim.addEndAct((frame) => { removeMe = true; });
            anim = deathAnim;
#if false 
           // TODO: FUNCTIONIZE THIS
            if (World.randomGenerator.NextDouble() < .2)
            {
                double rotation = World.randomGenerator.NextDouble() * Math.PI * 2;
                HealthOrb healthorb = new HealthOrb(world as GameWorld, this.position, new Vector2((float)Math.Sin(rotation), (float)Math.Cos(rotation)) * 2);
                world.addActor(healthorb);
                rotation += Math.PI * 2 / 3;
                healthorb = new HealthOrb(world as GameWorld, this.position, new Vector2((float)Math.Sin(rotation), (float)Math.Cos(rotation)) * 2);
                world.addActor(healthorb);
                rotation += Math.PI * 2 / 3;
                healthorb = new HealthOrb(world as GameWorld, this.position, new Vector2((float)Math.Sin(rotation), (float)Math.Cos(rotation)) * 2);
                world.addActor(healthorb);
            }
#endif
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

#endif