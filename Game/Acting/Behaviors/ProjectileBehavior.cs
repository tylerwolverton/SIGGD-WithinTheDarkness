using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Acting
{
    public class ProjectileBehavior : Behavior
    {
        public float minspeed;
        protected Actor actor;
        public ProjectileBehavior(Actor actor, float minspeed)
            : base(actor)
        {
            this.minspeed = minspeed;
            this.actor = actor;
        }

        public override void run()
        {
            if (actor.velocity.LengthSquared() < minspeed)
            {
                actor.removeMe = true;
            }
        }

        public override void saveState(System.IO.BinaryWriter writer)
        {
            base.saveState(writer);
        }

        public override void loadState(System.IO.BinaryReader reader)
        {
            base.loadState(reader);
        }
    }
}