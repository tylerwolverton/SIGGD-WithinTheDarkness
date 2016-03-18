using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.Acting.Attacks
{
    public class ShootAttack : Attack
    {
        public Actor target { get; set; }

        // Direction in which to shoot
        public Vector2 direction { get; set; }

        // Offset from which to launch projectile
        public Vector2 offset { get; set; }

        // Radius of shooting circle (-1 is actor size)
        public float radius { get; set; }

        public float speed { get; set; }

        public int projectileId { get; set; }

        private readonly Actor actor;

        public ShootAttack(Actor actor)
            : base(actor)
        {
            this.actor = actor;
            radius = -1;
        }

        public Actor getProjectile()
        {

            Vector2 uDir = Vector2.Normalize(direction);
            float r = (radius < 0) ? actor.size / 2 : radius;

            return actor.world.actorFactory.createActor(projectileId, actor.position + offset + uDir * r, 
                uDir * speed + actor.velocity);
        }

        public override void run()
        {
            // If there's a target, shoot the target
            // Otherwise, shoot in the specified direction
            if (target != null)
                direction = target.position - actor.position;

            Vector2 uDir = Vector2.Normalize(direction);

            float r = (radius < 0) ? actor.size / 2 : radius;

            actor.world.addActor(actor.world.actorFactory.createActor(projectileId, actor.position + offset + uDir * r,
                uDir * speed + actor.velocity));
            
        }

        public override void saveState(System.IO.BinaryWriter writer)
        {
            base.saveState(writer);
            writer.Write(direction.X);
            writer.Write(direction.Y);
            writer.Write(offset.X);
            writer.Write(offset.Y);
        }

        public override void loadState(System.IO.BinaryReader reader)
        {
            base.loadState(reader);
            direction = new Vector2(reader.ReadSingle(),reader.ReadSingle());
            offset = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }
    }
}