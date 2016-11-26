using MyGame.Server;
using MyGame.Client;
using MyGame.RtsCommands;

namespace MyGame.materiel.RtsCommandMessages
{
    class CreateCompany : RtsCommandMessage
    {
        public CreateCompany(LocalPlayer player)
        {
            player.SendTCP(this);
        }

        public static void ExecuteCommand(RtsCommandMessage message, ServerGame game, RemotePlayer player)
        {
            player.GameObject.AddCompany(game);
        }
    }
}