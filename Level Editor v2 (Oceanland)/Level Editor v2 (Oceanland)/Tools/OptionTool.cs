using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
    public class OptionToolGUI : ToolOptionsGUI
    {
        public ListContainer[] holdThis;
        public GUI.CheckBoxContainer solid;
        public GUI.CheckBoxContainer opaque;
        public GUI.CheckBoxContainer grid;
        public Label coords;  // Coordinates of the current tile

        public OptionToolGUI(EditorGUI theInterface)
            : base(theInterface)
        {
            orientation = Orientation.VERTICAL;
            holdThis = new ListContainer[2];
            holdThis[0] = new ListContainer(theInterface);
            holdThis[1] = new ListContainer(theInterface);
            coords = new Label(theInterface);
            coords.forcedLocation = new Vector2(200, 0);

            solid = new GUI.CheckBoxContainer(theInterface, "Solidity Marker", true);
            solid.children[0].clickEvent += (lol) => { (theInterface.tileEngine as Editor).solid = !(theInterface.tileEngine as Editor).solid; };
            opaque = new GUI.CheckBoxContainer(theInterface, "Opaque Marker", true);
            opaque.children[0].clickEvent += (lol) => { (theInterface.tileEngine as Editor).opaque = !(theInterface.tileEngine as Editor).opaque; };
            grid = new GUI.CheckBoxContainer(theInterface, "Grid", true);
            grid.children[0].clickEvent += (lol) => { (theInterface.tileEngine as Editor).grid = !(theInterface.tileEngine as Editor).grid; };

            Add(solid);
            Add(opaque);
            Add(grid);
            Add(coords);
        }

        //public override Vector2 preferredSize { get { return this.preferredSize; } }
    }

    public class OptionTool : Tool
    {

        public OptionTool(Editor editor)
            : base(editor)
        {
            isDragging = false;
            gui = new OptionToolGUI(editor.graphicsComponent.guiComponent as EditorGUI);
            gui.size = new Vector2(300, 200);
        }

        public override void doMoveAction()
        {
            Tile victim = getMouseWorldPos();
            OptionToolGUI g = gui as OptionToolGUI;
            if (victim == null)
                g.coords.text = null;
            else 
                g.coords.text = "X: " + victim.xIndex + ", Y: " + victim.yIndex;
        }


    }
}
