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
        public MoveCompany(LocalPlayer player, Company co, Vector2 position1, Vector2 position2)
        {
            this.Append(co.ID);
            this.Append(position1);
            this.Append(position2);
            player.SendTCP(this);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game, RemotePlayer player)
        {
            int companyId = message.ReadInt();
            Vector2 position1 = message.ReadVector2();
            Vector2 position2 = message.ReadVector2();

            Company co = game.GameObjectCollection.Get<Company>(companyId);
            if (player.Owns(co))
            {
                co.Move(position1, position2);
            }
        }
    }
}
