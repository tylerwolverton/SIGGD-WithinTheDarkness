using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace Engine.Acting
{
    public class EditorActorFactory : GameActorFactory
    {
        private EditorWorld world;
        

        //Someone comment what this is, please
        //private const int PROJ_BEGIN = 5;

        public EditorActorFactory(World world):base() {
            this.world = world as EditorWorld;

            
        }

        public override Actor createActor(int id, Vector2 position, Microsoft.Xna.Framework.Vector2? velocity = null, double color = -1)
        {
            return new EmptyActor(world, position, id);
            
        }
    
    }
}
