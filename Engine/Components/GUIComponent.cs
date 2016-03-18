using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Engine.GUI;
using System.Diagnostics;

namespace Engine
{

    public class GUIComponent : Component
    {
        public readonly GraphicsComponent graphics;
        public readonly ResourceComponent resources;

        public GUIItem focus { get; set; }  //Which GUI Item is currently in focus?
        
        public SpriteFont font;
        public TextureSet guiTextures { get; protected set; }   //The set of all images used by the GUI.

        //The inputs accepted by the GUI at all times
        public enum StaticBindings
        {
            CLICK,
            RIGHTCLICK,
            KEYPRESS,
        }
        public InputBinding[] staticBindings;

        //The container currently in focus
        private GUIContainer current_;
        public GUIContainer current
        {
            get
            {
                return current_;
            }
            set
            {
                InputComponent input = graphics.tileEngine.inputComponent;

                // Entering the gui
                if (value != null && !input.hasSaved())
                {
                    input.save();

                    if (value.bindings != null) {
                        input.replace(value.bindings);
                    }
                    input.append(staticBindings);
                } else if (value != null) {  // Changing the Gui
                    if (value.bindings != null) {  // 
                        input.replace(value.bindings);
                        input.append(staticBindings);
                       
                    }

                    // If not null, we have already appended our static bindings (since we're already in the gui)
                } else {   // Exiting the GUI
                    input.restore();
                }

                current_ = value;
            }
        }

        public GUIComponent(GraphicsComponent theGraphics) : base(theGraphics.tileEngine)
        {

            this.graphics = theGraphics;
            this.resources = tileEngine.resourceComponent;

            staticBindings = new InputBinding[Enum.GetValues(typeof(StaticBindings)).Length];
        }

        public override void Initialize()
        {
            try
            {
                font = resources.contentManager.Load<SpriteFont>("Fonts/Courier New");
            }
            catch (Exception e) { throw new System.ArgumentException("GUIComponent requires 'Courier New.spritefont' in [Game]Content's 'Fonts' directory!", "GUIComponent"); }

            //Load the textures used by the GUI
            guiTextures = resources.getTextureSet(Path.Combine("000_GUI"));            

            //Set up the inputs used by the GUI.
            this[StaticBindings.CLICK] = new SinglePressBinding(tileEngine.inputComponent,
                    null, null, InputComponent.MouseButton.LEFT);
            (this[StaticBindings.CLICK] as SinglePressBinding).downEvent += onClick;
            (this[StaticBindings.CLICK] as SinglePressBinding).upEvent += releaseClick;

            this[StaticBindings.RIGHTCLICK] = new SinglePressBinding(tileEngine.inputComponent,
                   null, null, InputComponent.MouseButton.RIGHT);

            (this[StaticBindings.RIGHTCLICK] as SinglePressBinding).downEvent += onRightClick;
            (this[StaticBindings.RIGHTCLICK] as SinglePressBinding).upEvent += releaseRightClick;

            this[StaticBindings.KEYPRESS] = new TextBinding(tileEngine.inputComponent);
            (this[StaticBindings.KEYPRESS] as TextBinding).charEntered += onKeypress;
        }
        
        public virtual void DrawMenu(SpriteBatch spriteBatch)
        {

            if (current == null)
                return;

            current.draw(spriteBatch, new Vector2(0,0));
        }

        public virtual void DrawCursor(SpriteBatch spriteBatch)
        {
            MouseState ms = tileEngine.inputComponent.currentMouseState;
            try
            {
                spriteBatch.Draw(guiTextures[0], new Vector2((float)ms.X, (float)ms.Y), Color.White);
            }
            catch (Exception e) { throw new System.ArgumentException("GUIComponent requires '000_cursor.png' in [Game]Content's 'GUI' directory!", "GUIComponent"); }
        }

        public override void Draw()
        {

            SpriteBatch spriteBatch = graphics.spriteBatch;

            Matrix scale = Matrix.CreateScale(graphics.camera.scale - graphics.camera.scaleChange);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, scale);
            SamplerState samplerstate = new SamplerState();
            samplerstate.Filter = TextureFilter.Point;
            spriteBatch.GraphicsDevice.SamplerStates[0] = samplerstate;
            DrawMenu(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin();
            DrawCursor(spriteBatch);
            spriteBatch.End();
        }
        public float fullscale;
        public float notfullscale;
        public InputBinding this[StaticBindings bind]
        {
            get
            {
                return staticBindings[(int)bind];
            }

            set
            {
                staticBindings[(int)bind] = value;
            }
        }
        protected void releaseClick()
        {
            MouseState mPos = tileEngine.inputComponent.currentMouseState;
            if (current != null)
            {
                float scale = graphics.camera.scale-graphics.camera.scaleChange;
                current.releaseClick(new Vector2(mPos.X / scale, mPos.Y / scale) - current.location);
            }
        }
        protected void onClick()
        {
            MouseState mPos = tileEngine.inputComponent.currentMouseState;
            if (current != null)
            {
                float scale = graphics.camera.scale - graphics.camera.scaleChange;
                current.handleClick(new Vector2(mPos.X / scale, mPos.Y / scale) - current.location);
            }
        }
        protected void releaseRightClick()
        {
            MouseState mPos = tileEngine.inputComponent.currentMouseState;
            if (current != null)
            {
                float scale = graphics.camera.scale - graphics.camera.scaleChange;
                current.releaseRightClick(new Vector2(mPos.X / scale, mPos.Y / scale) - current.location);
            }
        }
        protected void onRightClick()
        {
            MouseState mPos = tileEngine.inputComponent.currentMouseState;
            if (current != null)
            {
                float scale = graphics.camera.scale - graphics.camera.scaleChange;
                current.handleRightClick(new Vector2(mPos.X / scale, mPos.Y / scale) - current.location);
            }
        }

        protected void onKeypress(char c)
        {
            if (focus != null) {
                focus.handleKeyPress(c);
            } 
            else if (current != null)
            {
                current.handleKeyPress(c);
            }
        }
    }
}
