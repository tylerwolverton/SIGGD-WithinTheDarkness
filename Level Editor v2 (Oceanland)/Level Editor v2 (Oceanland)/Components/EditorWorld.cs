using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Acting;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class EditorWorld : World
    {
        public Editor editor;
        //public EditorActorFactory actorFactory;
        public EmptyActor player;
        public MapFile.TileData currentTile;
        public int currentActor;
        public bool scrollLock = false;
        public bool isInitializedAlready = false;
        public EditorWorld(MirrorEngine theEngine, String theMap) : base(theEngine, theMap)
        {
            tileTextureSet = tileEngine.resourceComponent.getTextureSet("Tiles");
            currentTile = new MapFile.TileData();
            currentTile.tileSetIndex = 0;
            currentTile.tileIndex = 0;
            currentActor = 0;
            editor = theEngine as Editor;
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void UnloadContent()
        {

            //for demo
            editor.audioComponent.stopSoundEngine();

            base.UnloadContent();
        }


        public override void Initialize()
        {     
                if (file == null)
                    return;

                // Load the map into memory from disk
                file.load();

                initTiles();
                actorFactory = new EditorActorFactory(this);
                initActors();

                sortActors();
                this.addActor(actorFactory.createActor(-1, new Vector2(16, 16)));

                this.player = actors.First.Value as EmptyActor;
                // GC the map
                //file = null;
        }

        public override void initActors()
        {

            actors = new LinkedList<Actor>();

            base.initActors();
            
         }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }
    }

}
