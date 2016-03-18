using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Engine.Tiles;
namespace Engine.Acting
{
    public class EmptyActor : Actor
    {
        public EmptyActor(EditorWorld editor, Vector2 position, int ActorID):base(editor, Vector2.Zero,Vector2.Zero, 0, Vector2.Zero, 0 )
        {
            this.actorcategory = ActorCategory.nocategory;
            this.ignoreAvE = true;
            this.position = position;
            this.world = editor;

            //this.glowchargerate = 3f;
            //this.glow = 3f;
            this.active = true;
            
            myBehavior = new EditBehavior(this);
            this.collisionmask = 0;

            switch(ActorID)
            {
                case -1://BORK
                    world2model = new Vector2(-32, -32);
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("018_EmptyActor");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 0: //Lucy
                    world2model = Constants.WORLD2MODEL_PLAYER;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("000_Lucy");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 1: //Octo
                    world2model = Constants.WORLD2MODEL_OCTO;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("012_Octo");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 2: //Blob
                    world2model = Constants.WORLD2MODEL_BLOB;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("001_Blob");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 3: //Sentinal
                    world2model = Constants.WORLD2MODEL_SENTINEL;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("015_Sentinel");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 4: //Spikon
                    world2model = Constants.WORLD2MODEL_SPIKON;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("014_Spikon");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 5: //Arrow
                    world2model = Constants.WORLD2MODEL_ARROW;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("005_Arrow");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 6: //OctoBall
                    world2model = Constants.WORLD2MODEL_OCTOBALL;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("012_Octo");
                    anim = new Animation(20, 20, 8f, true);
                    break;
                case 7:
                    world2model = new Vector2(-8, -16);
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("006_HealthOrb");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 8:
                    world2model =Constants.WORLD2MODEL_FIRESHURIKEN;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("003_FireShuriken");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 9: 
                    world2model =Constants.WORLD2MODEL_ICESPIKE;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("008_IceSpike");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 10:
                    world2model = Constants.WORLD2MODEL_NUMBERS;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("011_Numbers");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 11:
                    world2model = Constants.WORLD2MODEL_FIRSTBOSS;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("004_FirstBoss");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 12:
                    world2model = Constants.WORLD2MODEL_LEVELUP;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("010_LevelUp");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 13:
                    world2model = Constants.WORLD2MODEL_LASER;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("009_Laser");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 14:
                    world2model = Constants.WORLD2MODEL_FIRSTBOSS;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("004_FirstBoss");
                    anim = new Animation(52, 52, 8f, true);
                    break;
                case 15:
                    world2model = Constants.WORLD2MODEL_MANAORB;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("007_ManaOrb");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 16:
                    world2model = Constants.WORLD2MODEL_ZAZZLE;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("016_Zazzle");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 17:
                    world2model = Constants.WORLD2MODEL_ZAZZLESHOT;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("016_Zazzle");
                    anim = new Animation(1, 1, 8f, true);
                    break;
                case 18:
                    world2model = Constants.WORLD2MODEL_BASILISK;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("039_Basilisk");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 19:
                    world2model = Constants.WORLD2MODEL_BLOBSPAWNER;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("019_BlobSpawner");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 20:
                    world2model = Constants.WORLD2MODEL_CHARGER;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("020_Charger");
                    anim = new Animation(5, 5, 8f, true);
                    break;
                case 21:
                    world2model = Constants.WORLD2MODEL_GRAPPLE;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("021_Grapple");
                    anim = new Animation(16, 16, 8f, true);
                    break;
                case 22:
                    world2model = Constants.WORLD2MODEL_GENERATOR;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("022_Generator");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 23:
                    world2model = Constants.WORLD2MODEL_EXPLOSION;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("023_Explosion");
                    anim = new Animation(1, 1, 8f, true);
                    break;
                case 24: //Torch
                    world2model = Constants.WORLD2MODEL_TORCH;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("024_Torch");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 25:
                    world2model = Constants.WORLD2MODEL_MAGICPRIMARY;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("025_MagicPrimary");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 26:
                    world2model = Constants.WORLD2MODEL_WIZBLOB;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("026_WizBlob");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 27:
                    world2model = Constants.WORLD2MODEL_LASERARROW;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("025_LaserLucy");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 28: //Disco Blob
                    world2model = Constants.WORLD2MODEL_DISCOBLOB;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("029_DiscoBlob");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 29: //FireBlob
                    world2model = new Vector2(-64, -64); //doesn't exist!?!?
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("026_WizBlob");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 30: //MR Hammer
                    world2model = Constants.WORLD2MODEL_MRHAMMER;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("030_Hammer");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 31: //GigaBlob
                    world2model = Constants.WORLD2MODEL_GIGABLOB;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("031_GigaBlob");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 32: //Stun Warning
                    world2model = Constants.WORLD2MODEL_STUNWARNING;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("038_StunWarning");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 33: //Lucy Portrait
                    world2model = Constants.WORLD2MODEL_DARKLUCYPORT;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("037_Details");
                    anim = new Animation(0, 0, 8f, true);
                    break;
                case 34: //Bloody Arrow
                    world2model = Constants.WORLD2MODEL_BLOODYARROW;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("037_Details");
                    anim = new Animation(1, 1, 8f, true);
                    break;
                case 35: //Bloody Lucy Fire Pillar
                    world2model = Constants.WORLD2MODEL_LUCYFIREPILLAR;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("004_FirstBoss");
                    anim = new Animation(52, 52, 8f, true);
                    break;
                case 36: //Wedding Photo
                    world2model = Constants.WORLD2MODEL_WEDDINGPHOTO;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("037_Details");
                    anim = new Animation(2, 2, 8f, true);
                    break;
                case 37: //Fancy Portrait
                    world2model = Constants.WORLD2MODEL_FANCYPORT;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("037_Details");
                    anim = new Animation(3, 3, 8f, true);
                    break;
                case 38: //Coat of Arms
                    world2model = new Vector2(-16, -16);
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("037_Details");
                    anim = new Animation(4, 4, 8f, true);
                    break;
                case 39: //Tandem Bike
                    world2model = Constants.WORLD2MODEL_TANDEM;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("037_Details");
                    anim = new Animation(5, 5, 8f, true);
                    break;
                case 40: //Knight Statue
                    world2model = Constants.WORLD2MODEL_KNIGHTSTATUE;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("037_Details");
                    anim = new Animation(6, 6, 8f, true);
                    break;
                case 41: //Octo Portrait
                    world2model = Constants.WORLD2MODEL_FANCYPORT;
                    textureSet = world.tileEngine.resourceComponent.getTextureSet("037_Details");
                    anim = new Animation(7, 7, 8f, true);
                    break;
            }
        }
       
    }

    public class EditBehavior : Behavior
    {

        private EmptyActor actor;
        
        public EditBehavior(EmptyActor actor) 
            : base(actor)
        { 
            this.actor = actor;
        }
        public override void run()
        {
            /*
            EditorInput input = actor.world.tileEngine.inputComponent as EditorInput;
            xmove = input[EditorInput.EditBindings.XMOVE] as AxisBinding;
            ymove = input[EditorInput.EditBindings.YMOVE] as AxisBinding;
            actor.force = Vector2.Zero;
            
                actor.force = new Vector2(xmove.position, -ymove.position);
            
            if (actor.force.LengthSquared() > 1)
            {
                actor.force = Vector2.Normalize(actor.force);
            }

            actor.force *= .7f; // scale by speed
            */
            // GLOWING
            int beams = 50;
            Color fifthwhite = new Color(0.15f, 0.15f, 0.15f);
            float increment = (float)Math.PI * 2 / beams;
            for (float x = 0; x < Math.PI * 2; x += increment)
            {
                actor.world.castRay(actor.position, new Vector2((float)Math.Cos(x), (float)Math.Sin(x)), fifthwhite);
            }
        }

    }
}
