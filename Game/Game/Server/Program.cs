
namespace MyGame.Server
{
    public static class Program
    {
        public static void ServerMain()
        {
            Lobby lobby = new Lobby();
            lobby.Run();
        }
    }
}
