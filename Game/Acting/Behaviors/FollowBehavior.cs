using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.Acting
{
    public class FollowBehavior : Behavior
    {

        protected readonly Actor actor;
        public Actor target { get; protected set; }

        protected readonly float force;

        bool hasVectorTarget = false;
        Vector2 vectorTarget;

        public FollowBehavior(Actor actor, Actor target, float force) : base(actor)
        {

            this.actor = actor;
            this.target = target;
            (target as ILife).life.deathEvent += delegate() { targetDied(); };

            this.force = force;
        }

        public FollowBehavior(Actor actor, Vector2 target, float force)
            : base(actor) //May want pathfinding. 
        {

            this.actor = actor;
            vectorTarget = target;
            hasVectorTarget = true;
            this.force = force;
        }

        public override void run()
        {
            if (target == null && !hasVectorTarget)
            {
                actor.force = Vector2.Zero;
                return;
            }

            if (hasVectorTarget)
                actor.force = vectorTarget - actor.position;
            else
                actor.force = target.position - actor.position;

            actor.force = Vector2.Normalize(actor.force);
            actor.force *= force;
        }

        public void targetDied()
        {
            target = null;
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