using Microsoft.Xna.Framework;
using MyGame.materiel.RtsCommandMessages;
using MyGame.materiel.GameObjects;

namespace MyGame.ClientUI
{
    class BaseSelected : UIContext
    {
        private Base selectedBase;

        public BaseSelected(UIContext nextInStack, Base selectedBase)
            : base(nextInStack)
        {
            this.selectedBase = selectedBase;
        }

        public override void DrawWorld(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            base.DrawWorld(gameTime, graphics);
            graphics.DrawCircle(selectedBase.Position, 50, Color.Red, 1);
        }

        public override void UpdateWithIOEvent(IO.IOEvent ioEvent)
        {
            if (ioEvent.Equals(constructCombat))
            {
                new BuildCombatVehicle(this.LocalPlayer, selectedBase);
            }
            else if (ioEvent.Equals(constructTransport))
            {
                new BuildTransport(this.LocalPlayer, selectedBase);
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
