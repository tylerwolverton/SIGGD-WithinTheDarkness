using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Acting;
using System.IO;
using Engine.Acting.Bosses;
using System.Diagnostics;
using Engine.Tiles;

namespace Engine
{

    public class GameWorld : World
    {

        private readonly Graven game;

        public Player player;
        public Player tempPlayer;
        public bool gameIsPaused = false;
        public bool gameIsDone;
        public bool hammerTriggered = false;
        public bool hammerDead = false;
        public bool bossTriggered = false;
        public bool blobTriggered = false;
        public bool blobDead = false;
        public int blobCount = 0;

        // To ensure player only activates contiguous action blocks once
        public int curAct = 0;
        public int curTag = 0;

        public GameWorld(MirrorEngine theEngine, String theMap) : base(theEngine, theMap)
        {
            
            tileTextureSet = tileEngine.resourceComponent.getTextureSet("Tiles");
            game = theEngine as Graven;
            actorFactory = new GameActorFactory(this);
        }

        public override void  LoadContent()
        {

 	        base.LoadContent();
        }

        public override void UnloadContent()
        {

            //for demo
            game.audioComponent.stopSoundEngine();

            base.UnloadContent();
        }

        public override void initActors()
        {

            actors = new LinkedList<Actor>();

            // Ensure that player is the first actor loaded
            int playerIndex = file.worldObjects.FindIndex((a) => (a.id == 0));
            if (playerIndex != 0 && playerIndex != -1) {
                MapFile.WorldObjectData player = file.worldObjects[playerIndex];
                file.worldObjects.RemoveAt(playerIndex);
                file.worldObjects.Insert(0, player);
            }

            base.initActors();
        }

        Boolean FLIPPER = false;

        public override void Start()
        {

            base.Start();

            bool rep = false;

            //Get world audioSet
            if (worldName == "012_Arena Mode") {
                songSet = game.resourceComponent.getAudioSet("019_Arena");
            } else if (worldName == "003_CastleFloor1") {
                songSet = game.resourceComponent.getAudioSet("023_Castle");
                rep = true;
            }
            /*else if (worldName == "010_Dungeon")
            {
                songSet = game.resourceComponent.getAudioSet("024_SeeNowDIE");
                rep = true;
            }*/else {
                songSet = game.resourceComponent.getAudioSet("018_Dungeon");
            }
            /*if (game.audioComponent.isSoundDone(songSet[1]))
            game.audioComponent.playSound(songSet[0], rep);*/
            gameIsDone = false;
            Acting.Enemies.Zazzle.zazzlesKilled = 0;
        }

        public override void Update(GameTime time)
        {

            base.Update(time);

            //For demo
            if (!FLIPPER)
            {
                /*for (int x = 46; x < 52; x++)
                {
                    if (worldName == "000_Dungeon" && this.getTileAt(player.position) == tileArray[x, 54])
                    {

                        FLIPPER = true;
                        for (int y = 46; y < 52; y++)
                        {
                            tileArray[y, 53].solid = true;
                            tileArray[y, 53].imgIndex = tileArray[52, 53].imgIndex;
                            tileArray[y, 52].imgIndex = tileArray[52, 52].imgIndex;
                        }

                        tileEngine.audioComponent.stopSound(songSet[0]);
                        tileEngine.audioComponent.stopSound(songSet[1]);
                        tileEngine.audioComponent.playSound(songSet[2], true);

                        addActor(new FirstBoss(this, new Vector2(49*Tile.size, 64*Tile.size)));

                        break;
                    }
                }*/

                if (player != null) {

                    Tile t = getTileAt(player.position);

                    /* Handle traps */
                    if (t.trap != null) {
                        t.trap.Trip(player);
                    }

                    // Handle actions (those where a block should only happen once).
                    if (t.action != curAct || t.tag != curTag) {  // Skip identical action tiles
                        switch (t.action) {
                            case 1:
                                addActor(actorFactory.createActor(t.tag, player.position));
                                break;

                            case 2:
                            case 3: //SUPER IMPORTANT: First 3 Digits is the Tile ID for the tiles changes to, Last 2 is the tag. Note: case 2 sets action to zero afterwards, 3 doesn't
                                Trace.WriteLine("X:" + t.xIndex + "Y:" + t.yIndex);
                                toggleDoor(t.tag);

                                // If this was 2 tile, disable any 2 tiles
                                if (t.action == 2) {
                                    int tmptag = t.tag % 100;  // Save tag, since it might get overwritten
                                    for (int i = 0; i < width; i++) {
                                        for (int j = 0; j < height; j++) {
                                            if (tileArray[i, j].action == 2 && (tileArray[i, j].tag % 100) == tmptag) {
                                                tileArray[i, j].action = 0;
                                                tileArray[i, j].tag = 0;
                                            }
                                        }
                                    }
                                }
                                break;
                            case 5:
                                //Switch to Goat music
                                hammerTriggered = true;
                                break;
                            case 10:
                                bossTriggered = true;
                                break;

                            case 7:
                            case 8:
                                forceDoor(t.tag);

                                // If this was 2 tile, disable any 2 tiles
                                if (t.action == 8) {
                                    int tmptag = t.tag % 100;  // Save tag, since it might get overwritten
                                    for (int i = 0; i < width; i++) {
                                        for (int j = 0; j < height; j++) {
                                            if (tileArray[i, j].action == 8 && (tileArray[i, j].tag % 100) == tmptag) {
                                                tileArray[i, j].action = 0;
                                                tileArray[i, j].tag = 0;
                                            }
                                        }
                                    }
                                }
                                break;
                            case 9:
                                blobTriggered = true;
                                break;

                        }
                    }

                    // Handle other actions (only happen once per tile)
                    Tile oldTile = getTileAt(player._oldPos);
                    if (oldTile != null && oldTile != t) {
                        switch (t.action) {

                            case 4:
                                addActor(actorFactory.createActor(t.tag, new Vector2(oldTile.x + Tile.size / 2, oldTile.y + Tile.size / 2))); // Spawns actor behind playah
                                break;
                        }
                    }

                    curAct = t.action;
                    curTag = t.tag;
                }

#if false
                /* Handle warp tiles */
                
                if (t.action == 1 && t.tag >= 0)
                {
                    /* Search for world with appropriate index */
                    foreach (var pair in game.resourceComponent.worldNames) {
                        if (pair.Value == t.tag) {
                            game.setWorld(pair.Key);
                            return;
                        }
                    }
                }
#endif

                
            }
        }

        // Toggle door (with a tag)
        public void toggleDoor(int tag)
        {
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    if ((tileArray[i, j].tag%100) == (tag%100) && tileArray[i, j].action == 0)
                    {
                        int tmpIndex = tileArray[i, j].imgIndex;
                        tileArray[i, j].imgIndex = tileArray[i, j].tag / 100;
                        tileArray[i, j].opaque = true;
                        tileArray[i, j].solid = true;
                        // Store old tile in tag so we can go back to it if necessary
                        tileArray[i, j].tag = (tileArray[i, j].tag % 100) + tmpIndex * 100;
                    }
                }
            }
        }

        public void forceDoor(int tag)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if ((tileArray[i, j].tag % 100) == (tag % 100) && tileArray[i, j].action == 0)
                    {
                        int tmpIndex = tileArray[i, j].imgIndex;
                        tileArray[i, j].imgIndex = tileArray[i, j].tag / 100;
                        if (tileArray[i, j].imgIndex == 64 || tileArray[i, j].imgIndex == 65 || tileArray[i, j].imgIndex == 66)
                            tileArray[i, j].opaque = true;
                        tileArray[i, j].solid = true;
                        // Store old tile in tag so we can go back to it if necessary
                        tileArray[i, j].tag = (tileArray[i, j].tag % 100) + tmpIndex * 100;
                    }
                }
            }
        }

        //State saving
        public override void saveState(BinaryWriter writer)
        {
            writer.Write(gameIsDone);
            player.savePlayerState(writer);
            base.saveState(writer);
        }

        public override void loadState(BinaryReader reader)
        {
            gameIsDone = reader.ReadBoolean();
            reader.ReadString();
            this.player = new Player(this, new Vector2(reader.ReadSingle(),reader.ReadSingle()));
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadDouble();
            player.loadState(reader);
            base.loadState(reader);
            actors.AddFirst(player);
            player.node = actors.First;
            sortActors();
        }
    }
}
