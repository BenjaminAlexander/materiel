using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using MyGame.GameServer;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;

namespace MyGame.materiel
{
    public class PlayerGameObject : GameObject
    {
        private static int nextColor = 0;
        private static Color[] colors = {Color.Blue, Color.Red, Color.Purple, Color.Orange };

        private IntegerGameObjectMember playerID;
        private IntegerGameObjectMember colorIndex;

        private GameObjectReferenceListField<Company> companies;

        public PlayerGameObject(Game1 game)
            : base(game)
        {
            playerID = new IntegerGameObjectMember(this, 0);
            colorIndex = new IntegerGameObjectMember(this, 0);
            companies = new GameObjectReferenceListField<Company>(this);
        }

        public static void ServerInitialize(PlayerGameObject obj, int id)
        {
            obj.playerID.Value = id;
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

        public int PlayerID
        {
            get
            {
                return this.playerID;
            }
        }

        public Company AddCompany(ServerGame serverGame)
        {
            Company co = Company.Factory(serverGame, this);
            companies.Value.Add(co);
            return co;
        }

        public void DrawHud(GameTime gameTime, MyGraphicsClass graphics, Company selected)
        {
            int count = 0;
            foreach (Company co in this.companies.Value)
            {
                if (co != null)
                {
                    co.DrawHud(new Vector2(0, count * 15), graphics);
                    if (co == selected)
                    {
                        graphics.DrawRectangle(new Vector2(0, count * 15), new Vector2(15), new Vector2(0), 0, Color.Red, 1);
                    }
                    count++;
                }
            }
        }

        public Company ClickCompany(Vector2 mouseScreen)
        {
            int count = 0;
            foreach (Company co in this.companies.Value)
            {
                if (co != null)
                {
                    Rectangle rect = new Rectangle(0, count * 15, 15, 15);
                    if (rect.Contains(mouseScreen))
                    {
                        return co;
                    }
                    count++;
                }
            }
            return null;
        }
    }
}
