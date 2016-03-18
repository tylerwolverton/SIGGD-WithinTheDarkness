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

    public class ArrowBehavior : ProjectileBehavior
    {
        public ArrowBehavior(Arrow actor) : base(actor,20)
        {
        }

        public override void run()
        {
            actor.imgIndex = Actor.indexFromDirection(actor.velocity, 16, .25f);
            (actor as Arrow).oldervelocity = (actor as Arrow).oldvelocity;
            (actor as Arrow).oldvelocity = (actor as Arrow).velocity;
            // GLOWING
            
            actor.world.getTileAt(actor.position).changeGlow(.3f * (float)(Color.Blue.R) / 256.0f, .3f * (float)(Color.Blue.G) / 256.0f, .3f * (float)(Color.Blue.B) / 256.0f);
            base.run();
        }
    }

    public class Arrow : Actor, ILife
    {

        public bool isSuper = false;

        public Animation[] arrowAnim;
        private int wallbounces = Constants.ARROW_WALLBOUNCE;
        public Vector2 oldvelocity, oldervelocity;

        public Life life { get; private set; }

        public Arrow(GameWorld world, Vector2 position, Vector2 direction)
            : base(world, position, direction, 4, Constants.WORLD2MODEL_ARROW, Constants.ARROW_FRAMES_PER_ARROW * Actor.indexFromDirection(direction, 16, .25f))
        {
            // Initialize array for arrow animations
            arrowAnim = new Animation[Constants.ARROW_ANIM_NUMBER];
            for(int i = 0; i < Constants.ARROW_ANIM_NUMBER; i++){
                arrowAnim[i] = new Animation(Constants.ARROW_FRAMES_PER_ARROW * i, Constants.ARROW_FRAMES_PER_ARROW * i + 1, 3f, true, 0, 0);
            }

            anim = arrowAnim[imgIndex / Constants.ARROW_FRAMES_PER_ARROW];
            myBehavior = new ArrowBehavior(this);
            active = true;
            frictionCoefficient = Constants.ARROW_FRICTION;
            elasticity = Constants.ARROW_ELASTICITY;
            mass = Constants.ARROW_MASS;

            // Life and death
            life = new Life(this, Constants.ARROW_HEALTH);
            life.deathEvent += delegate() { removeMe = true; };
            // TODO: Damage!
            //damage = ;

            this.setGlow(Constants.ARROW_GLOW);

            // MASKING
            this.actorcategory = ActorCategory.friendlyprojectile;
            this.collisionmask = ActorCategory.enemy | ActorCategory.enemyprojectile;
            this.collisionimmunitymask = ActorCategory.friendly;
            this.damageimmunitymask = ActorCategory.friendlyprojectile;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("005_Arrow");
            textureSet = world.tileEngine.resourceComponent.getTextureSet("005_Arrow");
        }

        public override void hitWall()
        {
            wallbounces--;
            if (wallbounces < 0)
            {
                this.life.life = 0;
            }
        }

        public override void collision(Actor a)
        {



            Vector2 dir = new Vector2(oldervelocity.X, oldervelocity.Y);
            if (dir != null)
            {
                dir.Normalize();
                dir *= this.velocity.Length() * 5;
                if (isSuper) dir *= Constants.ARROW_SUPERIMPULSE;
                a.addImpulse(dir);
            }

            ILife l = a as ILife;
            if (l != null)
            {
                l.life.life -=  Constants.ARROW_DAMAGE ;
                if (isSuper) l.life.life -= Constants.ARROW_SUPERDAMAGE;
            }

            if (!isSuper) removeMe = true;
        }
    }
}