using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.GUI;
using Microsoft.Xna.Framework.Input;


namespace Engine
{
    public class GameGUIComponent : GUIComponent
    {

        public GUIContainer hud;
        public GUIContainer pauseMenu;
        public GUIContainer gameOverMenu;
        public GUIContainer youWinMenu;
        public GUIContainer mainMenu;
        public GUIContainer credits;

        public GameGUIComponent(GraphicsComponent theGraphics)
            : base(theGraphics)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();
            guiTextures = resources.getTextureSet("001_GameGUI");

            // TODO: Fix CornerContainer so you can specify which corner, without adding the others as null
            hud = new CornerContainer(this);

            hud.children.Add(new StatusBar(this));
            hud.children.Add(null);
            hud.children.Add(null);
            hud.children.Add(null);

            mainMenu = new MainMenu(this);
            pauseMenu = new ListContainer(this, ListContainer.Orientation.VERTICAL);
            PauseMenu inner = new PauseMenu(this);
            pauseMenu.children.Add(inner);
            pauseMenu.bindings = inner.bindings;
            youWinMenu = new YouWinMenu(this);


            gameOverMenu = new GameOverMenu(this);

            credits = new Credits(this);


            //mainMenu = new ListContainer(this, ListContainer.Orientation.VERTICAL);
            //mainMenu = new MainMenu(this);//this is not ready yet, isnt formatting

            float width = graphics.camera.screenWidth/graphics.camera.scale;
            float height = graphics.camera.screenHeight / graphics.camera.scale;
            mainMenu.location = new Vector2(0, -50);
            //mainMenu.forcedSize = hud.forcedSize = pauseMenu.forcedSize = gameOverMenu.forcedSize = youWinMenu.forcedSize = new Vector2(width, height);
            mainMenu.size = hud.size = pauseMenu.size = gameOverMenu.size = youWinMenu.size = new Vector2(width, height);
            
            youWinMenu.location = new Vector2(0, 0);


            mainMenu.forcedSize = new Vector2(width, height);
            hud.pack();
            pauseMenu.size = new Vector2(guiTextures[5].Bounds.Width,guiTextures[5].Bounds.Height);
            pauseMenu.location = new Vector2(width / 2 - guiTextures[5].Bounds.Width / 2 + 30, height / 2 - guiTextures[5].Bounds.Height / 2);
            pauseMenu.pack();
            gameOverMenu.pack();
            mainMenu.pack();

            credits.location = new Vector2(0, height);
            credits.size = new Vector2(width, height);

            // Center game over menu
            gameOverMenu.location = new Vector2(width/2 - gameOverMenu.preferredSize.X / 2, height/2 - gameOverMenu.preferredSize.Y / 2);

            current = null;
            
            
        }

        

        public void OnUp()
        {

        }

        public override void DrawMenu(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (current == pauseMenu || current == mainMenu)
            {
                Camera camera = graphics.camera;
                Rectangle rect = new Rectangle(0, 0, camera.screenWidth, camera.screenHeight);
                spriteBatch.Draw(guiTextures[11], rect, Color.White);
            }

            base.DrawMenu(spriteBatch);
        }


        // When you are in magic stance and you cannot cast a fire pillar at your cursor's location, turn the cursor red
        public override void DrawCursor(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            MouseState ms = tileEngine.inputComponent.currentMouseState;
       
            Color BLAH = Color.White;
            if (tileEngine.world != null && (tileEngine.world as GameWorld).player != null)
            { 
               if((tileEngine.world as GameWorld).player != null && !tileEngine.world.hasLineOfSight((tileEngine.world as GameWorld).player.position, tileEngine.world.getMouseWorldVector(),false) && (((tileEngine.world as GameWorld).player.myBehavior as PlayerBehavior).stance == PlayerBehavior.Stance.magic))
                   BLAH = Color.Red;
               spriteBatch.Draw(engineTextures[0], new Vector2((float)ms.X, (float)ms.Y), BLAH);
            }
        }
    }
}
