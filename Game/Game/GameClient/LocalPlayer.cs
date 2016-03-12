using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using System.Net;
using MyGame.Utils;
using MyGame.GameServer;
using MyGame.GameStateObjects;
using System.Threading;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.IO.Events;
using MyGame.GameStateObjects.PhysicalObjects;
using MyGame.materiel;
using MyGame.DrawingUtils;
using Microsoft.Xna.Framework.Input;
using MyGame.RtsCommands;

namespace MyGame.GameClient
{
    //TODO: name this class better
    //maybe combine with local player
    public class LocalPlayer : BasePlayer<GameObjectUpdate, SetWorldSize>, IOObserver
    {
        private ClientGame game;

        private IOEvent leftMousePress = new LeftMousePressed();
        private IOEvent leftMouseRelease = new LeftMouseReleased();
        private IOEvent constructCombat = new KeyPressEvent(Keys.C);
        private IOEvent createCompany = new KeyPressEvent(Keys.Z);

        private PlayerGameObject playerGameObject = null;

        private Base selectedBase = null;
        private Company selectedCompany = null;

        public LocalPlayer(IPAddress serverAddress, ClientGame game) : base(serverAddress)
        {
            this.game = game;

            this.game.InputManager.Register(leftMousePress, this);
            this.game.InputManager.Register(leftMouseRelease, this);
            this.game.InputManager.Register(constructCombat, this);
            this.game.InputManager.Register(createCompany, this);
        }

        public void SetPlayerGameObject(PlayerGameObject obj)
        {
            if (obj.PlayerID == this.Id)
            {
                this.playerGameObject = obj;
            }
        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            if(selectedBase != null)
            {
                graphics.DrawCircle(selectedBase.Position, 50, Color.Red, 1);
            }
        }
        /*
        public void DrawHud(GameTime gameTime, MyGraphicsClass graphics)
        {
            PlayerGameObject obj = this.GameObject;
            if (obj != null)
            {
                obj.DrawHud(gameTime, graphics, selectedCompany);
            }
        }*/

        public override GameObjectUpdate GetUDPMessage(UdpTcpPair client)
        {
            return new GameObjectUpdate(client);
        }

        public override SetWorldSize GetTCPMessage(UdpTcpPair client)
        {
            return new SetWorldSize(client);
        }

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (ioEvent.Equals(leftMousePress))
            {
                selectedCompany = null;
                selectedBase = null;

                Vector2 sceenPosition = IOState.MouseScreenPosition();

                selectedCompany = this.GameObject.ClickCompany(sceenPosition);

                if (selectedCompany == null)
                {
                    Vector2 worldPosition = game.Camera.ScreenToWorldPosition(sceenPosition);

                    List<CompositePhysicalObject> clickList = game.GameObjectCollection.Tree.GetObjectsInCircle(worldPosition, 25f);
                    if (clickList.Count > 0 && clickList[0] is Base)
                    {
                        selectedBase = (Base)clickList[0];
                    }
                }
            }
            else if (ioEvent.Equals(constructCombat))
            {
                if (selectedBase != null)
                {
                    RtsCommand command = new BuildCombatVehicle(this, selectedBase);
                }
            }
            else if (ioEvent.Equals(createCompany))
            {
                RtsCommand command = new CreateCompany(this);
            }
        }

        public PlayerGameObject GameObject
        {
            get
            {
                List<PlayerGameObject> playerList = this.game.GameObjectCollection.GetMasterList().GetList<PlayerGameObject>();
                foreach (PlayerGameObject player in playerList)
                {
                    if (player.PlayerID == this.Id)
                    {
                        return player;
                    }
                }
                return null;
            }
        }

        public void DrawHud(GameTime gameTime, MyGraphicsClass myGraphicsClass)
        {
            if (playerGameObject != null)
            {
                playerGameObject.DrawHud(gameTime, myGraphicsClass, selectedCompany);
            }
        }
    }
}
