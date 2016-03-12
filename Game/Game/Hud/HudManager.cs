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
    class HudManager
    {
        private LocalPlayer localPlayer;
        private PlayerGameObject playerGameObject;

        private List<HudObject> hudObjects = new List<HudObject>();

        public HudManager(PlayerGameObject playerGameObject, LocalPlayer localPlayer)
        {
            this.playerGameObject = playerGameObject;
            this.localPlayer = localPlayer;
        }

        public void Update()
        {

        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics) 
        {
            graphics.Begin();
            foreach (HudObject obj in hudObjects)
            {
                obj.Draw(gameTime, graphics);
            }
            graphics.End();
        }
    }
}
*/