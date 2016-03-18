using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Engine.Acting.Attacks;
using Engine;
using Engine.Tiles;
using Engine.Acting;
using System.Diagnostics;
using Engine.Acting.Enemies;
using Engine.Textures;

namespace Engine
{
    public class BlobSpawnerBehavior : Behavior
    {
        Actor actor;
        private bool seen = false;
        private Actor target;
        public Animation spawningNone;
        private Animation spawningGreen;
        private Animation spawningBlue;
        private Animation spawningRed;
        private Animation spawningWhite;
        double color = -1;

        public BlobSpawnerBehavior(Actor actor, Actor target)
            : base(actor) 
        {
            this.actor = actor;
            
                
            this.target = target;
            color = actor.world.tileEngine.randGen.NextDouble();

            spawningGreen = new Animation(0, 9, 8f, false, 20, 20);
            spawningBlue  = new Animation(10, 18, 8f, false, 20, 20);
            spawningRed = new Animation(19, 27, 8f, false, 20, 20);
            spawningWhite = new Animation(28, 36, 8f, false, 20, 20);

            Animation.Action spawnSound = (frame) =>
            {
                actor.world.tileEngine.audioComponent.playSound(actor.audioSet[0], false);
            };
            spawningGreen.addFrameAct(7, spawnSound);
            spawningBlue.addFrameAct(16, spawnSound);
            spawningRed.addFrameAct(25, spawnSound);
            spawningWhite.addFrameAct(34, spawnSound);


            spawningGreen.addEndAct((frame) =>
            {
                
                    Blob b = (Blob)actor.world.actorFactory.createActor(actor.world.actorFactory.getActorId("Blob"), actor.position + new Vector2(74, 80), null, color);
                    actor.world.addActor(b);
                    (b as ILife).life.deathEvent += spawn;
                    blobCount++;
                    spawnCount++;
                    actor.anim = spawningNone;
            });
            
            
            spawningBlue.addEndAct((frame) =>
            {
                        Blob b = (Blob)actor.world.actorFactory.createActor(actor.world.actorFactory.getActorId("Blob"), actor.position + new Vector2(74, 80), null, color);
                        actor.world.addActor(b);
                        (b as ILife).life.deathEvent += spawn;
                        blobCount++;
                        spawnCount++;
                        actor.anim = spawningNone;

            });
           
            
            spawningRed.addEndAct((frame) =>
            {
               
                        Blob b = (Blob)(actor.world.actorFactory as GameActorFactory).createActor(actor.world.actorFactory.getActorId("Blob"), actor.position + new Vector2(74, 80), null, color);
                        actor.world.addActor(b);
                        (b as ILife).life.deathEvent += spawn;
                        blobCount++;
                        spawnCount++;
                        actor.anim = spawningNone;

            });
            spawningWhite.addEndAct((frame) =>
            {

                Blob b = (Blob)(actor.world.actorFactory as GameActorFactory).createActor(actor.world.actorFactory.getActorId("Blob"), actor.position + new Vector2(74, 80), null, color);
                actor.world.addActor(b);
                (b as ILife).life.deathEvent += spawn;
                blobCount++;
                spawnCount++;
                actor.anim = spawningNone;

            });
        }

        public bool needToSpawn = true;

        public void spawn()
        {
            blobCount--;
        }

        public int blobCount = 0;
        public int spawnCount = 0;
        public override void run() 
        {
            if (seen) {
                if ((target.position - actor.position).Length() < Constants.BLOB_BEHAVIOR_FOLLOWDIST )
                {
                    if (actor.anim != spawningBlue && actor.anim != spawningGreen && actor.anim != spawningRed && actor.anim != spawningWhite && (actor.world as GameWorld).player != null && (actor.world as GameWorld).blobCount < Constants.MAX_WORLD_BLOBS)
                    {
                        if (blobCount < Constants.NUM_BLOBS && (spawnCount < Constants.MAX_BLOBS || actor.world.worldName.Equals("012_Arena Mode")))
                        {
                            color = actor.world.tileEngine.randGen.NextDouble();
                            if (color < 0.5)
                            {
                                actor.anim = spawningGreen;
                            }
                            else if (color < 0.8)
                                actor.anim = spawningBlue;
                            else if (color < 0.995)
                                actor.anim = spawningRed;
                            else
                                actor.anim = spawningWhite;
                        }
                    }
                    
                }
                else
                {
                    seen = false;
                }
            }
            else
            {
                if ((target.position - actor.position).Length() < Constants.BLOB_BEHAVIOR_FOLLOWDIST && actor.world.hasLineOfSight(actor.position, target.position, true))
                {
                    seen = true;
                }
            }


        }

    }

    public class BlobSpawner : Actor 
    {

        const bool ACTIVE = true;

        const String TEX_DIR = "Sprites\\019_BlobSpawner";
        const int IMG_INDEX = 00;
        const int SIZE = 64;

        public BlobSpawner(GameWorld world, Vector2 position)
            : base(world, position, new Vector2(0, 0), SIZE, Constants.WORLD2MODEL_BLOBSPAWNER, IMG_INDEX)
        {
            actorName = "BlobSpawner";
            myBehavior = new BlobSpawnerBehavior(this, world.player);
            (myBehavior as BlobSpawnerBehavior).spawningNone = new Animation(0, 0, 8f, false, 20, 20);
            anim = (myBehavior as BlobSpawnerBehavior).spawningNone;

            active = ACTIVE;
            textureSet = world.tileEngine.resourceComponent.getTextureSet("019_BlobSpawner");

            //DAT MASKING
            this.actorcategory = 0;
            this.mass = Constants.BLOB_SPAWNER_MASS;
            this.frictionCoefficient = Constants.BLOB_SPAWNER_FRICTION;

            audioSet = world.tileEngine.resourceComponent.getAudioSet("025_BlobSpawn");
        }
    }
}