using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class Button : Label
    {
        protected Texture2D _normTexture;
        public Texture2D normTexture { get { return _normTexture; } set { _normTexture = value;  this.bgImage = _normTexture; } }
        public Texture2D clicTexture { get; set; }

        public Button(GUIComponent theInterface, string text = "button")
            : base(theInterface)
        {
            this.text = text;
            //TODO: put real textures in
            stretch = true;
            normTexture = theInterface.guiTextures[20];
            clicTexture = theInterface.guiTextures[21];
            this.bgImage = normTexture;
            clickEvent += (mLoc) => { bgImage = clicTexture; };
            (theInterface[GUIComponent.StaticBindings.CLICK] as SinglePressBinding).upEvent += delegate() { bgImage = normTexture; };
        }

    
    }
    
}
