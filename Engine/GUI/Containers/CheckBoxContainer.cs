using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.GUI
{
    public class CheckBoxContainer : ListContainer
    {
        public Label box;
        public Label mot;
        public bool isChecked;

        public CheckBoxContainer(GUIComponent theInterface, String text, bool checkorama)
            : base(theInterface)
        {
            orientation = Orientation.HORIZONTAL;
            isChecked = checkorama;
            box = new Label(theInterface, null);
            box.margin = 5;
            box.bgImage = isChecked ? theInterface.guiTextures[12] : theInterface.guiTextures[11];
            box.clickEvent += (loc) => {
                isChecked = !isChecked; 
                box.bgImage = (isChecked) ? theInterface.guiTextures[12] : theInterface.guiTextures[11]; };
            mot = new Label(theInterface, text);
            mot.margin = 5;
            children.Add(box);
            children.Add(mot);
        }
        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.Vector2 absParLoc)
        {
            base.draw(spriteBatch, absParLoc);
        }

    }
}
