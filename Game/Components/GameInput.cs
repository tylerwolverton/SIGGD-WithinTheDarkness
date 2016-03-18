using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Engine.Input;

namespace Engine
{
    public class GameInput : InputComponent
    {
        private readonly Graven game;

        public enum PlayBindings
        {
            XMOVE,
            YMOVE,
            XAIM,
            YAIM,
            SPRINT,
            PAUSE,
            PRIMARYATTACK,
            SECONDARYATTACK,
            SWORDSTANCE,
            BOWSTANCE,
            MAGICSTANCE,
            LCYCLESTANCE,
            RCYCLESTANCE,
            ASSSTANCE,
            r,
            TESTSKILL1,
            //GODMODE,
            //NOCLIP
        }

        private InputBinding[] playBindings;
        


        public GameInput(MirrorEngine theEngine)
            : base(theEngine)
        {
            game = theEngine as Graven;

            playBindings = new InputBinding[Enum.GetValues(typeof(PlayBindings)).Length];
            
        }

        public override void Initialize()
        {
            base.Initialize();

            // IN GAME BINDINGS
            this[PlayBindings.XMOVE] = new AxisBinding(this,
                new List<AxisBinding.Axis> { AxisBinding.Axis.LEFT_THUMB_X}, new List<Keys> { Keys.D, Keys.Right }, new List<Keys> { Keys.A, Keys.Left});
            this[PlayBindings.YMOVE] = new AxisBinding(this,
                new List<AxisBinding.Axis> { AxisBinding.Axis.LEFT_THUMB_Y}, new List<Keys> { Keys.W, Keys.Up }, new List<Keys> { Keys.S, Keys.Down });
            this[PlayBindings.XAIM] = new AxisBinding(this,
                new List<AxisBinding.Axis> { AxisBinding.Axis.MOUSE_X, AxisBinding.Axis.RIGHT_THUMB_X }, null, null);
            this[PlayBindings.YAIM] = new AxisBinding(this,
                new List<AxisBinding.Axis> { AxisBinding.Axis.MOUSE_Y, AxisBinding.Axis.RIGHT_THUMB_Y }, null, null);
            this[PlayBindings.SWORDSTANCE] = new SinglePressBinding(this,
                    null, new List<Buttons> { Buttons.DPadLeft }, null);
            this[PlayBindings.BOWSTANCE] = new SinglePressBinding(this,
                    null, new List<Buttons> { Buttons.DPadUp }, null);
            this[PlayBindings.MAGICSTANCE] = new SinglePressBinding(this,
                    null, new List<Buttons> { Buttons.DPadRight }, null);
            this[PlayBindings.LCYCLESTANCE] = new SinglePressBinding(this,
                    new List<Keys> { Keys.Q }, new List<Buttons> { Buttons.LeftShoulder }, null); 
            this[PlayBindings.RCYCLESTANCE] = new SinglePressBinding(this,
                     new List<Keys> { Keys.E }, new List<Buttons> { Buttons.RightShoulder }, null);
            this[PlayBindings.SPRINT] = new SinglePressBinding(this,
                    new List<Keys> { Keys.LeftShift, Keys.RightShift }, new List<Buttons> { Buttons.X }, null);
            this[PlayBindings.PAUSE] = new SinglePressBinding(this,
                    new List<Keys> { Keys.P, Keys.Escape, Keys.Pause }, new List<Buttons> { Buttons.Start }, null);
            this[PlayBindings.PRIMARYATTACK] = new SinglePressBinding(this,
                    null, new List<Buttons> { Buttons.RightTrigger }, MouseButton.LEFT);
            this[PlayBindings.SECONDARYATTACK] = new SinglePressBinding(this,
                    null, new List<Buttons> { Buttons.LeftTrigger }, MouseButton.RIGHT);
            this[PlayBindings.r] = new SinglePressBinding(this,
                    new List<Keys> { Keys.R }, new List<Buttons> { Buttons.Y }, null);
            this[PlayBindings.ASSSTANCE] = new SinglePressBinding(this,
                    new List<Keys> { Keys.Space }, new List<Buttons> { Buttons.Y }, null);
            this[PlayBindings.TESTSKILL1] = new SinglePressBinding(this,
                     new List<Keys> { Keys.O }, null, null);
            //this[PlayBindings.GODMODE] = new SinglePressBinding(this,
           //       new List<Keys> { Keys.G }, null, null);
            //this[PlayBindings.NOCLIP] = new SinglePressBinding(this,
            //       new List<Keys> { Keys.N }, null, null);

        }

        // The following methods retrieve a particular InputBinding, depending on the enum passed 
        // They're written as indexers for convenience
        public InputBinding this[PlayBindings bind]
        {
            get {
                return playBindings[(int)bind];
            }

            set {
                playBindings[(int)bind] = value;
            }
        }

        public void enablePlayBindings()
        {
            replace(playBindings);
        }
    }
}
