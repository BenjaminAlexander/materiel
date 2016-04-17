using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using MyGame.Client;
using MyGame.materiel;
using Microsoft.Xna.Framework;
using MyGame.IO.Events;
using MyGame.IO;

namespace MyGame.ClientUI
{
    class VehicleSelected : UIContext
    {
        private Vehicle vehicle;

        public VehicleSelected(UIContext nextInStack, Vehicle vehicle)
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
                    if (vehicle is CombatVehicle)
                    {
                        new AddVehicleToCompany(this.LocalPlayer, rightClickCompany, (CombatVehicle)vehicle);
                    }
                    this.LocalPlayer.PopUIContext();
                    new CompanySelected(this.NextInStack, rightClickCompany);
                }
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
