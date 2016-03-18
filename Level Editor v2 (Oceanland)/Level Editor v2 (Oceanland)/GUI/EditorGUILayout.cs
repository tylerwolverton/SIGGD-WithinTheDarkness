using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Engine.GUI
{
    public class EditorGUILayout : AbsoluteContainer
    {
        public TableContainer row;
        EditorGUI theGui;
        public InputBinding[] preferredBindings;

        public EditorGUILayout(EditorGUI theInterface) : base(theInterface)
        {
            theGui = theInterface;
        }

        public void Initialize()
        {
            float width = guiComponent.graphics.camera.screenWidth / guiComponent.graphics.camera.scale;
            float height = guiComponent.graphics.camera.screenHeight / guiComponent.graphics.camera.scale;

            Vector2 iconSize = new Vector2(20, 20);
            int margin= 0;
            
            size = new Vector2(width, .25f * height);
            location = new Vector2(0, .75f * height);

            row = new TableContainer(guiComponent, 6,3,3);
            row.align = ListContainer.Align.CENTER;
            Button temp = new Button(guiComponent, "");
            temp.margin = margin;
            temp.normTexture = guiComponent.guiTextures[38];
            temp.clicTexture = guiComponent.guiTextures[49];
            temp.forcedSize = iconSize;

            //Create all the buttons for selecting Tools
            //New Map Tool
            temp.normTexture = guiComponent.guiTextures[38];
            temp.forcedSize = iconSize;

            temp.clickEvent += (loc) => 
            {
                (guiComponent.tileEngine as Editor).activeTool.active = false;

                (guiComponent.tileEngine as Editor).activeTool = (guiComponent.tileEngine as Editor).newTool;
                (guiComponent.tileEngine as Editor).activeTool.active = true;
                guiComponent.current.pack();
            };
            row.Add(temp);
            
            //Save Tool
            temp = new Button(guiComponent, "");
            temp.margin = margin;
            temp.normTexture = guiComponent.guiTextures[30];
            temp.clicTexture = guiComponent.guiTextures[41];
            temp.forcedSize = iconSize;
            temp.clickEvent += (loc) =>
            {
                (guiComponent.tileEngine as Editor).activeTool.active = false;

                (guiComponent.tileEngine as Editor).activeTool = (guiComponent.tileEngine as Editor).saveTool;
                (guiComponent.tileEngine as Editor).activeTool.active = true;
                guiComponent.current.pack();
            };
            row.Add(temp);

            //Load Tool
            temp = new Button(guiComponent, "");
            temp.normTexture = guiComponent.guiTextures[36];
            temp.clicTexture = guiComponent.guiTextures[47];
            temp.margin = margin;
            temp.forcedSize = iconSize;
            temp.clickEvent += (loc) =>
            {
                (guiComponent.tileEngine as Editor).activeTool.active = false;

                (guiComponent.tileEngine as Editor).activeTool = (guiComponent.tileEngine as Editor).loadTool;
                (guiComponent.tileEngine as Editor).activeTool.active = true;
                guiComponent.current.pack();
            };
            row.Add(temp);

            //Pencil Tool
            temp = new Button(guiComponent, "");
            temp.margin = margin;
            temp.normTexture = guiComponent.guiTextures[31];
            temp.clicTexture = guiComponent.guiTextures[42];
            temp.forcedSize = iconSize;
            temp.clickEvent += (loc) => 
            {
                (guiComponent.tileEngine as Editor).activeTool.active = false;

                (guiComponent.tileEngine as Editor).activeTool = (guiComponent.tileEngine as Editor).pencilTool;
                (guiComponent.tileEngine as Editor).activeTool.active = true;
                guiComponent.current.pack();
            };
            row.Add(temp);

            temp = new Button(guiComponent, "");
            temp.margin = margin;
            temp.normTexture = guiComponent.guiTextures[32];
            temp.clicTexture = guiComponent.guiTextures[43];


            temp.forcedSize = iconSize;

            temp.clickEvent += (loc) =>
            {
                (guiComponent.tileEngine as Editor).activeTool.active = false;

                (guiComponent.tileEngine as Editor).activeTool = (guiComponent.tileEngine as Editor).flagTool;
                (guiComponent.tileEngine as Editor).activeTool.active = true;
                guiComponent.current.pack();
            };
            row.Add(temp);

            temp = new Button(guiComponent, "");
            temp.normTexture = guiComponent.guiTextures[33];
            temp.clicTexture = guiComponent.guiTextures[44];
            temp.margin = margin;
            temp.forcedSize = iconSize;
            temp.clickEvent += (loc) =>
            {
                (guiComponent.tileEngine as Editor).activeTool.active = false;

                (guiComponent.tileEngine as Editor).activeTool = (guiComponent.tileEngine as Editor).eraseTool;
                (guiComponent.tileEngine as Editor).activeTool.active = true;
                guiComponent.current.pack();
            };
            row.Add(temp);
            temp = new Button(guiComponent, "");
            temp.normTexture = guiComponent.guiTextures[34];
            temp.clicTexture = guiComponent.guiTextures[45];
            temp.margin = margin;
            temp.forcedSize = iconSize;
            temp.clickEvent += (loc) =>
            {
                (guiComponent.tileEngine as Editor).activeTool.active = false;

                (guiComponent.tileEngine as Editor).activeTool = (guiComponent.tileEngine as Editor).fillTool;
                (guiComponent.tileEngine as Editor).activeTool.active = true;
                guiComponent.current.pack();
            };
            row.Add(temp);
            temp = new Button(guiComponent, "");
            temp.normTexture = guiComponent.guiTextures[35];
            temp.clicTexture = guiComponent.guiTextures[46];
            temp.margin = margin;
            temp.forcedSize = iconSize;
            temp.clickEvent += (loc) =>
            {
                (guiComponent.tileEngine as Editor).activeTool.active = false;

                (guiComponent.tileEngine as Editor).activeTool = (guiComponent.tileEngine as Editor).actorTool;
                (guiComponent.tileEngine as Editor).activeTool.active = true;
                guiComponent.current.pack();
            };
            row.Add(temp);


            temp = new Button(guiComponent, "");
            temp.normTexture = guiComponent.guiTextures[40];
            temp.clicTexture = guiComponent.guiTextures[48];
            temp.margin = margin;

            temp.forcedSize = iconSize;
            temp.clickEvent += (loc) =>
            {
                (guiComponent.tileEngine as Editor).activeTool.active = false;

                (guiComponent.tileEngine as Editor).activeTool = (guiComponent.tileEngine as Editor).optionTool;
                (guiComponent.tileEngine as Editor).activeTool.active = true;
                guiComponent.current.pack();
            };
            row.Add(temp);

            temp = new Button(guiComponent, "");
            temp.normTexture = guiComponent.guiTextures[50];
            temp.clicTexture = guiComponent.guiTextures[51];
            temp.margin = margin;

            temp.forcedSize = iconSize;
            temp.clickEvent += (loc) =>
            {
                (guiComponent.tileEngine as Editor).activeTool.active = false;

                (guiComponent.tileEngine as Editor).activeTool = (guiComponent.tileEngine as Editor).stampTool;
                (guiComponent.tileEngine as Editor).activeTool.active = true;
                guiComponent.current.pack();
            };
            row.Add(temp);

            row.Add(null);
            row.Add(null);
            row.Add(null);

            Add(row);
            Add(new ToolOptionsGUI(guiComponent as EditorGUI));
        }
        public override void performLayout()
        {
            float width = guiComponent.graphics.camera.screenWidth / (guiComponent.graphics.camera.scale-guiComponent.graphics.camera.scaleChange);
            float height = guiComponent.graphics.camera.screenHeight / (guiComponent.graphics.camera.scale - guiComponent.graphics.camera.scaleChange);
            row.forcedSize = new Vector2(0.2f * width, 0.25f * height);
            row.forcedLocation = Vector2.Zero;
            

            row.stretch = true;
            row.bgImage = guiComponent.guiTextures[22];
            
            row.location = row.forcedLocation;
            
            children[0] = row;
            row.size = row.forcedSize;
            if ((guiComponent.tileEngine as Editor).activeTool != null)
                children[1] = (guiComponent.tileEngine as Editor).activeTool.gui;
            children[1].size = new Vector2(0.8f * width, 0.25f * height);
            children[1].location = new Vector2(0.2f*width,0.0f);
            children[1].bgImage = guiComponent.guiTextures[22];
            if (guiComponent.current != null)
                children[1].pack();
            
        }


        public override void handleClick(Vector2 localPos)
        {
            base.handleClick(localPos);

            if (!guiComponent.current.contains(localPos))
                (guiComponent.tileEngine as Editor).activeTool.doLeftDownAction();
        }
        public override void handleRightClick(Vector2 localPos)
        {
            base.handleClick(localPos);

            if (!guiComponent.current.contains(localPos))
                (guiComponent.tileEngine as Editor).activeTool.doRightDownAction();
        }

        public override void releaseClick(Vector2 localPos)
        {

            base.releaseClick(localPos);
            //if ((guiComponent.tileEngine as Editor).activeTool.isDragging)

            if (!guiComponent.current.contains(localPos))
            {
                 (guiComponent.tileEngine as Editor).activeTool.doLeftUpAction();
                return;
            }
            else if ((guiComponent.tileEngine as Editor).activeTool != (guiComponent.tileEngine as Editor).fillTool)
            {
                (guiComponent.tileEngine as Editor).activeTool.doLeftUpAction(); 
                return;
            }
        }
        public override void releaseRightClick(Vector2 localPos)
        {

            base.releaseClick(localPos);
            //if ((guiComponent.tileEngine as Editor).activeTool.isDragging)

            if (!guiComponent.current.contains(localPos))
            {
                (guiComponent.tileEngine as Editor).activeTool.doRightUpAction();
                return;
            }
            else if ((guiComponent.tileEngine as Editor).activeTool != (guiComponent.tileEngine as Editor).fillTool)
            {
                (guiComponent.tileEngine as Editor).activeTool.doRightUpAction();
                return;
            }
        }
        public override void handleKeyPress(char c)
        {
            base.handleKeyPress(c);
            if (c == 'z')
                Tool.undo();
            else if (c == 'r')
                Tool.redo();
            else if (c == 's')
            {
                guiComponent.tileEngine.world.file.name = "../../../../../Game/GameContent/Maps/" + ((guiComponent.tileEngine as Editor).saveTool.gui as SaveToolGUI).name.text + ".map";
                guiComponent.tileEngine.world.file.save();
            }
                
        }
        
    }
}
