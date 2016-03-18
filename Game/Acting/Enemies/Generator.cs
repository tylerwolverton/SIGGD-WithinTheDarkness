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
using Engine.Acting.Enemies;
using Engine.Textures;

namespace Engine.Acting.Enemies
{
    public class GeneratorBehaviour : Behavior
    {
        protected readonly Actor actor;
        private bool seen = false;
        public static bool fork = false;
        private Actor target;
        public Animation spawningNone;
        private Animation spawningCharger;
        protected readonly PathfindBehavior pathFindBehavior;
        protected readonly MaintainDistBehavior maintainDistBehavior;
        protected readonly ShootAttack shootBehavior;

        public GeneratorBehaviour(Actor actor, Actor target)
            : base(actor)
        {
            this.actor = actor;
            this.target = target;
            spawningCharger = new Animation(0, 9, 8f, true, -0, -0);
            this.pathFindBehavior = new PathfindBehavior(actor, target, Constants.GENERATOR_BEHAVIOR_PATHFIND);
            this.maintainDistBehavior = new MaintainDistBehavior(actor, target, Constants.GENERATOR_SPAWNCHARGER_RANGE, Constants.GENERATOR_BEHAVIOR_PATHFIND);  // TODO: make constant

            spawningCharger.addEndAct((frame) =>
            {

                Charger b = (Charger)actor.world.actorFactory.createActor(actor.world.actorFactory.getActorId("Charger"), actor.position + new Vector2(0, 0), null);
                if (b != null)
                {
                    actor.world.addActor(b);
                    ChargerBehavior.current_chargers++;
                }
                (b as ILife).life.deathEvent += spawn;
                chargerCount++;
                actor.anim = spawningNone;
            });
        }

        public bool needToSpawn = true;

        public void spawn()
        {
            chargerCount--;
        }

        public int chargerCount = 0;
        public override void run()
        {
            if (seen)
            {
                float temp_dist = (target.position - actor.position).Length();
                
                   /* if (temp_dist < Constants.GENERATOR_SPAWNCHARGER_RANGE + 20)
                    {  // TODO: Magic number
                        maintainDistBehavior.run();
                    }
                    else
                    {*/
                        //pathFindBehavior.run();
                   // }
                    if (actor.anim != spawningCharger && (actor.world as GameWorld).player != null)
                    {
                        if (chargerCount < Constants.NUM_CHARGERS && temp_dist < Constants.GENERATOR_SPAWNCHARGER_RANGE && ChargerBehavior.current_chargers < Constants.CHARGER_MAX_GLOBAL_NUMBER)
                        {
                            actor.anim = spawningCharger;
                        }
                    }
                
                /*else
                {
                    seen = false;
                }*/
            }
            else
            {
                if ((target.position - actor.position).Length() < Constants.GENERATOR_SPAWNCHARGER_RANGE && actor.world.hasLineOfSight(actor.position, target.position, true))
                {
                    seen = true;
                }
            }
        }

        public void saveState(System.IO.BinaryWriter writer)
        {
            base.saveState(writer);
            writer.Write(needToSpawn);
            writer.Write(fork);
        }

        public void loadState(System.IO.BinaryReader reader)
        {
            base.loadState(reader);
            needToSpawn = reader.ReadBoolean();
            fork = reader.ReadBoolean();
        }
    }

    public class Generator : Actor, ILife
    {
        const bool ACTIVE = true;
        public Life life { get; private set; }
        public int expvalue;
        public int damage;
        const String TEX_DIR = "Sprites\\022_Generator";
        const int IMG_INDEX = 00;

        public Generator(GameWorld world, Vector2 position)
            : base(world, position, new Vector2(0, 0), Constants.GENERATOR_SIZE, Constants.WORLD2MODEL_GENERATOR, IMG_INDEX)
        {
            this.actorName = "Generator";
            myBehavior = new GeneratorBehaviour(this, world.player);
            (myBehavior as GeneratorBehaviour).spawningNone = new Animation(0, 13, 8f, false, -0,-0);
            anim = (myBehavior as GeneratorBehaviour).spawningNone;

            active = ACTIVE;
            audioSet = world.tileEngine.resourceComponent.getAudioSet("020_FirePillar");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("022_Generator");

            //DAT MASKING
            this.actorcategory = ActorCategory.enemy;
            this.collisionmask = ActorCategory.enemy | ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            life = new Life(this, Constants.GENERATOR_HEALTH);
            this.expvalue = Constants.GENERATOR_EXP;
            this.damage = Constants.GENERATOR_DAMAGE;
            this.mass = Constants.GENERATOR_MASS;
            this.frictionCoefficient = Constants.GENERATOR_FRICTION;

            life.lifeChangingEvent += getHurt;
            life.deathEvent += delegate() { Generator_deathEvent(this); };
        }

        void Generator_deathEvent(Actor deadActor)
        {
            (world as GameWorld).player.totalEXP += this.expvalue;
            Animation deathAnim = new Animation(5, 5, 1f, false);
            deathAnim.addEndAct((frame) => { removeMe = true; });
            anim = deathAnim;

            deadActor.world.addActor(new Explosion(deadActor.world as GameWorld, deadActor.position, new Vector2(0, 0)));

            if (world.tileEngine.randGen.NextDouble() < Constants.GENERATOR_DROPCHANCE)
            {
                Pickup.spawn(this, world.actorFactory.getActorId("HealthOrb"), 3);
            }
            deadActor.world.tileEngine.audioComponent.playSound( audioSet[0], false);
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
