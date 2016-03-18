using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Engine
{

    public class Label : GUIItem
    {

        public string text { get; set; }
        public Color color { get; set; }

        public bool selected { get; set; } // \/\/\/\/\/\/
        public Label above { get; set; }   // for menus
        public Label below { get; set; }    //^^^^^^^^^^^
        
        public override Vector2 preferredSize
        {

            get
            {
                float width = 0, height = 0;
                float widthb = 0, heightb = 0;
                if (bgImage != null)
                {
                    widthb = bgImage.Width;
                    heightb = bgImage.Height;
                }

                
                if (text != null)
                {
                    Vector2 dim = guiComponent.font.MeasureString(text);
                    width = dim.X; height = dim.Y;
                }

                Vector2 ideal = new Vector2(Math.Max(widthb, width), Math.Max(heightb,height));

                if (forcedSize != NotForcedSize)
                {
                    ideal = forcedSize;
                }
                return ideal;
            }
        }



        public Label(GUIComponent theInterface, string text = null) : base(theInterface)
        {

            this.text = text;
            this.color = Color.Black;
            selected = false;
        }

        public override void draw(SpriteBatch spriteBatch , Vector2 absParLoc)
        {
            if (bgColor != null)
            {
                Vector2 absLoc = absParLoc + location;
                Rectangle rect = new Rectangle((int)absLoc.X, (int)absLoc.Y, (int)this.size.X, (int)this.size.Y);
                spriteBatch.Draw(this.guiComponent.guiTextures[10], rect, bgColor);
            }
            if (bgImage != null)
            {
                if (!stretch)
                {
                    spriteBatch.Draw(bgImage, absParLoc + location, Color.White);
                }
                else
                {
                    Vector2 absLoc = absParLoc + location;
                    Rectangle rect = new Rectangle((int)absLoc.X, (int)absLoc.Y,(int)this.size.X,(int)this.size.Y);
                    spriteBatch.Draw(bgImage, rect, Color.White);
                }
            }

            if (text != null && text != "")
            {


                if (forcedSize != NotForcedSize)
                {
                    String newText;
                    Vector2 size = guiComponent.font.MeasureString(text);
                    int charSize = (int)size.X / text.Length;
                    if (size.X > forcedSize.X)
                    {
                        float extra = (size.X - forcedSize.X) / charSize; //extra characters
                        float lastInd = text.Length - extra;
                        newText = text.Substring(0, (int)lastInd) + "\n";
                        while (extra > 0)
                        {
                            if (extra > forcedSize.X / charSize)
                            {
                                newText += text.Substring((int)(lastInd + 0.01), (int)forcedSize.X / charSize) + "\n";
                            }
                            else
                            {
                                newText += text.Substring((int)(lastInd + 0.01)) + "\n";
                            }
                            lastInd += forcedSize.X / charSize;
                            extra -= forcedSize.X / charSize;
                            spriteBatch.DrawString(guiComponent.font, newText, absParLoc + location, color);
                        }
                    }
                    else
                    {
                        spriteBatch.DrawString(guiComponent.font, text, absParLoc + location, color);
                    }

                }
                else
                {
                    spriteBatch.DrawString(guiComponent.font, text, absParLoc + location, color);
                }
            }
            
        }
    }
}
