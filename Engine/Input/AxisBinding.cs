using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Engine
{

    // Binding for an analog input, from an origin
    public class AxisBinding : InputBinding
    {

        public static Vector2 origin;

        public enum Axis
        {
            MOUSE_X,
            MOUSE_Y,
            LEFT_THUMB_X,
            LEFT_THUMB_Y,
            RIGHT_THUMB_X,
            RIGHT_THUMB_Y,
        }

        public List<Axis> axes { get; private set; }
        public List<Keys> posKeys { get; private set; }
        public List<Keys> negKeys { get; private set; }

        public float position { get; private set; }

        public AxisBinding(InputComponent input, List<Axis> axes, List<Keys> posKeys, List<Keys> negKeys)
            : base(input)
        {

            this.axes = axes;
            this.posKeys = posKeys;
            this.negKeys = negKeys;
        }

        // Check if this binding has been activated, and if so, dispatch events and change state
        public override void Update(GameTime gameTime)
        {
            reset();
        }

        public override void reset()
        {

            position = 0;

            // To interpret the mouse
            Camera camera = input.tileEngine.graphicsComponent.camera;

            if (input.currentGamePadState.IsConnected) 
            {
                if (axes != null) 
                {
                    foreach (Axis axis in axes) 
                    {
                        switch (axis) 
                        {
                            case Axis.LEFT_THUMB_X:
                                position += input.currentGamePadState.ThumbSticks.Left.X;
                                break;

                            case Axis.LEFT_THUMB_Y:
                                position += input.currentGamePadState.ThumbSticks.Left.Y;
                                break;

                            case Axis.RIGHT_THUMB_X:
                                position += input.currentGamePadState.ThumbSticks.Right.X;
                                break;

                            case Axis.RIGHT_THUMB_Y:
                                position += input.currentGamePadState.ThumbSticks.Right.Y;
                                break;
                        }
                    }
                }
            } 
            else 
            {
                if (axes != null && camera != null) 
                {
                    foreach (Axis axis in axes) 
                    {

                        Vector2 offset = new Vector2((float)(input.currentMouseState.X - origin.X)/camera.screenWidth/2, -(float) (input.currentMouseState.Y - origin.Y)/camera.screenHeight/2);

                        switch (axis) 
                        {

                            case Axis.MOUSE_X:
                                position += offset.X;
                                break;

                            case Axis.MOUSE_Y:
                                position += offset.Y;
                                break;
                        }
                    }
                }

                if (posKeys != null) 
                {
                    foreach (Keys key in posKeys) 
                    {
                        if (input.currentKeyboardState.IsKeyDown(key)) 
                        {
                            position += 1f;
                        }
                    }
                }

                if (negKeys != null) 
                {
                    foreach (Keys key in negKeys) 
                    {
                        if (input.currentKeyboardState.IsKeyDown(key)) 
                        {
                            position += -1f;
                        }
                    }
                }
            }

            position = MathHelper.Clamp(position, -1, 1);
        }
    }
}
