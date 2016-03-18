using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;
namespace Engine
{
    class EditorInput : InputComponent
    {
        private MirrorEngine theEngine;
        public EditorInput(Editor editor) : base(editor as MirrorEngine)
        {
            theEngine = editor as Editor;

            editBindings = new InputBinding[Enum.GetValues(typeof(EditBindings)).Length];


        }

        public override void Initialize()
        {
            base.Initialize();
            

            this[EditBindings.MOUSELEFT] = new SinglePressBinding(this,
                    null, new List<Buttons> { Buttons.RightTrigger}, MouseButton.LEFT);
            this[EditBindings.MOUSERIGHT] = new SinglePressBinding(this,
                    null, new List<Buttons> { Buttons.LeftTrigger }, MouseButton.RIGHT);
            this[EditBindings.XMOVE] = new AxisBinding(this,
                new List<AxisBinding.Axis> { AxisBinding.Axis.LEFT_THUMB_X }, new List<Keys> { Keys.D, Keys.Right }, new List<Keys> { Keys.A, Keys.Left });
            this[EditBindings.YMOVE] = new AxisBinding(this,
                new List<AxisBinding.Axis> { AxisBinding.Axis.LEFT_THUMB_Y }, new List<Keys> { Keys.W, Keys.Up }, new List<Keys> { Keys.S, Keys.Down });
            this[EditBindings.XAIM] = new AxisBinding(this,
                new List<AxisBinding.Axis> { AxisBinding.Axis.MOUSE_X, AxisBinding.Axis.RIGHT_THUMB_X }, null, null);
            this[EditBindings.YAIM] = new AxisBinding(this,
                new List<AxisBinding.Axis> { AxisBinding.Axis.MOUSE_Y, AxisBinding.Axis.RIGHT_THUMB_Y }, null, null);
            this[EditBindings.GUICLICK] = new SinglePressBinding(this,
                    null, null, MouseButton.LEFT);
            this[EditBindings.SCROLLLOCK] = new SinglePressBinding(this,
                    new List<Keys> { Keys.Space }, new List<Buttons> { Buttons.X}, MouseButton.MIDDLE);
            this[EditBindings.ZOOMIN] = new SinglePressBinding(this, new List<Keys> { Keys.Add, Keys.OemPlus }, null, null);
            this[EditBindings.ZOOMOUT] = new SinglePressBinding(this, new List<Keys> { Keys.Subtract, Keys.OemMinus }, null, null);
            this[EditBindings.FULLSCREEN] = new SinglePressBinding(this, new List<Keys> { Keys.F }, null, null);
            this[EditBindings.QUIT] = new SinglePressBinding(this, new List<Keys> { Keys.Escape }, null, null);
            replace(editBindings);
        }
        public enum EditBindings
        {
            MOUSELEFT,
            MOUSERIGHT,
            XMOVE,
            YMOVE,
            XAIM,
            YAIM,
            GUICLICK,
            SCROLLLOCK,
            ZOOMIN,
            ZOOMOUT,
            FULLSCREEN,
            QUIT,
        }
        private InputBinding[] editBindings;

        public InputBinding this[EditBindings bind]
        {
            get
            {
                return editBindings[(int)bind];
            }

            set
            {
                editBindings[(int)bind] = value;
            }
        }
    }
}
