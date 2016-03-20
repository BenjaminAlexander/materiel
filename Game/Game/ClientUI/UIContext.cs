using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.IO.Events;
using MyGame.IO;
using Microsoft.Xna.Framework.Input;
using MyGame.Client;
using MyGame.DrawingUtils;
using Microsoft.Xna.Framework;
using MyGame.materiel;

namespace MyGame.ClientUI
{
    public abstract class UIContext : IOObserver
    {
        protected static IOEvent leftMousePress = new LeftMousePressed();
        protected static IOEvent leftMouseRelease = new LeftMouseReleased();
        protected static IOEvent rightMousePress = new RightMousePressed();
        protected static IOEvent rightMouseRelease = new RightMouseReleased();
        protected static IOEvent constructCombat = new KeyPressEvent(Keys.C);
        protected static IOEvent constructTransport = new KeyPressEvent(Keys.V);
        protected static IOEvent createCompany = new KeyPressEvent(Keys.Z);

        public static void RegisterIO(InputManager ioManager, IOObserver ioObserver)
        {
            ioManager.Register(leftMousePress, ioObserver);
            ioManager.Register(leftMouseRelease, ioObserver);
            ioManager.Register(rightMousePress, ioObserver);
            ioManager.Register(rightMouseRelease, ioObserver);
            ioManager.Register(constructCombat, ioObserver);
            ioManager.Register(constructTransport, ioObserver);
            ioManager.Register(createCompany, ioObserver);
        }

        private ClientGame game;
        private LocalPlayer localPlayer;
        private UIContext nextInStack;
        private PlayerGameObject playerObject;

        protected ClientGame Game
        {
            get
            {
                return game;
            }
        }

        protected LocalPlayer LocalPlayer
        {
            get
            {
                return localPlayer;
            }
        }

        protected PlayerGameObject PlayerObject
        {
            get
            {
                return playerObject;
            }
        }

        public UIContext NextInStack
        {
            get
            {
                return nextInStack;
            }
        }

        protected UIContext(UIContext nextInStack)
            : this(nextInStack.game, nextInStack.localPlayer, nextInStack.playerObject)
        {
            this.nextInStack = nextInStack;
        }

        protected UIContext(ClientGame game, LocalPlayer localPlayer, PlayerGameObject playerGameObject)
        {
            this.game = game;
            this.localPlayer = localPlayer;
            this.localPlayer.PushUIContext(this);
            this.playerObject = playerGameObject;
        }

        protected void UpdateNextInStackIO(IOEvent ioEvent)
        {
            if (this.nextInStack != null)
            {
                this.nextInStack.UpdateWithIOEvent(ioEvent);
            }
        }

        public abstract void UpdateWithIOEvent(IOEvent ioEvent);

        public virtual void DrawWorld(GameTime gameTime, MyGraphicsClass graphics) 
        {
            if (this.nextInStack != null)
            {
                this.nextInStack.DrawWorld(gameTime, graphics);
            }
        }

        public virtual void DrawScreen(GameTime gameTime, MyGraphicsClass graphics) 
        {
            if (this.nextInStack != null)
            {
                this.nextInStack.DrawScreen(gameTime, graphics);
            }
        }
    }
}
