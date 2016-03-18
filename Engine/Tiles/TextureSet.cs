using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Engine
{

    public class TextureSet
    {

        public readonly ResourceComponent resourceComponent;
        public readonly string tileSetPath;

        private List<Texture2D> textures;

        public TextureSet(ResourceComponent resourceComponent, string tileSetName)
        {

            this.resourceComponent = resourceComponent;
            this.tileSetPath = tileSetName;
            textures = new List<Texture2D>();
        }

        public void LoadContent()
        {

            string tsPath = Path.Combine(resourceComponent.contentManager.RootDirectory, tileSetPath);
            IEnumerable<string> textureNames;
            try
            {
                textureNames = Directory.EnumerateFiles(tsPath);
            }
            catch (Exception e) { return; }

            foreach (string s in textureNames)
            {

                string contPath = Path.Combine(tileSetPath, Path.GetFileNameWithoutExtension(s));
                if (!contPath.Contains("humbs"))
                textures.Add(resourceComponent.contentManager.Load<Texture2D>(contPath));
            }
        }

        public int Count()
        {
            return textures.Count;
        }
        
        public Texture2D this[int index] 
        {
            get
            {
                if (index > textures.Count - 1)
                {
                    index = textures.Count - 1;
                }

                if (textures.Count == 0) return null;

                return textures[index];
            }
        }
    }
}
