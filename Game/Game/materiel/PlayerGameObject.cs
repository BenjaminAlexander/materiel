using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using MyGame.GameServer;
using Microsoft.Xna.Framework;
namespace MyGame.materiel
{
    public class PlayerGameObject : GameObject
    {
        private static int nextColor = 0;
        private static Color[] colors = {Color.Blue, Color.Red, Color.Purple, Color.Orange };

        private IntegerGameObjectMember id;
        private IntegerGameObjectMember colorIndex;

        public PlayerGameObject(Game1 game)
            : base(game)
        {
            id = new IntegerGameObjectMember(this, 0);
            colorIndex = new IntegerGameObjectMember(this, 0);
        }

        public static void ServerInitialize(PlayerGameObject obj, int id)
        {
            obj.id.Value = id;
            obj.colorIndex.Value = nextColor;
            nextColor = nextColor + 1 % colors.Length;
        }

        public static PlayerGameObject Factory(ServerGame game, RemotePlayer player)
        {
            PlayerGameObject obj = new PlayerGameObject(game);
            PlayerGameObject.ServerInitialize(obj, player.Id);
            game.GameObjectCollection.Add(obj);
            return obj;
        }

        public Color Color
        {
            get
            {
                return colors[colorIndex.Value];
            }
        }

    }
}
