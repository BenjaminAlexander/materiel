using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Server;
using MyGame.Client;
using MyGame.RtsCommands;
using MyGame.materiel.GameObjects;

namespace MyGame.materiel.RtsCommandMessages
{
    class BuildTransport : RtsCommandMessage
    {
        public BuildTransport(LocalPlayer player, Base baseObj)
        {
            this.Append(baseObj.ID);
            player.SendTCP(this);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game, RemotePlayer player)
        {
            int baseObjID = message.ReadInt();

            Base obj = game.GameObjectCollection.Get<Base>(baseObjID);
            if (player.Owns(obj))
            {
                obj.BuildTransportVehicle();
            }
        }
    }
}
