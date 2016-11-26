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
using MyGame.GameStateObjects;
using MyGame.materiel.RtsCommandMessages;
using MyGame.materiel.GameObjects;

namespace MyGame.ClientUI
{
    public class RootContext : UIContext
    {
        public RootContext(ClientGame game, LocalPlayer localPlayer, PlayerGameObject playerGameObject)
            : base(game, localPlayer, playerGameObject)
        {
        }

        public override void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            this.PlayerObject.DrawCompanyList(gameTime, graphics, this.Game.Camera);
        }

        public override void UpdateWithIOEvent(IO.IOEvent ioEvent)
        {
            if (ioEvent.Equals(leftMousePress))
            {
                Vector2 sceenPosition = IOState.MouseScreenPosition();

                Company selectedCompany = this.PlayerObject.ClickCompany(sceenPosition);
                if (selectedCompany != null)
                {
                    new CompanySelected(this, selectedCompany);
                }
                else
                {
                    Vector2 worldPosition = this.Game.Camera.ScreenToWorldPosition(sceenPosition);

                    List<PhysicalObject> clickList = this.Game.GameObjectCollection.Tree.GetObjectsInCircle(worldPosition, 25f);
                    if (clickList.Count > 0)
                    {
                        if (clickList[0] is Base)
                        {
                            new BaseSelected(this, (Base)clickList[0]);
                        }
                        else if (clickList[0] is CombatVehicle)
                        {
                            CombatVehicle vic = (CombatVehicle)clickList[0];
                            if ((Company)vic.Company != null)
                            {
                                new CompanySelected(this, vic.Company);
                            }
                            else
                            {
                                new CombatVehicleSelected(this, vic);
                            }
                        }
                        else if (clickList[0] is Transport)
                        {
                            Transport vic = (Transport)clickList[0];
                            if ((Company)vic.Company != null)
                            {
                                new CompanySelected(this, vic.Company);
                            }
                            else
                            {
                                new TransportVehicleSelected(this, vic);

                            }
                        }
                    }
                }
            }
            else if (ioEvent.Equals(createCompany))
            {
                new CreateCompany(this.LocalPlayer);
            }
        }
    }
}
