using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{

    public class ListContainer : GUIContainer
    {

        public enum Orientation
        {
            HORIZONTAL,
            VERTICAL,
        }
        public enum Align
        {
            LEFT,
            CENTER,
            RIGHT,
        }

        public Align align { get; set; }
        public Orientation orientation { get; set; }

        public override Vector2 preferredSize
        {

            get 
            {
                float width = 0, height = 0;
                float widthb = 0, heightb = 0;
                if (bgImage != null && !stretch)
                {
                    widthb = bgImage.Width;
                    heightb = bgImage.Height;
                }
                if (orientation == Orientation.HORIZONTAL) 
                {
                    foreach (GUIItem child in children) 
                    {
                        if (child != null)
                        {
                            Vector2 pSize = child.preferredSize;
                            width += pSize.X + 2 * child.marginL;
                            height = Math.Max(height, pSize.Y + 2 * child.marginT);
                        }
                    }
                } 
                else if (orientation == Orientation.VERTICAL) 
                {
                    foreach (GUIItem child in children) 
                    {
                        if (child != null)
                        {
                            Vector2 pSize = child.preferredSize;
                            width = Math.Max(width, pSize.X + 2 * child.marginL);
                            height += pSize.Y + 2 * child.marginT;
                        }
                    }
                }
                width = Math.Max(width, widthb);
                height = Math.Max(height, heightb);
                Vector2 ideal = new Vector2(width, height);
                if (forcedSize != NotForcedSize)
                    ideal = forcedSize;
                return ideal;
            }
        }

        public ListContainer(GUIComponent theInterface,  Orientation orientation = Orientation.HORIZONTAL, Align align = Align.LEFT)
            : base(theInterface)
        {
            this.orientation = orientation;
            this.align = align;
        }

        public override void performLayout()
        {

            Vector2 pSize = preferredSize;
            Vector2 center = pSize / 2;

            if (orientation == Orientation.HORIZONTAL) 
            {
                float width = 0;
                float height = 0;
                foreach (GUIItem child in children)
                {
                    if (child != null)
                    {
                        width += 2 * child.marginL + child.preferredSize.X;//for center on y axis
                        height = Math.Max(height, child.preferredSize.Y);
                    }
                }

                float curX;
                switch (align)
                {
                    case Align.LEFT:
                        curX = center.X - pSize.X / 2;
                        break;
                    case Align.CENTER:
                        curX = center.X - width/ 2;
                        break;
                    default:
                        curX = center.X - pSize.X / 2;
                        break;
                }

                foreach (GUIItem child in children)
                {

                    if (child == null) continue;
                    child.size = new Vector2(child.preferredSize.X, height);
                    if (child.forcedLocation != NotForcedLocation)
                        child.location = child.forcedLocation;
                    else
                        switch (align)
                        {
                            case Align.LEFT:
                                child.location = new Vector2(curX + child.marginL, 0);
                                break;

                            case Align.CENTER:
                                child.location = new Vector2(curX + child.marginL, this.size.Y / 2 - child.preferredSize.Y / 2);
                                break;
                        }

                    curX += child.size.X + 2 * child.marginL;
                }
                /*
                float curX = 0;
                float height = 0;
                foreach (GUIItem child in children) 
                {

                    height = Math.Max(height, child.preferredSize.Y);
                }

                //height = Math.Max(height, this.size.Y);
                foreach (GUIItem child in children)
                {

                    
                    child.size = new Vector2(child.preferredSize.X, height);
                    if (child.forcedLocation != NotForcedLocation)
                        child.location = child.forcedLocation;
                    else
                        child.location = new Vector2(curX + child.marginL, center.Y - child.size.Y / 2);

                    curX += child.size.X + 2 * child.marginL;
                }*/
            } 
            else if (orientation == Orientation.VERTICAL) 
            {
                float width = 0;
                float height = 0;
                foreach (GUIItem child in children)
                {
                    height += 2 * child.marginT + child.preferredSize.Y;//for center on y axis
                    width = Math.Max(width, child.preferredSize.X);
                }

                float curY;
                switch(align) {
                    case Align.LEFT:
                        curY = center.Y - pSize.Y / 2;
                        break;
                    case Align.CENTER:
                        curY = center.Y - height / 2;
                        break;
                    default:
                        curY = center.Y - pSize.Y / 2;
                        break;
                }

                foreach (GUIItem child in children) 
                {

                    child.size = new Vector2(width,child.preferredSize.Y);
                    if (child.forcedLocation != NotForcedLocation)
                        child.location = child.forcedLocation;
                    else
                        switch (align) {
                            case Align.LEFT:
                                child.location = new Vector2(0, curY+child.marginT);
                                break;

                            case Align.CENTER:
                                child.location = new Vector2(this.size.X/ 2 - child.preferredSize.X / 2, curY+child.marginT);
                                break;
                        }

                    curY += child.size.Y + 2 * child.marginT;
                }
            }
        }
    }
}


