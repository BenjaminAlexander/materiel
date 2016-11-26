using Microsoft.Xna.Framework;
using MyGame.Server;
using MyGame.materiel.GameObjects;

namespace MyGame
{
    public class ServerLogic
    {
        public ServerLogic(ServerGame game, Lobby lobby, Vector2 worldSize)
        {
            RemotePlayer player1 = null;
            RemotePlayer player2 = null;

            foreach (RemotePlayer player in lobby.Clients)
            {
                player.CreatPlayerGameObject(game);
            }

            if (lobby.Clients.Count > 0)
            {
                player1 = lobby.Clients[0];
            }

            if (lobby.Clients.Count > 1)
            {
                player2 = lobby.Clients[1];
            }

            Base player2Base = Base.BaseFactory(game, new Vector2((float)(0.84 * worldSize.X), (float)(0.5 * worldSize.Y)), 1);
            Base.BaseFactory(game, new Vector2((float)(0.84 * worldSize.X), (float)(0.25 * worldSize.Y)), float.PositiveInfinity);
            Base.BaseFactory(game, new Vector2((float)(0.84 * worldSize.X), (float)(0.75 * worldSize.Y)), float.PositiveInfinity);

            Base.BaseFactory(game, new Vector2((float)(0.67 * worldSize.X), (float)(0.11 * worldSize.Y)), float.PositiveInfinity);
            Base.BaseFactory(game, new Vector2((float)(0.67 * worldSize.X), (float)(0.37 * worldSize.Y)), float.PositiveInfinity);
            Base.BaseFactory(game, new Vector2((float)(0.67 * worldSize.X), (float)(0.63 * worldSize.Y)), float.PositiveInfinity);
            Base.BaseFactory(game, new Vector2((float)(0.67 * worldSize.X), (float)(0.89 * worldSize.Y)), float.PositiveInfinity);

            Base.BaseFactory(game, new Vector2((float)(0.5 * worldSize.X), (float)(0.5 * worldSize.Y)), float.PositiveInfinity);
            Base.BaseFactory(game, new Vector2((float)(0.5 * worldSize.X), (float)(0.25 * worldSize.Y)), float.PositiveInfinity);
            Base.BaseFactory(game, new Vector2((float)(0.5 * worldSize.X), (float)(0.75 * worldSize.Y)), float.PositiveInfinity);

            Base.BaseFactory(game, new Vector2((float)(0.33 * worldSize.X), (float)(0.11 * worldSize.Y)), float.PositiveInfinity);
            Base.BaseFactory(game, new Vector2((float)(0.33 * worldSize.X), (float)(0.37 * worldSize.Y)), float.PositiveInfinity);
            Base.BaseFactory(game, new Vector2((float)(0.33 * worldSize.X), (float)(0.63 * worldSize.Y)), float.PositiveInfinity);
            Base.BaseFactory(game, new Vector2((float)(0.33 * worldSize.X), (float)(0.89 * worldSize.Y)), float.PositiveInfinity);

            Base player1Base = Base.BaseFactory(game, new Vector2((float)(0.16 * worldSize.X), (float)(0.5 * worldSize.Y)), 1);
            Base.BaseFactory(game, new Vector2((float)(0.16 * worldSize.X), (float)(0.25 * worldSize.Y)), float.PositiveInfinity);
            Base.BaseFactory(game, new Vector2((float)(0.16 * worldSize.X), (float)(0.75 * worldSize.Y)), float.PositiveInfinity);

            player1Base.SetPlayerInControll(player1);
            Company.Factory(game, player1.GameObject);

            for(int i = 0; i < 5; i++)
            {
                CombatVehicle.CombatVehicleFactory(game.GameObjectCollection, player1.GameObject, player1Base.Position + new Vector2(i * 20, 100));
                Transport.TransportFactory(game.GameObjectCollection, player1.GameObject, player1Base.Position + new Vector2(i * 20, 200));
            }


            player2Base.SetPlayerInControll(player2);
        }
    }
}
