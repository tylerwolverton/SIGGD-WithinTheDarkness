using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace Engine
{

    public class AudioSet
    {

        public readonly ResourceComponent resourceComponent;
        public readonly string audioSetPath;

        public List<SoundEffectInstance> soundEffects;
        

        public AudioSet(ResourceComponent resourceComponent, string audioSetPath)
        {

            this.resourceComponent = resourceComponent;
            this.audioSetPath = audioSetPath;
            soundEffects = new List<SoundEffectInstance>();
        }

        public void LoadContent()
        {

            string tsPath = Path.Combine(resourceComponent.contentManager.RootDirectory, audioSetPath);
            IEnumerable<string> soundEffectNames;
            try
            {
                soundEffectNames = Directory.EnumerateFiles(tsPath);
            }
            catch (Exception e) { return; }

            foreach (string s in soundEffectNames)
            {

                string contPath = Path.Combine(audioSetPath, Path.GetFileNameWithoutExtension(s));
                soundEffects.Add(resourceComponent.contentManager.Load<SoundEffect>(contPath).CreateInstance());
            }
        }

        public int Count()
        {
            return soundEffects.Count;
        }

        public SoundEffectInstance this[int index]
        {
            get
            {
                if (index > soundEffects.Count - 1)
                {
                    index = soundEffects.Count - 1;
                }

                return soundEffects[index];
            }
        }
    }
}
