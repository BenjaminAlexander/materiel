using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameServer;
using MyGame.materiel;
using MyGame.GameStateObjects;

namespace MyGame.RtsCommands
{
    class BuildCombatVehicle : RtsCommand
    {
        private int baseObjID;

        public BuildCombatVehicle(Base baseObj)
        {
            this.baseObjID = baseObj.ID;
        }

        public BuildCombatVehicle(RtsCommandMessage message)
        {
            this.baseObjID = message.ReadInt();
        }

        public override RtsCommandMessage GetMessage()
        {
            RtsCommandMessage message = new RtsCommandMessage(this);
            message.Append(baseObjID);
            return message;
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
