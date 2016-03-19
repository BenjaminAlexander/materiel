using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using MyGame.Utils;
using MyGame.GameStateObjects;
using MyGame.materiel;
using MyGame.RtsCommands;

namespace MyGame.Server
{
    public class RemotePlayer : BasePlayer<GameMessage, RtsCommandMessage>
    {
        private PlayerGameObject playerGameObject = null;

        public RemotePlayer() : base()
        {
        }

        public PlayerGameObject GameObject
        {
            get
            {
                return playerGameObject;
            }
        }

        public void CreatPlayerGameObject(ServerGame game)
        {
            playerGameObject = PlayerGameObject.Factory(game, this);
        }

        public void HandleAllTCPMessages(ServerGame game)
        {
            Queue<RtsCommandMessage> queue = this.DequeueAllIncomingTCP();
            while (queue.Count > 0)
            {
                RtsCommandMessage message = queue.Dequeue();
                message.Execute(game, this);
            }
        }

        public bool Owns(IPlayerControlled obj)
        {
            PlayerGameObject playerObject = obj.ControllingPlayer;
            if (playerObject != null && obj != null)
            {
                return playerObject.PlayerID == this.Id;
            }
            return false;
        }
    }
}
