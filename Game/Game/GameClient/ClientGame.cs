﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.Utils;
using MyGame.GameServer;
using System.Net;

namespace MyGame.GameClient
{
    public class ClientGame : Game1
    {
        private LocalPlayer serverConnection;

        public ClientGame(IPAddress serverAddress)
            : base()
        {
            this.serverConnection = new LocalPlayer(serverAddress, this);

            SetWorldSize m = serverConnection.GetSetWorldSize();
            this.SetWorldSize(m.WorldSize);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            //haddle all available messages.  this is done again after the gameObject updates but before draw
            Queue<GameObjectUpdate> messageQueue = this.serverConnection.DequeueAllIncomingUDP();
            while (messageQueue.Count > 0)
            {
                GameObjectUpdate m = messageQueue.Dequeue();
                m.Apply(this, gameTime);
            }

            base.Update(gameTime);
            this.GameObjectCollection.ClientUpdate(gameTime);

            GameObjectField.SetModeDraw();
            this.Camera.Update(secondsElapsed);
        }

        protected override void Draw(GameTime gameTime)
        {
            GameObjectField.SetModeDraw();
            base.Draw(gameTime);

            GameObjectField.SetModeSimulation();

            this.GraphicsObject.Begin(Matrix.Identity);

            this.GraphicsObject.End();
            
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            serverConnection.Disconnect();
        }
    }
}