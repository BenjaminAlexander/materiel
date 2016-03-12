using System;
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
    class AddVehicleToCompany : RtsCommand
    {
        private int companyId;
        private int vehicleId;

        public AddVehicleToCompany(LocalPlayer player, Company co, Vehicle vic)
        {
            this.companyId = co.ID;
            this.vehicleId = vic.ID;

            RtsCommandMessage message = new RtsCommandMessage(this);
            message.Append(this.companyId);
            message.Append(this.vehicleId);
            player.SendTCP(message);
        }

        public AddVehicleToCompany(RtsCommandMessage message)
        {
            this.companyId = message.ReadInt();
            this.vehicleId = message.ReadInt();
        }

        public override void Execute(ServerGame game)
        {
            GameObject co = game.GameObjectCollection.Get(this.companyId);
            GameObject vic = game.GameObjectCollection.Get(this.vehicleId);
            if (co is Company && vic is Vehicle)
            {
                ((Company)co).AddVehicle((Vehicle)vic);
            }
        }
    }
}
