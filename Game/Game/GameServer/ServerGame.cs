﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame;
using MyGame.Networking;
using Microsoft.Xna.Framework;
using MyGame.PlayerControllers;
using MyGame.GameStateObjects;

namespace MyGame.GameServer
{
    class ServerGame : Game1
    {
        private Lobby lobby;
        private ServerLogic serverLogic = null;

        //TODO: there needs to be a better way to set up game-mode-ish parameters
        private static Vector2 SetWorldSize(Lobby lobby)
        {
            Vector2 worldSize = new Vector2(20000);
            lobby.BroadcastTCP(new SetWorldSize(worldSize));
            return worldSize;
        }

        public ServerGame(Lobby lobby)
            : base(0/*TODO: 0 is the player ID of the server, this should probably change*/, SetWorldSize(lobby))
        {
            this.lobby = lobby;

            foreach (Client client in lobby.Clients)
            {
                StaticNetworkPlayerManager.Add(client.GetID());
            }


        }

        protected override void Initialize()
        {
            base.Initialize();
            serverLogic = new ServerLogic(this.WorldSize, this.InputManager);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            Queue<GameMessage> messageQueue = lobby.DequeueAllInboundMessages();
            while (messageQueue.Count > 0)
            {
                GameMessage m = messageQueue.Dequeue();
                if (m is GameUpdate)
                {
                    ((GameUpdate)m).Apply(this);
                }
            }
            lobby.BroadcastUDP(serverLogic.GetStaticControllerFocusUpdateMessages());
            serverLogic.Update(secondsElapsed);
            base.Update(gameTime);

            Queue<GameMessage> gameObjectUpdates = StaticGameObjectCollection.Collection.ServerUpdate(gameTime);
            lobby.BroadcastUDP(gameObjectUpdates);
            this.Camera.Update(true, secondsElapsed);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}