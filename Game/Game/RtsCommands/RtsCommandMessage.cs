using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using Microsoft.Xna.Framework;

namespace MyGame.RtsCommands
{
    public class RtsCommandMessage : TcpMessage
    {
        private RtsCommand command;

        public RtsCommand Command
        {
            get
            {
                return command;
            }
        }

        public RtsCommandMessage(RtsCommand command)
            : base(new GameTime())
        {
            this.command = command;
            this.Append(command.TypeID());
        }

        public RtsCommandMessage(UdpTcpPair pair)
            : base(pair)
        {
            this.ResetReader();
            int commandTypeID = this.ReadInt();
            this.command = RtsCommand.ConstructCommand(commandTypeID, this);
        }
    }
}
