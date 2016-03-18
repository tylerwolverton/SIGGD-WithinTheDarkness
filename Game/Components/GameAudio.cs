using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

//Currently Not in Use
namespace Engine
{
    class GameAudio : AudioComponent
    {

        public GameAudio(MirrorEngine theEngine) : base(theEngine)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if ((tileEngine.world as GameWorld) != null && !(tileEngine.world as GameWorld).gameIsDone && isSoundDone((tileEngine.world as GameWorld).songSet[0]) && isSoundDone((tileEngine.world as GameWorld).songSet[2]))
            {


                if ((((tileEngine.world as GameWorld).hammerTriggered && !(tileEngine.world as GameWorld).hammerDead)) || ((tileEngine.world as GameWorld).blobTriggered && !(tileEngine.world as GameWorld).blobDead))
                {
                    if (!isSoundDone((tileEngine.world as GameWorld).songSet[1]))
                        stopSound((tileEngine.world as GameWorld).songSet[1]);
                    if (!isSoundDone((tileEngine.world as GameWorld).songSet[0]))
                        stopSound((tileEngine.world as GameWorld).songSet[0]);
                    if (!isSoundDone((tileEngine.world as GameWorld).songSet[2]))
                        stopSound((tileEngine.world as GameWorld).songSet[2]);

                    if (isSoundDone((tileEngine.world as GameWorld).songSet[3]))
                    {
                        playSound((tileEngine.world as GameWorld).songSet[3], false);
                    }
                }
                else
                {
                    if ((tileEngine.world as GameWorld).bossTriggered && (tileEngine as Graven).bossAlive)
                    {
                        if (!isSoundDone((tileEngine.world as GameWorld).songSet[1]))
                            stopSound((tileEngine.world as GameWorld).songSet[1]);
                        if (!isSoundDone((tileEngine.world as GameWorld).songSet[0]))
                            stopSound((tileEngine.world as GameWorld).songSet[0]);
                        if (!isSoundDone((tileEngine.world as GameWorld).songSet[3]))
                            stopSound((tileEngine.world as GameWorld).songSet[3]);

                        if (isSoundDone((tileEngine.world as GameWorld).songSet[2]))
                        {
                            playSound((tileEngine.world as GameWorld).songSet[2], false);
                        }
                    }
                    
                    else
                    {
                        if (!isSoundDone((tileEngine.world as GameWorld).songSet[3]))
                            stopSound((tileEngine.world as GameWorld).songSet[3]);
                        if (!isSoundDone((tileEngine.world as GameWorld).songSet[2]))
                            stopSound((tileEngine.world as GameWorld).songSet[2]);

                        if (isSoundDone((tileEngine.world as GameWorld).songSet[1]))
                        {
                            playSound((tileEngine.world as GameWorld).songSet[1], false);
                        }
                    }
                }
                    
            }        
        }

        public override void  LoadContent()
        {
            base.LoadContent();
        }
    }
}
