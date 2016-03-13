using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Server;
using MyGame.materiel;
using MyGame.GameStateObjects;
using MyGame.Client;
using Microsoft.Xna.Framework;

namespace MyGame.RtsCommands
{
    class MoveCompany : RtsCommand
    {
        private int companyId;
        private Vector2 position;

        public MoveCompany(LocalPlayer player, Company co, Vector2 position)
        {
            this.companyId = co.ID;
            this.position = position;

            RtsCommandMessage message = new RtsCommandMessage(this);
            message.Append(this.companyId);
            message.Append(this.position);
            player.SendTCP(message);
        }

        public MoveCompany(RtsCommandMessage message)
        {
            this.companyId = message.ReadInt();
            this.position = message.ReadVector2();
        }

        public override void Execute(ServerGame game)
        {
            GameObject co = game.GameObjectCollection.Get(this.companyId);
            if (co is Company)
            {
                ((Company)co).Move(this.position);
            }
        }
    }
}
