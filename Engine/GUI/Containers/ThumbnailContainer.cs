using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Engine.GUI
{
    public class ThumbnailContainer :   ListContainer
    {
        public Thumbnail[] thumbs;
        public Button left;
        public Button right;
        public TextureSet thumbPics;


        public ThumbnailContainer(GUIComponent theInterface, int tilesOrActors = 0)
            : base(theInterface)
        {
            this.orientation = Orientation.HORIZONTAL;
            if(tilesOrActors==0)loadTiles();
            if (tilesOrActors == 1) loadActors();
            int len = thumbPics.Count();
            thumbs = new Thumbnail[len];
            TableContainer table;
            if(1 == tilesOrActors)
                table = new TableContainer(theInterface, 2, (len + 1) / 2+1, 8);
            else
                
                table = new TableContainer(theInterface, 4, (len+1)/4+1, 16);
            for (int i = 0; i < len; i++)
            {
                thumbs[i] = new Thumbnail(theInterface, thumbPics[i], i);
                if (tilesOrActors == 1) thumbs[i].forcedSize = new Vector2(32, 32);
                thumbs[i].margin = 2;
                table.Add(thumbs[i]);
            }
            left = new Button(theInterface, "");
            left.normTexture = theInterface.guiTextures[23];
            left.clicTexture = theInterface.guiTextures[24];
            left.clickEvent += (lol) =>
            {
                table.left();
            };
            right = new Button(theInterface, "");
            right.normTexture = theInterface.guiTextures[25];
            right.clicTexture = theInterface.guiTextures[26];
            right.clickEvent += (lol) =>
            {
                table.right();
            };
            this.Add(left);
            this.Add(table);
            this.Add(right);
        }
        public void loadTiles()
        {
            thumbPics = guiComponent.tileEngine.resourceComponent.getTextureSet("Tiles");
             
        }
        public void loadActors()
        {
            thumbPics = guiComponent.tileEngine.resourceComponent.getTextureSet("028_ActorIcons");
        }
        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.Vector2 absParLoc)
        {
            base.draw(spriteBatch, absParLoc);
        }
  
    }
}
