using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using System.Net;
using MyGame.Utils;
using MyGame.GameServer;
using MyGame.GameStateObjects;
using System.Threading;
using Microsoft.Xna.Framework;

namespace MyGame.GameClient
{
    //TODO: name this class better
    //maybe combine with local player
    public class LocalPlayer : BasePlayer<GameObjectUpdate, UdpMessage>
    {

        public LocalPlayer(IPAddress serverAddress, ClientGame game) : base(serverAddress)
        {
        }


        public override GameObjectUpdate GetUDPMessage(UdpTcpPair client)
        {
            return new GameObjectUpdate(client);
        }
    }
}
