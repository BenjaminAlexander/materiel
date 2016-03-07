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
    public class LocalPlayer : BasePlayer<GameObjectUpdate, UdpMessage>, IOObserver
    {
        private ClientGame game;

        private IOEvent leftMousePress = new LeftMousePressed();
        private IOEvent leftMouseRelease = new LeftMouseReleased();
        private IOEvent constructCombat = new KeyPressEvent(Keys.C);

        private Base selectedBase = null;

        public LocalPlayer(IPAddress serverAddress, ClientGame game) : base(serverAddress)
        {
            this.game = game;

            this.game.InputManager.Register(leftMousePress, this);
            this.game.InputManager.Register(leftMouseRelease, this);
            this.game.InputManager.Register(constructCombat, this);
        }

        public void Update(GameTime gameTime)
        {
            //Does this need to exist?
        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            if(selectedBase != null)
            {
                graphics.DrawCircle(selectedBase.Position, 50, Color.Red, 1);
            }
        }

        public override GameObjectUpdate GetUDPMessage(UdpTcpPair client)
        {
            return new GameObjectUpdate(client);
        }

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (ioEvent.Equals(leftMousePress))
            {
                Vector2 sceenPosition = IOState.MouseScreenPosition();
                Vector2 worldPosition = game.Camera.ScreenToWorldPosition(sceenPosition);

                List<CompositePhysicalObject> clickList = game.GameObjectCollection.Tree.GetObjectsInCircle(worldPosition, 25f);
                if (clickList.Count > 0 && clickList[0] is Base)
                {
                    selectedBase = (Base)clickList[0];
                }
            }
            else if (ioEvent.Equals(constructCombat))
            {
                if (selectedBase != null)
                {
                    RtsCommand command = new BuildCombatVehicle(selectedBase);
                    this.SendTCP(command.GetMessage());
                }
            }
        }
    }
}
