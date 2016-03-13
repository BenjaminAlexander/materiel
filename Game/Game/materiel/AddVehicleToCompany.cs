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
    class AddVehicleToCompany : RtsCommand
    {
        public static void SendCommand(LocalPlayer player, Company co, Vehicle vic)
        {
            RtsCommandMessage message = new RtsCommandMessage(typeof(AddVehicleToCompany));
            message.Append(co.ID);
            message.Append(vic.ID);
            player.SendTCP(message);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game)
        {
            int companyId = message.ReadInt();
            int vehicleId = message.ReadInt();

            Company co = game.GameObjectCollection.Get<Company>(companyId);
            Vehicle vic = game.GameObjectCollection.Get<Vehicle>(vehicleId);
            if (co != null && vic != null)
            {
                co.AddVehicle(vic);
            }
        }
    }
}
