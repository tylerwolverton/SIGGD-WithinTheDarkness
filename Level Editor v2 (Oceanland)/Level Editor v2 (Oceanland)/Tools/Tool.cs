using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
    public class Tool 
    {

        public Editor editor;                   //derp <-- that clears it all up
        public Boolean active { get; set; }     //Whether or not the tool is active
        public static Stack<ToolAction> toolAction = new Stack<ToolAction>();    //List of actions performed 
        public static Stack<ToolAction> undos = new Stack<ToolAction>();         //List of Undos since last action
        public Boolean isDragging;
        public ToolOptionsGUI gui;

        public static void redo()
        {
            if (undos.Count() > 0)
            {
                ToolAction tool = undos.Pop();
                toolAction.Push(tool);
                tool.doAction();
            }
        }

        public static void undo()
        {
            if (toolAction.Count() > 0)
            {
                ToolAction tool = toolAction.Pop();
                undos.Push(tool);
                tool.undoAction();
            }
        }
        /*
        public InputBinding[] bindings;
        public enum ToolBindings
        {
            LEFT,
            RIGHT,
            MIDDLE,
        }*/

        public class ToolAction {
            public Tool parent;
            public ToolAction(Tool parent) {
                this.parent = parent;
            }
            public void doAction() {
                parent.redoAction(this);
            }
            public void undoAction() {
                parent.undoAction(this);
            }
        }

        public Tool(Editor editor)
        {
            this.editor = editor;
            
            EditorInput input = this.editor.inputComponent as EditorInput;
            gui = new ToolOptionsGUI(editor.graphicsComponent.guiComponent as EditorGUI);

            toolAction = new Stack<ToolAction>();
            undos = new Stack<ToolAction>();
            /*
            bindings = new InputBinding[Enum.GetValues(typeof(ToolBindings)).Length];
            bindings[(int)ToolBindings.LEFT] = new SinglePressBinding(input, null, null, InputComponent.MouseButton.LEFT);
            bindings[(int)ToolBindings.RIGHT] = new SinglePressBinding(input, null, null, InputComponent.MouseButton.RIGHT);

            (bindings[(int)ToolBindings.LEFT] as SinglePressBinding).downEvent += doLeftAction;
            (bindings[(int)ToolBindings.RIGHT] as SinglePressBinding).downEvent += doRightAction;
             */
        }

        
        public virtual void undoAction(ToolAction action) { }

        public virtual void redoAction(ToolAction action) { }

        public virtual void doLeftDownAction() 
        { 
          
        }
        public virtual void doLeftUpAction()
        {

        }
        public virtual void doRightUpAction() 
        { 
          
        }
        public virtual void doRightDownAction() { }

        public virtual void doMoveAction() { }

        public Tile getMouseWorldPos()
        {
            Matrix screen2game = Matrix.Invert(editor.graphicsComponent.game2screen);

            MouseState ms = editor.inputComponent.currentMouseState;
            Vector2 screenPos = new Vector2(ms.X, ms.Y);

            return editor.world.getTileAt(Vector2.Transform(screenPos, screen2game));
        }
        public Vector2 getMouseWorldVector()
        {
            Matrix screen2game = Matrix.Invert(editor.graphicsComponent.game2screen);

            MouseState ms = editor.inputComponent.currentMouseState;
            Vector2 screenPos = new Vector2(ms.X, ms.Y);
            return Vector2.Transform(screenPos, screen2game);
        }
    }

    public class ToolOptionsGUI : ListContainer { 
        public ToolOptionsGUI(EditorGUI theInterface) : base(theInterface) 
        { 
            stretch = true; 
        } 
    public override Vector2 preferredSize { 
        get { return Vector2.Zero; } 
        } 
    }
    public struct TextureData
    {
        public int x;
        public int y;
        public byte tileSetIndex;
        public byte tileIndex;
        public TextureData(int x, int y, byte tileSetIndex, byte tileIndex)
        {
            this.x = x;
            this.y = y;
            this.tileSetIndex = tileSetIndex;
            this.tileIndex = tileIndex;
        }
    }
}
