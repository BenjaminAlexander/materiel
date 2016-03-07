using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.GameServer;

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

        public static Vehicle VehicleFactory(ServerGame game, Vector2 position)
        {
            Vehicle vehicle = new Vehicle(game);
            Vehicle.ServerInitialize(vehicle, position);
            game.GameObjectCollection.Add(vehicle);
            return vehicle;
        }
    }
}
