using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
    public class EraserToolGUI : ToolOptionsGUI
    {
        public EraserToolGUI(EditorGUI theInterface)
            : base(theInterface)
        {
        }

        //public override Vector2 preferredSize { get { return this.preferredSize; } }
    }

    public class EraserTool : Tool
    {

        public EraserTool(Editor editor)
            : base(editor)
        {
            isDragging = false;
            gui = new EraserToolGUI(editor.graphicsComponent.guiComponent as EditorGUI);
            gui.size = new Vector2(300, 200);
        }

        public override void doLeftDownAction()
        {
            isDragging = true;
            Tile victim = getMouseWorldPos();
            if (victim != null)
            {
                for (int i = 0; i < editor.world.file.worldObjects.Count(); i++)
                {
                    if (victim.xIndex == editor.world.file.worldObjects.ElementAt(i).x && victim.yIndex == editor.world.file.worldObjects.ElementAt(i).y)
                    {
                        toolAction.Push(new EraserToolAction(editor.world.file.worldObjects.ElementAt(i),this));
                        editor.world.file.worldObjects.RemoveAt(i);
                        undos.Clear();
                        break;
                    }
                }
                
            }

            editor.world.initActors();
        }

        public override void doMoveAction()
        {
            if(isDragging)
            {
            Tile victim = getMouseWorldPos();
            if (victim != null)
            {
                for (int i = 0; i < editor.world.file.worldObjects.Count(); i++)
                {
                    if (victim.xIndex == editor.world.file.worldObjects.ElementAt(i).x && victim.yIndex == editor.world.file.worldObjects.ElementAt(i).y)
                    {
                        toolAction.Push(new EraserToolAction(editor.world.file.worldObjects.ElementAt(i), this));
                        editor.world.file.worldObjects.RemoveAt(i);
                        undos.Clear();
                        break;
                    }
                }
            }
            }
        }

        public override void doLeftUpAction()
        {
            isDragging = false;
        }

        public override void undoAction(ToolAction action)
        {
            MapFile.WorldObjectData w = (action as EraserToolAction).actor;
            editor.world.file.worldObjects.Add(w);
            editor.world.initActors();
        }
        public override void redoAction(ToolAction action)
        {
            MapFile.WorldObjectData w = (action as EraserToolAction).actor;
            editor.world.file.worldObjects.Remove(w);
            editor.world.initActors();
        }

        public class EraserToolAction : ToolAction
        {
            public MapFile.WorldObjectData actor;
            public EraserToolAction(MapFile.WorldObjectData actor, Tool parent) : base(parent)
            {
                this.actor = actor;
            }
        }
    }
}
