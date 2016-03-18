using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{

    abstract public class Behavior
    {

        public Behavior()
        {
        }

        abstract public void run();

        public virtual void saveState(System.IO.BinaryWriter writer)
        {
        }

        public virtual void loadState(System.IO.BinaryReader reader)
        {
        }
    }
}
