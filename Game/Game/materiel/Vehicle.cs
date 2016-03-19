using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.Server;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.materiel
{
    public class Vehicle : PhysicalObject, IPlayerControlled
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Enemy"), Color.White, TextureLoader.GetTexture("Enemy").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        private FloatGameObjectMember materiel;
        private GameObjectReferenceField<Company> company;
        private GameObjectReferenceField<PlayerGameObject> controllingPlayer;
        private FloatGameObjectMember maxSpeed;
        private Vector2GameObjectMember targetPosition;

        public Vehicle(GameObjectCollection collection)
            : base(collection)
        {
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
            company = new GameObjectReferenceField<Company>(this);
            materiel = new FloatGameObjectMember(this, 0);
            maxSpeed = new FloatGameObjectMember(this, 100);
            targetPosition = new Vector2GameObjectMember(this, new Vector2(0));
        }

        public static void ServerInitialize(Vehicle vic, PlayerGameObject controllingPlayer, Vector2 position)
        {
            vic.controllingPlayer.Value = controllingPlayer;
            vic.maxSpeed.Value = 100;
            vic.targetPosition.Value = position;
            PhysicalObject.ServerInitialize(vic, position, 0);

        }

        public static Vehicle VehicleFactory(GameObjectCollection collection, PlayerGameObject controllingPlayer, Vector2 position)
        {
            Vehicle vehicle = new Vehicle(collection);
            Vehicle.ServerInitialize(vehicle, controllingPlayer, position);
            collection.Add(vehicle);
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

        public Vector2 TargetPosition
        {
            set
            {
                this.targetPosition.Value = value;
            }
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
            this.Position = Utils.PhysicsUtils.MoveTowardBounded(this.Position, this.targetPosition.Value, maxSpeed.Value * seconds);
        }

        public override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
        }

        public GameObjectReference<PlayerGameObject> ControllingPlayer
        {
            get 
            { 
                return controllingPlayer.Value; 
            }
        }
    }
}
