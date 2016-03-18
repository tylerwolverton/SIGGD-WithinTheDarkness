/*The reason we don't use this is because stealing from Graven improves modularity.
  If games based on the engine name things in a similar naming convention, the Editor can just
 * use their content directly because it's in the same namespace.
 */
#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Engine;

namespace Engine
{
    public class EditorResources : ResourceComponent
    {

        //public int dieSound;

        public EditorResources(ContentManager cm, MirrorEngine theEngine)
            : base(cm, theEngine)
        {
            string path;

            path = Path.Combine("Test", "GUI");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Tiles");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "005_Arrow");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "001_Blob");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "000_Lucy");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "006_HealthOrb");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "007_ManaOrb");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "012_Octo");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "014_Spikon");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "015_Sentinel");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "003_FireShuriken");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "008_IceSpike");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "011_Numbers");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "004_FirstBoss");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "010_LevelUp");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "009_Laser");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "016_Zazzle");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "002_BlueBlob");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "013_RedBlob");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "017_DemonQuestionMark");
            textureSets.Add(path, new TextureSet(this, path));

            path = Path.Combine("Test", "Sprites", "018_EmptyActor");
            textureSets.Add(path, new TextureSet(this, path));
        }

        public override void LoadContent()
        {

            base.LoadContent();
        }
    }
}
#endif