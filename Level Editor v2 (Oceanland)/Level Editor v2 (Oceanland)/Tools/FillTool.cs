using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
    public class FillTool : Tool
    {

        public FillTool(Editor editor):base(editor)
        {
            isDragging = false;
            
            gui = new PencilToolGUI(editor.graphicsComponent.guiComponent as EditorGUI);
            gui.size = new Vector2(300, 200);
        }
        public override void doLeftUpAction()
        {
            isDragging = false; // why

            Tile victim = getMouseWorldPos();
            if (victim != null)
            {
                byte[] selectedTile = new byte[2];
                selectedTile[0] = (this.editor.world as EditorWorld).currentTile.tileSetIndex;
                selectedTile[1] = (this.editor.world as EditorWorld).currentTile.tileIndex;
                FillToolAction action = fill(selectedTile,victim);
                if (action != null)
                {
                    toolAction.Push(action);
                    undos.Clear();
                }
                editor.world.initTiles();
            }
        }
        public override void undoAction(ToolAction action)
        {
            foreach (TextureData chg in (action as FillToolAction).changed)
            {
                editor.world.file.tiles[0, chg.x, chg.y].tileIndex = chg.tileIndex;
                editor.world.file.tiles[0, chg.x, chg.y].tileSetIndex = chg.tileSetIndex;
            }
            editor.world.initTiles();
        }
        public override void redoAction(ToolAction action)
        {
            FillToolAction data = (FillToolAction)action;
            fill(data.texture, data.startTile);
            editor.world.initTiles();
        }

        public FillToolAction fill(byte[] texture, Tile startTile)
        {
            LinkedList<TextureData> changed = new LinkedList<TextureData>(); // for undo

            MapFile.TileData replace = editor.world.file.tiles[0, startTile.xIndex, startTile.yIndex];
            if (replace.tileSetIndex == texture[0] && replace.tileIndex == texture[1])
                return null; // not sure if want to count do-nothing action as action
            Queue<Tile> applyToMe = new Queue<Tile>();
            MapFile.TileData d;

            applyToMe.Enqueue(startTile);

            d = editor.world.file.tiles[0, startTile.xIndex, startTile.yIndex];
            changed.AddLast(new TextureData(startTile.xIndex, startTile.yIndex, d.tileSetIndex, d.tileIndex));
            editor.world.file.tiles[0, startTile.xIndex, startTile.yIndex].tileSetIndex = texture[0];
            editor.world.file.tiles[0, startTile.xIndex, startTile.yIndex].tileIndex = texture[1];

            while (applyToMe.Count() > 0)
            {
                Tile mid = applyToMe.Dequeue();
                foreach (Tile tile in mid.adjacent)
                {
                    if (tile != null)
                    {
                        d = editor.world.file.tiles[0, tile.xIndex, tile.yIndex];
                        if (d.tileSetIndex == replace.tileSetIndex && d.tileIndex == replace.tileIndex)
                        {
                            applyToMe.Enqueue(tile);
                            changed.AddLast(new TextureData(tile.xIndex, tile.yIndex, d.tileSetIndex, d.tileIndex));
                            editor.world.file.tiles[0, tile.xIndex, tile.yIndex].tileSetIndex = texture[0];
                            editor.world.file.tiles[0, tile.xIndex, tile.yIndex].tileIndex = texture[1];
                        }
                    }
                }
            }
            return new FillToolAction(changed, texture, startTile,this);
        }
        
        public class FillToolAction : ToolAction
        {
            public LinkedList<TextureData> changed; // original vals of changed tiles
            public byte[] texture; // data necessary to recreate action once undone
            public Tile startTile; // duplicate info of first item on stack, but makes redo easier, mod redo if want to remove.

            public FillToolAction(LinkedList<TextureData> changed, byte[] texture, Tile startTile, Tool parent)
                : base(parent)
            {
                this.changed = changed;
                this.texture = texture;
                this.startTile = startTile;
            }
        }
    }
        
}
