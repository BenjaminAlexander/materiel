using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Server;
using MyGame.Client;
using Microsoft.Xna.Framework;
using MyGame.RtsCommands;

namespace MyGame.materiel.RtsCommandMessages
{
    class SetCompanyPositions : RtsCommandMessage
    {
        public SetCompanyPositions(LocalPlayer player, Company co, List<Vector2> positions)
        {
            this.Append(co.ID);
            this.Append(positions.Count);
            foreach (Vector2 pos in positions)
            {
                this.Append(pos);
            }
            player.SendTCP(this);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game, RemotePlayer player)
        {
            int companyId = message.ReadInt();
            int positionCount = message.ReadInt();
            List<Vector2> positions = new List<Vector2>();

            for (int i = 0; i < positionCount; i++)
            {
                positions.Add(message.ReadVector2());
            }

            Company co = game.GameObjectCollection.Get<Company>(companyId);
            if (player.Owns(co))
            {
                co.SetPositions(game.GameObjectCollection, positions);
            }
        }
    }
}