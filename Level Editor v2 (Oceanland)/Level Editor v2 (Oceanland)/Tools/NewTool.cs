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
    public class NewToolGUI : ToolOptionsGUI
    {
        public ListContainer[] holdThis;
        public TextArea name;
        public Button newMap;
        public TextArea width;
        public Label wLabel;
        public TextArea height;
        public Label hLabel;

        public NewToolGUI(EditorGUI theInterface)
            : base(theInterface)
        {
            orientation = Orientation.VERTICAL;
            holdThis = new ListContainer[2];
            holdThis[0] = new ListContainer(theInterface);
            holdThis[1] = new ListContainer(theInterface);
            newMap = new Button(theInterface, " New ");
            newMap.margin = 2;
            newMap.clickEvent += (lol) =>
            {
                guiComponent.tileEngine.world.file.free();
                guiComponent.tileEngine.world.file = new MapFile(new FileStream("../../../../../Game/GameContent/Maps/" + name.text + ".map", FileMode.Create));
                ((guiComponent.tileEngine as Editor).saveTool.gui as SaveToolGUI).name.text = name.text;
                guiComponent.tileEngine.world.file.tiles = new MapFile.TileData[1, int.Parse(width.text), int.Parse(height.text)];
                guiComponent.tileEngine.world.file.tileSetNames = new string[1];
                guiComponent.tileEngine.world.file.tileSetNames[0] = "Tiles";
                guiComponent.tileEngine.world.file.soundSetName = "000_Lucy";
                
                guiComponent.tileEngine.world.file.worldObjects = new List<MapFile.WorldObjectData>();
                for (int i = 0; i < int.Parse(width.text); i++)
                {
                    for (int j = 0; j < int.Parse(height.text); j++)
                    {
                        guiComponent.tileEngine.world.file.tiles[0, i, j] = new MapFile.TileData();
                        guiComponent.tileEngine.world.file.tiles[0, i, j].tileSetIndex = 0;
                        guiComponent.tileEngine.world.file.tiles[0, i, j].tileIndex = 16;
                    }
                }
                guiComponent.tileEngine.world.file.name = name.text;
                guiComponent.tileEngine.world.initTiles();
                guiComponent.tileEngine.world.initActors();
                guiComponent.tileEngine.world.sortActors();
                guiComponent.tileEngine.world.addActor(guiComponent.tileEngine.world.actorFactory.createActor(-1, new Vector2(16, 16)));
                (guiComponent.tileEngine.world as EditorWorld).player = guiComponent.tileEngine.world.actors.First.Value as Acting.EmptyActor;
            };
            name = new TextArea(theInterface);
            name.bgColor = Color.AntiqueWhite;
            name.forcedSize = new Vector2(120, 15);

            width = new TextArea(theInterface);
            width.bgColor = Color.AntiqueWhite;
            width.forcedSize = new Vector2(40, 15);
            width.marginL = 2;

            height = new TextArea(theInterface);
            height.bgColor = Color.AntiqueWhite;
            height.forcedSize = new Vector2(40, 15);
            height.marginL = 2;

            wLabel = new Label(theInterface, "Width");
            wLabel.marginL = 2;
            hLabel = new Label(theInterface, "Height");
            hLabel.marginL = 2;


            holdThis[0].Add(name);
            holdThis[0].Add(newMap);
            holdThis[1].Add(width);
            holdThis[1].Add(wLabel);
            holdThis[1].Add(height);
            holdThis[1].Add(hLabel);

            Add(holdThis[0]);
            Add(holdThis[1]);
        }

        //public override Vector2 preferredSize { get { return this.preferredSize; } }
    }

    public class NewTool : Tool
    {

        public NewTool(Editor editor)
            : base(editor)
        {
            isDragging = false;
            gui = new NewToolGUI(editor.graphicsComponent.guiComponent as EditorGUI);
            gui.size = new Vector2(300, 200);
        }

        public override void doLeftDownAction()
        {
            
        }


    }
}
