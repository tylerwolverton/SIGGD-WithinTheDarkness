using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{

    public class Camera
    {

        private readonly GraphicsComponent graphics;

        //Window dimensions
        private float _scale=1;
        public float scale { 
            get { return _scale; } 
            set { 
                scaleChange += value - _scale; 
                _scale = value; }
 
        }
        public int screenWidth { get; set; }
        public int screenHeight { get; set; }
        public float scaleChange =0;
        //Camera location
        public Vector2 position { get; set; }
        public Vector2 velocity { get; set; }

        //View
        public int destHeight { get; set; }  // The ideal height of the view rectangle in game pixels

        public RectangleF ViewRect
        {
            get
            {
                return new RectangleF(position.X - screenWidth/2/scale, position
                    .Y - screenHeight/2/scale, screenWidth/scale, screenHeight/scale);
            }
            set
            {
                position = new Vector2(value.topLeft.X + screenWidth/2/scale, value.topLeft.Y + screenHeight/2/scale);
            }
        }

        public Camera(GraphicsComponent graphics, Vector2 position, int width, int height, int destHeight)
        {

            this.graphics = graphics;
            this.position = position;
            this.screenWidth = width;
            this.screenHeight = height;
            this.scale = height / destHeight;
            this.destHeight = destHeight;
        }

        public void setView()
        {
            this.screenWidth = graphics.graphics.GraphicsDevice.Viewport.Width;
            this.screenHeight = graphics.graphics.GraphicsDevice.Viewport.Height;
            this.scale = screenHeight / destHeight;
        }

        public void Update(GameTime time)
        {
            position += velocity * (float)time.ElapsedGameTime.TotalSeconds;
        }
    }
}
