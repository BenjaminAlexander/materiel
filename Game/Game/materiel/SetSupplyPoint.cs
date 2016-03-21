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
    class SetSupplyPoint : RtsCommandMessage
    {
        public SetSupplyPoint(LocalPlayer player, Base baseObj, Company co)
        {
            this.Append(baseObj.ID);
            this.Append(co.ID);
            player.SendTCP(this);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game, RemotePlayer player)
        {
            int baseObjID = message.ReadInt();
            int coID = message.ReadInt();

            Base obj = game.GameObjectCollection.Get<Base>(baseObjID);
            Company co = game.GameObjectCollection.Get<Company>(coID);
            if (player.Owns(obj) && player.Owns(co))
            {
                co.ResupplyPoint = obj;
            }
        }
    }
}
