﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGame.IO;
using MyGame.GameStateObjects;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BaseGame : Game
    {
        protected GraphicsDeviceManager graphics;
        private MyGraphicsClass myGraphicsObject;
        private Camera camera;
        private InputManager inputManager;
        private GameObjectCollection gameObjectCollection;
        private Vector2 worldSize;

        public InputManager InputManager
        {
            get { return inputManager; }
        }

        public Camera Camera
        {
            get { return camera; }
        }

        public MyGraphicsClass GraphicsObject
        {
            get { return myGraphicsObject; }
        }

        public GameObjectCollection GameObjectCollection
        {
            get { return gameObjectCollection; }
        }

        public BaseGame()
            : base()
        {
            GameObjectTypes.Initialize();

            this.inputManager = new InputManager();

            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.HardwareModeSwitch = false;
            this.graphics.IsFullScreen = false;

            //TODO: correctly set these
            this.graphics.PreferredBackBufferWidth = 1300;
            this.graphics.PreferredBackBufferHeight = 700;

            this.Window.IsBorderless = false;
            this.Window.AllowUserResizing = false;
            this.InactiveSleepTime = new TimeSpan(0);
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.graphics.ApplyChanges();


        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.camera = new Camera(new Vector2(0), 1f, 0, this.graphics, inputManager);

            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            myGraphicsObject = new MyGraphicsClass(this.graphics, spriteBatch, this.camera);

            gameObjectCollection = new GameObjectCollection(worldSize);
        }

        protected void SetWorldSize(Vector2 worldSize)
        {
            this.worldSize = worldSize;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of the content.
        /// </summary>
        protected override void LoadContent()
        {
            Content.RootDirectory = "Content";
            MyGraphicsClass.LoadContent(Content);
            TextureLoader.LoadContent(Content);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);

            if (this.IsActive)
            {
                inputManager.Update();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            this.GameObjectCollection.Draw(gameTime, this.GraphicsObject, this.camera);
        }
    }
}
