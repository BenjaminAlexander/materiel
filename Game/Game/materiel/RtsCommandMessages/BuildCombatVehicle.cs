using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Server;
using MyGame.Client;
using MyGame.RtsCommands;

namespace MyGame.materiel.RtsCommandMessages
{
    class BuildCombatVehicle : RtsCommandMessage
    {
        public BuildCombatVehicle(LocalPlayer player, Base baseObj)
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
                obj.BuildCombatVehicle();
            }
        }
    }
}
