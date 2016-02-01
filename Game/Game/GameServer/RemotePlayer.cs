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

namespace MyGame.GameServer
{
    public class RemotePlayer : BasePlayer<UdpMessage, GameObjectUpdate>
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
    }
}
