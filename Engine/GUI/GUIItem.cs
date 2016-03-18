using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{

    public abstract class GUIItem
    {

        protected readonly GUIComponent guiComponent;

        public delegate void ClickHandler(Vector2 localPos);
        public event ClickHandler clickEvent;
        public delegate void ReleaseHandler(Vector2 localPos);
        public event ReleaseHandler releaseEvent;
        public delegate void RightClickHandler(Vector2 localPos);
        public event RightClickHandler clickRightEvent;
        public delegate void ReleaseRightHandler(Vector2 localPos);
        public event ReleaseRightHandler releaseRightEvent;
        public Vector2 location { get; set; }
        public Vector2 size { get; set; }
        public Vector2 forcedSize { get; set; }
        public Vector2 forcedLocation { get; set; }
        public Texture2D bgImage;
        public Color bgColor;
        public bool stretch = false;
        public bool focusable = false;
        public Vector2 NotForcedSize = new Vector2(-1,-1);
        public Vector2 NotForcedLocation = new Vector2(-1, -1);
        public abstract Vector2 preferredSize
        {
            get;
        }

        public float margin
        {
            get
            {
                return marginT;
            }
            set
            {
                marginT = value;
                marginL = value;
            }

        }

        public float marginT { get; set; }
        public float marginL { get; set; }

        public InputBinding[] bindings;
        
        public GUIItem(GUIComponent theInterface, InputBinding[] bindings = null)
        {
            this.guiComponent = theInterface;
            this.marginT = 0;
            this.marginL = 0;
            this.forcedSize = NotForcedSize;
            this.forcedLocation = NotForcedLocation;
        }

        public virtual bool contains(Vector2 pos)
        {

            return 0 <= pos.X && pos.X <=  size.X &&
                0 <= pos.Y && pos.Y <=  size.Y;
        }
        public virtual void pack() { }
        public virtual void handleClick(Vector2 localPos)
        {
            guiComponent.focus = (focusable) ? this : null;

            if (clickEvent != null)
            {
                
                clickEvent(localPos);
            }
        }
        public virtual void handleRightClick(Vector2 localPos)
        {
            guiComponent.focus = (focusable) ? this : null;

            if (clickRightEvent != null)
            {

                clickRightEvent(localPos);
            }
        }
        public virtual void releaseClick(Vector2 localPos)
        {
            if (releaseEvent != null)
            {
                releaseEvent(localPos);
            }
        }

        public virtual void releaseRightClick(Vector2 localPos)
        {
            if (releaseRightEvent != null)
            {
                releaseRightEvent(localPos);
            }
        }
        // Called whenever this GUIItem has focus and a key is pressed
        public virtual void handleKeyPress(char c)
        {
        }
        
        public abstract void draw(SpriteBatch spriteBatch, Vector2 absParLoc);
    }
}
