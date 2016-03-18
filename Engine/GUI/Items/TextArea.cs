using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class TextArea : Label
    {
        public TextArea(GUIComponent gui) :base(gui,"")
        {
            focusable = true;
        }

        public override void handleKeyPress(char c) {
            if (c == '\b')
            {
                if (text.Length > 0)
                {
                    text = text.Substring(0, text.Length - 1);
                }
            }
            else
            {
                if (c == ' ')
                {
                    text += '_';
                }
                else
                {
                    text += c;
                }
            }
        }
    }
}
