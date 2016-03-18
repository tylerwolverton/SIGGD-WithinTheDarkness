using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Engine.Input;
using Engine.Tiles;


namespace Engine.GUI
{
    public class PauseMenu : ListContainer
    {
        //private Texture2D bgImage;

        //Pause menu should have volume control, back to main menu,
        public enum PauseBindings
        {
            UNPAUSE,
            GUICLICK,
            LEVELX,
            LEVELA,
            LEVELB,
            RESTART,
            BOSSWARP0,
            BOSSWARP1,
            BOSSWARP2,
            BOSSWARP3,
            BOSSWARP4,
            MAIN,
            FULLSCREEN,
        }


        private ListContainer cornerContainer;
        private ListContainer charSheetList;
        private ListContainer statList;
        
        private Label expLabel;
        private Label levelLabel;

        private Label strLabel;
        private Label strButton;
        private Label strNum;

        private Label skLabel;
        private Label skButton;
        private Label skNum;

        private Label healthLabel;
        private Label healthButton;
        private Label healthNum;

        private Label manaLabel;
        private Label manaButton;
        private Label manaNum;

        private ListContainer buttonCont;
        private ListContainer labelCont;
        private ListContainer numberCont;

        private SinglePressBinding levelX, levelA, levelB, bossWarp0, bossWarp1,bossWarp2,bossWarp3,bossWarp4;

        public override Vector2 preferredSize
        {
            get
            {
                return new Vector2(200,150);
            }
        }
        public GUIComponent theGui;
        public PauseMenu(GUIComponent theGui)
            : base(theGui)
        {
            this.theGui = theGui;
            bindings = new InputBinding[Enum.GetValues(typeof(PauseBindings)).Length];
            this.orientation = ListContainer.Orientation.VERTICAL;
            InputComponent input = guiComponent.graphics.tileEngine.inputComponent;
            this[PauseBindings.UNPAUSE] = new SinglePressBinding(input,
                    new List<Keys> { Keys.P, Keys.Escape }, new List<Buttons> { Buttons.Start }, null);
            this[PauseBindings.LEVELX] = new SinglePressBinding(input,
                    null, new List<Buttons> { Buttons.X }, null);
            this[PauseBindings.LEVELA] = new SinglePressBinding(input,
                    null, new List<Buttons> { Buttons.A }, null);
            this[PauseBindings.LEVELB] = new SinglePressBinding(input,
                    null, new List<Buttons> { Buttons.B }, null);
            this[PauseBindings.RESTART] = new SinglePressBinding(input,
                    new List<Keys> {Keys.Back }, new List<Buttons> { Buttons.Back }, null);
            this[PauseBindings.GUICLICK] = new SinglePressBinding(input,
                    null, null, Engine.InputComponent.MouseButton.LEFT);
            this[PauseBindings.BOSSWARP1] = new SinglePressBinding(input,
                     null, new List<Buttons> { Buttons.LeftTrigger }, null);
            this[PauseBindings.BOSSWARP2] = new SinglePressBinding(input,
                     null, new List<Buttons> { Buttons.RightTrigger }, null);
            this[PauseBindings.BOSSWARP3] = new SinglePressBinding(input,
                     null, new List<Buttons> { Buttons.LeftStick }, null);
            this[PauseBindings.BOSSWARP4] = new SinglePressBinding(input,
                     null, new List<Buttons> { Buttons.RightStick }, null);
            this[PauseBindings.BOSSWARP0] = new SinglePressBinding(input,
                new List<Keys> { Keys.Y }, null, null);
            this[PauseBindings.MAIN] = new SinglePressBinding(input,
                new List<Keys> { Keys.Q }, null, null);
            this[PauseBindings.FULLSCREEN] = new SinglePressBinding(input,
                new List<Keys> { Keys.F1 }, null, null);
            levelX = this[PauseBindings.LEVELX] as SinglePressBinding;
            levelA = this[PauseBindings.LEVELA] as SinglePressBinding;
            levelB = this[PauseBindings.LEVELB] as SinglePressBinding;
            bossWarp0 = this[PauseBindings.BOSSWARP0] as SinglePressBinding;
            bossWarp1 = this[PauseBindings.BOSSWARP1] as SinglePressBinding;
            bossWarp2 = this[PauseBindings.BOSSWARP2] as SinglePressBinding;
            bossWarp3 = this[PauseBindings.BOSSWARP3] as SinglePressBinding;
            bossWarp4 = this[PauseBindings.BOSSWARP4] as SinglePressBinding;


            /* TODO: Reimplement restart */
            //(this[PauseBindings.RESTART] as SinglePressBinding).downEvent += delegate() { (theGui.tileEngine as Graven).enterWorld(theGui.tileEngine.world); };
            (this[PauseBindings.UNPAUSE] as SinglePressBinding).downEvent += (theGui.tileEngine as Graven).OnUnPause;
            (this[PauseBindings.BOSSWARP0] as SinglePressBinding).downEvent += warpToBoss;
            (this[PauseBindings.MAIN] as SinglePressBinding).downEvent += (theGui.tileEngine as Graven).OnMain;
            //(this[PauseBindings.MAIN] as SinglePressBinding).downEvent += theGui.graphics.toggleFullscreen;
            expLabel = new Label(theGui);
            manaLabel = new Label(theGui);
            
            Add(expLabel);
            Add(manaLabel);
            expLabel.color = Color.Red;
            manaLabel.color = Color.Red;
            expLabel.text =  "       --PAUSED--      ";
            manaLabel.text = "Press Q to quit to menu";
            
            
        }

        public override void performLayout()
        {
            base.performLayout();
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 absParLoc)
        {

            
            pack();

            base.draw(spriteBatch, absParLoc);
        }

       

        public void warpToBoss()
        {
            if (bossWarp1.isPressed && bossWarp2.isPressed && bossWarp3.isPressed && bossWarp4.isPressed || bossWarp0.isPressed)
            {
                if(theGui.tileEngine.world.worldName == "000_Dungeon"){
                    Player p = (guiComponent.tileEngine.world as GameWorld).player;
                    p.position = new Microsoft.Xna.Framework.Vector2(48 * Tile.size, 42 * Tile.size);
                    p.skillPoints = 10;
                
                    GameWorld tempWorld = theGui.tileEngine.world as GameWorld;
                    tempWorld.player.position = new Microsoft.Xna.Framework.Vector2(48 * Tile.size, 42 * Tile.size);
                }
            }
        }

        public InputBinding this[PauseBindings bind]
        {
            get {
                return bindings[(int)bind]; }

            set {
                bindings[(int)bind] = value;
            }
        }

    }
}
