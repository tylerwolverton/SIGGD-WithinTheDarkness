using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Engine
{

    // Binding for text input
    public class TextBinding : InputBinding
    {
        // Delegate type for character typed
        public delegate void CharEvent(char c);
        
        // Events
        public event CharEvent charEntered;

        public TextBinding(InputComponent input)
            : base(input)
        {
        
        }

        // Check if this binding has been activated, and if so, dispatch events and change state
        public override void Update(GameTime gameTime)
        {
            if (charEntered == null)
                return;

            // Text
            for (int i = (int)Keys.A; i <= (int)Keys.Z; i++)
            {
                bool cur = input.currentKeyboardState[(Keys)i] == KeyState.Down;
                bool prev = input.previousKeyboardState[(Keys)i] == KeyState.Down;
                if (cur && !prev)
                {
                    bool shift = input.currentKeyboardState[Keys.LeftShift] == KeyState.Down || input.currentKeyboardState[Keys.RightShift] == KeyState.Down;
                    char c = (char)((shift ? 'A' : 'a') + (i - (int)Keys.A));
                    charEntered(c);
                    return;
                }
            }

            // Numbers
            for (int i = (int)Keys.D0; i <= (int)Keys.D9; i++)
            {
                bool cur = input.currentKeyboardState[(Keys)i] == KeyState.Down;
                bool prev = input.previousKeyboardState[(Keys)i] == KeyState.Down;
                if (cur && !prev)
                {
                    char c = (char)('0' + (i - (int)Keys.D0));
                    charEntered(c);
                    return;
                }
            }

            // Space
            bool curUnd = input.currentKeyboardState[Keys.Space] == KeyState.Down;
            bool prevUnd = input.previousKeyboardState[Keys.Space] == KeyState.Down;
            if (curUnd && !prevUnd)
            {
                charEntered(' ');
                return;
            }

            // Backspace
            bool curBack = input.currentKeyboardState[Keys.Back] == KeyState.Down;
            bool prevBack = input.previousKeyboardState[Keys.Back] == KeyState.Down;
            if (curBack && !prevBack)
            {
                charEntered('\b');
                return;
            }
        }

        public override void reset()
        {
            // This binding stores no state
        }
    }
}
