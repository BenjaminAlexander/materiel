using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.GameStateObjects;
using MyGame.materiel.RtsCommandMessages;
using MyGame.materiel.GameObjects;

namespace MyGame.ClientUI
{
    class SetTransportResupplyContext : UIContext
    {
        private TransportVehicleSelected vehicle;
        private Base base1 = null;
        private Base base2 = null;

        private List<Vector2> positions = new List<Vector2>();

        public SetTransportResupplyContext(TransportVehicleSelected nextInStack)
            : base(nextInStack)
        {
            vehicle = nextInStack;
        }

        public override void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            base.DrawScreen(gameTime, graphics);
            if(base1 != null)
            {
                Vector2 screenPos = this.Game.Camera.WorldToScreenPosition(base1.Position);
                graphics.DrawCircle(screenPos, 10, Color.Red, 1f);
            }

            if (base2 != null)
            {
                Vector2 screenPos = this.Game.Camera.WorldToScreenPosition(base2.Position);
                graphics.DrawCircle(screenPos, 10, Color.Red, 1f);
            }

        }

        public override void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if(ioEvent.Equals(rightMousePress))
            {
                Vector2 sceenPosition = IOState.MouseScreenPosition();
                Vector2 worldPosition = this.Game.Camera.ScreenToWorldPosition(sceenPosition);

                List<Base> clickList = this.Game.GameObjectCollection.GetObjects<Base>(worldPosition);
                if (clickList.Count > 0)
                {
                    if(base1 == null)
                    {
                        base1 = clickList[0];
                    }
                    else
                    {
                        base2 = clickList[0];
                    }
                }

            }
            else if (ioEvent.Equals(ctrRelease))
            {
                if (base1 != null && base2 != null)
                {
                    new SetTransportSupplyRoute(this.LocalPlayer, vehicle.SelectedVehicle, base1, base2);
                }
                this.LocalPlayer.PopUIContext();
                this.UpdateNextInStackIO(ioEvent);
            }
            else
            {
                this.UpdateNextInStackIO(ioEvent);
            }
        }
    }
}
