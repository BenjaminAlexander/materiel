﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameServer;
using MyGame.materiel;
using MyGame.GameStateObjects;
using MyGame.GameClient;

namespace MyGame.RtsCommands
{
    class CreateCompany : RtsCommand
    {
        private int playerObjId;

        public CreateCompany(LocalPlayer player)
        {
            this.playerObjId = player.GameObject.ID;

            RtsCommandMessage message = new RtsCommandMessage(this);
            message.Append(this.playerObjId);
            player.SendTCP(message);
        }

        public CreateCompany(RtsCommandMessage message)
        {
            this.playerObjId = message.ReadInt();
        }

        public override void Execute(ServerGame game)
        {
            GameObject obj = game.GameObjectCollection.Get(playerObjId);
            if (obj is PlayerGameObject)
            {
                ((PlayerGameObject)obj).AddCompany(game);
            }
        }
    }
}