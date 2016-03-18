using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Engine
{

    // Binding for a single press/depress cycle, with associated state
    public class SinglePressBinding : InputBinding
    {

        public List<Keys> keys { get; set; }
        public List<Buttons> buttons { get; set; }
        public InputComponent.MouseButton? mouseButton { get; set; }

        // Delegate type
        public delegate void PressEvent();
        
        // Events
        public PressEvent downEvent = null;
        public PressEvent upEvent = null;

        // State
        public bool isPressed { get; private set; }

        public SinglePressBinding(InputComponent input, List<Keys> keys, List<Buttons> buttons, InputComponent.MouseButton? mouseButton)
            : base(input)
        {

            this.keys = keys;
            this.buttons = buttons;
            this.mouseButton = mouseButton;
        }

        // Check if this binding has been activated, and if so, dispatch events and change state
        public override void Update(GameTime gameTime)
        {

            // Update State (pressed if any of the keys is pressed
            bool oldPressed = isPressed;
            reset();  // Update the state based on the keyboard

            // Invoke state change events
            if (downEvent != null && isPressed && !oldPressed)
            {
                downEvent.Invoke();
            }
            else
            {
                if (upEvent != null && !isPressed && oldPressed)
                {
                    upEvent.Invoke();
                }
            }
        }

        public override void reset()
        {

            isPressed = false;
            if (keys != null) 
            {
                foreach (Keys key in keys)
                {
                    if (input.currentKeyboardState[key] == KeyState.Down)
                    {
                        isPressed = true;
                        return;
                    }
                }
            }

            if (buttons != null) 
            {
                foreach (Buttons button in buttons)
                {
                    if (input.currentGamePadState.IsConnected && 
                            input.currentGamePadState.IsButtonDown(button))
                    {
                        isPressed = true;
                        return;
                    }
                }
            }

            if (mouseButton == InputComponent.MouseButton.LEFT && input.currentMouseState.LeftButton == ButtonState.Pressed)
            {
                isPressed = true;
                return;
            }

            if (mouseButton == InputComponent.MouseButton.MIDDLE && input.currentMouseState.MiddleButton == ButtonState.Pressed)
            {
                isPressed = true;
                return;
            }

            if (mouseButton == InputComponent.MouseButton.RIGHT && input.currentMouseState.RightButton == ButtonState.Pressed)
            {
                isPressed = true;
                return;
            }
        }
    }
}
