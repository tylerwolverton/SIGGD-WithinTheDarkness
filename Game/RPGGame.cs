using Engine.Components; //Unify

namespace Engine
{
    public class RPGGame : TileEngine
    {

        public enum GameState
        {
            PLAY,
            PAUSE,
            GAMEOVER,
            YOUWIN,
            MAIN,
        }

        
        public enum MapID//Keeping these named after their actual filenames. please rename the filenames
        {

            MainMenu,
            RealMap1,
            RealMap2,
            Arena,
        }

        public MapID mapNum = MapID.MainMenu;

        public GameState curState { get; set; }
        public bool bossAlive = true;
        public int menuSelection;

        public RPGGame() 
        {

            // NOTE: This defines the order of the game loop as well7
            resourceComponent = new GameResources(this.Content, this);
            inputComponent = new GameInput(this);
            physicsComponent = new GamePhysics(this);
            graphicsComponent = new GameGraphics(this);
            audioComponent = new GameAudio(this);
            OnMain();
        }

        protected override void Initialize()
        {
            base.Initialize();

            GameInput input = inputComponent as GameInput;
            
            // Add hooks for changes to global game state
            (input[GameInput.PlayBindings.PAUSE] as SinglePressBinding).downEvent += OnPause;
            
            
            (input[GameInput.GameOverBindings.NEWGAME] as SinglePressBinding).downEvent += OnMain;
            
            (input[GameInput.YouWinBindings.NEWGAME] as SinglePressBinding).downEvent += OnMain;
            
            
        }

        // Game State Machine
        private void OnPause()
        {
            curState = GameState.PAUSE;
        }

        public void OnUnPause()
        {
            curState = GameState.PLAY;
        }

        public Player savePlayer;
        public void transitionSave()
        {
            savePlayer = (world as GameWorld).player;
        }

        public Player transitionLoad()
        {
            return savePlayer;
        }

        public void OnNewGame()
        {

            if (mapNum == MapID.RealMap1)
                OnNewGame1();

            else if (mapNum == MapID.RealMap2)
                OnNewGame2();

            else if (mapNum == MapID.Arena)
                OnNewGame3();
        }

        public void OnNewGame1()
        {

            if (world != null)
            {
                world.UnloadContent();
            }

            world = new GameWorld(this, "RealMap1.map");
            world.Initialize();
            world.LoadContent();
            (world.tileEngine as RPGGame).bossAlive = true;
            BlobBehavior.fork = false;
            BlobBehavior.arena = false;
            curState = GameState.PLAY;
        }

        public void OnNewGame2()
        {

            if (world != null)
            {
                world.UnloadContent();
            }

            world = new GameWorld(this, "RealMap2.map");
            world.Initialize();
            world.LoadContent();
            (world.tileEngine as RPGGame).bossAlive = true;
            BlobBehavior.fork = false;
            BlobBehavior.arena = false;
            (world as GameWorld).player.level = 5;
            (world as GameWorld).player.skillPoints = 7;
            curState = GameState.PLAY;
        }

        public void OnNewGame3()
        {

            if (world != null)
            {
                world.UnloadContent();
            }

            world = new GameWorld(this, "Arena.map");
            world.Initialize();
            world.LoadContent();
            (world.tileEngine as RPGGame).bossAlive = true;
            BlobBehavior.fork = false;
            BlobBehavior.arena = true;
            (world as GameWorld).player.level = 5;
            (world as GameWorld).player.skillPoints = 7;
            curState = GameState.PLAY;
        }

        public void OnMain()
        {

            mapNum = MapID.MainMenu;

            if (world != null)
            {
                world.UnloadContent();
            }
            
            world = new GameWorld(this, "MainMenuMap.map");
            world.Initialize();
            world.LoadContent();
            (world.tileEngine as RPGGame).bossAlive = true;
            BlobBehavior.fork = false;
            
            curState = GameState.MAIN;
        }

        public void playerDied(Actor actor)
        {
            if (bossAlive)
                curState = GameState.GAMEOVER;
            else
                curState = GameState.YOUWIN;
        }
    }
}