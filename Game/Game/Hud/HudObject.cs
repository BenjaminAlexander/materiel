/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
using MyGame.GameClient;
using MyGame.materiel;

namespace MyGame.Hud
{
    class HudObject
    {
        private Vector2 screenPosition;
        private Company co;

        public HudObject(Company co, Vector2 screenPosition)
        {
            this.screenPosition = screenPosition;
            this.co = co;
        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            graphics.DrawDebugFont(this.co.GetHudText(), this.screenPosition, 1);
        }
    }
}*/
