using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.Acting
{
    public abstract class ActorFactory
    {
        protected Dictionary<string, int> names;

        // This function will be called during world initialization to create an actor identified by a numeric identifier
        public abstract Actor createActor(int id, Vector2 position, Vector2? velocity = null, double color = -1);

        public int getActorId(string name)
        {
            return names[name];
        }
    }
}
