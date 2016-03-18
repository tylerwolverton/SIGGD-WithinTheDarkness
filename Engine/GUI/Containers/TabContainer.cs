using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.GUI
{
    /* A GUIContainer that allows for tabbed elements, with each tab being its own GUIContainer and
     * built-in support for tab switch buttons.
     */
    public class TabContainer : GUIContainer
    {
        public ListContainer tabs;              //All the tab buttons in the TabContainer and their ListContainer
        public List<GUIContainer> containers;   //All the tabs in the TabContainer
        public GUIContainer primary;    //The current main tab
        public int current = 0;     //Which tab is the current main tab?
        public int tabCount = 0;    //How many tabs are there?
        GUIComponent theInterface;



        public enum TabOrientation { TOP, BOTTOM, LEFT, RIGHT } //Which side are the tabs on?
        public TabOrientation orientation; //This keeps track of where the tabs are.

        /*Returns an empty tab container with no tabs or buttons.
         * TODO: Allow instantiation of orientations other than BOTTOM
         */
        public TabContainer(GUIComponent theInterface, TabOrientation orientation)
            : base(theInterface)
        {
            //Durrrrrrrrrrrrr
            this.bgImage = theInterface.guiTextures[22];
            stretch = true;
            this.orientation = orientation;
            this.theInterface = theInterface;
            //Make the tabs line up properly with the orientation
            if (orientation == TabOrientation.BOTTOM)
            {
                tabs = new ListContainer(guiComponent, ListContainer.Orientation.HORIZONTAL);
            }
            
            //Initialize layout
            containers = new List<GUIContainer>();
            children.Add(null);
            children.Add(tabs);
            primary = new ListContainer(theInterface); //Just an empty ListContainer to take up space until you make your own
        }


        /* Calculate the ideal size of the TabContainer.
         * TODO: Make it take into account ALL the tabs and orientations, not just the current tab and BOTTOM orientation.
         */
        public override Vector2 preferredSize
        {
            get
            {
                float widthb = 0, heightb = 0;
                if (bgImage != null && !stretch)
                {
                    widthb = bgImage.Width;
                    heightb = bgImage.Height;
                }
                float width = 0;
                float height = 0;
                foreach(GUIContainer container in containers)
                {
                    width = Math.Max(width, container.preferredSize.X);
                    height = Math.Max(height, container.preferredSize.Y);
                }
                width = Math.Max(widthb,Math.Max(width, tabs.preferredSize.X));
                height = Math.Max(heightb, height+ tabs.preferredSize.Y);  
                //height = height + tabs.preferredSize.Y; //Tabs are eight high.
                Vector2 ideal = new Vector2(width, height);
                if (forcedSize != NotForcedSize)
                    ideal = forcedSize;
                return ideal;
            }
            
        }

        /*TabContainer's special situation requires a special HandleClick.
         *It first looks through each of its tabs for the click. Then it 
         *looks through JUST the open tab.
         */
        public override void handleClick(Vector2 localPos)
        {
            if (tabs != null && tabs.contains(localPos))
            {
                tabs.handleClick(localPos - tabs.location);
            }

            if (primary != null && primary.contains(localPos))
            {
                primary.handleClick(localPos - primary.location);
            }
        }

        /* Add a button to the TabContainer. Automatically sets up clickEvent for the new TabButton.
         */
        public void addButton(String text)
        {
            TabButton tb = new TabButton(guiComponent, text);

            //Update a couple variables and put the button in its place
            tabs.children.Add(tb);
            tb.index = tabCount;
            tabCount++;
            //tb.parent = this;

            //Made the TabButton conform to the TabContainer's orientation. That's what she said.
            tb.transform(orientation);

            //Add a clickEvent that makes the tab switch properly on click.
            tb.clickEvent += (pos) =>
            {
                current = tb.index;
                primary = containers[tb.index];
                children[0] = primary;
                performLayout();
                
            };
           
        }

        /* Add a new tab to the TabContainer
         */
        public void addContainer(GUIContainer container)
        {
            //Add a new tab to the TabContainer
            containers.Add(container);

            //If there is now only one tab, make this tab the main tab.
            if (containers.Count == 1)
            {
                primary = container;
                children[0] = primary;
            }

            //Make it alllll pretty.
            //performLayout();
        }


        /* Arranges the contents of the TabContainer. Dan, I don't think I wrote this the way your
         * convention says I should, but I was half-asleep when you told me. So tell me how it is 
         * supposed to be, please?
         * 
         * TODO: Support for all orientations
         */
        public override void performLayout()
        {
            if (orientation == TabOrientation.BOTTOM)
            {
                //Makes the tab take up all but tab row of the TabContainer.
                primary.size = new Vector2(Math.Min(this.preferredSize.X, this.size.X), Math.Min(this.preferredSize.Y, this.size.Y) - tabs.preferredSize.Y);
                if (primary.forcedLocation != NotForcedLocation)
                    primary.location = primary.forcedLocation;
                else
                    primary.location = new Vector2(0,0);
                primary.performLayout(); //Arrange the primary tab's innards

                //Make the tabs take up their preferred size.
                tabs.size = new Vector2(Math.Min(this.preferredSize.X, this.size.X), tabs.preferredSize.Y);
                if (tabs.forcedLocation != NotForcedLocation)
                    tabs.location = tabs.forcedLocation;
                else
                    tabs.location = new Vector2(0, this.size.Y - tabs.preferredSize.Y);
                tabs.performLayout(); //Arrange the tabs' innards
            }
            
        }

        
        

    }
}
