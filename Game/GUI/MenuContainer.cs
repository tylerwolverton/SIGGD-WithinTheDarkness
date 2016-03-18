using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine.Input;

namespace Engine.GUI
{
    public class MenuItem : Label
    {
        public delegate void SelectHandler();
        public event SelectHandler selectHandler;
        public void runEvent()
        {
            if(selectHandler != null)
                selectHandler();
        }

        public MenuItem(GUIComponent theInterface, String text) : base(theInterface, text) { }
    }

    public class MenuContainer : ListContainer
    {
        
        
        public Color defaultColor = Color.White;
        public Color selectedColor = Color.Red;
        public int selected = 0;
        public int top;
        public int bottom;
        public Texture2D happy;

        public enum MenuContainerBindings 
        { 
            UP,
            DOWN,
            SELECT
        }

        public MenuContainer(GameGUIComponent theInterface, Orientation orientation = Orientation.HORIZONTAL)
            : base(theInterface, orientation)
        {
            top = 0;
            bottom = 0;
            GameInput input = this.guiComponent.tileEngine.inputComponent as GameInput;
            bindings = new InputBinding[Enum.GetValues(typeof(MenuContainerBindings)).Length];

            //Default Menu Bindings
            InputComponent iComp = guiComponent.graphics.tileEngine.inputComponent;
            bindings[(int)MenuContainerBindings.SELECT] = new SinglePressBinding(iComp,
                new List<Keys> { Keys.Space, Keys.Enter }, new List<Buttons> { Buttons.Start, Buttons.A }, null);
            bindings[(int)MenuContainerBindings.UP] = new SinglePressBinding(iComp,
                new List<Keys> { Keys.W, Keys.Up }, new List<Buttons> { Buttons.DPadUp, Buttons.LeftThumbstickUp }, null);
            bindings[(int)MenuContainerBindings.DOWN] = new SinglePressBinding(iComp,
                new List<Keys> { Keys.S, Keys.Down }, new List<Buttons> { Buttons.DPadDown, Buttons.LeftThumbstickDown }, null);


            (bindings[(int)MenuContainerBindings.DOWN] as SinglePressBinding).downEvent += OnDown;
            (bindings[(int)MenuContainerBindings.UP] as SinglePressBinding).downEvent += OnUp;
            (bindings[(int)MenuContainerBindings.SELECT] as SinglePressBinding).downEvent += OnSelect; 
        }

        public void Add(GUIItem item)
        {
            bottom++;
            children.Add(item);
        }

        public override void performLayout()
        {
            base.performLayout();
            for (int i = 0; i < children.Count; i++)
            {
                if (i == selected)
                {
                    (children[i] as MenuItem).color = selectedColor;
                    if ((children[i] as MenuItem).text.Equals("Dungeon"))
                    {
                        happy = guiComponent.guiTextures[53];
                    }
                    else if ((children[i] as MenuItem).text.Equals("Arena"))
                    {
                        happy = guiComponent.guiTextures[54];
                    }
                    else
                    {
                        happy = guiComponent.guiTextures[1];
                    }

                }
                else
                    (children[i] as MenuItem).color = defaultColor;
            }

           
        }
        public void OnSelect()
        {
            (children[selected] as MenuItem).runEvent();
        }
        
        public void OnDown()
        {
            if (selected < bottom-1)
                selected++;
            guiComponent.current.pack();
        }
        public void OnUp()
        {
            if (selected > top)
                selected--;
            guiComponent.current.pack();
        }
        
    }
}
