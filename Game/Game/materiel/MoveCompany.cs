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
    class MoveCompany : RtsCommandMessage
    {
        public MoveCompany(LocalPlayer player, Company co, Vector2 position)
        {
            this.Append(co.ID);
            this.Append(position);
            player.SendTCP(this);
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
