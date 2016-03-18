using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Engine.Textures;

namespace Engine.Acting.Projectiles
{
    public class FireBlob : Actor, ILife
    {

        public Life life { get; private set; }

        const bool ACTIVE = true;
        const int DAMAGE = 28;
        const int MASS = 15;
        const float ELASTICITY = 0.5f;
        const float FRICTION = 0.001f;
        public int damage = Constants.FIREBLOB_DAMAGE;

        public FireBlob(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 16, new Vector2(-16f, -16f), 0)
        {
            life = new Life(this, 20);
            life.deathEvent += delegate() { FireBlob_deathEvent(this); };

            anim = new Animation(0,3,8f,true,-16,-16);
            myBehavior = new ProjectileBehavior(this, 0);

            active = ACTIVE;
            frictionCoefficient = FRICTION;
            elasticity = ELASTICITY;
            
            mass = MASS;

            // MASKING
            this.actorcategory = ActorCategory.enemyprojectile;
            this.collisionmask = ActorCategory.friendly | ActorCategory.enemyprojectile;
            this.collisionimmunitymask = ActorCategory.enemyprojectile;
            this.damageimmunitymask = ActorCategory.enemyprojectile;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("012_Octo");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("040_FireBlob");
        }

        public void FireBlob_deathEvent(Actor a)
        {
            removeMe = true;
        }

        public override void hitWall()
        {
            removeMe = true;
        }

        public override void collision(Actor a)
        {

            a.addImpulse(velocity * 6);
            Life.collisionDamage(this, a, this.damage);
            if (velocity.Length() < 5)
            {
                removeMe = true;
            }
        }
    }
}