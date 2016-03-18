using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.GUI
{
    public class Slider : AbsoluteContainer
    {
        public delegate void SlideHandler();
        public event SlideHandler slideEvent;
        public Label slidey;
        public float percent;             //slidey percentage
        public Slider(GUIComponent theGui,float def = .5f)
            : base(theGui)
        {
            percent = def;
            this.bgImage = theGui.guiTextures[15];
            this.stretch = true;
            slidey = new Label(theGui);
            slidey.bgImage = theGui.guiTextures[16];
            slidey.forcedSize = new Vector2 (16,16);
            this.clickEvent += Change;
            this.Add(slidey);
        }

        public override void performLayout()
        {
            slidey.forcedLocation = new Vector2((percent) * this.size.X-slidey.size.X/2, 0);
            base.performLayout();
            
        }
        public void Change(Vector2 loc)
        {
            if (loc.X / this.size.X <= 1f && 0 < loc.X / this.size.X && loc.Y > 0 && loc.Y < this.size.Y)
                percent = loc.X / this.size.X;
            else if (loc.X / this.size.X > 1f)
                percent = 1f;
            else if (loc.X / this.size.X < 0f)
                percent = 0f;
            performLayout();
            slideEvent();
        }
    }
}
