using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;
using Engine.Tiles;

namespace Engine.Acting.Projectiles
{

    public class DeathTileBallBehavior : FollowBehavior
    {

        Vector2 destination;

        const int TARGET_RADIUS = 5;
        const int FORCE = 10;

        public DeathTileBallBehavior(Actor actor, Vector2 destination)
            : base(actor, destination, FORCE)
        {
            this.destination = destination;
        }

        public override void run()
        {

            if (actor.anim == null)
            {
                base.run();

                if ((actor.position - destination).Length() < TARGET_RADIUS)
                {
                    actor.removeMe = true;
                    actor.world.addActor(new DeathTile((actor.world as GameWorld), actor.position));
                }
            }
        }
    }

    public class DeathTileBall : Actor
    {

        const bool ACTIVE = true;

        const int MASS = 5;
        const float FRICTION = 0.5f;
        const float ELASTICITY = 0.1f;
        
        const string TEX_DIR = "017_DemonQuestionMark";
        const int IMG_INDEX = 78;
        const float SIZE = 10f;

        public DeathTileBall(GameWorld world, Vector2 position, Vector2 velocity, Vector2 target)
            : base(world, position, velocity, SIZE, new Vector2(-32f, -32f), IMG_INDEX)
        {

            anim = null;

            myBehavior = new DeathTileBallBehavior(this, target);

            active = ACTIVE;
            mass = MASS;
            frictionCoefficient = FRICTION;
            elasticity = ELASTICITY;

            // MASKING
            this.actorcategory = ActorCategory.enemyprojectile;
            this.collisionmask = ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy | ActorCategory.enemyprojectile;

            textureSet = world.tileEngine.resourceComponent.getTextureSet(TEX_DIR);
        }

        public override void collision(Actor a)
        {
            base.collision(a);
        }

        public override void hitWall()
        {
            removeMe = true;
        }
    }
}