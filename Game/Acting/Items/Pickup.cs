using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Acting.Items;

namespace Engine.Acting
{
    public class Pickup
    {
        public static void spawn(Actor a, int id, int num) {
            for (int i = 0; i < num; i++) {
                double rotation = a.world.tileEngine.randGen.NextDouble() * Math.PI * 2;
                HealthOrb healthorb = new HealthOrb(a.world as GameWorld, a.position, new Vector2((float)Math.Sin(rotation), (float)Math.Cos(rotation)) * 2);
                a.world.addActor(healthorb);
                rotation += Math.PI * 2 / num;
            }
        }
    }
}
