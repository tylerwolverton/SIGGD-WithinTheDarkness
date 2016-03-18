using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Tiles;
using Engine.Acting;
using System.Diagnostics;

namespace Engine
{

    public class Actor
    {


        //////////////////////////////////////////////////////////////
        // NEW MEMBERS: Check this stuff out:
        //////////////////////////////////////////////////////////////

        public string actorName { get; protected set; }   //Only set this if the actor supports state saving
        public World world;

        public TextureSet textureSet;
        public AudioSet audioSet;


        // WORLD INFORMATION
        public Vector2 position { get; set; }

        /// <summary>
        /// defines what an actor does
        /// </summary>
        public Behavior myBehavior { get; set; }

        /// <summary>
        /// determines whether an actor's  behavior is updated
        /// </summary>
        public bool active { get; set; }

        /// <summary>
        /// determines whether actor leaves trail
        /// </summary>
        public bool leaveTrail;
        private int trailCounter;
        public int trailLength;
        public int[] oldimgindex;
        public int[] oldxoffset;
        public int[] oldyoffset;
        public Vector2[] oldpos;
        // If set, actor is removed at the end of the current World update
        public bool removeMe { get; set; }

        // Event that happens when actor is removed
        public delegate void RemEvent();
        public event RemEvent removeEvent;
        public void notifyRemoved()
        {
            if (removeEvent != null)
            {
                removeEvent();
            }
        }

        //////////////////////////////////////////////////////////////
        // End New Members
        //////////////////////////////////////////////////////////////



        //////////////////////////////////////////////////////////////
        // BEGIN PHYSICS
        //////////////////////////////////////////////////////////////

        public bool ignoreAvE { get; set; }  // Ignore Actor vs Environment rectification
        
        [Flags]
        public enum ActorCategory
        {

            nocategory = 0x00,
            friendly = 0x01,
            enemy = 0x02,
            friendlyprojectile = 0x04,
            enemyprojectile = 0x08,
            powerup = 0x10,
        }

        /// <summary>
        /// all of the categories that this actor falls under
        /// </summary>
        public ActorCategory actorcategory;

        /// <summary>
        /// both actors must contain atleast one category in each others collision mask to collide.
        /// </summary>
        public ActorCategory collisionmask;

        /// <summary>
        /// this does not receive a collision effect if the collider is classified under ANY of the listed categories.
        /// </summary>
        public ActorCategory collisionimmunitymask;

        /// <summary>
        /// this does not receive damage from collider if collider is classified under ANY of the listed categories.
        /// </summary>
        public ActorCategory damageimmunitymask;

        /// <summary>
        /// in an actor vs actor collision, the actor with the larger mass will push back harder.
        /// </summary>
        public float mass;

        /// <summary>
        /// Change in position since the last frame
        /// </summary>
        public Vector2 velocity
        {
            get
            {
                return position - _oldPos;
            }

            set
            {
                _oldPos = position - value;
            }
        }

        /// <summary>
        /// The sum of the forces acting on the object
        /// </summary>
        public Vector2 force { get; set; }

        /// <summary>
        /// The amount of friction the actor experiences from 0 to 1 
        /// 0 = no friction
        /// 1 = total friction
        /// </summary>
        public float frictionCoefficient
        {
            get
            {
                return _frictionCoeff;
            }
            set
            {
                if (value <= 1 && value >= 0)
                {
                    _frictionCoeff = value;
                    _frictionMult = 1 - _frictionCoeff;
                }
                else
                {
                    throw new Exception("Physics:  Invalid coefficient of friction provided");
                }
            }
        }

        private float _elasticity;

        /// <summary>
        /// How much an actor bounces off of a wall.
        /// 0 = no bounce
        /// 1 = total bounce
        /// </summary>
        public float elasticity
        {
            get
            {
                return _elasticity;
            }
            set
            {
                if (value <= 1 && value >= 0)
                {
                    _elasticity = value;
                }
                else
                {
                    throw new Exception("Physics:  Invalid elasticity provided");
                }
            }
        }

        /// <summary>
        /// The diameter of the bounding circle
        /// </summary>
        public float size { get; set; }

        /// <summary>
        /// The width of the bounding rectangle
        /// </summary>
        public float width { get; set; }

        /// <summary>
        /// The height of the bounding rectangle
        /// </summary>
        public float height { get; set; }

        /// <summary>
        /// previous position of the actor
        /// </summary>
        public Vector2 _oldPos { get; set; }
        public float _frictionMult;
        public float _frictionCoeff;

        //////////////////////////////////////////////////////////////
        // END PHYSICS
        //////////////////////////////////////////////////////////////





        //New members stuff was here.






        public Color glow;
        public float glowR, glowB, glowG;

        public float glowchargerate;
        public double color;  //Moved here from Blob because GameActorFactory requires it

        /// <summary>
        /// the tile the actor is standing on
        /// </summary>
        public Tile curTile
        {
            get
            {
                return world.getTileAt(position.X, position.Y);
            }
        }



        /// <summary>
        /// pan the graphical component of the actor in the x direction
        /// </summary>
        public int xoffset { get; set; } 

        /// <summary>
        /// pan the graphical component of the actor in the y direction
        /// </summary>
        public int yoffset { get; set; }

        // DISPLAY INFORMATION
        public Vector2 world2model;  // Added to the game coordinates to get the sprite coordinates. Allows collision to be in a different position than drawing

        public int imgIndex { get; set; }

        /// <summary>
        /// the animation that the actor displays
        /// </summary>
        public Animation anim
        {
            get
            {
                return _anim;
            }
            set
            {
                if (_anim != null && _anim != value)
                    _anim.reset();
                _anim = value;
            }
        }

        private Animation _anim;

        public LinkedListNode<Actor> node { get; set; }
        public delegate bool ActorPredicate(Actor a);

        public Actor(World world, Vector2 position, Vector2 velocity, float size, Vector2 world2model, int imgIndex)
        {
            this.actorName = "none";
            // BEGIN HEALTH INIT
            // END HEALTH INIT

            // BEGIN PHYSICS INIT           
            this.size = size;
            this.width = size;//NOTE: These are also hardcoded to be the same as size - this may change later.
            this.height = size / 2;
            this.position = position;
            this.velocity = velocity;
            this.frictionCoefficient = 0.25f; // Hardcoded for now.  Will eventually change based on tile you are walking on.
            this.elasticity = .25f; // Hardcoded for now.
            this.mass = 25; // Hardcoded for now.
            // END PHYSICS INIT

            this.world = world;
            this.node = node;
            this.world2model = world2model;
            this.imgIndex = imgIndex;

            if (world.getTileAt(position) != null)
            {
                this.glow = world.getTileAt(position).glow;
            }
            this.glowchargerate = .3f;
            this.color = -1;
        }



        public Color setGlow(float color)
        {
            glowR = color;
            glowB = color;
            glowG = color;

            this.glow = new Color(glowR, glowG, glowB);

            return this.glow;
        }

        public Color setGlow(float red, float green, float blue)
        {
            glowR = red;
            glowB = blue;
            glowG = green;

            this.glow = new Color(glowR, glowG, glowB);

            return this.glow;
        }

        public Color changeGlow(float color)
        {
            glowR += color;
            glowB += color;
            glowG += color;

            this.glow = new Color(glowR, glowG, glowB);

            return this.glow;
        }

        public Color changeGlow(float red, float green, float blue)
        {
            glowR += red;
            glowB += blue;
            glowG += green;

            this.glow = new Color(glowR, glowG, glowB);

            return this.glow;
        }

       

        public void Update()
        {

            if (active)
            {
                myBehavior.run();
            }
            animate();
        }
        public void UpdateGlow(Tile t)
        {
            if (t != null)
            {
                changeGlow((t.glowR - this.glowR) * this.glowchargerate,
                           (t.glowG - this.glowG) * this.glowchargerate,
                           (t.glowB - this.glowB) * this.glowchargerate);

            }
        }



        //////////////////////////////////////////////////////////////
        // BEGIN PHYSICS
        //////////////////////////////////////////////////////////////
        public virtual void collision(Actor a)
        {
        }

        public virtual void hitWall()
        {
        }

        public void addImpulse(Vector2 impulse)
        {
            this._oldPos -= impulse/this.mass;
        }

        

        //////////////////////////////////////////////////////////////
        // END PHYSICS
        //////////////////////////////////////////////////////////////


        public virtual void animate()
        {

            // Update Animation
            if (anim != null)
            {

                anim.run();
                if (anim != null)
                {// Anim might have set itself to null
                    if (leaveTrail)
                    {
                        oldimgindex[trailCounter] = imgIndex;
                        oldxoffset[trailCounter] = xoffset;
                        oldyoffset[trailCounter] = yoffset;
                        oldpos[trailCounter] = _oldPos;
                        trailCounter = (trailCounter + 1) % trailLength;
                    }
                    imgIndex = anim.curFrame;
                    xoffset = anim.xoffset;
                    yoffset = anim.yoffset;
                }
            }
        }

        /// <summary>
        /// used to map rotation to frames.  Assumes frames are arranged assending in clockwise order.
        /// </summary>
        /// <param name="direction">a vector in the pie slice of the subdivided circle</param>
        /// <param name="subdivisions">range of returned integers</param>
        /// <param name="rotationoffset">I'm Ben and I like to not provide explanations for some of my params. 
        /// Rotationoffset is radians/pi for the offset needed to line up the southern sprite.</param>
        /// <returns></returns>
        public static int indexFromDirection(Vector2 direction, int subdivisions, float rotationoffset)
        {
            float halfsubdivs = subdivisions / 2;

            /*
             * It divides subdivisions by two so that we can work with atan's mapping to -pi/2 to pi/2 properly.
             * 
             * We use halfsubdivs + subdivisions*rotationoffset+.5 so that we can shift the result of atan out of that range and into to the proper quadrant.
             * 
             * atan2 gives the angle in the range -pi/2 to pi/2
             * 
             * We divide by pi so that we end up with a proportion instead of a radian. 
             */
            return (int)Math.Floor((halfsubdivs + subdivisions*rotationoffset+.5 + halfsubdivs * Math.Atan2(direction.Y, direction.X) / (Math.PI)) % subdivisions);
        }

        public IEnumerable<Actor> getConeAround(float radius, Vector2 direction, float coneWidth, ActorPredicate selector)
        {

            if (selector == null)
            {
                selector = (a) => true;
            }

            LinkedList<Actor> nearbyActors = new LinkedList<Actor>();

            LinkedListNode<Actor> begin, end;
            float yBegin = position.Y - radius;
            float yEnd = position.Y + radius;
            float xBegin = position.X - radius;
            float xEnd = position.X + radius;
            float rsquared = radius * radius;

            for (begin = this.node; begin.Previous != null && begin.Previous.Value.position.Y >= yBegin; begin = begin.Previous) ;
            for (end = this.node; end != null && end.Value.position.Y <= yEnd; end = end.Next) ;

            LinkedListNode<Actor> iter;
            for (iter = begin; iter != end; iter = iter.Next)
            {

                if (iter.Value.position.X < xBegin || iter.Value.position.X > xEnd)
                {
                    continue;
                }

                float dx = iter.Value.position.X - position.X;
                float dy = iter.Value.position.Y - position.Y;

                if (Math.Sqrt(dx * dx + dy * dy) - iter.Value.size < radius)
                {
                    nearbyActors.AddFirst(iter.Value);
                }
            }

            Vector2 beginVect = Vector2.Transform(direction, Matrix.CreateRotationZ(MathHelper.ToRadians(-coneWidth/2)));
            Vector2 endVect = Vector2.Transform(direction, Matrix.CreateRotationZ(MathHelper.ToRadians(coneWidth/2)));

            if (coneWidth != 360)
            {

                iter = nearbyActors.First;
                while (iter != null)
                {

                    LinkedListNode<Actor> tmp = iter.Next;

                    // Remove Actor if not in Cone
                    Vector2 v = iter.Value.position - position;

                    float crossP1 = beginVect.X * v.Y - v.X * beginVect.Y;
                    float crossP2 = v.X * endVect.Y - endVect.X * v.Y;
                    
                    bool hasLOS = world.hasLineOfSight(this.position, iter.Value.position, false);

                    if (!(crossP1 > 0 && crossP2 > 0) || !hasLOS)
                    {
                        nearbyActors.Remove(iter);
                    }

                    iter = tmp;
                }
            }

            return nearbyActors;
        }

        public void trail(bool trail, int length)
        {
            
            leaveTrail = trail;
            if (trail)
            {
                trailCounter = 0;
                trailLength = length;
                oldimgindex = new int[length];
                oldxoffset = new int[length];
                oldyoffset = new int[length];
                oldpos = new Vector2[length];
            }

        }

        //State saving
        public virtual void saveState(System.IO.BinaryWriter writer)
        {
            writer.Write(actorName);
            writer.Write(position.X);
            writer.Write(position.Y);
            writer.Write(velocity.X);
            writer.Write(velocity.Y);
            writer.Write(color);
            writer.Write(ignoreAvE);
            writer.Write(active);
            writer.Write(removeMe);
            if (this is ILife)
            {
                (this as ILife).life.saveState(writer);
            }
            myBehavior.saveState(writer);
        }

        public virtual void loadState(System.IO.BinaryReader reader)
        {
            ignoreAvE = reader.ReadBoolean();
            active = reader.ReadBoolean();
            removeMe = reader.ReadBoolean();
            if (this is ILife)
            {
                (this as ILife).life.loadState(reader);
            }
            myBehavior.loadState(reader);
        }

        
    }
}
