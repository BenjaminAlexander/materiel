﻿using Microsoft.Xna.Framework;
using MyGame.Client;

namespace MyGame.Server
{
    public class ServerGame : BaseGame
    {
        private static Vector2 worldSize = new Vector2(8000, 8000);
        private Lobby lobby;
        private ServerLogic serverLogic = null;

        //TODO: there needs to be a better way to set up game-mode-ish parameters
        public ServerGame(Lobby lobby)
            : base()
        {
            this.lobby = lobby;

            lobby.BroadcastTCP(new SetWorldSize(worldSize));
            this.SetWorldSize(worldSize);
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.SetWorldSize(worldSize);
            serverLogic = new ServerLogic(this, lobby, worldSize);
        }

        protected override void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            base.Update(gameTime);
            this.lobby.HandleAllTCPMessages(this);
            this.GameObjectCollection.ServerUpdate(lobby, gameTime);
            this.Camera.Update(secondsElapsed);
        }
    }
}
