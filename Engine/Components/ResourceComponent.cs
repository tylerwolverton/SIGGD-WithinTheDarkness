using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Engine
{

    public class ResourceComponent : Component
    {
        
        public ContentManager contentManager;
        public Dictionary<string, TextureSet> textureSets;
        public Dictionary<string, AudioSet> audioSets;
        public Dictionary<string, uint> worldNames;
        public World[] worldArray;

        private const string worldPath = "Maps";

        public ResourceComponent(ContentManager contentManager, MirrorEngine theEngine) : base(theEngine)
        {

            //Dynamic Loading
            this.contentManager = contentManager;
            contentManager.RootDirectory = "Content";

            //Load TextureSets
            textureSets = new Dictionary<string, TextureSet>();
            discoverGUI();
            discoverTiles();
            discoverSprites();
            
            //Load AudioSets
            audioSets = new Dictionary<string, AudioSet>();
            discoverAudio();

            //Load Worlds
            worldNames = new Dictionary<string, uint>();
            discoverWorlds();
        }

        public override void LoadContent()
        {

            foreach (var tSet in textureSets)
            {
                tSet.Value.LoadContent();
            }

            foreach(var aSet in audioSets)
            {
                aSet.Value.LoadContent();
            }
        }

        public TextureSet getTextureSet(string name)
        {
            return textureSets[name];
        }

        public AudioSet getAudioSet(string name)
        {
            return audioSets[name];
        }

        private void discoverGUI()
        {

            string contentPath = Path.Combine(contentManager.RootDirectory, "GUI");
            IEnumerable<string> guiNames;
            try
            {
                guiNames = Directory.EnumerateDirectories(contentPath);
            }
            catch (Exception e) { return; }

            foreach (string s in guiNames)
            {
                string name = Path.GetFileNameWithoutExtension(s);
                string path = Path.Combine("GUI", name);
                textureSets.Add(name, new TextureSet(this, path));
            }
        }

        private void discoverTiles()
        {

            string contentPath = Path.Combine(contentManager.RootDirectory, "Tiles");
            IEnumerable<string> tileNames;
            try
            {
                tileNames = Directory.EnumerateDirectories(contentPath);
            }
            catch (Exception e) { return; }

            foreach(string s in tileNames){
                string name = Path.GetFileNameWithoutExtension(s);
                string path = Path.Combine("Tiles", name);
                textureSets.Add(name, new TextureSet(this, path));
            }
        }

        private void discoverSprites()
        {

            string contentPath = Path.Combine(contentManager.RootDirectory, "Sprites");
            IEnumerable<string> textureNames;
            try
            {
                textureNames = Directory.EnumerateDirectories(contentPath);
            }
            catch (Exception e) { return; }

            foreach (string s in textureNames)
            {
                string name = Path.GetFileNameWithoutExtension(s);
                string path = Path.Combine("Sprites", name);
                textureSets.Add(name, new TextureSet(this, path));
            }
        }

        private void discoverAudio()
        {

            string contentPath = Path.Combine(contentManager.RootDirectory, "Audio");
            IEnumerable<string> audioNames;
            try
            {
                audioNames = Directory.EnumerateDirectories(contentPath);
            }
            catch (Exception e) { return; }

            foreach (string s in audioNames)
            {   
                string name = Path.GetFileNameWithoutExtension(s);
                string path = Path.Combine("Audio", name);
                audioSets.Add(name, new AudioSet(this, path));
            }
        }

        private void discoverWorlds()
        {
            string path = Path.Combine(contentManager.RootDirectory, worldPath);
            uint i = 0;
            IEnumerable<string> mapNames;
            try
            {
                mapNames = Directory.EnumerateFiles(path);
            }
            catch (Exception e) { return; }

            foreach (string s in mapNames)
            {
                worldNames.Add(s, i++);
            }

            worldArray = new World[i];
        }
    }
}
