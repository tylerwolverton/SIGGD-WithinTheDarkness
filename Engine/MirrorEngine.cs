using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Engine.Components;

namespace Engine
{
    public abstract class MirrorEngine : Microsoft.Xna.Framework.Game
    {
        // Game wide random number generator
        public Random randGen = new Random();

        public string currentWorldPath { get; protected set; }

        public ResourceComponent resourceComponent { get; set; }
        public InputComponent inputComponent { get; set; }
        public World world { get; set; }
        public PhysicsComponent physicsComponent { get; set; }
        public GraphicsComponent graphicsComponent { get; set; }
        public AudioComponent audioComponent { get; set; }

        public MirrorEngine()
        {
            base.IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {

            base.Initialize();

            if (resourceComponent != null)
            {
                resourceComponent.Initialize();
            }
            if (inputComponent != null)
            {
                inputComponent.Initialize();
            }
            if (world != null)
            {
                world.Initialize();
            }
            if (physicsComponent != null)
            {
                physicsComponent.Initialize();
            }
            if (graphicsComponent != null)
            {
                graphicsComponent.Initialize();
            }
            if (audioComponent != null)
            {
                audioComponent.Initialize();
            }
        }

        protected override void LoadContent()
        {

            base.LoadContent();

            if (resourceComponent != null)
            {
                resourceComponent.LoadContent();
            }
            if (inputComponent != null)
            {
                inputComponent.LoadContent();
            }
            if (world != null)
            {
                world.LoadContent();
            }
            if (physicsComponent != null)
            {
                physicsComponent.LoadContent();
            }
            if (graphicsComponent != null)
            {
                graphicsComponent.LoadContent();
            }
            if (audioComponent != null)
            {
                audioComponent.LoadContent();
            }
        }

        protected override void Update(GameTime gameTime)
        {

            base.Update(gameTime);

            if (resourceComponent != null && resourceComponent.isActive) {
                resourceComponent.Update(gameTime);
            }
            if (inputComponent != null && inputComponent.isActive) {
                inputComponent.Update(gameTime);
            }
            if (world != null && world.isActive) {
                world.Update(gameTime);
            }
            if (physicsComponent != null && physicsComponent.isActive) {
                physicsComponent.Update(gameTime);
            }
            if (graphicsComponent != null && graphicsComponent.isActive) {
                graphicsComponent.Update(gameTime);
            }
            if (audioComponent != null && audioComponent.isActive) {
                audioComponent.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {   

            base.Draw(gameTime);

            //Some of these can be assumed to never draw
            if (resourceComponent != null)
            {
                resourceComponent.Draw();
            }
            if (inputComponent != null)
            {
                inputComponent.Draw();
            }
            if (world != null)
            {
                world.Draw();
            }
            if (physicsComponent != null)
            {
                physicsComponent.Draw();
            }
            if (graphicsComponent != null)
            {
                graphicsComponent.Draw();
            }
            if (audioComponent != null)
            {
                audioComponent.Draw();
            }
        }

        protected override void UnloadContent()
        {

            base.UnloadContent();

            if (resourceComponent != null)
            {
                resourceComponent.UnloadContent();
            }
            if (inputComponent != null)
            {
                inputComponent.UnloadContent();
            }
            if (world != null)
            {
                world.UnloadContent();
            }
            if (physicsComponent != null)
            {
                physicsComponent.UnloadContent();
            }
            if (graphicsComponent != null)
            {
                graphicsComponent.UnloadContent();
            }
            if (audioComponent != null)
            {
                audioComponent.UnloadContent();
            }
        }

        public virtual void setWorld(string str)
        {
            if (str != null)
            {

                uint windex = resourceComponent.worldNames[str];
                World w = resourceComponent.worldArray[windex];

                if (w == null)
                {
                    resourceComponent.worldArray[windex] = w = loadWorld(str);
                }

                world = w;

                world.isActive = true;
                physicsComponent.isActive = true;
                currentWorldPath = str;
            }
        }

        public virtual void killWorld()
        {
            resourceComponent.worldArray[resourceComponent.worldNames[currentWorldPath]] = null;
        }

        public abstract World loadWorld(string str);

        public virtual void saveState(BinaryWriter writer)
        {
            writer.Write(currentWorldPath);
        }

        public virtual void loadState(BinaryReader reader)
        {
            currentWorldPath = reader.ReadString();
        }
    }
}

        

        

        

        