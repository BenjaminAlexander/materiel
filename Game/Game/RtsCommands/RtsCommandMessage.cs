using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using Microsoft.Xna.Framework;
using MyGame.Server;
using System.Reflection;

namespace MyGame.RtsCommands
{
    public delegate void CommandDelegate(RtsCommandMessage message, ServerGame game);

    public class RtsCommandMessage : TcpMessage
    {

        private static Type[] commandTypeArray;
        private static Dictionary<int, CommandDelegate> commandDelegates = new Dictionary<int, CommandDelegate>();

        public static void InitializeRTS()
        {
            IEnumerable<Type> types =
                Assembly.GetAssembly(typeof(RtsCommand))
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(RtsCommand)));
            types = types.OrderBy(t => t.Name);
            commandTypeArray = types.ToArray();

            for (int i = 0; i < commandTypeArray.Length; i++)
            {
                commandDelegates[i] = (CommandDelegate)Delegate.CreateDelegate(typeof(CommandDelegate), commandTypeArray[i].GetMethod("ExecuteCommand"));
            }
        }

        public static int TypeID(Type t)
        {
            for (int i = 0; i < commandTypeArray.Length; i++)
            {
                if (commandTypeArray[i] == t)
                {
                    return i;
                }
            }
            throw new Exception("Type not found");
        }

        public void Execute(ServerGame game)
        {
            this.ResetReader();
            int typeID = this.ReadInt();
            commandDelegates[typeID](this, game);
        }

        public RtsCommandMessage(Type t)
            : base(new GameTime())
        {
            this.Append(RtsCommandMessage.TypeID(t));
        }

        public RtsCommandMessage(UdpTcpPair pair)
            : base(pair)
        {

        }
    }
}
