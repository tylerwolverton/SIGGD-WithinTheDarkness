using Engine.Components;
using System;
using System.IO;
using Engine.GUI;
using Engine; //Unify
using Engine.Textures;
using Engine.Input;
namespace Engine
{
    public class Graven : MirrorEngine
    {

        public bool bossAlive = true;
        public int menuSelection;

        public Graven()
        {

            // NOTE: This defines the order of the game loop as well
            resourceComponent = new ResourceComponent(this.Content, this); //new GameResources(this.Content, this);
            inputComponent = new GameInput(this);
            //Some of our current physics should probably be in Game Physics
            physicsComponent = new PhysicsComponent(this); //new GamePhysics(this); 
            graphicsComponent = new GameGraphics(this);
            audioComponent = new GameAudio(this); //new GameAudio(this);
        }

        protected override void Initialize()
        {
            base.Initialize();

            GameInput input = inputComponent as GameInput;
            
            // Add hooks for changes to global game state
            (input[GameInput.PlayBindings.PAUSE] as SinglePressBinding).downEvent += OnPause;
            OnMain();
        }

        // Game State Machine
        private void OnPause()
        {
            enterGUI((graphicsComponent.guiComponent as GameGUIComponent).pauseMenu, true, false);
            (world as GameWorld).gameIsPaused = true;
            audioComponent.pauseSoundEngine();
        }

        public void OnUnPause()
        {
            setWorld(null);//probably should have separate function for exiting the gui/pause menu
            world.isActive = true;
            physicsComponent.isActive = true;
            (world as GameWorld).gameIsPaused = false;
            audioComponent.resumeSoundEngine();
        }

        public override void setWorld(string str)
        {
            
            base.setWorld(str);
            if (world == null)
            {
                loadWorld(currentWorldPath);
                return;
            }
            Acting.Enemies.BlobBehavior.fork = false;
            (inputComponent as GameInput).enablePlayBindings();
            GameGUIComponent gamegui = graphicsComponent.guiComponent as GameGUIComponent;

            // If we're in a map with a player (i.e., not credits)
            if ((world as GameWorld).player != null) {
                gamegui.current = gamegui.hud;
            } else {
                gamegui.current = null;
            }

            bossAlive = true;
            world.Start();
        }

        public void OnMain()
        {
            enterGUI((graphicsComponent.guiComponent as GameGUIComponent).mainMenu, true, true);
            audioComponent.playSound(resourceComponent.getAudioSet("026_Main")[0], true);
            
        }

        public void playerDied()
        {
                // Enter GAMEOVER gui
                enterGUI((graphicsComponent.guiComponent as GameGUIComponent).gameOverMenu, false, false);
        }

        public void playerWon()
        {
            // Enter YOUWIN gui
            enterGUI((graphicsComponent.guiComponent as GameGUIComponent).youWinMenu, true, true);
        }

        public void enterGUI(GUIContainer item, bool pauseWorld, bool endWorld)
        {
            if (endWorld)
            {
                if (world != null)
                {
                    world.UnloadContent();
                }
                world = null;
            } else {
                if (pauseWorld)
                {
                    world.isActive = false;
                    physicsComponent.isActive = false;
                }
            }

            (graphicsComponent.guiComponent as GameGUIComponent).current = item;
        }

        public override World loadWorld(string str)
        {
            World w = new GameWorld(this, str);
            w.Initialize();
            w.LoadContent();
            return w;
        }

        //State saving
        public override void saveState(BinaryWriter writer)
        {
            writer.Write(menuSelection);
            writer.Write(bossAlive);
            (world as GameWorld).saveState(writer);
        }

        public override void loadState(BinaryReader reader)
        {
            menuSelection = reader.ReadInt32();
            bossAlive = reader.ReadBoolean();
            (world as GameWorld).loadState(reader);
        }

        //These are the only two state saving/loading functions that should ever be called externally
        public void saveGameState()
        {
            const int MAGIC = 0x40826119;
            string fileName = "TestSaveFile1.sav";
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fileStream);
            writer.Write(MAGIC);
            base.saveState(writer);
            this.saveState(writer);
            writer.Write(MAGIC);
            fileStream.Close();
        }

        public void loadGameState()
        {
            const int MAGIC = 0x40826119;
            string fileName = "TestSaveFile1.sav";
            if (!File.Exists(fileName))
            {
                throw new Exception("State loading error: save file does not exist");
            }
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            BinaryReader reader = new BinaryReader(fileStream);
            if (reader.ReadInt32() != MAGIC)
            {
                throw new Exception("State loading error: file provided is not a save file");
            }
            world.UnloadContent();
            killWorld();
            base.loadState(reader);
            world = new GameWorld(this, currentWorldPath);
            world.LoadContent();
            this.loadState(reader);
            if (reader.ReadInt32() != MAGIC)
            {
                throw new Exception("State loading error: loading state from file failed");
            }
            fileStream.Close();
            (inputComponent as GameInput).enablePlayBindings();
            world.isActive = true;
            physicsComponent.isActive = true;
            bossAlive = true;
            world.Start();
        }
    }
}