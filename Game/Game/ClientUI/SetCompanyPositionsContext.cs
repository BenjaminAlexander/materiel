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

namespace MyGame.ClientUI
{
    class SetCompanyPositionsContext : UIContext
    {
        private CompanySelected coSelected;
        private List<Vector2> positions = new List<Vector2>();

        public SetCompanyPositionsContext(CompanySelected nextInStack)
            : base(nextInStack)
        {
            coSelected = nextInStack;
        }

        public override void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            base.DrawScreen(gameTime, graphics);
            foreach(Vector2 pos in positions)
            {
                Vector2 screenPos = this.Game.Camera.WorldToScreenPosition(pos);
                graphics.DrawCircle(screenPos, 10, Color.Red, 1f);
            }
        }

        public override void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if(ioEvent.Equals(rightMousePress))
            {
                Vector2 sceenPosition = IOState.MouseScreenPosition();
                Vector2 worldPosition = this.Game.Camera.ScreenToWorldPosition(sceenPosition);
                positions.Add(worldPosition);
            }
            else if (ioEvent.Equals(ctrRelease))
            {
                if (positions.Count != 0)
                {
                    new SetCompanyPositions(this.LocalPlayer, coSelected.SelectedCompany, positions);
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
