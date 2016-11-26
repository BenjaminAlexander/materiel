using MyGame.Server;
using MyGame.Client;
using MyGame.RtsCommands;
using MyGame.materiel.GameObjects;

namespace MyGame.materiel.RtsCommandMessages
{
    class SetSupplyPoint : RtsCommandMessage
    {
        public SetSupplyPoint(LocalPlayer player, Base baseObj, Company co)
        {
            this.Append(baseObj.ID);
            this.Append(co.ID);
            player.SendTCP(this);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game, RemotePlayer player)
        {
            int baseObjID = message.ReadInt();
            int coID = message.ReadInt();

            Base obj = game.GameObjectCollection.Get<Base>(baseObjID);
            Company co = game.GameObjectCollection.Get<Company>(coID);
            if (player.Owns(obj) && player.Owns(co))
            {
                co.ResupplyPoint = obj;
            }
        }
    }
}
