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
    class CreateCompany : RtsCommandMessage
    {
        public CreateCompany(LocalPlayer player)
        {
            this.Append(player.Id);
            player.SendTCP(this);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game)
        {
            int playerObjId = message.ReadInt();
            PlayerGameObject obj = game.GameObjectCollection.Get<PlayerGameObject>(playerObjId);
            if (obj != null)
            {
                obj.AddCompany(game);
            }
        }
    }
}