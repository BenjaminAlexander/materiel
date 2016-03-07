using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Net.Sockets;
using System.Reflection;

namespace MyGame.Networking
{
    public abstract class TcpMessage : GameMessage
    {
        private static Dictionary<int, ConstructorInfo> messageConstructors = new Dictionary<int, ConstructorInfo>();

        public TcpMessage(GameTime gameTime)
            : base(gameTime)
        {
        }

        public TcpMessage(UdpTcpPair pair) : base(pair.ClientStream)
        {
        }

        public override void Send(UdpTcpPair pair)
        {
            pair.ClientStream.Write(this.MessageBuffer, 0, this.Size);
            pair.ClientStream.Flush();
        }

        /*
        private new static void Initialize()
        {
            IEnumerable<Type> types =
                Assembly.GetAssembly(typeof(TcpMessage))
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(TcpMessage)));
            types = types.OrderBy(t => t.Name);
            Type[] messageTypeArray = types.ToArray();

            for (int i = 0; i < messageTypeArray.Length; i++)
            {
                int typeID = GameMessage.TypeID(messageTypeArray[i]);

                var constructorParams = new Type[1];
                constructorParams[0] = typeof(UdpTcpPair);

                ConstructorInfo constructor = messageTypeArray[i].GetConstructor(constructorParams);
                messageConstructors[typeID] = constructor;
            }
        }

        private static TcpMessage ConstructMessage(UdpTcpPair pair)
        {
            ConstructorInfo constructor = messageConstructors[BitConverter.ToInt32(b, 0)];

            var constuctorParams = new object[2];
            constuctorParams[0] = b;
            constuctorParams[1] = length;

            var message = (GameMessage)constructor.Invoke(constuctorParams);
            return message;
        }*/
    }
}
