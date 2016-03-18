using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Engine.GUI
{

    /* A TabButton is the thing you click on to change tabs
     * in a tabbed interface. TabContainer uses them.
     */
    public class TabButton : Button
    {
        public int index { get; set; }
        public TabButton(GUIComponent theInterface, String text=" tab ") : base(theInterface)
        {
            this.text = text;
            normTexture = theInterface.guiTextures[18];
            clicTexture = theInterface.guiTextures[19];
            bgImage = normTexture;
            clickEvent += (tab) => { bgImage = clicTexture; };
        }


        /*TODO: Properly orient the labels. This has only been implemented for the BOTTOM orientation. 
         */
        public void transform(TabContainer.TabOrientation orientation)
        {
        }


       
    }
}
