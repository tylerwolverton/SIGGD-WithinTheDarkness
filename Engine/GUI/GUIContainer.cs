using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Engine.GUI;

namespace Engine
{

    public abstract class GUIContainer : GUIItem
    {

       
        public List<GUIItem> children;
        

        public GUIContainer(GUIComponent theInterface)
            : base(theInterface)
        {
            children = new List<GUIItem>();            
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 absParLoc)
        {
            Vector2 absLoc = absParLoc + location;
            if (bgColor != null)
            {
                
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
                    
                    Rectangle rect = new Rectangle((int)absLoc.X, (int)absLoc.Y, (int)this.size.X, (int)this.size.Y);
                    spriteBatch.Draw(bgImage, rect, Color.White);
                }
            }
            foreach (GUIItem child in children)
            {
                if (child != null) 
                {
                    child.draw(spriteBatch, absLoc);
                }
            }
        }

        // Sets the locations and sizes for this container's children.
        public abstract void performLayout();

        public override void pack()
        {

            performLayout();

            foreach (GUIItem child in children) 
            {
                GUIContainer c = child as GUIContainer;
                if (c != null) {
                    c.pack();
                }
            }
        }
        
        public override void handleClick(Vector2 localPos)
        {
            base.handleClick(localPos);

            foreach (GUIItem child in children)
            {
                if (child != null && child.contains(localPos-child.location))
                {
                    child.handleClick(localPos - child.location);
                    return;
                }
            }
        }
        public override void handleRightClick(Vector2 localPos)
        {
            base.handleClick(localPos);

            foreach (GUIItem child in children)
            {
                if (child != null && child.contains(localPos - child.location))
                {
                    child.handleRightClick(localPos - child.location);
                    return;
                }
            }
        }
        public override void releaseClick(Vector2 localPos)
        {
            base.releaseClick(localPos);

            foreach (GUIItem child in children)
            {
                if (child != null && child.contains(localPos - child.location))
                {
                    child.releaseClick(localPos - child.location);
                    return;
                }
            }
        }
        public override void releaseRightClick(Vector2 localPos)
        {
            base.releaseClick(localPos);

            foreach (GUIItem child in children)
            {
                if (child != null && child.contains(localPos - child.location))
                {
                    child.releaseRightClick(localPos - child.location);
                    return;
                }
            }
        }
        public virtual void Add(GUIItem the)
        {
            this.children.Add(the);
        }

    }
}
