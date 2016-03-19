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
    class CompanySelected : UIContext
    {
        private Company selectedCompany;

        public CompanySelected(UIContext nextInStack, Company company)
            : base(nextInStack)
        {
            this.selectedCompany = company;
        }

        public override void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            base.DrawScreen(gameTime, graphics);
            this.PlayerObject.DrawCompanySelection(gameTime, graphics, selectedCompany);
        }

        public override void UpdateWithIOEvent(IO.IOEvent ioEvent)
        {
            if (ioEvent.Equals(rightMousePress))
            {
                Vector2 sceenPosition = IOState.MouseScreenPosition();
                Vector2 worldPosition = this.Game.Camera.ScreenToWorldPosition(sceenPosition);
                new MoveCompany(this.LocalPlayer, this.selectedCompany, worldPosition);
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
