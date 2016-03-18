using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
namespace Engine.GUI
{
    class MainMenu : AbsoluteContainer
    {

        public MenuContainer menu;
        public Label happyImage;

        public override void performLayout()
        {
            base.performLayout();
            if (menu.selected == 1)
            {
                happyImage.bgImage = guiComponent.guiTextures[1];
            }
            else if (menu.selected == 2)
            {
                happyImage.bgImage = guiComponent.guiTextures[2];
            }
            else if (menu.selected == 3)
            {
                happyImage.bgImage = guiComponent.guiTextures[3];
            }
            else
                happyImage.bgImage = null;
        }

        public MainMenu(GameGUIComponent theInterface) : base(theInterface)
        {
            menu = new MenuContainer(theInterface, ListContainer.Orientation.VERTICAL);
            MenuItem graven = new MenuItem(theInterface, "");
            graven.bgImage = guiComponent.guiTextures[0];
            
            menu.Add(graven);
            happyImage = new Label(theInterface);
            /* Dynamically display available maps */
            foreach (var m in guiComponent.tileEngine.resourceComponent.worldNames) {
                string p = m.Key;
                string str = Path.GetFileNameWithoutExtension(m.Key);
                str = str.Substring(str.IndexOf('_')+1);
                MenuItem map = new MenuItem(theInterface, str);

                map.selectHandler += () => { guiComponent.tileEngine.setWorld(p); guiComponent.tileEngine.audioComponent.stopSoundEngine(); };
                
                menu.Add(map);
            }

            MenuItem controls = new MenuItem(theInterface, "Controls");
            MenuItem credits = new MenuItem(theInterface, "Credits");
            MenuItem exit = new MenuItem(theInterface, "Exit");

            credits.selectHandler += () => { guiComponent.current = (guiComponent as GameGUIComponent).credits; };
            exit.selectHandler += () => { guiComponent.tileEngine.Exit(); };
            
            menu.Add(controls);
            menu.Add(credits);
            menu.Add(exit);

            menu.top = 1;
            menu.selected = 1;
            menu.forcedLocation = new Microsoft.Xna.Framework.Vector2(25,50);
            menu.forcedSize = new Microsoft.Xna.Framework.Vector2(0, 0);
            children.Add(menu);
            children.Add(happyImage);
            happyImage.stretch = false;
            happyImage.forcedLocation = new Vector2(100, 160);
            happyImage.forcedSize = new Vector2(512, 512);
            menu.happy = null;
            stretch = true;
            //bgImage = theInterface.guiTextures[6];
            bindings = menu.bindings;
        }
    }
}
