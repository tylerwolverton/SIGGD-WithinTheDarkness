using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Engine.Components;

namespace Engine
{

    public class AudioComponent : Component
    {

        ResourceComponent resourceComponent;
        public Dictionary<string, AudioSet> audioSets;

        public AudioComponent(MirrorEngine theEngine) : base(theEngine)
        {

            resourceComponent = theEngine.resourceComponent;
            audioSets = resourceComponent.audioSets;
        }

        /////////////////////////
        //Sound engine management
        /////////////////////////

        //Sets master volume from 0 to 1
        public void setMasterVolume(float v)
        {

            if (v < 0.0f)
                v = 0.0f;
            else if (v > 1.0f)
                v = 1.0f;

            SoundEffect.MasterVolume = v;
        }
        
        public void pauseSoundEngine()
        {
            foreach (KeyValuePair<string, AudioSet> pair in audioSets)
            {
                foreach (SoundEffectInstance s in pair.Value.soundEffects)
                {
                        s.Pause();
                }
            }
        }

        public void resumeSoundEngine()
        {
            foreach (KeyValuePair<string, AudioSet> pair in audioSets)
            {
                foreach (SoundEffectInstance s in pair.Value.soundEffects)
                {
                    if (s.State == SoundState.Paused)
                    {
                        s.Resume();
                    }
                }
            }
        }

        public void stopSoundEngine()
        {
            foreach (KeyValuePair<string, AudioSet> pair in audioSets)
            {
                foreach (SoundEffectInstance s in pair.Value.soundEffects)
                {

                    try
                    {
                        s.Stop();
                    }
                    catch (Exception) { }
                }
            }
        }

        

        /////////////////////////
        //Sound Effect Management
        /////////////////////////

        public void setSoundVolume(SoundEffectInstance s, float v)
        {

            if (v > 1.0f) v = 1.0f;
            else if (v < 0.0f) v = 0.0f;

            s.Volume = v;
        }

        public void setSoundPitch(SoundEffectInstance s, float p)
        {

            if (p > 1.0f) p = 1.0f;
            else if (p < 0.0f) p = 0.0f;

            s.Pitch = p;
        }

        public bool isSoundDone(SoundEffectInstance s)
        {

            return s.State == SoundState.Stopped;
        }

        public void playSound(SoundEffectInstance s, bool loop)
        {
            //If you are playing, SHUT UP and restart
            if (!this.isSoundDone(s))
            {
                s.Stop();
            }


            try //Setting sounds to loop in XNA is odd... 
            {
                s.IsLooped = loop;
            }
            catch (Exception) { }

            s.Play(); //Play anyway!
        }

        public void pauseSound(SoundEffectInstance s)
        {
            s.Pause();
        }
        
        public void resumeSound(SoundEffectInstance s)
        {
            s.Resume();
        }

        public void stopSound(SoundEffectInstance s)
        {
            s.Stop();
        }
    }
}
