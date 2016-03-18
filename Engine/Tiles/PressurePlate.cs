using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Engine
{

    public class PressurePlate
    {

        public Delegate method = null;
        public object[] args;
        public Actor prey;

        public object Trip(Actor prey)
        {

            this.prey = prey;
            return method.DynamicInvoke(args);
        }

        public PressurePlate(int choice)
        {
        
        }
    }
}
