using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine.Input;

namespace Engine.GUI
{
    public class Credits : ListContainer
    {
        private enum CreditBindings
        {
            END,
        }

        private string[] creds = {
            "ALL CAPS GAMES",
            " ",
            "PRESIDENT & EXECUTIVE DIRECTOR",
            "EVAN HISSEY",
            " ",
            " ",
            "MIRROR ENGINE",
            " ",
            "DANIEL THORNBURGH - CO-LEAD PROGRAMMER",
            "SCOTT WILKEWITZ - CO-LEAD PROGRAMMER",
            " ",
            "NATHANIEL CHERRY - PROGRAMMER",
            "BEN PIETRZAK - PROGRAMMER",
            " ",
            " ",
            " ",
            "OCEANLAND LEVEL EDITOR",
            " ",
            "HARRISON GENTRY - LEAD PROGRAMMER",
            " ",
            "CALEB BENNETT - PROGRAMMER",
            "IAN WATTERSON - PROGRAMMER",
            "JAMES WATTERSON - PROGRAMMER",
            " ",
            " ",
            " ",
            "GAME MECHANICS",
            " ",
            "TYLER WOLVERTON - LEAD PROGRAMMER",
            " ",
            "NICK ADINAMIS - DESIGN",
            "ERIC BEARDINGTON - DESIGN",
            "YULAI LIU - DESIGN",
            "JP - DESIGN",
            "MARK SANDY - DESIGN",
            "RICKY KENNEDY - PROGRAMMER",
            " ",
            " ",
            " ",
            "GRAPHICS",
            " ",
            "HYUNG Q KIM - LEAD ARTIST",
            " ",
            "AMANDA POSTOLOWSKI - ENVIRONMENT ARTIST",
            "EDWARD DANG - SPRITE ARTIST",
            "MICHAEL GREENE - SPRITE ARTIST",
            "KYLE HURD - SPRITE ARTIST",
            "TAMMY TRIEU - SPRITE ARTIST",
            "MARK ANDERSON - TILE ARTIST",
            " ",
            " ",
            " ",
            "AUDIO",
            " ",
            "BLAKE DISSELBERGER - COMPOSER",
            "DANNY FRITZ - SOUND EFFECT RECORDING",
            "CORINNE GENTRY - VOICE ACTOR",
            " ",
            " ",
            " ",
            "SONGS BY VOLCANO-THEMED BATHROOM",
            " ",
            " ",
            " ",
            "SPECIAL THANKS",
            " ",
            "JOSH ANDREJKO",
            "KEVIN BRENENEMAN",
            "FRANK BUIBISH",
            "CASEY CHASTAIN",
            "ZACHERY DECOCQ",
            "SHAWN GARRISON",
            "CHRISTEN GOTTSCHLICH",
            "SYDNEY GREAVES",
            "JUSTIN MITCHENER",
            "RAHUL MOHANDAS",
            "BEN STERRETT",
            " ",
            " ",
            " ",
            "PURDUE SIGGD(C)",
            " ",
            " ",
            " ",

        };

        private const float SPEED = 1f;

        public Credits(GUIComponent theInterface) : base(theInterface, Orientation.VERTICAL, Align.CENTER)
        {
            foreach (string s in creds) {
                Label l = new Label(theInterface, s);
                l.color = Color.Green;
                Add(l);
            }

            bindings = new InputBinding[Enum.GetValues(typeof(CreditBindings)).Length];
            bindings[(int)CreditBindings.END] = new SinglePressBinding(guiComponent.tileEngine.inputComponent,
                new List<Keys> { Keys.Escape, Keys.Space, Keys.Enter }, null, null);
            (bindings[(int)CreditBindings.END] as SinglePressBinding).downEvent += quit;

        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 absParLoc)
        {
            base.draw(spriteBatch, absParLoc);
            location = new Vector2(location.X, location.Y - SPEED);
            performLayout();

            // Return to main menu if done
            if (location.Y + preferredSize.Y < 0) {
                guiComponent.current = (guiComponent as GameGUIComponent).mainMenu;
                quit();
            }
        }

        private void quit()
        {
            location = new Vector2(0, guiComponent.graphics.camera.screenHeight / guiComponent.graphics.camera.scale);

            //Send to main menu
            Graven tempGraven = guiComponent.tileEngine as Graven;

            //tempGraven.killWorld();
            tempGraven.OnMain();
        }
    }
}
