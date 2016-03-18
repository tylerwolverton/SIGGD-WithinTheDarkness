using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.GUI;
using Microsoft.Xna.Framework.Input;
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
    public class EditorGUI : GUIComponent
    {

        public EditorGUILayout egl;

        public EditorGUI(GraphicsComponent theGraphics)
            : base(theGraphics)
        {
            egl = new EditorGUILayout(this);
            
        }

        public float fullscale;
        public float notfullscale;
        



        public override void Initialize()
        {
            base.Initialize();
            float width = graphics.camera.screenWidth / graphics.camera.scale;
            float height = graphics.camera.screenHeight / graphics.camera.scale;
            EditorInput input = tileEngine.inputComponent as EditorInput;
            
            
            
            
            egl.size = new Vector2(width, .25f*height);
            egl.location = new Vector2(0, .75f * height);
            egl.Initialize();
            
            
           

      

            egl.pack();
            current = egl;
            focus = current;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void DrawMenu(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.DrawMenu(spriteBatch);
        }
    }
}