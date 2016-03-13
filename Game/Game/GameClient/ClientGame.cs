using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.Utils;
using MyGame.GameServer;
using System.Net;

namespace MyGame.GameClient
{
    public class ClientGame : Game1
    {
        private LocalPlayer serverConnection;

        public LocalPlayer LocalPlayer
        {
            get
            {
                return serverConnection;
            }
        }

        public ClientGame(IPAddress serverAddress)
            : base()
        {
            this.serverConnection = new LocalPlayer(serverAddress, this);

            SetWorldSize m = serverConnection.DequeueIncomingTCP();
            this.SetWorldSize(m.WorldSize);
        }

        protected override void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

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
            this.GraphicsObject.BeginWorld();
            this.serverConnection.Draw(gameTime, this.GraphicsObject);
            this.GraphicsObject.End();

            this.GraphicsObject.Begin();
            this.serverConnection.DrawHud(gameTime, this.GraphicsObject);
            this.GraphicsObject.End();

            GameObjectField.SetModeSimulation();            
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            serverConnection.Disconnect();
        }
    }
}