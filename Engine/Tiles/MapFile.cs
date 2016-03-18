using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Engine
{

    public class MapFile
    {

        public struct TileData
        {

            public byte tileSetIndex;
            public byte tileIndex;
            public byte flags;   
            public ushort action;
            public ushort tag;
            public byte solid;

            /* Flags Reference: (with bit 1 being the lowest order bit)
             *  Bit 1:  Opacity
             *  Bit 2:  Change map   (using tag as the new map number)
             */
        }

        public struct WorldObjectData
        {

            public byte id;
            public byte layer;
            public ushort x, y;
        }

        public string name { get; set; }
        public string[] tileSetNames { get; set; }
        public string soundSetName { get; set; }
        public TileData[,,] tiles { get; set; }
        public List<WorldObjectData> worldObjects { get; set; }

        private FileStream fileStream;

        public MapFile(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        public void load()
        {

            BinaryReader file = new BinaryReader(fileStream);
            
            const uint MAGIC = 0x17011138;
            uint magic;

            if (file == null)
            {
                return;
            }

            // Magic Number
            magic = file.ReadUInt32();
            if (magic != MAGIC)
            {
                throw new Exception("Map load error: File is not a map");
            }

            // Name
            name = file.ReadString();

            // TileSets
            byte nTileSets = file.ReadByte();
            tileSetNames = new string[nTileSets];
            for (int i = 0; i < nTileSets; i++)
            {
                tileSetNames[i] = file.ReadString();
            }

            // Sound Set
            soundSetName = file.ReadString();

            // Tiles
            byte nLayers = file.ReadByte();
            byte width = file.ReadByte();
            byte height = file.ReadByte();
            tiles = new TileData[nLayers,width,height];
            for (int layer = 0; layer < nLayers; layer++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {

                        TileData td = new TileData();
                        td.tileSetIndex = file.ReadByte();
                        td.tileIndex = file.ReadByte();
                        td.flags = file.ReadByte();
                        td.action = file.ReadUInt16();
                        td.tag = file.ReadUInt16();
                        td.solid = file.ReadByte();
                        tiles[layer, x, y] = td;
                    }
                }
            }

            // World Objects
            ushort nWorldObjects = file.ReadUInt16();
            worldObjects = new List<WorldObjectData>();
            for (int i = 0; i < nWorldObjects; i++)
            {

                WorldObjectData wod = new WorldObjectData();
                wod.id = file.ReadByte();
                wod.layer = file.ReadByte();
                wod.x = file.ReadUInt16();
                wod.y = file.ReadUInt16();
                worldObjects.Add(wod);
            }

            // Magic Number
            magic = file.ReadUInt32();
            if (magic != MAGIC)
            {
                throw new Exception("Map load error: File is not a map");
            }

            this.fileStream.Close();
        }

        public void save()
        {

            this.fileStream.Close();
            this.fileStream = new FileStream(name, FileMode.Create);
            BinaryWriter file = new BinaryWriter(fileStream);

            const uint MAGIC = 0x17011138;

            if (file == null)
            {
                return;
            }

            // Magic Number
            file.Write(MAGIC);

            // Name
            file.Write(name);

            // TileSets
            if (tileSetNames.Length > byte.MaxValue)
            {
                throw new Exception("Map write error: too many tilesets.");
            }

            file.Write((byte)tileSetNames.Length);
            foreach (string tileSetName in tileSetNames)
            {
                file.Write(tileSetName);
            }

            // Sound Set
            file.Write(soundSetName);

            // Tiles
            if (tiles.GetLength(0) > byte.MaxValue)
            {
                throw new Exception("Map write error: too many layers.");
            }

            if (tiles.GetLength(1) > byte.MaxValue)
            {
                throw new Exception("Map write error: too wide.");
            }

            if (tiles.GetLength(2) > byte.MaxValue)
            {
                throw new Exception("Map write error: too tall.");
            }

            byte nLayers = (byte)tiles.GetLength(0);
            byte width = (byte)tiles.GetLength(1);
            byte height = (byte)tiles.GetLength(2);
            file.Write(nLayers); file.Write(width); file.Write(height);

            for (int layer = 0; layer < nLayers; layer++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {

                        TileData td = tiles[layer, x, y];
                        file.Write(td.tileSetIndex);
                        file.Write(td.tileIndex);
                        file.Write(td.flags);
                        file.Write(td.action);
                        file.Write(td.tag);
                        file.Write(td.solid);
                    }
                }
            }



            if (worldObjects.ToArray().GetLength(0) > ushort.MaxValue)
            {
                throw new Exception("Map write error: too tall.");
            }

            file.Write((ushort)worldObjects.ToArray().GetLength(0));

            WorldObjectData tmp;
            for (int i = 0; i < worldObjects.ToArray().GetLength(0); i++)
            {

                WorldObjectData wod = worldObjects[i];
                if (wod.id == 0)
                {

                    tmp = worldObjects[0];
                    worldObjects[0] = wod;
                    worldObjects[i] = tmp;
                }
            }


            foreach (WorldObjectData wod in worldObjects)
            {

                file.Write(wod.id);
                file.Write(wod.layer);
                file.Write(wod.x);
                file.Write(wod.y);
            }

            // Magic Number
            file.Write(MAGIC);

            fileStream.Close();
        }

        public void free()
        {
            this.fileStream.Close();
        }
    }
}
