using System.Collections.Generic;
using MyGame.Networking;
using MyGame.materiel;
using MyGame.RtsCommands;
using MyGame.materiel.GameObjects;

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
