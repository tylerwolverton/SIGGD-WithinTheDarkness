using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
    public class ActorToolGUI : ToolOptionsGUI
    {
        public ActorToolGUI(EditorGUI theInterface)
            : base(theInterface)
        {

            GUI.EditorThumbnailContainer thumbs = new GUI.EditorThumbnailContainer(theInterface, 1);
            for (int i = 0; i < thumbs.thumbs.Length; i++)
            {
                Thumbnail t = thumbs.thumbs[i];
                t.clickEvent += (lol) =>
                {
                    //FOR GLORY
                    //thumbs.thumbs[(theInterface.tileEngine.world as EditorWorld).currentTile.tileIndex > thumbs.thumbs.Length ? (theInterface.tileEngine.world as EditorWorld).currentTile.tileIndex - thumbs.thumbs.Length : (theInterface.tileEngine.world as EditorWorld).currentTile.tileIndex].select = 0;
                    foreach (Thumbnail h in thumbs.thumbs)
                        h.select = 0;
                    (theInterface.tileEngine.world as EditorWorld).currentActor = t.tileIndex;
                    t.select = 1;
                };
            }
            thumbs.forcedLocation = new Vector2(0, 10);
            
            thumbs.thumbs[0].select = 1;
            this.Add(thumbs);
            this.forcedSize = new Vector2(300, 200);

        }

        //public override Vector2 preferredSize { get { return this.preferredSize; } }
    }

    public class ActorTool : Tool
    {

        public ActorTool(Editor editor)
            : base(editor)
        {
            isDragging = false;
            gui = new ActorToolGUI(editor.graphicsComponent.guiComponent as EditorGUI);
            gui.size = new Vector2(300, 200);
        }

        public override void doLeftDownAction()
        {
            Tile victim = getMouseWorldPos();
            if (victim != null)
            {
                Actor p = editor.world.actorFactory.createActor((editor.world as EditorWorld).currentActor, new Vector2(victim.x,victim.y), null);
                MapFile.WorldObjectData w = new MapFile.WorldObjectData();
                w.id = (byte)(editor.world as EditorWorld).currentActor;
                w.x = (ushort)victim.xIndex;
                w.y = (ushort)victim.yIndex;
                editor.world.file.worldObjects.Add(w);

                undos.Clear();

                toolAction.Push(new ActorToolAction(w,this));
            }

            editor.world.actors.Clear();
            editor.world.initActors();
        }
        public override void undoAction(ToolAction action) {
            MapFile.WorldObjectData w = (action as ActorToolAction).actor;
            editor.world.file.worldObjects.Remove(w);
            editor.world.initActors();
        }
        public override void redoAction(ToolAction action)
        {
            MapFile.WorldObjectData w = (action as ActorToolAction).actor;
            editor.world.file.worldObjects.Add(w);
            editor.world.initActors();
        }

        public class ActorToolAction : ToolAction
        {
            public MapFile.WorldObjectData actor;
            public ActorToolAction(MapFile.WorldObjectData actor, Tool parent):base(parent) {
                this.actor = actor;
            }
        }

    }
}
