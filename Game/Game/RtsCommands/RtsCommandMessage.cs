using System;
using System.Collections.Generic;
using System.Linq;
using MyGame.Networking;
using Microsoft.Xna.Framework;
using MyGame.Server;
using System.Reflection;
using System.Net.Sockets;

namespace MyGame.RtsCommands
{
    public delegate void CommandDelegate(RtsCommandMessage message, ServerGame game, RemotePlayer player);

    public class RtsCommandMessage : GameMessage
    {

        private static Type[] commandTypeArray;
        private static Dictionary<int, CommandDelegate> commandDelegates = new Dictionary<int, CommandDelegate>();
        private static Dictionary<Type, int> commandIDs = new Dictionary<Type, int>();

        public static void InitializeRTS()
        {
            IEnumerable<Type> types =
                Assembly.GetAssembly(typeof(RtsCommandMessage))
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(RtsCommandMessage)));
            types = types.OrderBy(t => t.Name);
            commandTypeArray = types.ToArray();

            for (int i = 0; i < commandTypeArray.Length; i++)
            {
                commandDelegates[i] = (CommandDelegate)Delegate.CreateDelegate(typeof(CommandDelegate), commandTypeArray[i].GetMethod("ExecuteCommand"));
                commandIDs[commandTypeArray[i]] = i;
            }
        }

        public void Execute(ServerGame game, RemotePlayer player)
        {
            this.ResetReader();
            int typeID = this.ReadInt();
            commandDelegates[typeID](this, game, player);
        }

        public RtsCommandMessage()
            : base(new GameTime())
        {
            this.Append(commandIDs[this.GetType()]);
        }

        public RtsCommandMessage(NetworkStream stream)
            : base(stream)
        {

        }
    }
}
