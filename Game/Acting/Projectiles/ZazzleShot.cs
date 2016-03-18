using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Engine.Tiles;
using Engine.Textures;

namespace Engine.Acting.Projectiles
{
    public class ZazzleShot : Actor
    {
        private int damage;
        public ZazzleShot(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 12, Constants.WORLD2MODEL_ZAZZLESHOT, 20)
        {

            anim = new Animation(0,8,1f,true,0,0);
            myBehavior = new ProjectileBehavior(this, 2);
            active = true;
            frictionCoefficient = 0.001f;
            elasticity = .5f;

            mass = 10;
            damage = 5;
            // MASKING
            this.actorcategory = ActorCategory.enemyprojectile;
            this.collisionmask = ActorCategory.friendly | ActorCategory.friendlyprojectile;
            this.collisionimmunitymask = ActorCategory.enemy;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            textureSet = world.tileEngine.resourceComponent.getTextureSet("035_ZazzleShot");
        }
        public override void collision(Actor a)
        {
            anim = new Animation(1,1, 3f, false, 0, 0);
            anim.addEndAct((frame) => { removeMe = true; });

            ILife live = a as ILife;
            if (live != null)
            {
                live.life.life -= damage;
            }
        }
    }
}
