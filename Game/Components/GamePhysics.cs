using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

//Currently Not in Use
//Some of Engine's physics should probably be in here
namespace Engine.Components
{
    public class GamePhysics : PhysicsComponent
    {
        Graven game;

        public GamePhysics(MirrorEngine theEngine)
            : base(theEngine)
        {
            game = theEngine as Graven;
        }
    }
}
