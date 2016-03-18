using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Diagnostics;

namespace Engine
{
    class EditorGraphics : GraphicsComponent
    {

        const bool FULLSCREEN = false;
        static Vector2 screenSize = new Vector2(500, 400);

        public bool grid = true;

        public float scrollSpeed = 3f;
        public EditorGraphics(MirrorEngine theEngine) : base(theEngine, screenSize, FULLSCREEN)
        {
            AMBIENT = 0f;
            guiComponent = new EditorGUI(this);
        }
        public override void Initialize()
        {
            base.Initialize();

        }
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            EditorWorld editorWorld = tileEngine.world as EditorWorld;

            base.Update(gameTime);
            if ((tileEngine as Editor).activeTool.active)
                (tileEngine as Editor).activeTool.doMoveAction();
            // Move camera position to center of player position
            
            
            AxisBinding xaim, yaim, xmove, ymove;
            EditorInput input = tileEngine.inputComponent as EditorInput;

            AxisBinding.origin = (new Vector2(.5f * camera.screenWidth , .4f * camera.screenHeight ));
            //AxisBinding.origin = new Vector2(0,0);
            xaim = input[EditorInput.EditBindings.XAIM] as AxisBinding;
            yaim = input[EditorInput.EditBindings.YAIM] as AxisBinding;
            xmove = input[EditorInput.EditBindings.XMOVE] as AxisBinding;
            ymove = input[EditorInput.EditBindings.YMOVE] as AxisBinding;
            Debug.WriteLine(xaim.position + "  " + yaim.position + "\n");
            
            camera.velocity = Vector2.Zero;
        
            camera.velocity = new Vector2(xaim.position, -yaim.position);
            camera.velocity *= 20f; // scale by speed
            if (camera.velocity.Length() > 2.5 && !editorWorld.scrollLock)
            {
                if (camera.position.X + camera.velocity.X < 0 || camera.position.X + camera.velocity.X >= tileEngine.world.width * Tile.size)
                    camera.velocity = new Vector2(0, camera.velocity.Y);
                if (camera.position.Y + camera.velocity.Y < 0 || camera.position.Y + camera.velocity.Y >= tileEngine.world.height * Tile.size)
                    camera.velocity = new Vector2(camera.velocity.X, 0);
                if (fullscreen)
                    camera.position += camera.velocity * 5 / (camera.scale - camera.scaleChange);
                if (!fullscreen)
                    camera.position += camera.velocity / (camera.scale - camera.scaleChange);
            }
            else
            {
                if (editorWorld.scrollLock && !(guiComponent.focus is TextArea))
                {
                    camera.velocity = new Vector2(xmove.position, -ymove.position);

                    if (camera.position.X + camera.velocity.X < 0 || camera.position.X + camera.velocity.X >= tileEngine.world.width * Tile.size)
                        camera.velocity = new Vector2(0, camera.velocity.Y);
                    if (camera.position.Y + camera.velocity.Y < 0 || camera.position.Y + camera.velocity.Y >= tileEngine.world.height * Tile.size)
                        camera.velocity = new Vector2(camera.velocity.X, 0);
                    if (fullscreen)
                        camera.position += camera.velocity * 15 / (camera.scale - camera.scaleChange);
                    if (!fullscreen)
                        camera.position += camera.velocity * 15 / (camera.scale - camera.scaleChange);
                }
            }



        }


        public override void Draw()
        {
            EditorWorld world = tileEngine.world as EditorWorld;

            Clear();

            if (world != null)
            {
                world.sortActors();

                BeginCamDraw(SpriteSortMode.Texture);
                DrawTiles(world);
                EndCamDraw();
                BeginCamDraw(SpriteSortMode.Deferred);
                DrawSprites(world);
                EndCamDraw();
            }

            guiComponent.Draw();
        }

        protected override void Clear()
        {
            graphics.GraphicsDevice.Clear(Color.Blue);
        }

        public override void DrawTiles(World world)
        {
            

            RectangleF viewRect = camera.ViewRect;
            
            // Calculate the range of tiles to consider for drawing
            int tileLeft = (int)viewRect.left / Tile.size;
            int tileTop = (int)viewRect.top / Tile.size;
            int tileRight = (int)viewRect.right / Tile.size;
            int tileBottom = (int)viewRect.bottom / Tile.size;

            // Make sure in tile bounds
            if (tileLeft < 0) tileLeft = 0;
            if (tileTop < 0) tileTop = 0;
            if (tileRight >= world.width) tileRight = world.width - 1;
            if (tileBottom >= world.height) tileBottom = world.height - 1;
            if (tileRight < 0 || tileBottom < 0) return; 


            for (int x = tileLeft; x <= tileRight; x++)
            {
                for (int y = tileTop; y <= tileBottom; y++)
                {
                    Tile t = world.tileArray[x, y];

                    

                    spriteBatch.Draw(world.tileTextureSet[t.imgIndex], new Vector2(t.x, t.y), Color.White);
                    if(t.solid&&(world.tileEngine as Editor).solid)
                        spriteBatch.Draw(guiComponent.guiTextures[27], new Vector2(t.x, t.y), Color.White);
                    if (t.opaque && (world.tileEngine as Editor).opaque)
                        spriteBatch.Draw(guiComponent.guiTextures[28], new Vector2(t.x + 8, t.y + 8), Color.White);
                    if (grid && (world.tileEngine as Editor).grid)
                        spriteBatch.Draw(guiComponent.guiTextures[29], new Vector2(t.x, t.y), Color.White);
                    if (t.tag != 0)
                        spriteBatch.Draw(guiComponent.guiTextures[52], new Vector2(t.x + 8, t.y), Color.White);
                    //t.MAKERED = false;
                }
            }
        }
    }
    
}
