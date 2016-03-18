using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Tiles;

namespace Engine
{

    public class Tile
    {

        public readonly World world;
        public int x { get; private set; }
        public int y { get; private set; }
        public int xIndex { get; private set; }
        public int yIndex { get; private set; }
        public bool solid;
        public bool opaque; //{ get; set; }
        public float val = 0f;

        public static bool noLOS;

        // rarely used cmp to colors ops, so just creating when asked for
        private Color _glow;
        private float R, B, G;
        public Color glow {
            get {
                if (_glowR==R && _glowB==B && _glowG==G)
                    return _glow;
                normalizeAboveThreshold();
                R = _glowR;
                G = _glowG;
                B = _glowB;
                _glow = new Color(_glowR * val, _glowG * val, _glowB * val); 
                return _glow; 
            } 
        }

        private float _glowR, _glowB, _glowG;


        public float glowR { 
            get { 
                normalizeAboveThreshold(); 
                return _glowR * val; 
            }
        } 
        public float glowG {
            get
            {
                normalizeAboveThreshold();
                return _glowG * val;
            }
        }
        public float glowB {
            get
            {
                normalizeAboveThreshold();
                return _glowB * val;
            }
        }

        private byte _flags;
        public byte flags
        {
            get
            {
                return _flags;
            }

            set
            {
                opaque = (value & 1) == 1 ? true : false;
                _flags = value;
            }
        }
        public float decay = .96f;
        public static float decayLOS = 0.90f;
        public static int size = 16;
        public static int halfsize = size/2;
        public static int logsize = (int) Math.Log(size, 2);
        public int imgIndex { get; set; }
        public int plateNum = 0;
        public PressurePlate trap = null;

        public int action;
        public int tag = -1;

        // Adjacent tiles
        public Tile right
        {
            get
            {
                if (xIndex == world.width - 1)
                {
                    return null;
                }

                return world.tileArray[xIndex + 1, yIndex];
            }
        }

        public Tile left
        {
            get
            {
                if (xIndex == 0)
                {
                    return null;
                }

                return world.tileArray[xIndex - 1, yIndex];
            }
        }

        public Tile up
        {
            get
            {
                if (yIndex == 0)
                {
                    return null;
                }

                return world.tileArray[xIndex, yIndex - 1];
            }
        }

        public Tile down
        {
            get
            {
                if (yIndex == world.height - 1)
                {
                    return null;
                }

                return world.tileArray[xIndex, yIndex + 1];
            }
        }

        public HeapNode<Tile> openPQNode { get; set; }
        public float fScore { get; set; }
        public float gScore { get; set; }

        // Adjacent is used in A-Star in a deep loop, these values never change,
        // lets not create an object every time.
        private List<Tile> _adjacent;
        public IEnumerable<Tile> adjacent
        {
            get
            {
                if(_adjacent==null)
                    _adjacent=new List<Tile> { up, right, down, left };
                return _adjacent;
            }
        }

        public Tile previous { get; set; }

        public float heuristic(Tile node)
        {

            Tile tile = node as Tile;
            return Math.Abs(tile.y - y) + Math.Abs(tile.x - x);
        }

        public bool MAKERED;
        // END INode Implementation

        public Tile(World world, int xIndex, int yIndex, int imgIndex)
        {

            this.world = world;
            this.xIndex = xIndex; this.yIndex = yIndex;
            this.x = xIndex * size; this.y = yIndex * size;
            this.imgIndex = imgIndex;

            this.setGlow(0.0f);

            solid = false;
            if (noLOS)
            {
                val = 1f;
                decayLOS = 1f;
            }
        }
        // If color values are outside of proper range, normalizes color.
        // (divides by offending components in order to keep all in range (.0f to 1.f))
        private void normalizeAboveThreshold()
        {
            if (_glowR > 1)
            {
                _glowB = _glowB / _glowR;
                _glowG = _glowG / _glowR;
                _glowR = 1.0f;
            }
            if (_glowB > 1)
            {
                _glowR = _glowR / _glowB;
                _glowG = _glowG / _glowB;
                _glowB = 1.0f;
            }
            if (_glowG > 1)
            {
                _glowB = _glowB / _glowG;
                _glowR = _glowR / _glowG;
                _glowG = 1.0f;
            }
        }
        public void changeGlow(float color)
        {
            _glowR += color;
            _glowB += color;
            _glowG += color;
        }

        public void setGlow(float color)
        {
            _glowR = color;
            _glowB = color;
            _glowG = color;
        }

        public void changeGlow(float red, float green, float blue)
        {
            _glowR += red;
            _glowB += blue;
            _glowG += green;
        }

        public void setGlow(float red, float green, float blue)
        {
            _glowR = red;
            _glowB = blue;
            _glowG = green;
        }

        public void saveState(System.IO.BinaryWriter writer)
        {
            writer.Write(flags);
            writer.Write(imgIndex);
            writer.Write(plateNum);
            writer.Write(tag);
            writer.Write(solid);
        }

        public void loadState(System.IO.BinaryReader reader)
        {
            flags = reader.ReadByte();
            imgIndex = reader.ReadInt32();
            plateNum = reader.ReadInt32();
            tag = reader.ReadInt32();
            solid = reader.ReadBoolean();
        }
        public void performDecay()
        {
            //normalizeAboveThreshold();
            _glowB *= decay;
            _glowR *= decay;
            _glowG *= decay;

            val*=decayLOS;
        }
    }
}
