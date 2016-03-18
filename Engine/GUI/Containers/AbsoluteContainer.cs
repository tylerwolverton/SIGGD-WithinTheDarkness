using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace Engine.GUI
{
    public class AbsoluteContainer : GUIContainer
    {
        public override Vector2 preferredSize
        {
            get
            {
                if (forcedSize == NotForcedSize) return Vector2.Zero;
                else return forcedSize;
            }
            
        }
        public AbsoluteContainer(GUIComponent theGui) : base(theGui) { }
        public override void performLayout()
        {
            foreach (GUIItem it in children)
            {
                it.location = it.forcedLocation == NotForcedLocation ? Vector2.Zero : it.forcedLocation; // I LOVE USING TERNARY
                it.size = it.forcedSize == NotForcedSize ? Vector2.Zero : it.forcedSize;
            }
        }
    }
}
