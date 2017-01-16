using MyGame.Server;
using MyGame.Client;
using MyGame.RtsCommands;
using MyGame.materiel.GameObjects;

namespace MyGame.materiel
{
    class AddCombatVehicleToCompany : RtsCommandMessage
    {
        public AddCombatVehicleToCompany(LocalPlayer player, Company co, CombatVehicle vic)
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
            CombatVehicle vic = game.GameObjectCollection.Get<CombatVehicle>(vehicleId);
            if (player.Owns(co) && player.Owns(vic))
            {
                co.AddVehicle(vic);
            }
        }
    }
}
