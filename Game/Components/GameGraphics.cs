using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Engine.Input;
using Engine.Tiles;

namespace Engine
{
    class GameGraphics : GraphicsComponent
    {

        const bool FULLSCREEN = false;
        static Vector2 screenSize = new Vector2(500, 400);

        public GameGraphics(MirrorEngine theEngine)
            : base(theEngine, screenSize, FULLSCREEN)
        {
            guiComponent = new GameGUIComponent(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            camera.scaleChange = 0;
        }
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {

            GameWorld gameWorld = tileEngine.world as GameWorld;


         	base.Update(gameTime);

            if (gameWorld == null)
                return;

            // Move camera position to center of player position
            if (gameWorld.player != null) {
                camera.position = new Vector2(gameWorld.player.position.X, gameWorld.player.position.Y);
            }
            camera.scaleChange = 0;
            // Ensure inside map
            RectangleF viewRect = camera.ViewRect;

            float hWidth = (viewRect.right - viewRect.left)/2;
            float hHeight = (viewRect.bottom - viewRect.top)/2;


            if (viewRect.left < 0)
            {
                camera.position = new Vector2(hWidth, camera.position.Y);
            }
            else if (viewRect.right > gameWorld.width * Tile.size)
            {
                camera.position = new Vector2(gameWorld.width*Tile.size - hWidth, camera.position.Y);
            }

            if (viewRect.top <= 0)
            {
                camera.position = new Vector2(camera.position.X, hHeight);
            }
            else if (viewRect.bottom >= gameWorld.height* Tile.size)
            {
                camera.position = new Vector2(camera.position.X, gameWorld.height*Tile.size-hHeight);
            }

            if (gameWorld.player != null) {
                AxisBinding.origin = Vector2.Transform(gameWorld.player.position, game2screen);
            }
        }
    }
}
