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
    class CreateCompany : RtsCommand
    {
        public static void SendCommand(LocalPlayer player)
        {
            RtsCommandMessage message = new RtsCommandMessage(typeof(CreateCompany));
            message.Append(player.Id);
            player.SendTCP(message);
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