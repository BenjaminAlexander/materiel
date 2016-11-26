using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using System.Net;

namespace MyGame.Client
{
    public class ClientGame : BaseGame
    {
        private LocalPlayer localPlayer;

        public LocalPlayer LocalPlayer
        {
            get
            {
                return localPlayer;
            }
        }

        public ClientGame(IPAddress serverAddress)
            : base()
        {
            this.localPlayer = new LocalPlayer(serverAddress, this);

            SetWorldSize m = localPlayer.DequeueIncomingTCP();
            this.SetWorldSize(m.WorldSize);
        }

        protected override void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            Queue<GameObjectUpdate> messageQueue = this.localPlayer.DequeueAllIncomingUDP();
            while (messageQueue.Count > 0)
            {
                GameObjectUpdate m = messageQueue.Dequeue();
                m.Apply(this, gameTime);
            }

            base.Update(gameTime);
            this.GameObjectCollection.ClientUpdate(gameTime);

            this.Camera.Update(secondsElapsed);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            this.localPlayer.Draw(gameTime, this.GraphicsObject);  
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            localPlayer.Disconnect();
        }
    }
}