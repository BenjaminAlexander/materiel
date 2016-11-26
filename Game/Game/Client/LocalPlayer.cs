using System.Net;
using MyGame.Server;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.materiel;
using MyGame.DrawingUtils;
using MyGame.ClientUI;
using MyGame.IO;
using MyGame.materiel.GameObjects;

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

        public bool Owns(IPlayerControlled obj)
        {
            PlayerGameObject playerObject = obj.ControllingPlayer;
            if (playerObject != null && obj != null)
            {
                return playerObject.PlayerID == this.Id;
            }
            return false;
        }
    }
}
