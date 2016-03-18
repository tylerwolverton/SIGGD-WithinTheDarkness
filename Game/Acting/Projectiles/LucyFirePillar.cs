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
    public class LucyFirePillarBehavior:Behavior
    {
        private int duration = Constants.LUCYFIREPILLAR_DURATION;
        public Actor actor;
        public LucyFirePillarBehavior(LucyFirePillar actor)
            : base(actor)
        {
            this.actor = actor;
        }

        public override void run()
        {
            duration--;

            if (duration == Constants.LUCYFIREPILLAR_DURATION / 2)
            {
                //Determine which actors are hit
                foreach (Actor a in actor.getConeAround(36, new Vector2(0, 0), 360, null))
                {
                    ILife liveAct = a as ILife;
                    if (liveAct != null && (a.actorcategory & Actor.ActorCategory.friendly) == 0)
                    {
                        liveAct.life.life -= Constants.LUCY_FIREPILLAR_DAMAGE;
                    }
                }
            }
            /*if (duration == Constants.LUCYFIREPILLAR_DURATION - 9)
            {
                actor.anim = (actor as LucyFirePillar).firePillarWall;
            }*/
            if ( duration == 9)
            {
                actor.anim = (actor as LucyFirePillar).firePillarEnd;
            }

            if (duration <= 0)
            {
                actor.removeMe = true;
            }

            // GLOWING
            int beams = 35;
            Color fifthwhite = new Color(0.5f, 0.1f, 0);
            float increment = (float)Math.PI * 2 / beams;
            for (float x = 0; x < Math.PI * 2; x += increment)
            {
                actor.world.castRay(actor.position, new Vector2((float)Math.Cos(x), (float)Math.Sin(x)), fifthwhite);
            }
        }
    }
    public class LucyFirePillar : FirePillar
    {
        public Animation firePillarBegin;
        public Animation firePillarWall;
        public Animation firePillarEnd;

        public LucyFirePillar(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, new Vector2(0, 0))
        {
            firePillarBegin = new Animation(48, 50, 3f, false, 0, 0);
            firePillarWall = new Animation(51, 59, 3f, true, 0, 0);
            firePillarEnd = new Animation(60, 62, 3f, false, 0, 0);
            anim = firePillarBegin;
            damage = Constants.LUCY_FIREPILLAR_DAMAGE;
            myBehavior = new LucyFirePillarBehavior(this); 
            //ignoreAvE = true;
            // MASKING
            this.actorcategory = ActorCategory.friendlyprojectile;
            this.collisionmask = ActorCategory.enemy | ActorCategory.enemyprojectile;
            this.collisionimmunitymask = ActorCategory.friendly;
            this.damageimmunitymask = ActorCategory.friendly;
        }
    }
}
