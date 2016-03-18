using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Engine.GUI
{
    class StatusBar : GUIItem
    {
        private const float barBegin = 38;
        private const float barEnd = 94;

        private Texture2D overlay;
        private Texture2D badassBar;
        private Texture2D healthBar;
        private Texture2D manaBar;
        private Texture2D goldOverlay;
        private Label killCount;

        public override Vector2 preferredSize
        {
            get {
                return new Vector2(overlay.Width, overlay.Height);
            }
        }

        public StatusBar(GameGUIComponent guiComponent)
            : base(guiComponent)
        {
            overlay = guiComponent.guiTextures[6];
            goldOverlay = guiComponent.guiTextures[7];
            healthBar = guiComponent.guiTextures[8];
            manaBar = guiComponent.guiTextures[9];
            badassBar = guiComponent.guiTextures[10];
            killCount = new Label(guiComponent, "Kills: 0");
            killCount.color = Color.AntiqueWhite;
        }


        public int displayKill = 0;
        public override void draw(SpriteBatch spriteBatch, Vector2 absParLoc)
        {

            //Are you kidding me? Redo this.


            Vector2 absLoc = absParLoc + location;
            Player p = ((guiComponent.tileEngine as Graven).world as GameWorld).player;

            if (guiComponent.tileEngine.world.worldName.Substring(0, 3).Equals("012"))
            {
                if (displayKill < p.killCount)
                    displayKill++;
                killCount.text = "Kills: " + displayKill;
                killCount.draw(spriteBatch, absLoc + new Vector2(38,24));
            }

            spriteBatch.Draw(healthBar, absLoc, new Rectangle(0, 0, (int)Math.Floor(barBegin + p.life.life/p.life.maxlife*(barEnd-barBegin)), healthBar.Height), Color.White);
            if(!(p.myBehavior as PlayerBehavior).assPermission)
                spriteBatch.Draw(manaBar, absLoc, new Rectangle(0, 0, (int)Math.Floor(barBegin + p.mana / Constants.LUCY_MAXMANA * (barEnd - barBegin)), manaBar.Height), Color.White);
            else
                spriteBatch.Draw(badassBar, absLoc, new Rectangle(0, 0, (int) Math.Floor(barBegin + p.badassMeter * (barEnd - barBegin)), badassBar.Height), Color.White );
            spriteBatch.Draw(overlay, absLoc, Color.White);
            spriteBatch.Draw(goldOverlay, absLoc, new Rectangle(0, 0, (p.isBadass ? goldOverlay.Width : (int)Math.Floor(barBegin + p.badassMeter * (barEnd - barBegin))), goldOverlay.Height), Color.White);
        }
    }
}
