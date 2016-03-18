using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Engine.Tiles;
using System.IO;
using Engine.Textures;

namespace Engine.Acting.Projectiles
{
    public class StunWarningBehavior : ProjectileBehavior
    {
        private Actor target;
        private int duration;

        public StunWarningBehavior(StunWarning actor, Actor target) : base(actor, 20)
        {
            this.actor = actor;
            this.target = target;
            this.duration = 18;
        }

        public override void run(){

            if (duration <= 0)
            {
                (actor as ILife).life.life = 0;
            }
            duration--;

            actor.position = target.position;

            //Glow
            int beams = 100;
            float increment = (float)Math.PI * 2 / beams;
            for (float x = 0; x < Math.PI * 2; x += increment)
            {
                actor.world.castRay(actor.position, new Vector2((float)Math.Cos(x), (float)Math.Sin(x)), new Color(0, 0, 0));
            }
            //base.run();
        }
    }

    public class StunWarning: Actor, ILife
    {

        public Life life { get; private set; }

        public StunWarning(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 12, Constants.WORLD2MODEL_STUNWARNING, 0)
        {
            this.actorName = "StunWarning";
            myBehavior = new StunWarningBehavior(this, world.player);
            active = true;

            this.anim = new Animation(0, 0, 1f, true);

            // Life and death
            life = new Life(this, Constants.MAGICPRIMARY_HEALTH);
            life.deathEvent += delegate() { removeMe = true; };

            this.actorcategory = ActorCategory.nocategory;

            textureSet = world.tileEngine.resourceComponent.getTextureSet("038_StunWarning");
        }
    }
}
