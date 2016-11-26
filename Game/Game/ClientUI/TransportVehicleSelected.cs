using MyGame.materiel;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.materiel.GameObjects;

namespace MyGame.ClientUI
{
    class TransportVehicleSelected : UIContext
    {
        private Transport vehicle;

        public Transport SelectedVehicle
        {
            get
            {
                return vehicle;
            }
        }

        public TransportVehicleSelected(UIContext nextInStack, Transport vehicle)
            : base(nextInStack)
        {
            this.vehicle = vehicle;
        }

        public override void DrawWorld(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            base.DrawWorld(gameTime, graphics);
            graphics.DrawCircle(vehicle.Position, 50, Color.Red, 1);
        }

        public override void UpdateWithIOEvent(IO.IOEvent ioEvent)
        {
            if (ioEvent.Equals(rightMousePress))
            {
                Vector2 sceenPosition = IOState.MouseScreenPosition();
                Company rightClickCompany = this.PlayerObject.ClickCompany(sceenPosition);

                if (rightClickCompany != null)
                {

                    new AddTransportVehicleToCompany(this.LocalPlayer, rightClickCompany, vehicle);
                    this.LocalPlayer.PopUIContext();
                    new CompanySelected(this.NextInStack, rightClickCompany);
                }
                else
                {
                    Vector2 worldPosition = this.Game.Camera.ScreenToWorldPosition(sceenPosition);

                }
            }
            else if (ioEvent.Equals(ctrPress))
            {
                new SetTransportResupplyContext(this);
                return;
            }
            else if (ioEvent.Equals(leftMousePress))
            {
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
