﻿using MyGame.Server;
using MyGame.Client;
using MyGame.RtsCommands;
using MyGame.materiel.GameObjects;

namespace MyGame.materiel.RtsCommandMessages
{
    class DeleteCompany : RtsCommandMessage
    {
        public DeleteCompany(LocalPlayer player, Company co)
        {
            this.Append(co.ID);
            player.SendTCP(this);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game, RemotePlayer player)
        {
            int coID = message.ReadInt();

            Company obj = game.GameObjectCollection.Get<Company>(coID);
            if (player.Owns(obj))
            {
                obj.Destroy();
            }
        }
    }
}
