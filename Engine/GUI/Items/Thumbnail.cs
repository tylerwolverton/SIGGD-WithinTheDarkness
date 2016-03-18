using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using System.Diagnostics;
using Engine.Acting;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class Thumbnail : GUIItem
    {
        public int select = 0;
        public int tileIndex;
        public override Vector2 preferredSize
        {
            get 
            {
                if (forcedSize == NotForcedSize) return new Vector2(bgImage.Width, bgImage.Height);
                else return forcedSize;
            }
        }
        public Thumbnail(GUIComponent theInterface, Texture2D theImage, int tileIndex)
            : base(theInterface)
        {
            this.tileIndex = tileIndex;
            stretch = true;
            bgImage = theImage;
        }
        public override void draw(SpriteBatch spriteBatch, Vector2 absParLoc)
        {
                Vector2 absLoc = absParLoc + location;
                Rectangle rect = new Rectangle((int)absLoc.X, (int)absLoc.Y, (int)this.size.X, (int)this.size.Y);
                Rectangle rect2 = new Rectangle((int)absLoc.X + 1, (int)absLoc.Y + 1, (int)this.size.X, (int)this.size.Y);
                if(select==1)
                     rect2 = new Rectangle((int)absLoc.X-1, (int)absLoc.Y-1, (int)this.size.X+2, (int)this.size.Y+2);
                spriteBatch.Draw(guiComponent.guiTextures[10], rect2, select == 1 ?  Color.Cyan : new Color(0,0,0,.5f));
                spriteBatch.Draw(bgImage, rect, Color.White);
            
        }
    }
}
