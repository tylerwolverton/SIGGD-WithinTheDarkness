using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Acting.Projectiles;
using Engine.Acting.Items;
using Engine.Acting.Enemies;
using Engine.Acting.Bosses;
using Engine.Tiles;

namespace Engine.Acting
{
    public class GameActorFactory : ActorFactory
    {
        private GameWorld world;
        private const int PROJ_BEGIN = 5;

        private int checkpointCount = 0;

        public GameActorFactory() { }

        public GameActorFactory(World world) {
            this.world = world as GameWorld;

            names = new Dictionary<string, int>()
            {
                {"Player", 0},////SAVEABLE
                {"Octo", 1},////
                {"Blob", 2},////
                {"Sentinel", 3},////
                {"Spikon", 4},////
                {"Arrow", 5},
                {"SlimeBall", 6},
                {"HealthOrb",7},
                {"FireShuriken",8},
                {"IceSpike",9},
                {"Numbers",10},
                {"FirstBoss",11},
                {"LevelUp",12},
                {"Laser",13},
                {"FirePillar",14},
                {"ManaOrb",15},
                {"Zazzle",16},////
                {"ZazzleShot",17},
                {"Basilisk", 18},
                {"BlobSpawner", 19},////
                {"Charger", 20},////
                {"Grapple",21},
                {"Generator", 22},////
                {"Explosion", 23},
                {"Torch", 24},
                {"MagicPrimary", 25},
                {"WizBlob", 26},////
                {"LaserArrow", 27},
                {"DiscoBlob", 28},
                {"FireBlob", 29},
                {"MrHammer", 30},
                {"GigaBlob", 31},
                {"StunWarning", 32},
                {"DarkLucyPortrait", 33},
                {"BloodyArrow", 34},
                {"LucyFirePillar", 35},
                {"Wedding Photo", 36},
                {"FancyPortrait", 37},
                {"CoatOfArms", 38},
                {"TandemBike", 39},
                {"KnightStatue", 40},
                {"OctoPortrait", 41},
                {"SwordSlash", 42},
            };
        }

        public override Actor createActor(int id, Vector2 position, Microsoft.Xna.Framework.Vector2? velocity = null, double color = -1)
        {
            Actor a = null;
            if ((position.X >= 0 && position.X < world.width * Tile.size && position.Y >= 0 && position.Y < world.height * Tile.size))
            switch (id)
            {
                case 0:
                    a = new Player(world, position);
                    return a;
                case 1:
                    a = new Octo(world, position);
                    break;
                case 2:
                    a = new Blob(world, position, velocity ?? Vector2.Zero, color);
                    break;
                case 3:
                    a = new Sentinel(world, position);
                    break;
                case 4:
                    a = new Spikon(world, position);
                    break;
                case 5:
                    a = new Arrow(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 6:
                    a = new OctoBall(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 7:
                    a = new HealthOrb(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 8:
                    a = new FireShuriken(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 9:
                    a = new IceSpike(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 10:
                    a = new Numbers(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 11:
                    a = new FirstBoss(world, position);
                    return a;
                case 12:
                    a = new LevelUp(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 13:
                    a = new Laser(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 14:
                    a = new FirePillar(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 15:
                    a = new ManaOrb(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 16:
                    a = new Zazzle(world, position);
                    return a;
                case 17:
                    a = new ZazzleShot(world, position, velocity ?? Vector2.Zero);
                    return a;             
                case 18:
                    a = new Basilisk(world, position);
                    return a;
                case 19:
                    a = new BlobSpawner(world, position);
                    return a;
                case 20:
                    a = new Charger(world, position);
                    return a;
                case 21:
                    a = new Grapple(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 22:
                    a = new Generator(world, position);
                    return a;
                case 23:
                    a = new Explosion(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 24:
                    a = new Torch(world, position);
                    return a;
                case 25:
                    a = new MagicPrimary(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 26:
                    a = new WizBlob(world, position);
                    return a;                  
                case 27:
                    a = new LaserArrow(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 28:
                    a = new DiscoBlob(world, position, velocity ?? Vector2.Zero, color, checkpointCount++);
                    break;
                case 29:
                    a = new FireBlob(world, position, velocity ?? Vector2.Zero);
                    break;
                case 30:
                    a = new MrHammer(world, position, velocity ?? Vector2.Zero);
                    break;
                case 31:
                    a = new GigaBlob(world, position, velocity ?? Vector2.Zero);
                    break;
                case 32:
                    a = new MagicPrimary(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 33:
                    a = new DarkLucyPortrait(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 34:
                    a = new BloodyArrow(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 35:
                    a = new LucyFirePillar(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 36:
                    a = new WeddingPhoto(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 37:
                    a = new FancyPortrait(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 38:
                    a = new CoatOfArms(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 39:
                    a = new TandemBike(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 40:
                    a = new KnightStatue(world, position, velocity ?? Vector2.Zero);
                    return a;
                case 41:
                    a = new SwordSlash(world, position, velocity ?? Vector2.Zero);
                    return a;
            }

            return a;
        }
    }
}
