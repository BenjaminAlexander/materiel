using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.GameStateObjects;
using MyGame.materiel.RtsCommandMessages;
using MyGame.materiel.GameObjects;

namespace MyGame.ClientUI
{
    class CompanySelected : UIContext
    {
        private Company selectedCompany;

        public Company SelectedCompany
        {
            get
            {
                return selectedCompany;
            }
        }

        public CompanySelected(UIContext nextInStack, Company company)
            : base(nextInStack)
        {
            this.selectedCompany = company;
        }

        public override void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            base.DrawScreen(gameTime, graphics);
            this.PlayerObject.DrawCompanySelection(gameTime, graphics, selectedCompany);
            this.selectedCompany.DrawScreen(gameTime, graphics, this.Game.Camera, Color.Red, 1);
        }

        public override void UpdateWithIOEvent(IO.IOEvent ioEvent)
        {
            if (ioEvent.Equals(rightMousePress))
            {
                Vector2 sceenPosition = IOState.MouseScreenPosition();

                Vector2 worldPosition = this.Game.Camera.ScreenToWorldPosition(sceenPosition);
                List<Base> clickList = this.Game.GameObjectCollection.GetObjects<Base>(worldPosition);
                if (clickList.Count > 0)
                {
                    new SetSupplyPoint(this.LocalPlayer, clickList[0], this.selectedCompany);
                    return;
                }
            }
            else if (ioEvent.Equals(ctrPress))
            {
                new SetCompanyPositionsContext(this);
                return;
            }
            else if (ioEvent.Equals(leftMousePress))
            {
                Vector2 sceenPosition = IOState.MouseScreenPosition();

                if (this.PlayerObject.ClickCompanyDelete(sceenPosition, this.selectedCompany))
                {
                    new DeleteCompany(this.LocalPlayer, selectedCompany);
                    this.LocalPlayer.PopUIContext();
                    return;
                }

                Vector2 worldPosition = this.Game.Camera.ScreenToWorldPosition(sceenPosition);

                List<Vehicle> clickList = this.Game.GameObjectCollection.GetObjects<Vehicle>(worldPosition);
                if (clickList.Count > 0)
                {
                    if (clickList[0].Company == this.selectedCompany)
                    {
                        this.LocalPlayer.PopUIContext();
                        if (clickList[0] is CombatVehicle)
                        {
                            new CombatVehicleSelected(this.NextInStack, (CombatVehicle)clickList[0]);
                        }
                        else if(clickList[0] is Transport)
                        {
                            new TransportVehicleSelected(this.NextInStack, (Transport)clickList[0]);
                        }
                        return;
                    }
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
