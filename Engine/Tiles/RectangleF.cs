using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{

    // Since XNA doesn't have floating point rectangles for some obscene reason
    public class RectangleF
    {

        public Vector2 topLeft { get; private set; }
        public Vector2 bottomRight { get; private set; }

        public float left
        {
            get
            {
                return topLeft.X;
            }
        }

        public float top
        {
            get
            {
                return topLeft.Y;
            }
        }

        public float right
        {
            get
            {
                return bottomRight.X;
            }
        }

        public float bottom
        {
            get
            {
                return bottomRight.Y;
            }
        }

        public RectangleF(Vector2 topLeft, Vector2 bottomRight)
        {

            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }

        public RectangleF(float x, float y, float width, float height)
        {

            if (width < 0 || height < 0) throw new Exception("Invalid rectangle: Negative width/height");

            this.topLeft = new Vector2(x, y);
            this.bottomRight = new Vector2(x+width, y+height);
        }
        
        public static bool intersects(RectangleF a, RectangleF b)
        {

            // The idea behind this bit of code is to add up all the possible ways they could NOT intersect, then negate it.
            return !(a.topLeft.X > b.bottomRight.X
                      || a.bottomRight.X < b.topLeft.X
                      || a.topLeft.Y > a.bottomRight.Y
                      || a.bottomRight.Y < b.topLeft.Y);
        }

        public void translate(Vector2 trans)
        {

            topLeft += trans;
            bottomRight += trans;
        }
    }
}
