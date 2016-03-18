using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class Constants
    {
        //ARROW
        public const float ARROW_SUPERIMPULSE = 70f;
        public const float ARROW_SUPERDAMAGE = 20f;
        public const int ARROW_DAMAGE = 20;
        public const int ARROW_SPEED = 10;
        public const float ARROW_FRICTION = 0.0001f;
        public const int ARROW_HEALTH = 100;
        public const int ARROW_MASS = 100;
        public const int ARROW_GLOW = 2;
        public const float ARROW_ELASTICITY = .2f;
        public const int ARROW_WALLBOUNCE = 0;

        //ARROW ANIMATION
        public const int ARROW_ANIM_NUMBER = 16;
        public const int ARROW_FRAMES_PER_ARROW = 2;

        //ASSASSINATE
        public const int ASSASSINATE_TIME_BETWEEN_DASHES = 25;
        public const int ASSASSINATE_FRAMES_TO_TARGET = 30;
        public const int ASSASSINATE_DISTANCE = 400;

        //BASILISK
        public const int BASILISK_HEALTH = 200;
        public const int BASILISK_EXP = 150;
        public const int BASILISK_DAMAGE = 14;
        public const int BASILISK_MASS = 60;
        public const float BASILISK_FRICTION = 0.1f;
        public const int BASILISK_SIZE = 32;
        public const int BASILISK_IMG_INDEX = 0;
            //CHARGE
            public const int BASILISK_CHARGE_DELAY = 200;
            public const int BASILISK_CHARGE_COOLDOWN = 300;
            //ACID PIT
            public const int BASILISK_ACID_PIT_DELAY = 300;
            public const int BASILISK_ACID_PIT_COOLDOWN = 150;
            public const int BASILISK_ACID_PIT_DIST = 125;
            

        //BLOB
        public const int GREEN_BLOB_DAMAGE = 5;
        public const int GREEN_BLOB_HEALTH = 60;
        public const int GREEN_BLOB_MASS = 35;
        public const float GREEN_BLOB_FRICTION = 0.13f;

        public const int BLUE_BLOB_DAMAGE = 5;
        public const int BLUE_BLOB_HEALTH = 50;
        public const int BLUE_BLOB_MASS = 30;
        public const float BLUE_BLOB_FRICTION = 0.095f;

        public const int RED_BLOB_DAMAGE = 5;
        public const int RED_BLOB_HEALTH = 45;
        public const int RED_BLOB_MASS = 15;
        public const float RED_BLOB_FRICTION = 0.05f;

        public const int WHITE_BLOB_DAMAGE = 0;
        public const int WHITE_BLOB_HEALTH = 10;
        public const int WHITE_BLOB_MASS = 5;
        public const float WHITE_BLOB_FRICTION = 0.005f;

        public const double BLOB_DROPCHANCE = .1;
        public const string BLOB_DIESOUND = "Test/Sound/blobDeath";
        public const string BLOB_HURTSOUND = "Test/Sound/blobHurt";

        public const int MAX_WORLD_BLOBS = 300;

        //BLOB BEHAVIOR
        public const int BLOB_BEHAVIOR_FOLLOWDIST = 400;
        public const float BLOB_BEHAVIOR_SHOTRATE = 100f;
        public const float BLOB_BEHAVIOR_SHOTCOOLDOWN = 160f;
        public const float BLOB_BEHAVIOR_PATHFIND = .09f;
        public const string BLOB_BEHAVIOR_SHOT = "Blob";

        //BLOB SPAWNER
        public const int NUM_BLOBS = 3;
        public const int MAX_BLOBS = 5;
        public const int BLOB_SPAWNER_MASS = 100;
        public const float BLOB_SPAWNER_FRICTION = 1f;

        //CHARGER
        public const int CHARGER_MAX_GLOBAL_NUMBER = 7; //The max chargers everywhere, they won't be spawned or split past this number
        public const int CHARGER_BEHAVIOR_FOLLOWDIST = 700;
        public const int CHARGER_BEHAVIOR_ATTACKDIST = 40;
        public const float CHARGER_BEHAVIOR_PATHFIND = 1.3f;
        public const float CHARGER_CLONE_RATE = 90f;
        public const int CHARGER_EXP = 5;
        public const int CHARGER_HEALTH = 15;
        public const int CHARGER_MASS = 50;
        public const float CHARGER_FRICTION = .8f;
        public const int CHARGER_DAMAGE = 2;

        //EXPLOSION
        public const int EXPLOSION_SIZE = 25;
        public const int EXPLOSION_HEALTH = 1;
        public const float EXPLOSION_FRICTION = .1f;

        //FIREBLOB
        public const int FIREBLOB_DAMAGE = 25;

        //FIRE SHURIKEN
        public const int FIRESHURIKEN_SPEED = 4;
        public const int FIRESHURIKEN_HEALTH = 75;
        public const int FIRESHURIKEN_SIZE = 24;
        public const float FIRESHURIKEN_FRICTION = 0.05f;
        public const int FIRESHURIKEN_MASS = 200;
        public const float FIRESHURIKEN_ELASTICITY = .5f;
        public const int FIRESHURIKEN_DAMAGE = 5;

        //GENERATOR
        public const int NUM_CHARGERS = 4; //The number of chargers each generator will try to spawn, but they won't exceed CHARGER_MAX_GLOBAL_NUMBER
        public const int GENERATOR_HEALTH = 40;
        public const int GENERATOR_EXP = 50;
        public const int GENERATOR_DAMAGE = 5;
        public const float GENERATOR_DROPCHANCE = 0.50f;
        public const float GENERATOR_MASS = float.MaxValue;
        public const float GENERATOR_FRICTION = 1f;
        public const int GENERATOR_FOLLOWDIST = 700;
        public const float GENERATOR_BEHAVIOR_PATHFIND = 0.09f;
        public const int GENERATOR_SPAWNCHARGER_RANGE = 360;
        public const int GENERATOR_SIZE = 31;

        //GIGA BLOB
        public const int GIGABLOB_HEALTH = 500;

        //GIGA BURST
        public const int GIGABURST_DAMAGE = 10;
        public const float GIGABURST_KNOCKBACK = 5000000f;

        //GRAPPLE
        public const int GRAPPLE_SPEED = 10;
        public const int GRAPPLE_COOLDOWN = 30;
        public const int GRAPPLE_MAXDISTANCE = 1;
        public const float GRAPPLE_FRICTION = 0.0001f;
        public const int GRAPPLE_HEALTH = 100;
        public const int GRAPPLE_GLOW = 2;
        public const int GRAPPLE_MASS = 100;
        public const float GRAPPLE_ELASTICITY = .2f;
        public const int GRAPPLE_DURATION = 20;
        public const int GRAPPLE_STUN_DURATION = 100;

        //ICE SPIKE
        public const int ICE_SIZE = 30;
        public const int ICE_HEALTH = 25;
        public const float ICE_FRICTION = .1f;
        public const int ICE_DURATION = 300;

        //LUCY
        public const float LUCY_MAXMANA = 100f;
        public const float LUCY_MANAREGEN = 1.0f;
        public const int LUCY_FIREPILLAR_DAMAGE = 15;
        public const int LUCYFIREPILLAR_DURATION = 18;
        public const int LUCY_GIGABURST_COST = 30;
        public const int LUCY_SWORD_COST = 0;
        public const int LUCY_ICE_COST = 50;
        public const int LUCY_SHURIKEN_COST = 20;
        public const int LUCY_LASER_COST = 25;
        public const int LUCY_ARROW_COST = 15;
        public const int LUCY_PULSE_RATE = 50;

        //MAGIC PRIMARY
        public const int MAGICPRIMARY_DAMAGE = 3;
        public const int MAGICPRIMARY_SPEED = 75;
        public const float MAGICPRIMARY_FRICTION = 1f;
        public const int MAGICPRIMARY_HEALTH = 100;
        public const int MAGICPRIMARY_MASS = 5000;
        public const float MAGICPRIMARY_GLOW = 2.0f;
        public const int MAGICPRIMARY_COLLISION = 0;

        //MR HAMMER
        public const int MRHAMMER_FOLLOWDIST = 600;
        public const float MRHAMMER_PATHFIND = .09f;
        public const int MRHAMMER_ATTACKDIST = 90;
        public const int MRHAMMER_HAMMER_DAMAGE = 25;
        public const float MRHAMMER_KNOCKBACK = 5000f;
        public const int MRHAMMER_DAMAGE = 12;
        public const int MRHAMMER_HEALTH = 1200;
        public const int MRHAMMER_MASS = 350;
        public const float MRHAMMER_FRICTION = 0.1f;
        
        //MR HAMMER CHARGE
        public const int MRHAMMER_CHARGEDIST = 325;
        public const int MRHAMMER_MIN_CHARGE = 175;
        public const int MRHAMMER_CHARGE_COUNT = 360;
        public const int MRHAMMER_CHARGE_TIMEOUT = 90;

        //OCTO 
        public const int OCTO_DAMAGE = 5;
        public const int OCTO_HEALTH = 100;
        public const int OCTO_MASS = 35;
        public const int OCTO_RADIUS = 16;

        //OCTOBALL
        public const int OCTOBALL_DAMAGE = 10;
        public const int OCTOBALL_MASS = 15;
        public const int OCTOBALL_RADIUS = 2;
        public const int OCTOBALL_HEALTH = 100;

        //SENTINEL
        public const int SENTINEL_EXP = 5;
        public const int SENTINEL_HEALTH = 40;
        public const int SENTINEL_MASS = 50;
        public const float SENTINEL_FRICTION = .08f;

        //SENTINEL BEHAVIOR
        public const int SENTINEL_BEHAVIOR_FOLLOWDIST = 1000;
        public const int SENTINEL_BEHAVIOR_FOLLOWDIST_RUN = 150;
        public const float SENTINEL_BEHAVIOR_PATHFIND = 10f;
        public const float SENTINEL_BEHAVIOR_SHOTRATE = 100f;
        public const float SENTINEL_BEHAVIOR_SHOTCOOLDOWN = 160f;
        public const string SENTINEL_BEHAVIOR_SHOT = "Laser";

        //SENTINAL LASER
        public const float SENTINAL_LASER_FRICTION = 0.0001f;
        public const float SENTINAL_LASER__ELASTICITY = .2f;
        public const float SENTINAL_LASER_MASS = 100.0f;
        public const int SENTINAL_LASER_DAMAGE = 5;

        //SPIKON
        public const int SPIKON_DAMAGE = 25;
        public const int SPIKON_EXP = 25;
        public const int SPIKON_HEALTH = 250;
        public const int SPIKON_MASS = 150;

        //SPIKON BEHAVIOR
        public const float SPIKON_BEHAVIOR_PATHFIND = .15f;
        public const int SPIKON_BEHAVIOR_FOLLOWDIST = 700;

       


        //WIZBLOB
        public const float WIZBLOB_FIREBLOB_SPEED = 1;
        public const float WIZBLOB_FRICTION = .1f;


        //ZAZZLE
        public const int ZAZZLE_SIZE = 32;

        //World2Model's (ordered by GameActorFactory
        public static readonly Vector2 WORLD2MODEL_PLAYER = new Vector2(-16, -50);
        public static readonly Vector2 WORLD2MODEL_OCTO = new Vector2(-16, -16);
        public static readonly Vector2 WORLD2MODEL_BLOB = new Vector2(-16, -24);
        public static readonly Vector2 WORLD2MODEL_SENTINEL = new Vector2(-16, -16);
        public static readonly Vector2 WORLD2MODEL_SPIKON = new Vector2(-16, -24);
        public static readonly Vector2 WORLD2MODEL_ARROW = new Vector2(-32, -32);
        public static readonly Vector2 WORLD2MODEL_OCTOBALL = new Vector2(-8, -31);
        public static readonly Vector2 WORLD2MODEL_HEALTHORB = new Vector2(-8, -16);
        public static readonly Vector2 WORLD2MODEL_FIRESHURIKEN = new Vector2(-16, -32);
        public static readonly Vector2 WORLD2MODEL_ICESPIKE = new Vector2(-16, -58);
        public static readonly Vector2 WORLD2MODEL_NUMBERS = new Vector2(-8, -66);
        public static readonly Vector2 WORLD2MODEL_FIRSTBOSS = new Vector2(-120, -170);
        public static readonly Vector2 WORLD2MODEL_LEVELUP = new Vector2(0, -76);
        public static readonly Vector2 WORLD2MODEL_LASER = new Vector2(-32, -32);
        public static readonly Vector2 WORLD2MODEL_FIREPILLAR = new Vector2(-64, -236);
        public static readonly Vector2 WORLD2MODEL_MANAORB = new Vector2(-8, -16);
        public static readonly Vector2 WORLD2MODEL_ZAZZLE = new Vector2(-16, -24);
        public static readonly Vector2 WORLD2MODEL_ZAZZLESHOT = new Vector2(-8, -31);
        public static readonly Vector2 WORLD2MODEL_BASILISK = new Vector2(-16, -24);
        public static readonly Vector2 WORLD2MODEL_BLOBSPAWNER = new Vector2(20, 20);
        public static readonly Vector2 WORLD2MODEL_CHARGER = new Vector2(-16, -24);
        public static readonly Vector2 WORLD2MODEL_GRAPPLE = new Vector2(-32, -32);
        public static readonly Vector2 WORLD2MODEL_GENERATOR = new Vector2(-16, -16);
        public static readonly Vector2 WORLD2MODEL_EXPLOSION = new Vector2(-30, -48);
        public static readonly Vector2 WORLD2MODEL_TORCH = new Vector2(-8, -16);
        public static readonly Vector2 WORLD2MODEL_MAGICPRIMARY = new Vector2(-8, -16);
        public static readonly Vector2 WORLD2MODEL_WIZBLOB = new Vector2(-16, -16);
        public static readonly Vector2 WORLD2MODEL_LASERARROW = new Vector2(-16, -16);
        public static readonly Vector2 WORLD2MODEL_DISCOBLOB = new Vector2(0, 0);
        public static readonly Vector2 WORLD2MODEL_FIREBLOB;  // Doesn't exist wtf fuck faces
        public static readonly Vector2 WORLD2MODEL_MRHAMMER = new Vector2(0, 24);
        public static readonly Vector2 WORLD2MODEL_GIGABLOB = 3 * WORLD2MODEL_BLOB;
        public static readonly Vector2 WORLD2MODEL_STUNWARNING = new Vector2(-64, -64);
        public static readonly Vector2 WORLD2MODEL_DARKLUCYPORT = new Vector2(-16, -16);
        public static readonly Vector2 WORLD2MODEL_BLOODYARROW = new Vector2(-24, -28);
        public static readonly Vector2 WORLD2MODEL_LUCYFIREPILLAR; //Unneeded kept for order sake
        public static readonly Vector2 WORLD2MODEL_WEDDINGPHOTO = new Vector2(-32, -32);
        public static readonly Vector2 WORLD2MODEL_FANCYPORT = new Vector2(-16, -16);
        public static readonly Vector2 WORLD2MODEL_COATOFARMS = new Vector2(-16, -16);
        public static readonly Vector2 WORLD2MODEL_TANDEM = new Vector2(-32, -32);
        public static readonly Vector2 WORLD2MODEL_KNIGHTSTATUE = new Vector2(-16, -32);
    }
}
