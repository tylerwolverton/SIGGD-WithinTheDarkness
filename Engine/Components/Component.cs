using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class Component
    {

        public bool isActive = true;
        public readonly MirrorEngine tileEngine;

        public Component(MirrorEngine theEngine)
        {
            this.tileEngine = theEngine;
        }
        
        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw()
        {
        }

        public virtual void UnloadContent()
        {
        }
    }
}
