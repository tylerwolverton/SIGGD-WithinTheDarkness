using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Engine.Input;

namespace Engine.GUI
{
    class GameOverMenu : ListContainer
    {
        public enum GameOverBindings
        {
            NEWGAME,
            CONTINUE
        }

        public GameOverMenu(GUIComponent gui) : base(gui, ListContainer.Orientation.VERTICAL)
        {
            Label overGame = new Label(gui, "GAME OVER");
            overGame.color = Color.Red;
            children.Add(overGame);
            Label spaceLabel = new Label(gui, "Continue? Y/N");
            spaceLabel.margin = 50;
            spaceLabel.color = Color.Red;
            children.Add(spaceLabel);

            // Align menu
            pack();

            //Game Over Bindings
            bindings = new InputBinding[Enum.GetValues(typeof(GameOverBindings)).Length];
            this[GameOverBindings.NEWGAME] = new SinglePressBinding(gui.tileEngine.inputComponent,
                new List<Keys> { Keys.N }, new List<Buttons> { Buttons.Start }, null);
            (this[GameOverBindings.NEWGAME] as SinglePressBinding).downEvent += newGame;
            this[GameOverBindings.CONTINUE] = new SinglePressBinding(gui.tileEngine.inputComponent,
                new List<Keys> { Keys.Y }, new List<Buttons> { Buttons.Start }, null);
            (this[GameOverBindings.CONTINUE] as SinglePressBinding).downEvent += cont;
        }

        public void newGame()
        {

            //Send to main menu
            Graven tempGraven = guiComponent.tileEngine as Graven;

            tempGraven.killWorld();
            tempGraven.OnMain();
        }

        public void cont()
        {
            Graven tempGraven = guiComponent.tileEngine as Graven;
            tempGraven.setWorld(null);
            (tempGraven.world as GameWorld).player.respawn();
        }

        public InputBinding this[GameOverBindings bind]
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
