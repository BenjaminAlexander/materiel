using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using MyGame.GameClient;
using MyGame.Utils;
using MyGame.GameStateObjects;
using MyGame.materiel;
using MyGame.RtsCommands;

namespace MyGame.GameServer
{
    public class RemotePlayer : BasePlayer<UdpMessage, RtsCommandMessage>
    {
        private PlayerGameObject playerGameObject = null;

        public RemotePlayer() : base()
        {
        }

        public void CreatPlayerGameObject(ServerGame game)
        {
            playerGameObject = PlayerGameObject.Factory(game, this);
        }

        public PlayerGameObject GameObject
        {
            get
            {
                return playerGameObject;
            }
        }

        public override UdpMessage GetUDPMessage(UdpTcpPair client)
        {
            throw new NotImplementedException();
        }

        public override RtsCommandMessage GetTCPMessage(UdpTcpPair client)
        {
            return new RtsCommandMessage(client);
        }

        public void HandleAllTCPMessages(ServerGame game)
        {
            Queue<RtsCommandMessage> queue = this.DequeueAllIncomingTCP();
            while (queue.Count > 0)
            {
                RtsCommandMessage message = queue.Dequeue();
                message.Execute(game);
            }
        }
    }
}
