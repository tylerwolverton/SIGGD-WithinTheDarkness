using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.Components
{
    class EditorPhysics : PhysicsComponent
    {
        Editor editor;
        public EditorPhysics(MirrorEngine theEngine)
            : base(theEngine)
        {
            editor = theEngine as Editor;

        }

        public override void  Update(GameTime gameTime)
        {
            
             	 //base.Update(gameTime);
        }
    }
}
