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
    class DeleteCompany : RtsCommandMessage
    {
        public DeleteCompany(LocalPlayer player, Company co)
        {
            this.Append(co.ID);
            player.SendTCP(this);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game, RemotePlayer player)
        {
            int coID = message.ReadInt();

            Company obj = game.GameObjectCollection.Get<Company>(coID);
            if (player.Owns(obj))
            {
                obj.Destroy();
            }
        }
    }
}
