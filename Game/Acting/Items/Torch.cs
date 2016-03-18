using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Engine.Tiles;
using Engine.Acting;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine.Acting.Attacks;
using Engine.Acting.Projectiles;
using Engine.Acting.Items;
using Engine.Textures;

namespace Engine
{
    public class TorchBehavior : Behavior
    {
        Actor actor;
        int beams;

        public TorchBehavior(Actor actor)
            : base(actor)
        {
            this.actor = actor;
            beams = 0;
        }

        public override void run()
        {
            // GLOWING
            beams = (beams + (actor.world.tileEngine.randGen.Next()%9))%60;
            Color fifthwhite = new Color(0.1f, 0.1f*.2f, 0);
            float increment = (float)Math.PI * 2 / (beams+20);
            if(Vector2.Distance(actor.position, (actor.world as GameWorld).player.position) < 700)
            for (float x = 0; x < Math.PI * 2; x += increment * ((float)actor.world.tileEngine.randGen.NextDouble()%2))
            {
                Vector2 dir = new Vector2((float)Math.Cos(x));
                actor.world.castRay(actor.position, new Vector2((float)Math.Cos(x), (float)Math.Sin(x)), fifthwhite , 8.0f * Tile.size + ((float)actor.world.tileEngine.randGen.NextDouble() % 2));
            }
            
        }
    }

    public class Torch : Actor
    {
      

        public Torch(GameWorld world, Vector2 position)
            : base(world, position, Vector2.Zero, 6, new Vector2(-8f, -16f), 0)
        {
            ignoreAvE = true;
            anim = new Animation(0, 2, 20f, true, 0, 0);
            myBehavior = new TorchBehavior(this);
            active = true;
            frictionCoefficient = 0.1f;
            elasticity = .5f;
            mass = 10;

            // MASKING
            this.actorcategory = 0;
            
            
            textureSet = world.tileEngine.resourceComponent.getTextureSet("024_Torch");
        }

        public override void collision(Actor a)
        {

            ILife liveActor = a as ILife;
            if (liveActor == null) return;

            liveActor.life.life += 15;
            this.removeMe = true;
        }
    }
}
