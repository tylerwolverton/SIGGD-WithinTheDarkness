using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Engine.Components
{

    public class PhysicsComponent : Component
    {

        //CONSTANTS
        //private const float CORNER_ASSIST = 0.3f;
        private const float BUFFER = 0.01f;

        public PhysicsComponent(MirrorEngine theEngine)
            : base(theEngine)
        {
        }

        private void actorVsActor(LinkedListNode<Actor> a)
        {

            Actor actor = a.Value;
            World world = actor.world;
            LinkedListNode<Actor> end;
                       
            float bound = actor.size / 2 + world._maxactorradius;
            float yEnd = actor.position.Y + bound;

            // Find begin and end of possible collisions and records them in yBegin and yEnd
            for (end = a; end != null && end.Value.position.Y <= yEnd; end = end.Next);

            // Check against possible collisions
            for (LinkedListNode<Actor> iter = a.Next; iter != end; iter = iter.Next)
            {

                Actor target = iter.Value;

                if (iter != a &&
                    (target.collisionmask & actor.actorcategory) > 0 &&
                    (actor.collisionmask & target.actorcategory) > 0)
                {

                    float deltax = (actor.position.X - target.position.X);
                    float deltay = (actor.position.Y - target.position.Y);
                    float distsquared = deltax * deltax + deltay * deltay;

                    // used to shake out perfect overlaps
                    if (distsquared < .1)
                    {

                        Random random = new Random();
                        deltax = (float)(.5 - random.NextDouble());
                        deltay = (float)(.5 - random.NextDouble());
                        distsquared = deltax * deltax + deltay * deltay;
                    }

                    float totalsize = (actor.size + target.size) / 2;
                    float totalsizesquared = totalsize * totalsize;

                    if (totalsizesquared > distsquared)
                    {

                        ////Collision Occured
                        if ((actor.collisionimmunitymask & target.actorcategory) == 0){
                            actor.collision(target);
                        }

                        if ((target.collisionimmunitymask & actor.actorcategory) == 0)
                        {
                            target.collision(actor);
                        }

                        deltax /= distsquared;
                        deltay /= distsquared;

                        float overlap = totalsize - (float)Math.Sqrt(distsquared);
                        float combinedmass = (actor.mass + target.mass);

                        Vector2 kick = new Vector2(overlap * deltax / 2 , overlap * deltay / 2 );

                        if (kick.Length() > 2)
                        {

                            kick.Normalize();
                            kick *= 2;
                        }

                        actor.force += kick / combinedmass * target.mass;
                        target.force -= kick / combinedmass * actor.mass;
                    }
                }
            }
        }

        // Verlet with actor vs environment
        private void verletWithEnvironment(Actor actor)
        {

            World world = actor.world;
            Vector2 tmpPos = actor.position;

            //desired change in position
            float dx = (actor.position.X - actor._oldPos.X) * actor._frictionMult + actor.force.X;
            float dy = (actor.position.Y - actor._oldPos.Y) * actor._frictionMult + actor.force.Y;

            // Terminal Velocity of Tile.size for all actors
            if (Math.Abs(dx) >= Tile.size || Math.Abs(dy) >= Tile.halfsize)
            {

                float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                float restitution = (dist - Tile.halfsize) / dist;
                dx -= dx * restitution;
                dy -= dy * restitution;
            }

            float halfSize = actor.size / 2;
            float halfWidth = actor.width / 2;
            float halfHeight = actor.height / 2;

            // direction of change
            float xdir = (dx > 0) ? 1 : (dx < 0) ? -1 : 0;
            float ydir = (dy > 0) ? 1 : (dy < 0) ? -1 : 0;
            float corner_assist = (float) Math.Sqrt(Math.Abs(dx) + Math.Abs(dy)) * (actor.frictionCoefficient);

            List<Vector2> checkVectorList = new List<Vector2>();//List of all vectors we are going to use to check for tiles.
            checkVectorList.Add(new Vector2(actor.position.X + dx + halfWidth * xdir, actor.position.Y + halfHeight));//Bottom corner vector
            checkVectorList.Add(new Vector2(actor.position.X + dx + halfWidth * xdir, actor.position.Y - halfHeight));//Top corner vector
            int tileStep = Tile.size/2;//Half of the global size of an ingame tile

            while (actor.position.Y + halfHeight - tileStep > actor.position.Y - halfHeight + tileStep)//While moving in steps of TILESIZE away from the corner vectors doesn't overlap
            {

                checkVectorList.Add(new Vector2(actor.position.X + dx + halfWidth * xdir, actor.position.Y + halfHeight - tileStep));//Add intermediate vectors to check
                checkVectorList.Add(new Vector2(actor.position.X + dx + halfWidth * xdir, actor.position.Y - halfHeight + tileStep));
                tileStep += Tile.size;
            }

            List<Tile> tileList = new List<Tile>();//Make a list of tiles

            foreach (var cv in checkVectorList)//For every vector we created
            {

                try//Attempt to...
                {
                    tileList.Add(world.getTileAt(cv.X, cv.Y));//Get the tile at that vector position
                }
                catch (ArgumentNullException) { }//If getTileAt returns null, it means we're checking against the boundaries of the map.
            }//We will handle null tiles shortly, so we can ignore this.

            // if there exists a tile in the list that is solid, or there was a null tile
            if ( tileList.Exists(s => s == null) || tileList.Exists(s => s.solid) )
            {

                actor.hitWall();
                // snap character to nearest wall (with some buffer) and possibly bounce it.
                if (halfWidth < Tile.size)
                {

                    if (xdir == 1)
                    {
                        actor.position = new Vector2((actor.curTile.x + Tile.size) - halfWidth - BUFFER, actor.position.Y);
                    }
                    else if (xdir == -1)
                    {
                        actor.position = new Vector2(actor.curTile.x + halfWidth + BUFFER, actor.position.Y);
                    }
                }
                else
                {

                    if (xdir == 1)
                    {
                        actor.position = new Vector2((actor.curTile.x + Tile.size) - BUFFER, actor.position.Y);
                    }
                    else if (xdir == -1)
                    {
                        actor.position = new Vector2(actor.curTile.x + BUFFER, actor.position.Y);
                    }
                }

                tmpPos.X -= (tmpPos.X - actor.position.X) + (tmpPos.X - actor.position.X) * actor.elasticity;

                // apply an unnatural perpendicular force 
                if (tileList[0] != null && tileList[1] != null)//Corner checking is done with the first two tiles added to the list
                {//(this only works because of the order in which the list was constructed)
                    
                    if (!tileList[0].solid && tileList[1].solid)
                    {
                        tmpPos.Y -= corner_assist;
                    }
                    else if (tileList[0].solid && !tileList[1].solid)
                    {
                        tmpPos.Y += corner_assist;
                    }
                }
            }
            else
            {

                // if no obstruction, then move freely.
                actor.position = new Vector2(actor.position.X + dx, actor.position.Y);
            }
            
            // same as for X
            checkVectorList = new List<Vector2>();//List of all vectors we are going to use to check for tiles.
            checkVectorList.Add(new Vector2(actor.position.X + halfWidth, actor.position.Y + dy + ydir * halfHeight));//Right corner vector
            checkVectorList.Add(new Vector2(actor.position.X - halfWidth, actor.position.Y + dy + ydir * halfHeight));//Left corner vector
           
            while (actor.position.X + halfWidth - tileStep > actor.position.X - halfWidth + tileStep)//While moving in steps of TILESIZE away from the corner vectors doesn't overlap
            {

                checkVectorList.Add(new Vector2(actor.position.X + halfWidth - tileStep, actor.position.Y + dy + ydir * halfHeight));//Add intermediate vectors to check
                checkVectorList.Add(new Vector2(actor.position.X - halfWidth + tileStep, actor.position.Y + dy + ydir * halfHeight));
                tileStep += Tile.size;
            }

            tileList = new List<Tile>();//Make a list of tiles
            
            foreach (var cv in checkVectorList)//For every vector we created
            {

                try//Attempt to...
                {
                    tileList.Add(world.getTileAt(cv.X, cv.Y));//Get the tile at that vector position
                }
                catch (ArgumentNullException) { }//If getTileAt returns null, it means we're checking against the boundaries of the map.
            }//We will handle null tiles shortly, so we can ignore this.

            // if there exists a tile in the list that is solid, or there was a null tile
            if ( tileList.Exists(s => s == null) || tileList.Exists(s => s.solid) )
            {

                actor.hitWall();
                // snap character to nearest wall (with some buffer) and possibly bounce it.
                if (halfHeight < Tile.size)
                {

                    if (ydir == 1)
                    {
                        actor.position = new Vector2(actor.position.X, (actor.curTile.y + Tile.size) - halfHeight - BUFFER);
                    }
                    else if (ydir == -1)
                    {
                        actor.position = new Vector2(actor.position.X, actor.curTile.y + halfHeight + BUFFER);
                    }
                }
                else
                {

                    if (ydir == 1)
                    {
                        actor.position = new Vector2(actor.position.X, (actor.curTile.y + Tile.size)  - BUFFER);
                    }
                    else if (ydir == -1)
                    {
                        actor.position = new Vector2(actor.position.X, actor.curTile.y  + BUFFER);
                    }
                }

                tmpPos.Y -= (tmpPos.Y - actor.position.Y) + (tmpPos.Y - actor.position.Y) * actor.elasticity;
                
                if (tileList[0] != null && tileList[1] != null)//Corner checking is done with the first two tiles added to the list
                {//(this only works because of the order in which the list was constructed)
                    
                    if (!tileList[0].solid && tileList[1].solid)
                    {
                        tmpPos.X -= corner_assist;
                    }
                    else if (tileList[0].solid && !tileList[1].solid)
                    {
                        tmpPos.X += corner_assist;
                    }
                }
            }
            else
            {
                
                actor.position = new Vector2(actor.position.X, actor.position.Y + dy);
            }

            // Update Old Position
            actor._oldPos = tmpPos;

            // Reset forces, so they don't build up if there's no behavior
            actor.force = Vector2.Zero;

        }

        public override void Update(GameTime gameTime)
        {

            World world = tileEngine.world;

            if (world == null)
                return;

            base.Update(gameTime);

            for (LinkedListNode<Actor> a = world.actors.First; a != null; a = a.Next)
            {
                actorVsActor(a); 
            }

            foreach (Actor a in world.actors)
            {
                if (a.ignoreAvE)
                    continue;
                verletWithEnvironment(a);
            }
        }
    }
}
