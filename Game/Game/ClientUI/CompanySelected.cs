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
        private bool rightMouseDown = false;
        private Vector2 rightMouseDownWorldPosition = new Vector2(0);


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

            if (rightMouseDown)
            {
                Vector2 point1 = this.Game.Camera.WorldToScreenPosition(rightMouseDownWorldPosition);
                Vector2 point2 = IOState.MouseScreenPosition();
                graphics.DrawLine(point1, point2, Color.Red, 1);
            }
        }

        public override void UpdateWithIOEvent(IO.IOEvent ioEvent)
        {
            if (ioEvent.Equals(rightMousePress))
            {
                rightMouseDown = true;
                Vector2 sceenPosition = IOState.MouseScreenPosition();
                rightMouseDownWorldPosition = this.Game.Camera.ScreenToWorldPosition(sceenPosition);
            }
            else if (ioEvent.Equals(rightMouseRelease))
            {
                if (rightMouseDown)
                {
                    Vector2 sceenPosition = IOState.MouseScreenPosition();
                    Vector2 rightMouseUpWorldPosition = this.Game.Camera.ScreenToWorldPosition(sceenPosition);
                    new MoveCompany(this.LocalPlayer, this.selectedCompany, rightMouseDownWorldPosition, rightMouseUpWorldPosition);
                }
                rightMouseDown = false;
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
