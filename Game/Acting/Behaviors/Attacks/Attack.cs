using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Acting.Attacks
{
    public abstract class Attack : Behavior
    {

        public Attack(Actor actor) : base(actor) { }

        public override void saveState(System.IO.BinaryWriter writer)
        {
            base.saveState(writer);
        }

        public override void loadState(System.IO.BinaryReader reader)
        {
            base.loadState(reader);
        }
    }
}
