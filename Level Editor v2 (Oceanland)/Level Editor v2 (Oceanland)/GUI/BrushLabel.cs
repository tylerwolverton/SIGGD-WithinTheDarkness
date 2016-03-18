using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Level_Editor_v2__Oceanland_.Brushes;
using Microsoft.Xna.Framework;

namespace Level_Editor_v2__Oceanland_.GUI
{
    public class BrushLabel : GUIItem
    {
        private Brush brush;  // Current brush displayed on this label

        public BrushLabel(GUIComponent gui, Brush b) : base(gui) {
            brush = b;
        }

        // Sets the brush displayed on this label
        public void setBrush(Brush b)
        {
            brush = b;
        }

        public override Vector2 preferredSize
        {
            get
            {
                if (brush == null)
                    return Vector2.Zero; 

                return new Vector2(brush.values.GetLength(0)*Tile.size, brush.values.GetLength(1)*Tile.size);
            }
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.Vector2 absParLoc)
        {
            TextureSet tiles = guiComponent.tileEngine.world.tileTextureSet;  // Texture set full o' tiles
            Vector2 loc = location + absParLoc;  // Location of this item

            // Brush width, height
            int bWidth = brush.values.GetLength(0), bHeight = brush.values.GetLength(1); 

            // Draw each of the tiles in the image
            for (int x = 0; x < bWidth; x++) {
                for (int y = 0; y < bWidth; y++) {
                    if (brush.values[x,y] < 0)
                        continue;

                    spriteBatch.Draw(tiles[brush.values[x,y]], new Vector2(loc.X + x*Tile.size, loc.Y + y*Tile.size), Color.White);
                }
            }
        }
    }
}
