using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Tiles;

namespace Engine.Acting
{
    public class PathfindBehavior : Behavior
    {
        protected readonly Actor actor;
        public Actor target { get; protected set; }
        public Boolean canSee { get; private set; }
        protected float force;

        public PathfindBehavior(Actor actor, Actor target, float force)
            : base(actor)
        {
            this.actor = actor;
            this.target = target;
            target.removeEvent += targetDied;
            this.force = force;
        }

        public override void run()
        {
            if (target == null)
            {
                actor.force = Vector2.Zero;
                return;
            }
            
            // Run A*
            Vector2 dest = AStar.findPath(actor.world, new Rectangle((int) (actor.position.X - actor.width/2), (int) (actor.position.Y - actor.height/2), (int) actor.width, (int) actor.height),
                                                        new Rectangle((int) (target.position.X - target.width / 2), (int) (target.position.Y - target.height / 2), (int) target.width, (int) target.height));
            if (dest == Vector2.Zero)
            {
                canSee = true;
                dest = target.position;
            }
            else
            {
                canSee = false;
            }

            actor.force += Vector2.Normalize(dest - actor.position) * force;
        }

        public void targetDied()
        {
            target = null;
        }

        public override void saveState(System.IO.BinaryWriter writer)
        {
            base.saveState(writer);
            writer.Write(canSee);
        }

        public override void loadState(System.IO.BinaryReader reader)
        {
            base.loadState(reader);
            canSee = reader.ReadBoolean();
        }
    }
}