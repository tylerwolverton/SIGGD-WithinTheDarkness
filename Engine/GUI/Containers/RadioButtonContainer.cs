using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.GUI
{

    public class RadioButtonContainer : ListContainer
    {
        public int selected
        {
            get
            {
                for (int i = 0; i < butts.Length; i++)
                {
                    if (butts[i].bgImage == this.guiComponent.guiTextures[14]) return i;
                }
                return -1;
            }
            set { selected = value; }
        }
        public Label[] labels;
        public RadioButton[] butts;
        public ListContainer buttons;
        public ListContainer fields;

       

        public RadioButtonContainer(GUIComponent theInterface, String[] text, int num = 0)
            : base(theInterface)
        {
            orientation = Orientation.HORIZONTAL;
            labels = new Label[text.Length];
            butts = new RadioButton[text.Length];
            buttons = new ListContainer(theInterface, Orientation.VERTICAL);
            fields = new ListContainer(theInterface, Orientation.VERTICAL);
            for (int i = 0; i < text.Length; i++)
            {
                labels[i] = new Label(theInterface, text[i]);
                labels[i].margin = 2;

                RadioButton b = new RadioButton(theInterface,i);
                b.clickEvent += (loc) =>
                {
                    foreach (RadioButton button in butts)
                        button.bgImage = theInterface.guiTextures[13];
                    //button.bgImage = theInterface.guiTextures[13];
                    if (b.bgImage == theInterface.guiTextures[13])
                        b.bgImage = theInterface.guiTextures[14];
                    else
                        b.bgImage = theInterface.guiTextures[13];
                    performLayout();
                };
                butts[i] = b;
                butts[i].bgImage = i == num ? theInterface.guiTextures[14] : theInterface.guiTextures[13];
                butts[i].margin = 2;
                

                fields.children.Add(labels[i]);
                buttons.children.Add(butts[i]);
            }
            
            children.Add(buttons);
            children.Add(fields);
        }
    }
}
