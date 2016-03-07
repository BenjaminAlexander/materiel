using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MyGame.GameServer;

namespace MyGame.RtsCommands
{
    public abstract class RtsCommand
    {
        private static Type[] commandTypeArray;
        private static Dictionary<int, ConstructorInfo> commandConstructors = new Dictionary<int, ConstructorInfo>();

        public static void Initialize()
        {
            IEnumerable<Type> types =
                Assembly.GetAssembly(typeof(RtsCommand))
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(RtsCommand)));
            types = types.OrderBy(t => t.Name);
            commandTypeArray = types.ToArray();

            for (int i = 0; i < commandTypeArray.Length; i++)
            {
                var constructorParams = new Type[1];
                constructorParams[0] = typeof(RtsCommandMessage);

                ConstructorInfo constructor = commandTypeArray[i].GetConstructor(constructorParams);
                commandConstructors[i] = constructor;
            }
        }

        public static RtsCommand ConstructCommand(int typeID, RtsCommandMessage message)
        {
            var constuctorParams = new object[1];
            constuctorParams[0] = message;

            RtsCommand command = (RtsCommand)commandConstructors[typeID].Invoke(constuctorParams);
            return command;
        }

        public int TypeID()
        {
            for (int i = 0; i < commandTypeArray.Length; i++)
            {
                if (commandTypeArray[i] == this.GetType())
                {
                    return i;
                }
            }
            throw new Exception("Type not found");
        }

        public abstract void Execute(ServerGame game);

    }
}
