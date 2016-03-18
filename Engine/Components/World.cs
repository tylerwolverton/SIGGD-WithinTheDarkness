using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Diagnostics;
using Engine.Acting;

namespace Engine
{
    public abstract class World : Component
    {

        public string worldName;                //The name of the map
        public Tile[,] tileArray { get; set; }  //A 2D array of all the tiles in the world
        public TextureSet tileTextureSet;       //A set of all the tile images for the world to use to display tiles
        public AudioSet songSet;                //A set of all the songs used in this map

        public LinkedList<Actor> actors { get; set; }
        public float _maxactorradius;           //used to determine which actors are potential collision candidates.
        public MapFile file;                    //The map, loaded from the saved map file (000_Dungeon.map, 001_Maze.map, etc)

        //Length of one "tick" of the engine. This is the minimum for how long the engine can take to go through its paces each time.
        public static readonly TimeSpan timePerTick = TimeSpan.FromSeconds(0.0166666666666667);

        //Time remaining until the tick is over. At the start, we aren't in a tick, so there is no time until the current tick is over.
        private TimeSpan timeRemainder = TimeSpan.Zero;

        
        public int width { get; private set; }      //Width of the map (in number of tiles)
        public int height { get; private set; }     //Height of the map (in number of tiles)

        //The object that we use to create new actors through actorFactory.createActor() and add actors with actorFactor.addActor()
        public ActorFactory actorFactory { get; protected set; }    


        //Prepare the world object so that it can work with other components to prepare for initialization.
        //World is passed the Engine object and the filename of a saved map.
        public World(MirrorEngine theEngine, string fileName) : base(theEngine)
        {

            //Load the map from the filename passed to it.
            worldName = Path.GetFileNameWithoutExtension(fileName);
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            this.file = new MapFile(fileStream);

            //Make sure there are resources. If the world has no resources, something has gone wrong and it should exit.
            ResourceComponent res = theEngine.resourceComponent;
            if (res == null)
            {
                throw new Exception("World: Error retrieving TileSet: ResourceComponent not initialized");
            }
        }


        //Copy the tiles from the loaded map's file and put them in the world's 2D array of the world
        public virtual void initTiles()
        {
            //Copy the tiles from the loaded map's file and put them in the world's 2D array of the world
            width = file.tiles.GetLength(1);
            height = file.tiles.GetLength(2);
            tileArray = new Tile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tileArray[x, y] = new Tile(this, x, y, file.tiles[0, x, y].tileIndex);
                    tileArray[x, y].solid = file.tiles[0, x, y].solid >= 1; //Load the solidity of a tile
                    tileArray[x, y].flags = file.tiles[0, x, y].flags;      //Load the tile's flags
                    tileArray[x, y].tag = file.tiles[0, x, y].tag;        //Load the tile's tags
                    tileArray[x, y].action = file.tiles[0, x, y].action;        //Load the tile's action
                }
            }
        }


        //Load all the actors from the loaded map's file and put them in their proper place in the world.
        public virtual void initActors()
        {            
            for (int i = 0; i < file.worldObjects.ToArray().GetLength(0); i++)
            {
                addActor(actorFactory.createActor(file.worldObjects[i].id, new Vector2(file.worldObjects[i].x * Tile.size + Tile.halfsize, file.worldObjects[i].y * Tile.size + Tile.halfsize)));
            }
        }

        //Once a world object has been created, it has to be filled with things. That is done here.
        public override void Initialize()
        {
            if (file == null)
                return;

            // Load the map from file
            file.load();

            //Load all the tiles from that file
            initTiles();

            //Load all the actors from that file
            initActors();


            //Make sure the actors that overlap do so properly
            sortActors();

            //Throw away the map. We don't need it now that we loaded everything.
            file = null;
            
        }

        //Do nothing
        public virtual void Start()
        {
        }


        //Each tick of the game, the world must be updated.
        public override void Update(GameTime time)
        {
            // This code updates only after a "tick" has happened.
            // This allows usd to separate the game's performance from the graphics framerate, which will make things both easier to write and smoother performance-wise

            
            //Make sure the actors that overlap do so properly.
            sortActors();

            //Update our variable that keeps track of how long we have left in this tick.
            timeRemainder += time.ElapsedGameTime;
            if (timeRemainder > timePerTick)  // Update until we're up to date, because ticks must not be skipped. It's okay to take longer than one tick time.
            {
                timeRemainder -= time.ElapsedGameTime;

                //Let every actor's behavior be run this tick.
                for (LinkedListNode<Actor> a = actors.First; a != null; a = a.Next)
                {
                    a.Value.Update();
                }

                //If an actor wants to be removed from the map, remove it. This can be because it died, because it no longer needs to exist, or some other reason.
                LinkedListNode<Actor> actor = actors.First;
                while (actor != null)
                {
                    LinkedListNode<Actor> tmpNext = actor.Next;

                    // This needs major cleanup
                    if (actor.Value.removeMe)
                    {
                        actor.Value.notifyRemoved();
                        actors.Remove(actor);
                    }

                    actor = tmpNext;
                }

                // Update Max Half-Height
                _maxactorradius = 0;
                foreach (Actor a in actors)
                {
                    float radius = a.size/2;
                    if (radius > _maxactorradius)
                        _maxactorradius = radius;
                }
                
                Tile tile;
               
                RectangleF viewRect = tileEngine.graphicsComponent.camera.ViewRect;

                // Calculate the range of tiles to consider for drawing
                int tileLeft = (int)(viewRect.left*2-viewRect.right) / Tile.size;
                int tileTop = (int)(viewRect.top*2-viewRect.bottom) / Tile.size;
                int tileRight = (int)(viewRect.right * 2 - viewRect.left) / Tile.size;
                int tileBottom = (int)(viewRect.bottom * 2 - viewRect.top) / Tile.size;

                // Make sure in tile bounds
                if (tileLeft < 0) tileLeft = 0;
                if (tileTop < 0) tileTop = 0;
                if (tileRight >= width) tileRight = width - 1;
                if (tileBottom >= height) tileBottom = height - 1;
                if (tileRight < 0 || tileBottom < 0) return;

                //Make each tile-quarter be properly lit
                for (int x = tileLeft; x <= tileRight; x++)
                {
                    for (int y = tileTop; y <= tileBottom; y++)
                    {
                        tile = tileArray[x, y];

                        tile.performDecay();
                    }
                }
            }

            sortActors();

            //Trace.WriteLine(stopwatch.ElapsedMilliseconds);
        }


        //Given a world coordinate, return the corresponding tile.
        public Tile getTileAt(float x, float y)
        {
            
            int xtile = (int)(x / Tile.size);
            int ytile = (int)(y / Tile.size);
            if (x >= 0 && y >= 0 && x < width*Tile.size && y < height*Tile.size)
            {
                return tileArray[xtile, ytile];
            }
            else
            {
                return null;
            }
        }

        //Given a world coordinate, return the corresponding tile.
        public Tile getTileAt(Vector2 position)
        {
            // probably a bad way to do overloading :|
            return getTileAt(position.X, position.Y);
        }

        public Tile getTile(int xIndex, int yIndex)
        {
            if (xIndex < 0 || xIndex >= width || yIndex < 0 || yIndex >= height)
                return null;

            return tileArray[xIndex, yIndex];
        }

        //Check if an area is clear of solidity before letting an actor spawn. Actors shouldn't spawn in solid walls. That'd be silly.
        public bool isAreaClear(RectangleF rect)
        {
            // TODO: expand to cover area with dimensions larger than tile.size
            Tile tile;

            tile = getTileAt(rect.left, rect.top);
            if (tile == null || tile.solid)
                return false;
            tile = getTileAt(rect.left, rect.bottom);
            if (tile == null || tile.solid)
                return false;
            tile = getTileAt(rect.right, rect.top);
            if (tile == null || tile.solid)
                return false;
            tile = getTileAt(rect.right, rect.bottom);
            if (tile == null || tile.solid)
                return false;

            return true;
        }


        //Once actorFactory creates an actor, you add that to the world through this function.
        public bool addActor(Actor a)
        {
            if (a == null)
                return false;
            if ((a.position.X >= 0 && a.position.X < width * Tile.size && a.position.Y >= 0 && a.position.Y < height * Tile.size))
            {
                if (!isAreaClear(new RectangleF(a.position.X, a.position.Y, a.width, a.height)) && !a.ignoreAvE)
                    return false;
                actors.AddFirst(a);
                a.node = actors.First;
                float radius = a.size / 2;
                if (radius > _maxactorradius)
                {
                    _maxactorradius = radius;
                }
            }
            return true;
        }

        //Checks if someone at "from" can see someone at "to".
        public bool hasLineOfSight(Vector2 from, Vector2 to, bool ignoreSolidity)
        {
            // Note: taken from http://www.cse.yorku.ca/~amana/research/grid.pdf
            Vector2 position = from, direction = to - from;
            Tile curTile = getTileAt(position);
            Tile destTile = getTileAt(to);

            if (curTile == null)
                return false;

            int X = curTile.xIndex, Y = curTile.yIndex;  // Indices of current tile
            int stepX = (direction.X >= 0) ? 1 : -1;     // Direction of scanning along X
            int stepY = (direction.Y >= 0) ? 1 : -1;     // Direction of scanning along Y

            // Handle horiz. and ver. cases separately
            if (direction.X == 0)
            {
                while (true)
                {
                    if (curTile.opaque || (!ignoreSolidity && curTile.solid)) return false;
                    if (curTile == destTile) return true;
                    Y += stepY;
                    if (Y < 0 || Y >= height) return false;
                    curTile = tileArray[X, Y];
                }
            }
            else if (direction.Y == 0)
            {
                while (true)
                {
                    if (curTile.opaque || (!ignoreSolidity && curTile.solid)) return false;
                    if (curTile == destTile) return true;
                    X += stepX;
                    if (X < 0 || X >= width) return false;
                    curTile = tileArray[X, Y];
                }
            }

            float tMaxX = (Tile.size * (X + (stepX + 1) / 2) - position.X) / direction.X;  // t at vert tile boundary 
            float tMaxY = (Tile.size * (Y + (stepY + 1) / 2) - position.Y) / direction.Y;  // t at horiz tile boundary
            float tDeltaX = Math.Abs(Tile.size / direction.X);     // T required to move Tile.size in X
            float tDeltaY = Math.Abs(Tile.size / direction.Y);     // T required to move Tile.size in Y

            while (true)
            {
                if (curTile.opaque || (!ignoreSolidity && curTile.solid)) return false;
                if (curTile == destTile) return true;
                   
                // Find next tile in ray
                if (tMaxX < tMaxY)
                {
                    tMaxX += tDeltaX;
                    X += stepX;
                }
                else
                {
                    tMaxY += tDeltaY;
                    Y += stepY;
                }

                if (X < 0 || X >= width || Y < 0 || Y >= height) return false;
                curTile = tileArray[X, Y];
            }
        }

        //Checks if one rectangle can see another.
        public bool hasLineOfSight(Rectangle from, Rectangle to)//Accurately checks two line of sight lines that completely encompass the given rectangles
        {
            int tempBuf = 0;
            if (from.Center.X == to.Center.X)//If the two rectangles are aligned horizontally
            {
                return (hasLineOfSight(new Vector2(from.Center.X, from.Top + tempBuf), new Vector2(to.Center.X, to.Top + tempBuf), false)
                      && hasLineOfSight(new Vector2(from.Center.X, from.Bottom - tempBuf), new Vector2(to.Center.X, to.Bottom - tempBuf), false));
            }
            else if (from.Center.Y == to.Center.Y)//If the two rectangles are aligned vertically
            {
                return (hasLineOfSight(new Vector2(from.Left + tempBuf, from.Center.Y), new Vector2(to.Left + tempBuf, to.Center.Y), false)
                      && hasLineOfSight(new Vector2(from.Right - tempBuf, from.Center.Y), new Vector2(to.Right - tempBuf, to.Center.Y), false));
            }
            else if ( (from.Center.X < to.Center.X && from.Center.Y > to.Center.Y) || (from.Center.X > to.Center.X && from.Center.Y < to.Center.Y) )//If the rectangles are arranged in a positively-sloped line
            {//(Since 0,0 is the top left corner, a positively sloped line will actually have opposite comparator checks for X and Y)
                return (hasLineOfSight(new Vector2(from.Left + tempBuf, from.Top + tempBuf), new Vector2(to.Left + tempBuf, to.Top + tempBuf), false)
                      && hasLineOfSight(new Vector2(from.Right - tempBuf, from.Bottom - tempBuf), new Vector2(to.Right - tempBuf, to.Bottom - tempBuf), false));
            }
            else//If the rectangles are arranged in a negatively sloped line.
            {
                return (hasLineOfSight(new Vector2(from.Right + tempBuf, from.Top + tempBuf), new Vector2(to.Right + tempBuf, to.Top + tempBuf), false)
                      && hasLineOfSight(new Vector2(from.Left - tempBuf, from.Bottom - tempBuf), new Vector2(to.Right - tempBuf, to.Bottom - tempBuf), false));
            }
        }
        public void castRay(Vector2 pos, Vector2 dir, Color color)
        {
            castRay(pos, dir, color, Math.Max(width,height));
        }

        //Sends out a ray of light that starts at position and moves out in direction with color.
        public void castRay(Vector2 position, Vector2 direction, Color color, float distance)
        {
            RectangleF viewRect = tileEngine.graphicsComponent.camera.ViewRect;
            if (!((position.X > viewRect.right && direction.X > 0) || (position.X < viewRect.left && direction.X < 0) ||
                (position.Y < viewRect.bottom && direction.Y > 0) || (position.Y > viewRect.top && direction.Y < 0)))
            {
                return;
            }
            float R = color.R / 256f; // Red component, 256 byte repr -> (0-1) float
            float G = color.G / 256f; // Green
            float B = color.B / 256f; // Blue

            R *= 0.04f;  // arbitrary scale so old stuff doesn't break
            G *= 0.04f;
            B *= 0.04f;

            castRay(position, direction, (tile) => { tile.changeGlow(R, G, B); });
        }
        public delegate void ModifyTile(Tile tile);
        
        //Sends out a ray of light that starts at position and moves out in direction with color.
        public void castRay(Vector2 position, Vector2 direction, ModifyTile mod)
        {
            // Note: taken from http://www.cse.yorku.ca/~amana/research/grid.pdf
            Tile curTile = getTileAt(position);

            if (curTile == null)
                return;


            int X = curTile.xIndex, Y = curTile.yIndex;  // Indices of current tile
            int stepX = (direction.X >= 0) ? 1 : -1;     // Direction of scanning along X
            int stepY = (direction.Y >= 0) ? 1 : -1;     // Direction of scanning along Y

            // Handle hor. and ver. cases separately
            if (direction.X == 0)
            {
                while (!curTile.opaque)
                {
                    mod(curTile);

                    Y += stepY;
                    if (Y < 0 || Y >= height) return;
                    curTile = tileArray[X, Y];
                }

                mod(curTile);  // Apply to the last tile
                return;
            }
            else if (direction.Y == 0)
            {
                while (!curTile.opaque)
                {
                    mod(curTile);
                    X += stepX;
                    if (X < 0 || X >= width) return;
                    curTile = tileArray[X, Y];

                }

                mod(curTile);  // Apply to the last tile
                return;
            }

            float tMaxX = (Tile.size * (X + (stepX + 1) / 2) - position.X) / direction.X;  // t at vert tile boundary 
            float tMaxY = (Tile.size * (Y + (stepY + 1) / 2) - position.Y) / direction.Y;  // t at horiz tile boundary
            float tDeltaX = Math.Abs(Tile.size / direction.X);     // T required to move Tile.size in X
            float tDeltaY = Math.Abs(Tile.size / direction.Y);     // T required to move Tile.size in Y

            while (!curTile.opaque)
            {

                mod(curTile);
                // Find next tile in ray
                if (tMaxX < tMaxY)
                {
                    tMaxX += tDeltaX;
                    X += stepX;
                }
                else
                {
                    tMaxY += tDeltaY;
                    Y += stepY;
                }

                if (X < 0 || X >= width || Y < 0 || Y >= height) return;
                curTile = tileArray[X, Y];
            }

            mod(curTile);  // Apply to the last tile
        }

        // Set glow for cast light, with inverse square lighting
        private void setGlow(Tile cTile, Vector2 center)
        {
            // Intensity constant
            const float INTCONST = 256f;
            cTile.changeGlow(INTCONST / (new Vector2(cTile.x, cTile.y) - center).LengthSquared());
            cTile.val = 1;
        }

        //Make sure that the actors that overlap do so properly.
        public void sortActors()
        {
            // Sort the actors
            
                LinkedListNode<Actor> iter = actors.First;
                while (iter != null)
                {
                    LinkedListNode<Actor> next = iter.Next;

                    while (iter.Previous != null)
                    {
                        if (iter.Value.position.Y >= iter.Previous.Value.position.Y)
                        {
                            break;
                        }

                        // Swap the unsorted value with the previous
                        LinkedListNode<Actor> tmp = iter.Previous;
                        actors.Remove(iter);
                        actors.AddBefore(tmp, iter);

                    }

                    iter = next;
                }
            
        }

        //Get a list of all the actors around a position in a radius in a direction of a certain cone width.
        //coneWidth is an angle in degrees ------------------------------------------------------------------V
        public IEnumerable<Actor> getConeAroundPos(Vector2 position,float radius, Vector2 direction, float coneWidth,Actor hint)
        {


            LinkedList<Actor> nearbyActors = new LinkedList<Actor>();

            LinkedListNode<Actor> begin, end;
            float yBegin = position.Y - radius;
            float yEnd = position.Y + radius;
            float xBegin = position.X - radius;
            float xEnd = position.X + radius;
            float rsquared = radius * radius;
            //use hint as starting point to search for position
            if (hint != null)
            {
                begin = hint.node;
                end = hint.node;
            }
            else
            {
                begin=actors.First;
                end=actors.First;
            }

            // Here, the algorithm composes a list of all actors that could possibly be in the cone (broad phase)
            // This list consists of all actors within circle formed by the cone

            // Here, we find the first and last actor that could be within the circle (considering only Y values).
                if (begin.Value.position.Y > yBegin)
                    for (; begin.Previous != null && begin.Previous.Value.position.Y-begin.Previous.Value.size >= yBegin; begin = begin.Previous) ;
                else
                    for (; begin.Next != null && begin.Next.Value.position.Y+begin.Next.Value.size <= yBegin; begin = begin.Next) ;

                if (end.Value.position.Y < yEnd)
                    for (; end != null && end.Value.position.Y+end.Value.size <= yEnd; end = end.Next) ;
                else
                    for (; end != null && end.Value.position.Y-end.Value.size >= yEnd; end = end.Previous) ;
            
                if (end!=null && end.Value.position.Y < begin.Value.position.Y)

                    return nearbyActors;

            // Determine which of the actors in the range are within the circle, by checking dist from center
            LinkedListNode<Actor> iter;
            for (iter = begin; iter != end; iter = iter.Next)
            {
                if (iter.Value.position.X < xBegin || iter.Value.position.X > xEnd)
                {
                    continue;
                }

                float dx = iter.Value.position.X - position.X;
                float dy = iter.Value.position.Y - position.Y;

                // Check the radius
                if (Math.Sqrt(dx * dx + dy * dy) - iter.Value.size < radius)
                {
                    nearbyActors.AddFirst(iter.Value);
                }
            }

            // Determine which of the "nearby" actors are actually in the cone

            // Beginning and ending vectors for the cone
            Vector2 beginVect = Vector2.Transform(direction, Matrix.CreateRotationZ(MathHelper.ToRadians(-coneWidth / 2)));
            Vector2 endVect = Vector2.Transform(direction, Matrix.CreateRotationZ(MathHelper.ToRadians(coneWidth / 2)));

            if (coneWidth != 360)
            {

                iter = nearbyActors.First;
                while (iter != null)
                {

                    LinkedListNode<Actor> tmp = iter.Next;

                    // Remove Actor if not in Cone
                    Vector2 v = iter.Value.position - position;
                    Vector2 perp = Vector2.Normalize(new Vector2(-v.Y, v.X))*iter.Value.size;

                    // Beginning and ending vectors for the actor's periphery (perpendicular to the line from the center to the actor's center):w
                    Vector2 beginAVect = v - perp;
                    Vector2 endAVect = v + perp;

                    // We are trying to determine whether the two begin/end pairs overlap.
                    // The easiest way to do this is to check if they *don't* overlap. This is the case,
                    // iff the end of one is before the beginning of the other.

                    // Compare the beginnings and endings
                    float crossP1 = beginVect.X * endAVect.Y - endAVect.X * beginVect.Y;
                    float crossP2 = beginAVect.X * endVect.Y - endVect.X * beginAVect.Y;

                    bool hasLOS = hasLineOfSight(position, iter.Value.position, false);

                    // If either of the beginnings comes after an ending, remove it from the nearby actors list
                    if (crossP1 > 0 || crossP2 > 0 || !hasLOS)
                    {
                        nearbyActors.Remove(iter);
                    }

                    iter = tmp;
                }
            }

            // What's left is all the actors in the cone.
            return nearbyActors;
        }


        //State saving
        public virtual void saveState(BinaryWriter writer)
        {
            writer.Write(worldName);
            writer.Write(_maxactorradius);
            writer.Write(actors.Count);
            //Save tiles
            writer.Write("Tiles");
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tileArray[x, y].saveState(writer);
                }
            }
            //Save actors
            LinkedListNode<Actor> currentActor = actors.First;
            writer.Write("Actors");
            while (currentActor != null)
            {
                if (currentActor.Value.actorName != "none")
                {
                    writer.Write("Yes!");
                    currentActor.Value.saveState(writer);
                }
                else
                {
                    writer.Write("No!");
                }
                currentActor = currentActor.Next;
            }
            writer.Write("Done");
        }

        public virtual void loadState(BinaryReader reader)
        {
            worldName = reader.ReadString();
            _maxactorradius = reader.ReadSingle();
            int actorCount = reader.ReadInt32();
            //Load tiles
            file.load();
            initTiles();
            if (reader.ReadString() != "Tiles")
            {
                throw new Exception("State loading error: desync occurred during world initialization");
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tileArray[x, y].loadState(reader);
                }
            }
            //Load actors
            actors = new LinkedList<Actor>();
            if (reader.ReadString() != "Actors")
            {
                throw new Exception("State loading error: desync occurred while loading tiles");
            }
            int newId;
            Vector2 newPos, newVel;
            double newColor;
            for (int x = 0; x < actorCount; x++)
            {
                if (reader.ReadString() == "Yes!")
                {
                    newId = actorFactory.getActorId(reader.ReadString());
                    newPos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    newVel = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    newColor = reader.ReadDouble();
                    actors.AddFirst(actorFactory.createActor(newId, newPos, newVel, newColor));
                    actors.First.Value.loadState(reader);
                    actors.First.Value.node = actors.First;
                }
            }
            sortActors();
            if (reader.ReadString() != "Done")
            {
                throw new Exception("State loading error: desync occurred while loading actors");
            }
        }

        public Tile getMouseWorldPos()
        {
            Matrix screen2game = Matrix.Invert(tileEngine.graphicsComponent.game2screen);

            MouseState ms = tileEngine.inputComponent.currentMouseState;
            Vector2 screenPos = new Vector2(ms.X, ms.Y);

            return tileEngine.world.getTileAt(Vector2.Transform(screenPos, screen2game));
        }
        public Vector2 getMouseWorldVector()
        {
            Matrix screen2game = Matrix.Invert(tileEngine.graphicsComponent.game2screen);

            MouseState ms = tileEngine.inputComponent.currentMouseState;
            Vector2 screenPos = new Vector2(ms.X, ms.Y);
            return Vector2.Transform(screenPos, screen2game);
        }
    }
}
