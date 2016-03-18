using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Diagnostics;

/*
 * The loading and storing of the memory should be moved to ResourceComponent.
 */

namespace Engine
{
    public class GraphicsComponent : Component
    {
        protected float AMBIENT = 0.0f;

        public GraphicsDeviceManager graphics { get; set; }
        public SpriteBatch spriteBatch { get; set; }
        public Camera camera { get; set; }
        public GUIComponent guiComponent;
        public Matrix game2screen;

        public Vector2 windowedSize;
        public bool fullscreen;

        // Clear color (to avoid unnecessary work)
        public static readonly Color clear = new Color(0, 0, 0, 0);

        // Global tint, with opacity
        public Color tint = clear;

        // Tint tex
        private Texture2D tintTex;
        

        // Number of ms since beginning of fade, number of ms at end of fade (fadeend is zero if not fading)
        private uint fadecount;
        private uint fadeend;
        // Fade color
        private Color fadeColor;

        // Action to fire when fade is over
        public delegate void Action();
        private Action fadedone;


        public GraphicsComponent(MirrorEngine theEngine, Vector2 windowedSize, bool fullscreen) : base(theEngine)
        {
            graphics = new GraphicsDeviceManager(theEngine);
            this.windowedSize = windowedSize;
            this.fullscreen = fullscreen;
            GameTime a;

            graphics.PreferredBackBufferWidth = (int) windowedSize.X;
            graphics.PreferredBackBufferHeight = (int) windowedSize.Y;

            if (fullscreen)
            {
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.ToggleFullScreen();
            }
        }

        public override void Initialize()
        {

            // BEGIN TEST CODE
            camera = new Camera(this, new Vector2(0, 0), graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height, 300);
            
            camera.velocity = new Vector2(0, 0);
            camera.position = new Vector2(0, 0);
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            // END TEST CODE

            if (guiComponent == null)
                throw new Exception("GRAPHICS COMPONENT ERROR: GUIComponent not specified before GraphicsComponent Initialization");
           
            guiComponent.Initialize();

            // White 1x1 texture, for tinting screen
            tintTex = tileEngine.resourceComponent.textureSets["GUI"][10];
        }

        protected virtual void Clear()
        {
            graphics.GraphicsDevice.Clear(Color.Black);
        }

        public override void Update(GameTime gameTime)
        {
            guiComponent.Update(gameTime);

            // Update fade (if there is one)
            if (fadeend != 0) {
                fadecount += (uint)gameTime.ElapsedGameTime.Milliseconds;

                // Alpha becomes proportion of fade time elapsed so far to total fade time
                // We have to do this goofy multiplication because XNA uses premultiplied alpha
                tint = fadeColor * ((float)fadecount / (float)fadeend);

                // Handle fade end
                if (fadecount >= fadeend) {
                    fadeend = 0;  // Set the state of the "fader" to "not fading"
                    if (fadedone != null) {
                        fadedone();
                    }
                }
            }
        }

        public virtual void BeginCamDraw(SpriteSortMode sort)
        {

            RectangleF viewRect = camera.ViewRect;

            // Set up game to screen translation matrix
            // This is what should be added to game position 0,0 to make it become it's cooresponding screen position.
            game2screen = Matrix.CreateTranslation(-viewRect.topLeft.X, -viewRect.topLeft.Y, 0) * Matrix.CreateScale(camera.scale);
            Vector2 v = Vector2.Transform(Vector2.Zero, game2screen);
            spriteBatch.Begin(sort, null, null, null, null, null, game2screen);
            SamplerState samplerstate = new SamplerState();
            samplerstate.Filter = TextureFilter.Point;
            spriteBatch.GraphicsDevice.SamplerStates[0] = samplerstate;
        }

        public virtual void EndCamDraw()
        {
            spriteBatch.End();
        }

        public virtual void DrawTiles(World world)
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
                    if (t.val < 0.0001f ) continue; // if not in player vision, don't draw
                    Color tint = t.glow;
                    //if (tint.R < 5 && tint.G < 5 && tint.B < 5) continue; // if near black, don't draw
                    if (t.opaque)
                    {
                        spriteBatch.Draw(world.tileTextureSet[t.imgIndex], new Vector2(t.x, t.y), (t.MAKERED) ? Color.Red : tint);
                    }
                    else
                    {
                        Color tint1 = Color.Lerp(t.left  == null || t.left.opaque ? tint : t.left.glow, t.up == null ? tint : t.up.glow, 0.5f);
                        Color tint2 = Color.Lerp(t.right == null || t.right.opaque ? tint : t.right.glow, t.up == null ? tint : t.up.glow, 0.5f);
                        Color tint3 = Color.Lerp(t.left  == null || t.left.opaque ? tint : t.left.glow, t.down == null ? tint : t.down.glow, 0.5f);
                        Color tint4 = Color.Lerp(t.right == null || t.right.opaque ? tint : t.right.glow, t.down == null ? tint : t.down.glow, 0.5f);

                        Texture2D tex = world.tileTextureSet[t.imgIndex];
                        int w = Tile.size / 2, h = Tile.size / 2;
                        spriteBatch.Draw(tex, new Vector2(t.x + 0, t.y + 0), new Rectangle(0, 0, w, h),
                            (t.MAKERED) ? Color.Red : Color.Lerp(tint, tint1, 0.5f));
                        spriteBatch.Draw(tex, new Vector2(t.x + w, t.y + 0), new Rectangle(w, 0, w, h),
                            (t.MAKERED) ? Color.Red : Color.Lerp(tint, tint2, 0.5f));
                        spriteBatch.Draw(tex, new Vector2(t.x + 0, t.y + h), new Rectangle(0, h, w, h),
                            (t.MAKERED) ? Color.Red : Color.Lerp(tint, tint3, 0.5f));
                        spriteBatch.Draw(tex, new Vector2(t.x + w, t.y + h), new Rectangle(w, h, w, h),
                            (t.MAKERED) ? Color.Red : Color.Lerp(tint, tint4, 0.5f));
                    }

                    //t.MAKERED = false;
                }
            }
        }

        public virtual void DrawSprites(World world)
        {

            // Draw Sprites
            foreach (Actor a in world.actors)
            {

                Vector2 spritePos = a.position + a.world2model;  // Get the upper left corner of the sprite
                Color tint;

                Vector2 aMiddle = new Vector2(a.textureSet[a.imgIndex].Width/2, a.textureSet[a.imgIndex].Height/2);
                Tile t = world.getTileAt(spritePos + aMiddle);
                if (t == null || t.val < 0.00001f)
                    t = world.getTileAt(spritePos);

                if (t == null || t.val < 0.00001f) continue; // if tile actor is on isn't visible, don't draw actor

                a.UpdateGlow(t);

                tint = a.changeGlow(AMBIENT);

                if (a.leaveTrail)
                {
                    for (int i = 0; i < a.trailLength; i++)
                    {
                        if (a.oldimgindex[i] != null)
                        {
                            Vector2 imagepos = a.oldpos[i] + a.world2model;
                            spriteBatch.Draw(a.textureSet[a.oldimgindex[i]],
new Vector2(imagepos.X + a.oldxoffset[i], imagepos.Y + a.oldyoffset[i]), // Have to snap position to nearest pixel coordinate to prevent bluring/vibrating
tint);
                        }
                    }
                }

                spriteBatch.Draw(a.textureSet[a.imgIndex],
                new Vector2(spritePos.X+a.xoffset, spritePos.Y+a.yoffset), // Have to snap position to nearest pixel coordinate to prevent bluring/vibrating
                tint);
            
            }
        }
 
        public override void Draw()
        {
            World world = tileEngine.world;

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

                // Draw tint (if there is one)
                if (!tint.Equals(clear)) {
                    spriteBatch.Begin();
                    spriteBatch.Draw(tintTex, new Rectangle(0, 0, camera.screenWidth, camera.screenHeight), tint);
                    spriteBatch.End();
                }
            }

            guiComponent.Draw();
        }

        public virtual void toggleFullscreen()
        {
            setFullscreen(!fullscreen);
        }


        public virtual void setFullscreen(bool fullscreen)
        {

            if (this.fullscreen == fullscreen) return;
            this.fullscreen = fullscreen;

            if (this.fullscreen)
            {
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                graphics.PreferredBackBufferWidth = (int) windowedSize.X;
                graphics.PreferredBackBufferHeight = (int) windowedSize.Y;
            }
            
            graphics.IsFullScreen = fullscreen;
            graphics.ApplyChanges();
            camera.setView();
            
            camera.scaleChange = 0;
        }

        // Set up a fade to color c, lasting "duration" milliseconds, and firing delegate "done" afterward
        public void fadeTo(Color c, uint duration, Action done = null)
        {
            fadeColor = c;
            
            fadecount = 0;
            fadeend = duration;
            fadedone = done;
        }
    }
}