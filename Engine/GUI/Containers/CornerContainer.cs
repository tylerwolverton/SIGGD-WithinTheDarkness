using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.GUI
{

    public class CornerContainer : GUIContainer
    {

        // ORDER: TOP-LEFT, TOP-RIGHT, BOTTOM-LEFT, BOTTOM-RIGHT

        public override Vector2 preferredSize
        {
            get
            {
                float widthb = 0, heightb = 0;
                if (bgImage != null && !stretch)
                {
                    widthb = bgImage.Width;
                    heightb = bgImage.Height;
                }
                float widthA = 0;
                float widthB = 0;
                float heightA = 0;
                float heightB = 0;
                if (children[0] != null)
                {
                    widthA += children[0].size.X;
                    heightA += children[0].size.Y;
                }
                if (children[1] != null)
                {
                    widthA += children[1].size.X;
                    heightB += children[1].size.Y;
                }
                if (children[2] != null)
                {
                    widthB += children[2].size.X;
                    heightA += children[2].size.Y;
                }
                if (children[3] != null)
                {
                    widthB += children[3].size.X;
                    heightB += children[3].size.Y;
                }
                
                float width = Math.Max(widthA, widthB);
                float height = Math.Max(heightA, heightB);
                Vector2 ideal = new Vector2(width, height);
                if (forcedSize != NotForcedSize)
                    ideal = forcedSize;
                return ideal;
            }
            
        }

        public CornerContainer(GUIComponent theGui) : base(theGui) { }

        

        public override void performLayout()
        {

            GUIItem item;

            item = children[0];
            if (item != null)
            {

                Vector2 pSize = item.preferredSize;
                if (item.forcedLocation != NotForcedLocation)
                    item.location = item.forcedLocation;
                else
                    item.location = new Vector2(item.marginL, item.marginT);
                item.size = pSize;
            }

            item = children[1];
            if (item != null)
            {

                Vector2 pSize = item.preferredSize;
                if (item.forcedLocation != NotForcedLocation)
                    item.location = item.forcedLocation;
                else
                    item.location = new Vector2(size.X - pSize.X - item.marginL*2, item.marginT);
                item.size = pSize;
            }

            item = children[2];
            if (item != null)
            {

                Vector2 pSize = item.preferredSize;
                if (item.forcedLocation != NotForcedLocation)
                    item.location = item.forcedLocation;
                else
                    item.location = new Vector2(0, size.Y-pSize.Y-item.marginT*2);
                item.size = pSize;
            }
            
            item = children[3];
            if (item != null)
            {

                Vector2 pSize = item.preferredSize;
                if (item.forcedLocation != NotForcedLocation)
                    item.location = item.forcedLocation;
                else
                    item.location = new Vector2(size.X - pSize.X - item.marginL*2, size.Y - pSize.Y - item.marginT*2);
                item.size = pSize;
            }
        }
    }
}
