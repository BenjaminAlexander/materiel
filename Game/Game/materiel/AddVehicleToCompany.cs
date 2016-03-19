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
    class AddVehicleToCompany : RtsCommandMessage
    {
        public AddVehicleToCompany(LocalPlayer player, Company co, Vehicle vic)
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
            Vehicle vic = game.GameObjectCollection.Get<Vehicle>(vehicleId);
            if (player.Owns(co) && player.Owns(co))
            {
                co.AddVehicle(vic);
            }
        }
    }
}
