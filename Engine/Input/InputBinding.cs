using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Engine
{

    public abstract class InputBinding
    {

        protected InputComponent input;

        public InputBinding(InputComponent input)
        {
            this.input = input;
        }

        // Dispatch to child class to check if binding has been activated
        public abstract void Update(GameTime gameTime);
        public abstract void reset();
    }
}
