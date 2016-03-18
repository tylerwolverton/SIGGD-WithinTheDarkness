using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Tiles;

namespace Engine.Acting
{
    public class MaintainDistBehavior : Behavior
    {
        protected readonly Actor actor;
        public Actor target { get; set; }
        public float radius { get; set; }
        public float speed { get; set; }
        public Boolean canSee { get; private set; }
        public Boolean inRange { get; set; }

        public MaintainDistBehavior(Actor actor, Actor target, float radius, float speed)
            : base(actor)
        {
            this.actor = actor;
            this.target = target;
            target.removeEvent += targetDied;
            this.radius = radius;
            this.speed = speed;
        }

        public override void run()
        {
            if (target == null)
            {
                actor.force = Vector2.Zero;
                return;
            }

            Vector2 playerpos = (actor.world as GameWorld).player.position;
            Vector2 moveDir = playerpos - actor.position;
            if (moveDir.Length() < radius)
            {
                moveDir = -moveDir;
            }

            Vector2 dest = AStar.findPath(actor.world, new Rectangle((int)(actor.position.X - actor.size / 2), (int)(actor.position.Y - actor.size / 2), (int)actor.width, (int)actor.height),
                                                        new Rectangle((int)(target.position.X - target.size / 2), (int)(target.position.Y - target.size / 2), (int)target.width, (int)target.height));
            if (dest == Vector2.Zero)
            {
                canSee = true;
                //dest = target.position;
            }
            else
            {
                canSee = false;
            }
            if (moveDir.LengthSquared() > radius)//if outside radius
            {
                inRange = false;
            }
            else if (moveDir.LengthSquared() < radius + 10)//if inside the radius
            {
                moveDir *= -1;
                inRange = false;
            }
            else//if inside the radius
            {
                moveDir = Vector2.Zero;
                inRange = true;
            }
            moveDir = Vector2.Normalize(moveDir);
            actor.position += moveDir * speed;
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
