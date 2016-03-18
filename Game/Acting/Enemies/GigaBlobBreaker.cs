using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Tiles;
using System.Diagnostics;
using System.IO;
using Engine.Acting.Bosses;
using Engine.Textures;

namespace Engine.Acting.Enemies
{
    public class ConglomeraterBehavior : Behavior
    {


        private int duration = 50;
        Actor actor;
        public ConglomeraterBehavior(GigaBlobBreaker actor)
            : base(actor)
        {
            this.actor = actor;
        }

        public override void run()
        {
            if (duration != 0)
            {
                foreach (Actor a in actor.getConeAround(250, Vector2.Zero, 360, null))
                {
                    if (a is Blob)
                    {
                        if (!(a is GigaBlob))
                        {
                        Vector2 impulse;
                        impulse = actor.position - a.position;
                        if (impulse.Length()<30)
                        {
                            (a as Blob).life.life = 0;
                        }
                        impulse.Normalize();
                        impulse *= 40;
                        a.addImpulse(impulse);
                        }
                    }


                }

                duration--;
            }
            else
            {
               /* float newLife = 0;
                foreach (Actor a in actor.getConeAround(50, Vector2.Zero, 360, null))
                {
                    if (a is Blob)
                    {
                        newLife += (a as Blob).life.life;
                        (a as Blob).life.life = 0;
                        Vector2 impulse;
                        impulse = -actor.position + a.position;
                        impulse.Normalize();
                        impulse *= 50;
                        a.addImpulse(impulse);
                    }


                }
               */
                    if (actor.world.worldName.Equals("012_Arena Mode"))
                    {
                        Actor newBlob = null;// = new GigaBlob((actor.world as GameWorld), actor.position, Vector2.Zero);
                        bool added = false;
                        while (newBlob == null || !added)
                        {
                            Vector2 blobpos = 25 * (new Vector2((float)actor.world.tileEngine.randGen.NextDouble() - .5f, (float)actor.world.tileEngine.randGen.NextDouble() - .5f));
                            newBlob = actor.world.actorFactory.createActor(actor.world.actorFactory.getActorId("GigaBlob"), actor.position + blobpos, blobpos);
                            if (newBlob != null)
                            {
                                newBlob.size = 0;
                                added = actor.world.addActor(newBlob);
                                newBlob.size = 28 * 4;

                            }
                        }
                        
                        
                    }
                
                (actor as GigaBlobBreaker).life.life = 0;
            }

        }
    }
    public class GigaBlobBreaker : Actor, ILife
    {
        public Animation defaultAnimation;
        public Life life { get; private set; }

        public GigaBlobBreaker(GameWorld world, Vector2 position)
            : base(world, position, Vector2.Zero, 1, new Vector2(-16f, -32f), 0)
        {
            defaultAnimation = new Animation(0, 2, 3f, true, 0, 0);

            anim = defaultAnimation;
            myBehavior = new ConglomeraterBehavior(this);
            active = true;
            frictionCoefficient = 1;
            elasticity = 0;
            mass = float.MaxValue;

            life = new Life(this, 1000);
            life.deathEvent += delegate() { removeMe = true; };
            this.setGlow(2.0f);
            textureSet = world.tileEngine.resourceComponent.getTextureSet("003_FireShuriken");
        }
        public override void hitWall()
        {
        }
        public override void collision(Actor a)
        {
        }
    }
}