﻿using MyGame.Server;
using MyGame.Client;
using MyGame.RtsCommands;
using MyGame.materiel.GameObjects;

namespace MyGame.materiel.RtsCommandMessages
{
    class SetTransportSupplyRoute : RtsCommandMessage
    {
        public SetTransportSupplyRoute(LocalPlayer player, Transport vic, Base base1, Base base2)
        {
            this.Append(vic.ID);
            this.Append(base1.ID);
            this.Append(base2.ID);
            player.SendTCP(this);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game, RemotePlayer player)
        {
            int vicID = message.ReadInt();
            int base1ID = message.ReadInt();
            int base2ID = message.ReadInt();

            Transport vic = game.GameObjectCollection.Get<Transport>(vicID);
            Base base1 = game.GameObjectCollection.Get<Base>(base1ID);
            Base base2 = game.GameObjectCollection.Get<Base>(base2ID);

            if (player.Owns(vic))
            {
                vic.SetResupplyRoute(base1, base2);
            }
        }
    }
}