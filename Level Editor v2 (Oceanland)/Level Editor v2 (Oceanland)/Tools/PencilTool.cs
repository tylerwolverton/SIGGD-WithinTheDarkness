using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
    public class PencilToolGUI : ToolOptionsGUI
    {
        public PencilToolGUI(EditorGUI theInterface)
            : base(theInterface)
        {
            
            GUI.EditorThumbnailContainer thumbs = new GUI.EditorThumbnailContainer(theInterface);
            for (int i = 0; i < thumbs.thumbs.Length; i++)
            {
                Thumbnail t = thumbs.thumbs[i];
                t.clickEvent += (lol) =>
                {
                    //FOR GLORY
                    //thumbs.thumbs[(theInterface.tileEngine.world as EditorWorld).currentTile.tileIndex > thumbs.thumbs.Length ? (theInterface.tileEngine.world as EditorWorld).currentTile.tileIndex - thumbs.thumbs.Length : (theInterface.tileEngine.world as EditorWorld).currentTile.tileIndex].select = 0;
                    foreach(Thumbnail h in thumbs.thumbs)
                        h.select = 0;
                    (theInterface.tileEngine.world as EditorWorld).currentTile.tileIndex = (byte)(t.tileIndex);
                    t.select = 1;
                };
            }
            thumbs.forcedLocation = new Vector2(0, 5);
            thumbs.thumbs[0].select = 1;
            this.align = Align.CENTER;
            this.Add(thumbs);
            this.forcedSize = new Vector2(300, 200);

        }

        //public override Vector2 preferredSize { get { return this.preferredSize; } }
    }
    public class PencilTool : Tool
    {
       
        public PencilTool(Editor editor)
            : base(editor)
        {
            isDragging = false;
            gui = new PencilToolGUI(editor.graphicsComponent.guiComponent as EditorGUI);
            gui.size = new Vector2(300,200);
        }

        public override void doLeftDownAction()
        {
            Tile victim = getMouseWorldPos();
            if (victim != null && !isDragging)
            {
                MapFile.TileData td = (editor.world as EditorWorld).file.tiles[0, victim.xIndex, victim.yIndex];
                if (td.tileIndex == (this.editor.world as EditorWorld).currentTile.tileIndex
                        && td.tileSetIndex == (this.editor.world as EditorWorld).currentTile.tileIndex)
                    return;
                LinkedList<TextureData> changed = new LinkedList<TextureData>();
                changed.AddLast(new TextureData(victim.xIndex, victim.yIndex, td.tileSetIndex, td.tileIndex));
                undos.Clear();

                td.tileIndex = (this.editor.world as EditorWorld).currentTile.tileIndex;
                td.tileSetIndex = (this.editor.world as EditorWorld).currentTile.tileSetIndex;
                (editor.world as EditorWorld).file.tiles[0, victim.xIndex, victim.yIndex] = td;

                toolAction.Push(new PencilToolAction(changed, new byte[] { td.tileSetIndex, td.tileIndex },this));

                editor.world.tileArray[victim.xIndex, victim.yIndex].imgIndex = 
                    (this.editor.world as EditorWorld).currentTile.tileIndex;
                isDragging = true;
            }
                
                //editor.world.initTiles();
        }

        public override void doMoveAction()
        {
            if (isDragging)
            {
                Tile victim = getMouseWorldPos();
                if (victim != null)
                {
                    MapFile.TileData td = (editor.world as EditorWorld).file.tiles[0, victim.xIndex, victim.yIndex];
                    if (td.tileIndex == (toolAction.Peek() as PencilToolAction).texture[1]
                        && td.tileSetIndex == (toolAction.Peek() as PencilToolAction).texture[0])
                        return;
                    (toolAction.Peek() as PencilToolAction).changed.AddLast(new TextureData(victim.xIndex, victim.yIndex, td.tileSetIndex, td.tileIndex));

                    td.tileSetIndex = (toolAction.Peek() as PencilToolAction).texture[0];
                    td.tileIndex = (toolAction.Peek() as PencilToolAction).texture[1];
                    
                    (editor.world as EditorWorld).file.tiles[0, victim.xIndex, victim.yIndex] = td;

                    editor.world.tileArray[victim.xIndex, victim.yIndex].imgIndex =
                        (this.editor.world as EditorWorld).currentTile.tileIndex;
                    
                }
                //editor.world.initTiles();
            }
        }

        public override void doLeftUpAction()
        {
            base.doLeftUpAction();
            isDragging = false;
        }
        public override void undoAction(ToolAction action)
        {
            foreach (TextureData chg in (action as PencilToolAction).changed)
            {
                editor.world.file.tiles[0, chg.x, chg.y].tileIndex = chg.tileIndex;
                editor.world.file.tiles[0, chg.x, chg.y].tileSetIndex = chg.tileSetIndex;
            }
            editor.world.initTiles();
        }
        public override void redoAction(ToolAction action)
        {
            foreach (TextureData chg in (action as PencilToolAction).changed)
            {
                editor.world.file.tiles[0, chg.x, chg.y].tileIndex = (action as PencilToolAction).texture[1];
                editor.world.file.tiles[0, chg.x, chg.y].tileSetIndex = (action as PencilToolAction).texture[0];
            }
            editor.world.initTiles();
        }
        public class PencilToolAction : ToolAction
        {
            public LinkedList<TextureData> changed; // original vals of changed tiles
            public byte[] texture; // data necessary to recreate action once undone

            public PencilToolAction(LinkedList<TextureData> changed, byte[] texture, Tool parent)
                :base(parent)
            {
                this.changed = changed;
                this.texture = texture;
            }
        }
    }
}
