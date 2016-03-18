using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{

    public class FlagToolGUI : ToolOptionsGUI
    {
        public GUI.RadioButtonContainer solid;
        public GUI.RadioButtonContainer opacity;
        public GUI.RadioButtonContainer flaggard;

        // Enter actions/tags to change current tile
        public ListContainer textBoxes;
        public TextArea tagBox;
        public TextArea actionBox;

        // Display current tile's current action/tag
        public ListContainer labels;
        public Label actionLabel;
        public Label tagLabel;

        public FlagToolGUI(EditorGUI theInterface)
            : base(theInterface)
        {
           
            string[] x;
            x = new string[3];
            x[0] = "On";
            x[1] = "Off";
            x[2] = "Ignore";
            string[] y;
            y = new string[3];
            y[0] = "On";
            y[1] = "Off";
            y[2] = "Ignore";

            tagBox = new TextArea(theInterface);
            tagBox.text = "x";
            actionBox = new TextArea(theInterface);
            actionBox.text = "x";

            tagLabel = new Label(theInterface, "Tag: ");
            actionLabel = new Label(theInterface, "Action: ");

            solid = new GUI.RadioButtonContainer(theInterface, x, 2);
            opacity = new GUI.RadioButtonContainer(theInterface, y, 2);
            textBoxes = new ListContainer(theInterface, Orientation.VERTICAL);
            labels = new ListContainer(theInterface, Orientation.VERTICAL);

            solid.forcedLocation = Vector2.Zero;
            opacity.forcedLocation = new Vector2(100,0);

            Add(solid);
            Add(opacity);

            textBoxes.Add(actionBox);
            textBoxes.Add(tagBox);
            Add(textBoxes);
            textBoxes.forcedLocation = new Vector2(200, 0);

            labels.Add(actionLabel);
            labels.Add(tagLabel);
            Add(labels);
            labels.forcedLocation = new Vector2(275, 0);

            tagBox.bgColor = Color.White;
            actionBox.bgColor = Color.White;

            this.forcedSize = new Vector2(300, 200);
       
            solid.forcedSize = new Vector2(100, 200);
            opacity.forcedSize = new Vector2(100, 200);
            tagBox.forcedSize = new Vector2(50, 20);
            actionBox.forcedSize = new Vector2(50, 20);
        }
    }

    public class FlagTool : Tool
    {
        private byte opacity_flag = (byte)0x01;

        private byte addFlag;
        private byte removeFlag;
        private byte flag;

        public FlagTool(Editor editor)
            : base(editor)
        {
            isDragging = false;
            addFlag = 0;
            removeFlag = 0;
            gui = new FlagToolGUI(editor.graphicsComponent.guiComponent as EditorGUI);
            gui.size = new Vector2(300,200);
        }

        public byte getSolid (Tile target)
        {
            if ((gui as FlagToolGUI).solid.selected == 0)
                return 1;
            if ((gui as FlagToolGUI).solid.selected == 1)
                return 0;
            else // if ((gui as FlagToolGUI).solid.selected == 2)
                return (target.solid) ? (byte)1 : (byte)0;
        }

        public byte getActiveFlags()
        {
            if ((gui as FlagToolGUI).opacity.selected == 1)
            {
                return 0;
            }
            else if ((gui as FlagToolGUI).opacity.selected == 0)
            {
                return 1;
            }
            return flag;
        }
        
        public override void doLeftDownAction()
        {
            isDragging = true;
            Tile target = getMouseWorldPos();
            MapFile.TileData td;

            if (target != null)
            {
                td = editor.world.file.tiles[0,target.xIndex, target.yIndex];

                td.flags = target.flags;
                td.tileIndex = (byte)target.imgIndex;
                td.tileSetIndex = 0;
                
                td.solid = getSolid(target);

                if ((gui as FlagToolGUI).opacity.selected == 0)
                    td.flags = 1;
                else if ((gui as FlagToolGUI).opacity.selected == 1)
                    td.flags = 0;
                else
                    td.flags = target.flags;

                try {
                    ushort action = ushort.Parse((gui as FlagToolGUI).actionBox.text);
                    td.action = action;
                } catch { }

                try {
                    ushort tag = ushort.Parse((gui as FlagToolGUI).tagBox.text);
                    td.tag = tag;
                } catch { }

                target.action = td.action;
                target.tag = td.tag;
                target.solid = (td.solid == 1);
                target.flags = td.flags;
                
                (editor.world as EditorWorld).file.tiles[0, target.x / Tile.size, target.y / Tile.size] = td;
            }
        }

        public override void doMoveAction()
        {
            
            Tile target = getMouseWorldPos();
            MapFile.TileData td;

            if (target == null) {
                (gui as FlagToolGUI).actionLabel.text = "Action:";
                (gui as FlagToolGUI).tagLabel.text = "Tag:";
            } else {
                (gui as FlagToolGUI).actionLabel.text = "Action: " + target.action;
                (gui as FlagToolGUI).tagLabel.text = "Tag: " + target.tag;
            }

            if (target != null && isDragging)
            {
                td = editor.world.file.tiles[0, target.xIndex, target.yIndex];
                try
                {
                    ushort action = ushort.Parse((gui as FlagToolGUI).actionBox.text);
                    td.action = action;
                }
                catch { }

                try
                {
                    ushort tag = ushort.Parse((gui as FlagToolGUI).tagBox.text);
                    td.tag = tag;
                }
                catch { }

                target.action = td.action;
                target.tag = td.tag;
                td.flags = target.flags;
                td.tileIndex = (byte)target.imgIndex;
                td.tileSetIndex = 0;
                
                td.solid = getSolid(target);
                
                if((gui as FlagToolGUI).opacity.selected == 0)
                    td.flags = 1;
                if((gui as FlagToolGUI).opacity.selected == 1)
                    td.flags = 0;

                editor.world.tileArray[target.x / Tile.size, target.y / Tile.size].solid = (td.solid == 1);
                
                editor.world.tileArray[target.x / Tile.size, target.y / Tile.size].flags = td.flags;

                (editor.world as EditorWorld).file.tiles[0, target.x / Tile.size, target.y / Tile.size] = td;
            }
        }
        
        public override void doLeftUpAction()
        {
            isDragging = false;
        }
    }
}
