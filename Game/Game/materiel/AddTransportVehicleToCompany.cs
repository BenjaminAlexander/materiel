using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Server;
using MyGame.Client;
using MyGame.RtsCommands;

namespace MyGame.materiel
{
    class AddTransportVehicleToCompany : RtsCommandMessage
    {
        public AddTransportVehicleToCompany(LocalPlayer player, Company co, Transport vic)
        {
            this.Append(co.ID);
            this.Append(vic.ID);
            player.SendTCP(this);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game, RemotePlayer player)
        {
            int companyId = message.ReadInt();
            int vehicleId = message.ReadInt();

            Company co = game.GameObjectCollection.Get<Company>(companyId);
            Transport vic = game.GameObjectCollection.Get<Transport>(vehicleId);
            if (player.Owns(co) && player.Owns(co))
            {
                co.AddTransportVehicle(vic);
            }
        }
    }
}
