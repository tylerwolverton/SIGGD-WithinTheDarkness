using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Level_Editor_v2__Oceanland_.Brushes;
using Level_Editor_v2__Oceanland_.GUI;
using System.Diagnostics;

namespace Engine
{
    // GUI for the stamp tool
    public class StampToolGUI : ToolOptionsGUI
    {
        private BrushLabel brushImg;  // The image for this brush   
        public Brush currentBrush;   // Current brush
        private int currentIndx;      // Index for the brush

        private Label name;           // Name label 

        // Constructor
        public StampToolGUI(EditorGUI theInterface)
            : base(theInterface)
        {
            this.forcedSize = new Vector2(300, 200);
            currentBrush = Brush.brushes[currentIndx = 0];
            brushImg = new BrushLabel(theInterface, currentBrush);
            brushImg.forcedLocation = new Vector2(10, 30);
            Add(brushImg);

            Label up = new Label(theInterface, "Up");
            up.forcedLocation = new Vector2(150, 20);
            up.clickEvent += (pos) =>
            {
                if (currentIndx != Brush.brushes.Count-1)
                    setBrush(++currentIndx);
            };
            Add(up);

            name = new Label(theInterface, currentBrush.name);
            name.forcedLocation = new Vector2(200, 20);
            Add(name);

            Label down = new Label(theInterface, "Down");
            down.forcedLocation = new Vector2(150, 50);
            down.clickEvent += (pos) =>
            {
                if (currentIndx != 0)
                    setBrush(--currentIndx);
            };
            Add(down);
        }

        // Set the current brush
        private void setBrush(int index)
        {
            currentBrush = Brush.brushes[index];
            brushImg.setBrush(currentBrush);
            name.text = currentBrush.name;
            currentIndx = index;

            pack();
        }
    }
    public class StampTool : Tool
    {
        // The uppperleft corner of the most recently drawn brush instance  
        private Tile origin; 

        public StampTool(Editor editor)
            : base(editor)
        {
            isDragging = false;

            // Discover all the brushes, and set the current to the first (for testing)
            Brush.discoverBrushes();

            gui = new StampToolGUI(editor.graphicsComponent.guiComponent as EditorGUI);
            gui.size = new Vector2(300,200);
        }

        // Callback for left mouse button down
        public override void doLeftDownAction()
        {
            Tile uLeft = getMouseWorldPos(); // Original position for the brush (upperLeft)

            if (uLeft != null && !isDragging)
            {
                origin = uLeft;

                applyBrush(origin.xIndex, origin.yIndex);

                isDragging = true;
            }
        }

        // Callback for mouse moved
        // Dragging the brush tool will round to multiple of the brush size, allowing you to paint an area with the brush
        public override void doMoveAction()
        {
            Tile victim = getMouseWorldPos();  // Tile the cursor is currently above
            Brush currentBrush = (gui as StampToolGUI).currentBrush;

            if (victim != null && isDragging)
            {
                // brushwidth, brushheight
                int brushW = currentBrush.values.GetLength(0), brushH = currentBrush.values.GetLength(1);

                // Skip if still within last drawn brush segment
                int offsetX = victim.xIndex - origin.xIndex, offsetY = victim.yIndex - origin.yIndex;
                if (offsetX >= 0 && offsetX < brushW && offsetY >= 0 && offsetY < brushH )
                    return;

                // Apply the brush to the nearest tile aligned with the origin
                int modX = origin.xIndex % brushW, modY = origin.yIndex % brushH;  // Offset of the upperleftmost possible brush from (0,0)
                victim = editor.world.tileArray[victim.xIndex - (victim.xIndex % brushW) + modX,
                                                victim.yIndex - (victim.yIndex % brushH) + modY];

                applyBrush(victim.xIndex, victim.yIndex);

                origin = victim;
            }
        }

        // Callback for left mouse button up 
        public override void doLeftUpAction()
        {
            isDragging = false;
        }

        // Apply's brush to map location (xIndex, yIndex)
        private void applyBrush(int xIndex, int yIndex)
        {
            Brush currentBrush = (gui as StampToolGUI).currentBrush;

            // brushwidth, brushheight
            int brushW = currentBrush.values.GetLength(0), brushH = currentBrush.values.GetLength(1);

            // Loop to apply brush to tiles (i and j are brush indices)
            for (int x = xIndex, i = 0; i < brushW && x < editor.world.width; x++, i++) {
                for (int y = yIndex, j = 0; j < brushH && y < editor.world.height; y++, j++) {
                    Tile t = editor.world.tileArray[x, y];  // Current tile being considered
                    MapFile.TileData td = (editor.world as EditorWorld).file.tiles[0, x, y];  // Data for current tile being considered

                    // Retrieve value from brush
                    int brushVal = currentBrush.values[i, j];  // Brush value for the current tile
                    if (brushVal < 0)
                        continue;

                    // Write tile data
                    td.tileIndex = (byte)brushVal;
                    td.tileSetIndex = 0;   // NOTE: This may not be true in the future. May have to alter brushes to support multiple tilesets
                    (editor.world as EditorWorld).file.tiles[0, x, y] = td;

                    // Write to world
                    editor.world.tileArray[x, y].imgIndex = brushVal;
                }
            }
        }

        public override void undoAction(ToolAction action)
        {
            foreach (TextureData chg in (action as StampToolAction).changed)
            {
                editor.world.file.tiles[0, chg.x, chg.y].tileIndex = chg.tileIndex;
                editor.world.file.tiles[0, chg.x, chg.y].tileSetIndex = chg.tileSetIndex;
            }
            editor.world.initTiles();
        }
        public override void redoAction(ToolAction action)
        {
            foreach (TextureData chg in (action as StampToolAction).changed)
            {
                editor.world.file.tiles[0, chg.x, chg.y].tileIndex = (action as StampToolAction).texture[1];
                editor.world.file.tiles[0, chg.x, chg.y].tileSetIndex = (action as StampToolAction).texture[0];
            }
            editor.world.initTiles();
        }

        // Undo record for the stamp tool
        public class StampToolAction : ToolAction
        {
            public LinkedList<TextureData> changed; // original vals of changed tiles
            public byte[] texture; // data necessary to recreate action once undone

            public StampToolAction(LinkedList<TextureData> changed, byte[] texture, Tool parent)
                :base(parent)
            {
                this.changed = changed;
                this.texture = texture;
            }
        }
    }
}
