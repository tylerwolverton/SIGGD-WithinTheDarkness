using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{

    public class InputComponent : Component
    {
       
        public KeyboardState previousKeyboardState { get; private set; }
        public KeyboardState currentKeyboardState { get; private set; }
        public GamePadState previousGamePadState { get; private set; }
        public GamePadState currentGamePadState { get; private set; }
        public MouseState previousMouseState { get; private set; }
        public MouseState currentMouseState { get; private set; }


        public enum MouseButton
        {
            LEFT,
            MIDDLE,
            RIGHT
        }

        private InputBinding[] bindings;
        private InputBinding[] savedBindings;

        public InputComponent(MirrorEngine theEngine) : base(theEngine)
        {

            previousKeyboardState = new KeyboardState();
            currentKeyboardState = new KeyboardState();
            previousGamePadState = new GamePadState();
            currentGamePadState = new GamePadState();
            previousMouseState = new MouseState();
            currentMouseState = new MouseState();
        }

        public override void Update(GameTime gameTime)
        {

            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            previousGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (bindings == null)
            {
                return;
            }

            foreach (InputBinding b in bindings)
            {
                b.Update(gameTime);
            }
        }

        public void replace(InputBinding[] bindings)
        {
            InputBinding[] oldBindings = this.bindings;
            this.bindings = bindings;

            if (this.bindings != null && this.bindings != oldBindings)  // Update new set of keybings
            {
                foreach (InputBinding bind in this.bindings)
                {
                    bind.reset();
                }
            }
        }
        
        public void append(InputBinding[] bindings)
        {
            if (this.bindings == null) 
            {
                replace(bindings);
            } 
            else 
            {
                replace(this.bindings.Concat(bindings).ToArray());
            }
        }

        public void save()
        {
            savedBindings = bindings;
        }
        
        public void restore()
        {
            replace(savedBindings);
            savedBindings = null;
        }

        public bool hasBindings()
        {
            return bindings != null;
        }
        public bool hasSaved()
        {
            return savedBindings != null;
        }
    }
}
