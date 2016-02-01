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

namespace MyGame.GameServer
{
    public class Player : BasePlayer<UdpMessage, GameObjectUpdate>
    {
       

        public Player() : base()
        {
        }

        public override UdpMessage GetUDPMessage(UdpTcpPair client)
        {
            throw new NotImplementedException();
        }
    }
}
