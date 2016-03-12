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
    public class Vehicle : SmallShip
    {
        private FloatGameObjectMember materiel;
        private GameObjectReferenceField<Company> company;
        private GameObjectReferenceField<PlayerGameObject> controllingPlayer;

        public Vehicle(Game1 game)
            : base(game)
        {
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
            company = new GameObjectReferenceField<Company>(this);
            materiel = new FloatGameObjectMember(this, 0);
        }

        public static void ServerInitialize(Vehicle vic, PlayerGameObject controllingPlayer, Vector2 position)
        {
            vic.controllingPlayer.Value = controllingPlayer;
            SmallShip.ServerInitialize(vic, position, new Vector2(0));
        }

        public static Vehicle VehicleFactory(ServerGame game, PlayerGameObject controllingPlayer, Vector2 position)
        {
            Vehicle vehicle = new Vehicle(game);
            Vehicle.ServerInitialize(vehicle, controllingPlayer, position);
            game.GameObjectCollection.Add(vehicle);
            return vehicle;
        }

        public void SetCompany(Company company)
        {
            if (this.company.Value != null)
            {
                this.company.Value.RemoveVehicle(this);
            }
            this.company.Value = company;
        }
    }
}
