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
    public class ChargerBehavior : Behavior
    {
        protected readonly Actor charger;
        protected readonly PathfindBehavior pathFindBehavior;

        // Initialize animations
        protected readonly Animation chargerAttackAnim = new Animation(16, 22, 4f, true, -48, -48);
        protected readonly Animation chargerNorm = new Animation(1, 15, 8f, true, 0, 0);

        private bool seen = false;
        private bool dmgDone = false;

        public static int current_chargers = 0;

        private float cloneCooldown = Constants.CHARGER_CLONE_RATE;

        private Actor target;

        int[] dmgSize = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,18,32, 48, 48, 48, 48, 48};



        public ChargerBehavior(Actor actor, Actor target)
            : base(actor)
        {
            this.charger = actor;
            this.target = target;
            this.pathFindBehavior = new PathfindBehavior(actor, target, Constants.CHARGER_BEHAVIOR_PATHFIND);


            // Add 
            Animation.Action chargerAttack = (frame) =>
            {
                foreach (Actor a in charger.getConeAround(dmgSize[frame], new Vector2(1.0f, 1.0f), 360, null))
                {
                    if (a == target)
                    {
                        if (!dmgDone)
                        {
                           charger.velocity = new Vector2(0.0f, 0.0f);
                            
                           Life.basicDamage(charger, a, Constants.CHARGER_DAMAGE);

                           dmgDone = true;
                        }
                    }
                }
            };
            chargerAttackAnim.addPredAct(((frame) => {return true;}), chargerAttack);
        }


        public override void run()
        {
            if (seen)
            {
                // Within attack distance
                if ((target.position - charger.position).Length() < Constants.CHARGER_BEHAVIOR_ATTACKDIST)
                {
                    dmgDone = false;
                    charger.anim = chargerAttackAnim;
                }
                // Within follow distance
                else if ((target.position - charger.position).Length() < Constants.CHARGER_BEHAVIOR_FOLLOWDIST)
                {
                    charger.anim = chargerNorm;

                    pathFindBehavior.run();
                    cloneCooldown--;
                    // If max chargers hasn't been met, fork
                    if (cloneCooldown <= 0.0f && ChargerBehavior.current_chargers < Constants.CHARGER_MAX_GLOBAL_NUMBER)
                    {
                        Charger c = (Charger)charger.world.actorFactory.createActor(charger.world.actorFactory.getActorId("Charger"), charger.position, null);
                        if (c != null)
                        {
                            ChargerBehavior.current_chargers++;
                            charger.world.addActor(c);
                        }
                        cloneCooldown = Constants.CHARGER_CLONE_RATE;
                    }
                }
                else
                {
                    seen = false;
                }
            }
            else
            {
                if ((target.position - charger.position).Length() < Constants.CHARGER_BEHAVIOR_FOLLOWDIST && charger.world.hasLineOfSight(charger.position, target.position, true))
                {
                    seen = true;
                }
            }
        }

        public override void saveState(System.IO.BinaryWriter writer)
        {
            base.saveState(writer);
            writer.Write(dmgDone);
        }

        public override void loadState(System.IO.BinaryReader reader)
        {
            base.loadState(reader);
            dmgDone = reader.ReadBoolean();
        }
    }

    public class Charger : Actor, ILife
    {

        public Vector2 startPos;
        public Life life { get; private set; }
        public int expvalue;
        public int damage;

        public Charger(GameWorld world, Vector2 position)
            : base(world, position, new Vector2(0, 0), 12, Constants.WORLD2MODEL_CHARGER, 1)
        {

            // Initialize general actor variables
            actorName = "Charger";
            this.expvalue = Constants.CHARGER_EXP;
            this.damage = Constants.CHARGER_DAMAGE;
            this.mass = Constants.CHARGER_MASS;
            startPos = position;

            // Initialize life variables
            life = new Life(this, Constants.CHARGER_HEALTH);
            life.lifeChangingEvent += getHurt;
            life.deathEvent += delegate() { Charger_deathEvent(this); };

            //Initialize animation
            anim = new Animation(1, 15, 8f, true, 0, 0);

            // Initilize behavior
            myBehavior = new ChargerBehavior(this, world.player);
            this.active = true;

            // MASKING
            this.actorcategory = ActorCategory.enemy;
            this.collisionmask = ActorCategory.enemy | ActorCategory.friendly | ActorCategory.friendlyprojectile; ;
            this.collisionimmunitymask = ActorCategory.enemy;

            // Initialize audio and texture sets
            audioSet = world.tileEngine.resourceComponent.getAudioSet("014_Spikon");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("020_Charger");           
        }

        void Charger_deathEvent(Actor deadActor)
        {

            (world as GameWorld).player.totalEXP += this.expvalue;
            (world as GameWorld).player.killedEnemy();

            // Set animation to death animation and decrement number of chargers
            Animation deathAnim = new Animation(4, 7, 10f, false, 0, 0);
            deathAnim.addEndAct((frame) => { removeMe = true; ChargerBehavior.current_chargers--; });
            anim = deathAnim;
        }

        public void getHurt(float oldHealth)
        {
            Numbers.spawn(this, (int)(oldHealth - life.life));
        }
    }
}

#endif