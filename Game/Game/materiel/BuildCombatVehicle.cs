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
    class BuildCombatVehicle : RtsCommand
    {
        public static void SendCommand(LocalPlayer player, Base baseObj)
        {
            RtsCommandMessage message = new RtsCommandMessage(typeof(BuildCombatVehicle));
            message.Append(baseObj.ID);
            player.SendTCP(message);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game)
        {
            int baseObjID = message.ReadInt();

            Base obj = game.GameObjectCollection.Get<Base>(baseObjID);
            if (obj != null)
            {
                obj.BuildCombatVehicle();
            }
        }
    }
}
