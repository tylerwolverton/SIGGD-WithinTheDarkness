using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Engine.Input;

namespace Engine.GUI
{
    class YouWinMenu : AbsoluteContainer
    {

        public enum YouWinBindings
        {
            CLOSE,
        }

        public YouWinMenu(GUIComponent theGUI)
            : base(theGUI)
        {
            
            stretch = true;
            bgImage = guiComponent.guiTextures[6]; //take this image out of the gui component

            this.pack();

            bindings = new InputBinding[Enum.GetValues(typeof(YouWinBindings)).Length];
            InputComponent input = guiComponent.graphics.tileEngine.inputComponent;
            this[YouWinBindings.CLOSE] = new SinglePressBinding(input, 
                new List<Keys> { Keys.Space, Keys.Escape, Keys.Enter }, 
                new List<Buttons> { Buttons.Start, Buttons.A, }, 
                InputComponent.MouseButton.LEFT);

            (this[YouWinBindings.CLOSE] as SinglePressBinding).downEvent += newGame;
        }

        public void newGame()
        {
            GameGUIComponent g = guiComponent as GameGUIComponent;
            g.current = g.credits;
#if false
#endif
        }

        public InputBinding this[YouWinBindings bind]
        {
            get
            {
                return bindings[(int)bind];
            }

            set
            {
                bindings[(int)bind] = value;
            }
        }
    }
}
