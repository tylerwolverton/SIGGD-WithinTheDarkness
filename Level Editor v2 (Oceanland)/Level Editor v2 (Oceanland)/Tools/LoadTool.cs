using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{

    public class LoadToolGUI : ToolOptionsGUI
    {
        public TextArea name;
        public Button load;

        public LoadToolGUI(EditorGUI theInterface)
            : base(theInterface)
        {
            load = new Button(theInterface, " Load ");
            load.margin = 2;
            load.clickEvent += (lol) => { (theInterface.tileEngine as Editor).OnStart(name.text); };
            name = new TextArea(theInterface);
            name.bgColor = Color.AntiqueWhite;
            name.forcedSize = new Vector2(120, 15);

            Add(name);
            Add(load);

        }

        //public override Vector2 preferredSize { get { return this.preferredSize; } }
    }

    public class LoadTool : Tool
    {
        public LoadTool(Editor editor)
            : base(editor)
        {
            isDragging = false;;
            gui = new LoadToolGUI(editor.graphicsComponent.guiComponent as EditorGUI);
            gui.size = new Vector2(300, 200);
        }

        public override void doLeftDownAction()
        {

        }

        public override void doMoveAction()
        {
            
        }

        public override void doLeftUpAction()
        {

        }
    }
}
