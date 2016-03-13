using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Server;
using MyGame.Client;
using Microsoft.Xna.Framework;
using MyGame.RtsCommands;

namespace MyGame.materiel
{
    class MoveCompany : RtsCommand
    {
        public static void SendCommand(LocalPlayer player, Company co, Vector2 position)
        {
            RtsCommandMessage message = new RtsCommandMessage(typeof(MoveCompany));
            message.Append(co.ID);
            message.Append(position);
            player.SendTCP(message);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game)
        {
            int companyId = message.ReadInt();
            Vector2 position = message.ReadVector2();

            Company co = game.GameObjectCollection.Get<Company>(companyId);
            if (co != null)
            {
                co.Move(position);
            }
        }
    }
}
