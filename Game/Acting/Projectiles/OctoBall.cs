using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace Engine.Acting.Projectiles
{
    public class OctoBall : Actor, ILife
    {

        public Life life {get; private set;}

        const bool ACTIVE = true;
        
        const float ELASTICITY = 0.5f;
        const float FRICTION = 0.001f;
        
        public OctoBall(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, Constants.OCTOBALL_RADIUS, Constants.WORLD2MODEL_OCTOBALL, 20)
        {
            life = new Life(this, Constants.OCTOBALL_HEALTH);
            life.deathEvent += delegate() { OctoBall_deathEvent(this); };
            
            anim = null;
            myBehavior = new ProjectileBehavior(this, 2);

            active = ACTIVE;
            frictionCoefficient = FRICTION;
            elasticity = ELASTICITY;
            
            mass = Constants.OCTOBALL_MASS;

            // MASKING
            this.actorcategory = ActorCategory.enemyprojectile;
            this.collisionmask = ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("012_Octo");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("012_Octo");
        }

        public void OctoBall_deathEvent(Actor a)
        {
            removeMe = true;
        }

        public override void collision(Actor a)
        {
            Life.collisionDamage(this, a, Constants.OCTOBALL_DAMAGE);
            a.addImpulse(velocity*6);

            if (velocity.Length() < 5)
            {
                removeMe = true;
            }
        }
    }
}