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
    public class RemotePlayer : BasePlayer<UdpMessage, GameObjectUpdate>
    {
        private PlayerGameObject playerGameObject = null;

        private ThreadSafeQueue<RtsCommandMessage> rtsCommandMessageQueue = new ThreadSafeQueue<RtsCommandMessage>();
        private Thread rtsCommandMessageReader;

        public RemotePlayer() : base()
        {
            this.rtsCommandMessageReader = new Thread(new ThreadStart(RtsCommandMessageReader));
            this.rtsCommandMessageReader.Start();
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

        public Queue<RtsCommandMessage> DequeueAllRtsCommandMessages()
        {
            return rtsCommandMessageQueue.DequeueAll();
        }

        private void RtsCommandMessageReader()
        {
            try
            {
                while (true)
                {
                    RtsCommandMessage m = this.GetRtsCommandMessage();
                    rtsCommandMessageQueue.Enqueue(m);
                }
            }
            catch (Exception)
            {
                //TODO: close the client game
            }
        }
    }
}
