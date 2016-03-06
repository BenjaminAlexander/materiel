using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
namespace MyGame.materiel
{
    class Vehicle : SmallShip
    {
        private FloatGameObjectMember materiel;

        public Vehicle(Game1 game)
            : base(game)
        {
            materiel = new FloatGameObjectMember(this, 0);
        }

        public static void ServerInitialize(Vehicle vic, Vector2 position)
        {
            ServerInitialize(vic, position, new Vector2(0));
        }
    }
}
