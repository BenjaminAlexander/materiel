using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MyGame.Utils;
using MyGame.Server;
using MyGame.GameStateObjects;
using System.Threading;
using Microsoft.Xna.Framework;
using MyGame.materiel;
using MyGame.DrawingUtils;
using Microsoft.Xna.Framework.Input;
using MyGame.RtsCommands;
using MyGame.ClientUI;
using MyGame.IO;
using MyGame.IO.Events;

namespace MyGame.Client
{
    //TODO: name this class better
    //maybe combine with local player
    public class LocalPlayer : BasePlayer<GameObjectUpdate, SetWorldSize>, IOObserver
    {
        private ClientGame game;

        private PlayerGameObject playerGameObject = null;
        private UIContext topOfStack = null;

        public LocalPlayer(IPAddress serverAddress, ClientGame game) : base(serverAddress)
        {
            this.game = game;
            UIContext.RegisterIO(game.InputManager, this);
        }

        public void PushUIContext(UIContext context)
        {
            this.topOfStack = context;
        }

        public void PopUIContext()
        {
            this.topOfStack = this.topOfStack.NextInStack;
        }

        public void SetPlayerGameObject(PlayerGameObject obj)
        {
            if (obj.PlayerID == this.Id)
            {
                this.playerGameObject = obj;
                this.PushUIContext(new RootContext(this.game, this, this.playerGameObject));
            }
        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            if (this.topOfStack != null)
            {
                graphics.BeginWorld();
                this.topOfStack.DrawWorld(gameTime, graphics);
                graphics.End();

                graphics.Begin();
                this.topOfStack.DrawScreen(gameTime, graphics);
                graphics.End();
            }
        }

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            this.topOfStack.UpdateWithIOEvent(ioEvent);
        }
    }
}
