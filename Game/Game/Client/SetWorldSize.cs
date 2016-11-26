using Microsoft.Xna.Framework;
using System.Net.Sockets;
using MyGame.Networking;

namespace MyGame.Client
{
    public class SetWorldSize : GameMessage
    {
        private Vector2 worldSize;
        public Vector2 WorldSize
        {
            get { return worldSize; }
        }

        public SetWorldSize(Vector2 worldSize)
            : base(new GameTime())
        {
            this.worldSize = worldSize;
            this.Append(worldSize);
        }

        public SetWorldSize(NetworkStream stream)
            : base(stream)
        {
            this.ResetReader();
            worldSize = this.ReadVector2();
            this.AssertMessageEnd();
        }
    }
}
