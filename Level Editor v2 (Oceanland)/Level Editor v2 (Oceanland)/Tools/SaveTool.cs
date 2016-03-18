using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Engine
{

    public class SaveToolGUI : ToolOptionsGUI
    {
        public TextArea name;
        public Button save;

        public SaveToolGUI(EditorGUI theInterface)
            : base(theInterface)
        {
            save = new Button(theInterface, " Save ");
            save.margin = 2;
            save.clickEvent += (lol) =>
            {
                guiComponent.tileEngine.world.file.name = "../../../../../Game/GameContent/Maps/" + name.text + ".map";
                guiComponent.tileEngine.world.file.save();
            };
            name = new TextArea(theInterface);
            name.bgColor = Color.AntiqueWhite;
            name.forcedSize = new Vector2(120, 15);
            string temp = guiComponent.tileEngine.world.file.name;
            temp = temp.Substring(37);
            temp = temp.Substring(0,temp.Length - 4);
            name.text = temp==null ?  "autosave" : temp;
            

            Add(name); 
            Add(save);

        }
        public override void performLayout()
        {
            base.performLayout();
        }

        //public override Vector2 preferredSize { get { return this.preferredSize; } }
    }

    public class SaveTool : Tool
    {
        public SaveTool(Editor editor)
            : base(editor)
        {
            isDragging = false; ;
            gui = new SaveToolGUI(editor.graphicsComponent.guiComponent as EditorGUI);
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
