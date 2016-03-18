using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.GUI
{
    public class RadioButton : Label
    {
        public int index;
        public RadioButton(GUIComponent theInterface, int index)
            : base(theInterface)
        {
            this.index = index;
        }
    }
}
