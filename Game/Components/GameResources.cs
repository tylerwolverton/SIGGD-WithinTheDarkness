using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Engine;

//Currently Not in Use
namespace Engine
{
    public class GameResources : ResourceComponent
    {

        public GameResources(ContentManager cm, MirrorEngine theEngine)
            : base(cm, theEngine)
        { 
        }

        public override void LoadContent()
        {

            base.LoadContent();
        }
    }
}
