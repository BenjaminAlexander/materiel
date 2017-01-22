using MyGame.GameStateObjects;
using MyGame.Server;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
using MyGame.Client;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.materiel.GameObjects
{
    public class PlayerGameObject : GameObject
    {
        private ClientGame clientGame = null;
        private LocalPlayer localPlayer = null;

        private static int nextColor = 0;
        private static Color[] colors = {Color.Blue, Color.Red, Color.Purple, Color.Orange };

        private IntegerGameObjectMember playerID;
        private IntegerGameObjectMember colorIndex;

        private GameObjectReferenceListField<Company> companies;

        private static int nextSpoofID = 10001;

        public PlayerGameObject(GameObjectCollection collection)
            : base(collection)
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
            PlayerGameObject obj = new PlayerGameObject(game.GameObjectCollection);
            game.GameObjectCollection.Add(obj);
            PlayerGameObject.ServerInitialize(obj, player.Id);
            return obj;
        }

        public static PlayerGameObject Factory(ServerGame game)
        {
            PlayerGameObject obj = new PlayerGameObject(game.GameObjectCollection);
            game.GameObjectCollection.Add(obj);
            PlayerGameObject.ServerInitialize(obj, nextSpoofID++);
            return obj;
        }

        public override void OnClientInitialization(ClientGame game)
        {
            base.OnClientInitialization(game);
            this.clientGame = game;

            if (this.clientGame.LocalPlayer.Id == this.playerID.Value)
            {
                this.localPlayer = this.clientGame.LocalPlayer;
                this.clientGame.LocalPlayer.SetPlayerGameObject(this);
            }
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

        public void RemoveCompany(Company co)
        {
            companies.RemoveAllReferences(co);
        }

        public void DrawCompanyList(GameTime gameTime, MyGraphicsClass myGraphicsClass, Camera camera)
        {
            int count = 0;
            foreach (Company co in this.companies.Value)
            {
                if (co != null)
                {
                    Vector2 textSize = MyGraphicsClass.Font.MeasureString(co.GetHudText());
                    myGraphicsClass.DrawDebugFont(co.GetHudText(), new Vector2(0, count), 1);
                    count = count + (int)(textSize.Y);
                    co.DrawScreen(gameTime, myGraphicsClass, camera, Color.Gray, .9f);
                }
            }
        }

        public void DrawCompanySelection(GameTime gameTime, MyGraphicsClass myGraphicsClass, Company selectedCo)
        {
            int count = 0;
            foreach (Company co in this.companies.Value)
            {
                if (co != null)
                {
                    Vector2 textSize = MyGraphicsClass.Font.MeasureString(co.GetHudText());
                    if (co == selectedCo)
                    {
                        myGraphicsClass.DrawRectangle(new Vector2(0, count), textSize, new Vector2(0), 0, Color.Red, 1);
                        myGraphicsClass.DrawDebugFont("X", new Vector2(textSize.X + 5, count), 1);
                    }
                    count = count + (int)(textSize.Y);
                }
            }
        }

        public bool ClickCompanyDelete(Vector2 mouseScreen, Company selectedCo)
        {
            int count = 0;
            foreach (Company co in this.companies.Value)
            {
                if (co != null)
                {
                    Vector2 textSize = MyGraphicsClass.Font.MeasureString(co.GetHudText());
                    Vector2 xSize = MyGraphicsClass.Font.MeasureString("X");
                    Rectangle rect = new Rectangle((int)(textSize.X + 5), count, (int)(xSize.X), (int)(xSize.Y));
                    if (rect.Contains(mouseScreen) && co == selectedCo)
                    {
                        return true;
                    }
                    count = count + (int)(textSize.Y);
                }
            }
            return false;
        }

        public Company ClickCompany(Vector2 mouseScreen)
        {
            int count = 0;
            foreach (Company co in this.companies.Value)
            {
                if (co != null)
                {
                    Vector2 textSize = MyGraphicsClass.Font.MeasureString(co.GetHudText());
                    Rectangle rect = new Rectangle(0, count, (int)(textSize.X), (int)(textSize.Y));
                    if (rect.Contains(mouseScreen))
                    {
                        return co;
                    }
                    count = count + (int)(textSize.Y);
                }
            }
            return null;
        }

        public bool Owns(IPlayerControlled obj)
        {
            PlayerGameObject playerObject = obj.ControllingPlayer;
            return playerObject == this;
        }
    }
}
