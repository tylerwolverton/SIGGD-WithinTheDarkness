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
    public class FirePillarBehavior : Behavior
    {

        private int duration = 60;
        public Actor actor;
        public FirePillarBehavior(FirePillar actor)
            : base(actor)
        {
            this.actor = actor;
        }

        public override void run()
        {
            
            if (--duration == 40)
            {
                (actor as FirePillar).damage = 15;
                actor.anim = (actor as FirePillar).firePillar;
            }
            if (duration <= 0)
            {
                actor.removeMe = true;
            }

            //Determine which actors are hit
            foreach (Actor a in actor.getConeAround(36, new Vector2(0, 0), 360, null))
            {
                ILife liveAct = a as ILife;
                if (liveAct != null && a.actorcategory != Actor.ActorCategory.enemy)
                {
                    liveAct.life.life -= (actor as FirePillar).damage;
                }
            }
            
            // GLOWING
            int beams = 50;
            Color fifthwhite = new Color(0.5f, 0.1f, 0);
            float increment = (float)Math.PI * 2 / beams;
            for (float x = 0; x < Math.PI * 2; x += increment)
            {
                actor.world.castRay(actor.position, new Vector2((float)Math.Cos(x), (float)Math.Sin(x)), fifthwhite);
            }
        }
    }
    public class FirePillar : Actor
    {

        public int damage;

        public Vector2 oldvelocity;
        public Vector2 oldervelocity;
        public Animation firePillar;
        public Animation preFirePillar;

        public FirePillar(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, new Vector2(0, 0), 5, Constants.WORLD2MODEL_FIREPILLAR, 48)
        {

            myBehavior = new FirePillarBehavior(this);

            preFirePillar = new Animation(63, 66, 10f, false, 0, 0);
            firePillar = new Animation(48, 62, 3f, true, 0, 0);
            anim = preFirePillar;

            active = true;
            frictionCoefficient = .1f;
            damage = 0;
            mass = 9001;

            this.setGlow(2.0f);

            // MASKING
            this.actorcategory = ActorCategory.enemyprojectile;
            this.collisionmask = ActorCategory.enemy | ActorCategory.enemyprojectile | ActorCategory.friendlyprojectile | ActorCategory.friendly;
            this.collisionimmunitymask = ActorCategory.nocategory;
            this.damageimmunitymask = ActorCategory.friendlyprojectile | ActorCategory.enemy;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("020_FirePillar");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("004_FirstBoss");

           world.tileEngine.audioComponent.playSound(audioSet[0], false);
        }

        public override void collision(Actor a)
        {
            /*ILife live = a as ILife;
            if (live != null)
            {
                live.life.life -= damage;
            }*/
        }
    }
}