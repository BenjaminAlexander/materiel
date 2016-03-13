using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Server;
using MyGame.materiel;
using MyGame.GameStateObjects;
using MyGame.Client;

namespace MyGame.RtsCommands
{
    class BuildCombatVehicle : RtsCommand
    {
        private int baseObjID;

        public BuildCombatVehicle(LocalPlayer player, Base baseObj)
        {
            this.baseObjID = baseObj.ID;

            RtsCommandMessage message = new RtsCommandMessage(this);
            message.Append(baseObjID);
            player.SendTCP(message);
        }

        public BuildCombatVehicle(RtsCommandMessage message)
        {
            this.baseObjID = message.ReadInt();
        }

        public override void Execute(ServerGame game)
        {
            GameObject obj = game.GameObjectCollection.Get(baseObjID);
            if (obj is Base)
            {
                ((Base)obj).BuildCombatVehicle();
            }
        }
    }
}
