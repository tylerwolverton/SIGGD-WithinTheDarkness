using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.GUI
{
    public class SliderContainer: ListContainer
    {
        public Slider slidey;
        public float percent { get { return slidey.percent; } }
        public Label option;
        public Label percentage;

        public SliderContainer(GUIComponent theGui, String optionName, float length = 100f)
            : base(theGui)
        {
            option = new Label(theGui, optionName);
            option.margin = 2;

            slidey = new Slider(theGui);
            slidey.forcedSize = new Vector2(length, 16);
            slidey.margin = 2;
            slidey.slideEvent += performLayout;

            percentage = new Label(theGui, (slidey.percent*100)+"%");
            percentage.margin = 2;

            this.Add(option);
            this.Add(slidey);
            this.Add(percentage);
        }
        public override void performLayout()
        {
            percentage.text = (slidey.percent * 100) + "%";
            base.performLayout();
        }
    }
}
